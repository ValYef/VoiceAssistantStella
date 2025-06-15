using System.Globalization;

namespace VoiceAssistant.Shared.Parsers
{
    public class VolumeParser : Interfaces.IVolumeParser
    {
        public bool TryParseVolume(string input, out double volume)
        {
            volume = 0;
            if (!input.StartsWith("Гучність:"))
                return false;

            var parts = input.Split(':');
            if (parts.Length < 2)
                return false;

            return double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out volume);
        }
    }
}
