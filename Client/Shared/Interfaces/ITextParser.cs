namespace VoiceAssistant.Shared.Interfaces
{
    public interface ITextParser
    {
        bool TryParseText(string input, out string text);
        bool TryParseStop(string input);
        bool IsDuplicate(string text);
    }
}
