namespace VoiceAssistant.Classes.Commands.Interfaces
{
    public interface ICommand
    {
        bool CanExecute(string command);
        string Execute();
    }
}
