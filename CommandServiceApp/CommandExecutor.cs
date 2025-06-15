using VoiceAssistant.Classes.Commands.CommandsExecutors;
using VoiceAssistant.Classes.Commands.Interfaces;

namespace VoiceAssistant.Classes.Commands
{
    public class CommandExecutor:ICommandExecutor
    {
        private readonly List<ICommand> _commands=new();
        public CommandExecutor()
        {
            _commands.Add(new CommandList());

            _commands.Add(new OpenBrowserCommand());
            _commands.Add(new OpenCalculatorCommand());
            _commands.Add(new OpenTelegramCommand());
            _commands.Add(new OpenYoutubeCommand());
            _commands.Add(new WhatDayCommand());
        }
        public string? ExecuteCommand(string recognizedText)
        {
            foreach (var command in _commands)
            {
                if (command.CanExecute(recognizedText))
                {
                    return command.Execute();
                }
            }
            return null;
        }
    }
}
