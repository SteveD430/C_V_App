﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using C_V_App.Exceptions;
using C_V_App.SerialPortWrappers;

namespace C_V_App.Models
{

    public delegate void CVMonitorDelegate(string message);

    public class CVScan
    {

        private IKeithley2400Model _keithley2400;
        private IWayneKerr4300Model _wayneKerr4300;

        public ICollection<double> Frequencies { get; set; }

        public StreamWriter ResultsStream { get; set; }

        public CVMonitorDelegate Monitor { get; set; }

        public void Multiscan (IKeithley2400Model keithley2400, 
            IWayneKerr4300Model wayneKerr4300, 
            ICVEnvironment cvEnv,
            CancellationToken token)
        {
            _keithley2400 = keithley2400;
            _wayneKerr4300 = wayneKerr4300;


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string f_setstring = ":MEAS:FREQ "; // WK AC probe frequency setup prefix
            string v_setstring = ":MEAS:LEV ";  // WK AC probe amplitude setup prefix

            string setstring = v_setstring += wayneKerr4300.Amplitude.ToString();
            wayneKerr4300.SerialSafeWrite(setstring);     // Measuring AC amplitude now also set to operator defined value
            setstring = f_setstring + wayneKerr4300.Amplitude.ToString();
            wayneKerr4300.SerialSafeWrite(setstring);     // Set the amplitude

            setstring = ":SENS:CURR:PROT ";
            setstring += keithley2400.CurrentLimit.ToString();
            keithley2400.SerialSafeWrite(setstring);            //Now set the Keithley 2400 I limit to configuration value

            int i = 0;
            foreach (double test_frequency in Frequencies)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                string setfrequency = f_setstring + test_frequency.ToString();
                wayneKerr4300.SerialSafeWrite(setfrequency);
                //set the frequency to each one listed
                C_V_Scan(token); //and run one scan at each frequency setting
                i++;
            }
            stopWatch.Stop();
            var elapsed = stopWatch.Elapsed;
            string completdMessage = $"CV Test Completed: Elapsed time {elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}:{elapsed.Milliseconds / 10:00} ";

            return; // End task;
        }

        private void C_V_Scan(CancellationToken token)
        {
            string response = "";
            // Write heads list to output file and it should have # on the front
            DataWriteLine($"# Version Information: {Assembly.GetEntryAssembly().GetName().Version.ToString()} ");
            DataWriteLine($"# {_wayneKerr4300.ReportingFields}");
            string notes = "# Using measurement frequency " + _wayneKerr4300.SerialSafeRead(":MEAS:FREQ?") + " Hz ";
            notes += " at a drive level of " + _wayneKerr4300.SerialSafeRead(":MEAS:LEV?") + " volts.";
            DataWriteLine(notes);
            string v_set = "SOUR:VOLT:LEV ";
            string v_send;
            for (double vout = _keithley2400.StartVoltage; vout <= _keithley2400.FinalVoltage; vout += _keithley2400.IncrementVoltage)
            {
                // Check for Cancellation.
                if (token.IsCancellationRequested)
                {
                    return;
                }
                // start of for voltage loop 
                ClearBuffers(); // clean up all the IO buffers on each loop
                v_send = v_set + string.Format("{0}", vout); // loop which increments voltage set by Keithley
                if (vout == _keithley2400.StartVoltage)
                    try
                    {   // Nasty mess put in to discard incorrect first value voltages from Keithley
                        Thread.Sleep(100);
                        int x = 10;
                        for (x = 1; x <= 10; x += 1)
                        {
                            v_send = v_set + string.Format("{0}", (vout * x / 10));
                            // send v_start out in steps gently
                            _keithley2400.SerialSafeWrite(v_send);
                            Thread.Sleep(100);
                            response = _keithley2400.SerialSafeRead("READ?");
                        }

                    }
                    catch (Exception ex)
                    {
                       throw new RetreivingDataException("An error occurred in code block which discards first Keithley values", ex);
                    }
                try
                {
                    _keithley2400.SerialSafeWrite(v_send);
                }
                catch (Exception tx)
                {
                    Monitor($"K24 Write Error: {tx.Message}");
                }
                Thread.Sleep(50); // Hold for 50 ms
                try
                {
                    response = _keithley2400.SerialSafeRead("READ?");
                }
                catch (Exception tx)
                {
                    Monitor($"K24 Write Error: {tx.Message}");
                }

                Monitor(response);   // so on its own this code at this line would produce an I-V if wired for it
                ResultsStream.Write("{0},", response); //File is written with V then I values from K2400

                // Now we should be able to add Func1 and Func2 values from the WK4300 to the string response 

                response =  _wayneKerr4300.SerialSafeRead(":TRIG");
                DataWriteLine(response);
                ClearBuffers(); // clean up all the IO buffers on each loop
            }   //end of measurement voltage loop
            _keithley2400.SerialSafeWrite("SOUR:VOLT:LEV 0"); // set the supply to zero volts at the end
            ClearBuffers(); // and clean up the serial buffers on the way out
        }

        private void DataWriteLine(string stuff)
        {
            try
            {
                ResultsStream.WriteLine(stuff);
            }
            catch (Exception ex)
            {
                Monitor($"Error writing to data file on disc: {ex.Message}");
            }
        }


        private void ClearBuffers()   //Sorts out the Keithley and Wayne Kerr buffers 
        {
            _keithley2400.ClearBuffers();
            _wayneKerr4300.ClearBuffers();
        }
    }
}