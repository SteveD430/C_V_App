using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using C_V_App.Exceptions;

namespace C_V_App.Models
{
    public enum ConfigCommandType { Comment, StartVoltage, FinalVoltage, VoltageIncrement, CurrentLimit, Amplitude, Frequency, End};

    public class CVConfiguration
    {
        private IDictionary<string, ConfigCommandType> _recognisedCommands;

        public string Comment { get; set; }

        public string EndComment { get; set; }

        public double StartVoltage { get; set; }

        public double FinalVoltage { get; set; }

        public double IncrementVoltage { get; set; }

        public double CurrentLimit { get; set; }

        public double Amplitude { get; set; }

        public List<double> TestWaveFrequency { get; private set; }

        private delegate void CommandExecution(string data);

        private IDictionary<ConfigCommandType, CommandExecution> _commandExecutions;

        public CVConfiguration()
        {
            _recognisedCommands = new Dictionary<string, ConfigCommandType>()
            {   {"scanstartvoltage", ConfigCommandType.StartVoltage },
                { "finalscanvoltage", ConfigCommandType.FinalVoltage },
                { "dccurrentlimit", ConfigCommandType.CurrentLimit },
                { "voltageincrement", ConfigCommandType.VoltageIncrement },
                { "acdriveamplitude", ConfigCommandType.Amplitude },
                { "testwavefrequency", ConfigCommandType.Frequency }
            };

            _commandExecutions = new Dictionary<ConfigCommandType, CommandExecution>()
            {
                {ConfigCommandType.StartVoltage, (data) => { StartVoltage = Double.Parse(data); } },
                {ConfigCommandType.FinalVoltage, (data) => { FinalVoltage = Double.Parse(data); } },
                {ConfigCommandType.CurrentLimit, (data) => { CurrentLimit = Double.Parse(data); } },
                {ConfigCommandType.VoltageIncrement, (data) => { IncrementVoltage = Double.Parse(data); } },
                {ConfigCommandType.Amplitude, (data) => { Amplitude = Double.Parse(data); } },
                {ConfigCommandType.Frequency, (data) => AddTestWaveFrequency(data) },
                {ConfigCommandType.Comment, (data) => Comment = data },
                {ConfigCommandType.End, (data) => EndComment = data }
            };

            TestWaveFrequency = new List<double>();
            
        }


        public void LoadConfiguration(string filename)
        {

            try
            {
                StreamReader stream = new StreamReader(filename);
                LoadConfiguration(stream);
            }
            catch (Exception ex)
            {
                throw new ErrorOpeningConfigFileException(filename);
            }
        }

        public void LoadConfiguration (StreamReader stream)
        {
            string line;
            ConfigCommand configCommand;

            TestWaveFrequency.Clear();
            while (((line = stream.ReadLine()) != null) &&
                (configCommand = Analyse(line)).ConfigCommandType != ConfigCommandType.End)
            {
                _commandExecutions[configCommand.ConfigCommandType](configCommand.CommandValue);
            }

        }
        public void SaveConfiguration(string filename)
        {
            StreamWriter writer = new StreamWriter(filename);
            SaveConfiguration(writer);
        }

        public void SaveConfiguration(StreamWriter stream)
        {
            stream.WriteLine($"# File Creation {DateTime.Today.ToShortDateString()}  {DateTime.Now.ToShortTimeString()}");
            stream.WriteLine($"Scan Start Voltage = {StartVoltage:f3}");
            stream.WriteLine($"Final Scan Voltage = {FinalVoltage:f3}");
            stream.WriteLine($"DC Current limit =  {CurrentLimit:f3}");
            stream.WriteLine($"Voltage Increment = {IncrementVoltage:f3}");
            stream.WriteLine($"AC Drive Amplitude = {Amplitude:f3}");
            foreach (var frequency in TestWaveFrequency)
            {
                stream.WriteLine($"Test Wave Frequency = {frequency}");
            }
            stream.WriteLine("END of configuration file");
            stream.Flush();
            stream.Close();
        }

        private ConfigCommand Analyse(string line)
        {
            if (line[0] == '#')
            {
                return new ConfigCommand(ConfigCommandType.Comment, "Comment", line);
            }
            if (line.StartsWith("END"))
            {
                return new ConfigCommand(ConfigCommandType.End, "END", line);
            }
            ConfigCommandType configType;
            var segments = string.Concat(line.Where(c => !char.IsWhiteSpace(c))).ToLower().Split('=');
            if (segments.Length == 2 && _recognisedCommands.TryGetValue(segments[0], out configType))
            {
                return new ConfigCommand(configType, segments[0], segments[1]);
            }
            throw new UnrecognisedConfigCommandException(line);
        }
        #region ConfigCommandDelegates

        public void AddTestWaveFrequency(string data)
        {
            TestWaveFrequency.Add(Double.Parse(data));
        }
        #endregion ConfigCommandDelegates
    }
}
