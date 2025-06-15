using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICommand = VoiceAssistant.Classes.Commands.Interfaces.ICommand;

namespace VoiceAssistant.Classes.Commands
{
    public enum CommandCategory
    {
        LaunchApp,
        DateTime,
        Info
    }
    public abstract class BaseCommand : ICommand
    {
        public abstract CommandCategory Category { get; }

        protected abstract List<string> Keywords { get; }

        public abstract string Execute();

        public bool CanExecute(string input)
        {
            return Keywords
                .Any(keyword => input.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }
}
