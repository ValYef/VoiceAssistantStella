using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VoiceAssistant.Shared.Helpers;
using VoiceAssistant.Shared.Interfaces;

namespace VoiceAssistant.Shared.Parsers
{
    public class TextParser : ITextParser
    {
        private static readonly HashSet<string> StopWordsSet =
            new(TextParserConfig.StopWords, StringComparer.OrdinalIgnoreCase);

        private string _lastRecognizedText = "";
        private DateTime _lastRecognizedTime = DateTime.UtcNow;

        public bool IsDuplicate(string text)
        {
            var now = DateTime.UtcNow;
            var isDuplicate = text == _lastRecognizedText
                && (now - _lastRecognizedTime).TotalMilliseconds
                  < TextParserConfig.DuplicateCheckIntervalMs;

            if (!isDuplicate)
            {
               _lastRecognizedText = text;
               _lastRecognizedTime = now;
            }
            return isDuplicate;
        }

        public bool TryParseStop(string input) =>
            StopWordsSet.Any(word => input.Contains(word, StringComparison.OrdinalIgnoreCase));

        public bool TryParseText(string input, out string text)
        {
            text = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                text = input.Trim();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Parsing Error]: {ex.Message}\nInput: {input}");
                return false;
            }
        }
    }
}
