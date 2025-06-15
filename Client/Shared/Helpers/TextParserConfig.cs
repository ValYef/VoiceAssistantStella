using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Shared.Helpers
{
    public static class TextParserConfig
    {
        public static string[] StopWords { get; } = DefaultStopWords;
        public static int DuplicateCheckIntervalMs { get; } = 1500;

        private static readonly string[] DefaultStopWords =
            { "стоп", "зупини", "не слухай", "хватить", "досить" };
    }
}
