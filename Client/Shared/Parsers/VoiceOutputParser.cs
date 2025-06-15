using VoiceAssistant.Shared.Interfaces;

namespace VoiceAssistant.Shared.Parsers
{
    public class VoiceOutputParser:IOutputParser
    {
        private readonly ITextParser _textParser;
        private readonly IVolumeParser _volumeParser;

        public VoiceOutputParser()
        {
            _textParser = new TextParser();
            _volumeParser = new VolumeParser();
        }
        public VoiceOutputParser(ITextParser textParser, IVolumeParser volumeParser)
        {
            _textParser = textParser;
            _volumeParser = volumeParser;
        }
        public bool TryParseVolume(string data, out double volume)
            => _volumeParser.TryParseVolume(data, out volume);

        public bool TryParseText(string data, out string text)
            => _textParser.TryParseText(data, out text);

        public bool TryParseStop(string data)=>_textParser.TryParseStop(data);

        public bool IsDuplicate(string text) => _textParser.IsDuplicate(text);
    }
}
