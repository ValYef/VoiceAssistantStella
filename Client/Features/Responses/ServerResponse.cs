using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Features.Responses
{
    public enum ResponseTrigger
    {
        Greeting,   // привіт, до побачення - відповідь завжди
        Command,    // відкрий браузер - тихо виконуємо, не відповідаємо голосом
        Unknown     // не розпізнали - відповідаємо тільки іноді (випадково)
    }
    public class ServerResponse
    {
        public string? Text { get; set; }
        public string? Answer { get; set; }
        public string? Intent { get; set; }
        public float Confidence { get; set; }
    }
}
