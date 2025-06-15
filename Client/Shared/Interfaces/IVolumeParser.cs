namespace VoiceAssistant.Shared.Interfaces
{
    public interface IVolumeParser
    {
        bool TryParseVolume(string input, out double volume);
    }
}
