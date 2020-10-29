using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text;
using C_V_App.Exceptions;
using C_V_App.SerialPortWrappers;

namespace C_V_App.Models
{

    public delegate void CVMonitorDelegate(string message);

    public class CVScan
    {

        private IKeithley2400Model _keithley2400;
        private IWayneKerr4300Model _wayneKerr4300;

        public string Description { get; set; }

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

            WriteComment(DateTime.Now.ToString());
            string senseMode = "KT24 sensing mode is " + keithley2400.SerialSafeRead(":SYSTem:RSENse?");
            WriteComment(senseMode);
            WriteComment(Description);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string f_setstring = ":MEAS:FREQ "; // WK AC probe frequency setup prefix
            string v_setstring = ":MEAS:LEV ";  // WK AC probe amplitude setup prefix

            string setstring = v_setstring += wayneKerr4300.Amplitude.ToString();
            wayneKerr4300.SerialSafeWriteWithDelay(setstring);     // Measuring AC amplitude now also set to operator defined value

            setstring = ":SENS:CURR:PROT ";
            setstring += keithley2400.CurrentLimit.ToString();
            keithley2400.SerialSafeWriteWithDelay(setstring);            //Now set the Keithley 2400 I limit to configuration value

            int i = 0;
            foreach (double test_frequency in Frequencies)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                string setfrequency = f_setstring + test_frequency.ToString();
                wayneKerr4300.SerialSafeWriteWithDelay(setfrequency);
                Thread.Sleep(2000);     // WK needs a HUGE delay in setting the frequency.
                                        // Without this pause, either the Frequency in not set as requested
                                        // Or the WK times-out when asking for the frequency.
                
                C_V_Scan(token); 
                i++;
            }
            stopWatch.Stop();
            var elapsed = stopWatch.Elapsed;
            string completdMessage = $"CV Test Completed: Elapsed time {elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}:{elapsed.Milliseconds / 10:00} ";

            return; // End task;
        }

        private void C_V_Scan(CancellationToken token)
        {
            string ktResponse = "";
            string wkResponse = "";
            // Write heads list to output file and it should have # on the front
            string version = "Not Known";
            if (Assembly.GetEntryAssembly() != null && Assembly.GetEntryAssembly().GetName() != null)
            {
                version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            WriteComment($"Version Information: {version}");
            WriteReportingFieldHeaders();
            string notes = "Using measurement frequency " + _wayneKerr4300.SerialSafeRead(":MEAS:FREQ?") + " Hz ";
            notes += " at a drive level of " + _wayneKerr4300.SerialSafeRead(":MEAS:LEV?") + " volts.";
            WriteComment(notes);
            string v_set = "SOUR:VOLT:LEV ";
            string v_send;
            for (double vout = _keithley2400.StartVoltage; !FinishedLoop(_keithley2400.StartVoltage, _keithley2400.IncrementVoltage, vout); vout += _keithley2400.IncrementVoltage)
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
                            ktResponse = _keithley2400.SerialSafeRead("READ?");
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
                    ktResponse = _keithley2400.SerialSafeRead("READ?");
                }
                catch (Exception tx)
                {
                    Monitor($"K24 Write Error: {tx.Message}");
                }

                // so on its own this code at this line would produce an I-V if wired for it
                ResultsStream.Write("{0},", ktResponse); //File is written with V then I values from K2400

                // Now we should be able to add Func1 and Func2 values from the WK4300 to the string response 

                wkResponse =  _wayneKerr4300.SerialSafeRead(":TRIG");
                DataWriteLine(wkResponse);
                Monitor(ktResponse + " " + wkResponse);
                ClearBuffers(); // clean up all the IO buffers on each loop
            }   //end of measurement voltage loop
            _keithley2400.SerialSafeWrite("SOUR:VOLT:LEV 0"); // set the supply to zero volts at the end
            ClearBuffers(); // and clean up the serial buffers on the way out
        }

        private bool FinishedLoop(double loopStart, double loopEnd, double currentValue)
        {
            int directionOfLoop = Math.Sign(loopEnd - loopStart);
            return directionOfLoop == 0 ? true : (directionOfLoop * Math.Sign(loopEnd - currentValue)) < 0;
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

        private void WriteComment (string comment)
        {
            ResultsStream.Write("# ");
            ResultsStream.WriteLine(comment);
        }

        private void WriteReportingFieldHeaders()
        {
            const string SPACER = ", ";
            string spacer = "";

            // Output reporting fileds as a Comma Separated List, Keithley first, then Wayne Kerr
            StringBuilder reportingHeaders = new StringBuilder();
            foreach (var header in _keithley2400.ReportingFields)
            {
                reportingHeaders.Append(spacer);
                reportingHeaders.Append(header);
                spacer = SPACER;
            }
            foreach (var header in _wayneKerr4300.ReportingFields)
            {
                reportingHeaders.Append(spacer);
                reportingHeaders.Append(header);
                spacer = SPACER;
            }
            WriteComment(reportingHeaders.ToString());
        }
    }
}
