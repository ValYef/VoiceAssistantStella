namespace VoiceAssistant.Shared.Interfaces
{
    public interface IOutputParser
    {
        public bool TryParseVolume(string data, out double volume);
        public bool TryParseText(string data, out string text);
        public bool TryParseStop(string data);
        public bool IsDuplicate(string text);
    }
}
