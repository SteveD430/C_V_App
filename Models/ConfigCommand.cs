
namespace C_V_App.Models
{
    public class ConfigCommand
    {
        public ConfigCommand (ConfigCommandType configCommandType, string command, string commandValue)
        {
            ConfigCommandType = configCommandType;
            Command = command;
            CommandValue = commandValue;
        }

        public ConfigCommandType ConfigCommandType  { get; private set;}

        public string Command { get; private set; }

        public string CommandValue { get; private set; }
    }
}
