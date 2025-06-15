namespace VoiceAssistant.Classes.Commands.Interfaces
{
    internal interface ICommandExecutor
    {
       string? ExecuteCommand(string recognizedText);
    }
}
