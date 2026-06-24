using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoiceAssistant.Features.Audio.AudioPlayer;

namespace VoiceAssistant.Features.Responses
{
    public class VoiceResponseService : IVoiceResponseService
    {
        public bool _isResponding { get; set; } = false;
        private readonly IAudioPlayer _audioPlayer;
        private string _lastAnswer = "";
        private DateTime _lastAnswerTime = DateTime.UtcNow;
        private static readonly Random _rng = new();

        private readonly Dictionary<string, List<string>> _responses = new()
        {
            { "Привіт",new(){"hi.wav","hello_imStella_how_r_u.wav" }},
            { "як справи", new() { "im_okay_how_r_u.wav" } },
            { "гаразд", new() { "potuzhno.wav" } },
            { "мова",new() { "please_speak_ukrainian.wav","speaking_ukrainian_hope_to_know_more_languages.wav" }},
            { "відкрий браузер", new() { "opening_browser.wav" } },
            { "відкрий калькулятор", new() { "opening_calculator.wav" } },
            { "потужн", new() { "potuzhno_totally.wav" } },
            { "дивист", new() { "wow_its_cool.wav" } },
        };

        private readonly List<string> _confiramtionKeywords = new()
        {
            "так", "добре", "да", "окей", "зрозуміл", "зрозумів", "звичайно"
        };
        private readonly List<string> _confirmationSounds=new()
        {
            "okay.wav", "okey.wav", "good.wav"
        };

        private readonly List<string> _negationKeywords = new()
        {
            "ні", "no", "not", "never", "nope"
        };
        private readonly List<string> _negationSounds = new()
        {
            "no.wav", "never.wav"
        };
        private readonly List<string> _successLogKeywords = new()
        {
            "Готова слухати! Скажіть 'стоп', щоб зупинити."
        };
        private readonly List<string> _successLogSounds = new()
        {
            "sound_loading_success.wav"
        };

        public VoiceResponseService(IAudioPlayer audioPlayer)
        {
            _audioPlayer=audioPlayer;
        }
        private void Respond(string recognizedText)
        {
            recognizedText = recognizedText.ToLower();

            foreach (var keyword in _successLogKeywords)
            {
                if (recognizedText.Contains(keyword))
                {
                    _audioPlayer.PlayRandom(_successLogSounds);
                    return;
                }
            }
            foreach (var keyword in _confiramtionKeywords)
            {
                if (recognizedText.Contains(keyword))
                {
                    _audioPlayer.PlayRandom(_confirmationSounds);
                    return;
                }
            }
            foreach (var keyword in _negationKeywords)
            {
                if (recognizedText.Contains(keyword))
                {
                    _audioPlayer.PlayRandom(_negationSounds);
                    return;
                }
            }
            foreach (var response in _responses)
            {
                if (recognizedText.Contains(response.Key, StringComparison.OrdinalIgnoreCase))
                {
                    _audioPlayer.PlayRandom(response.Value);
                    return;
                }
            }
            // If no response is found
            // _audioPlayer.PlayRandom(_defaultResponses);
        }

        private async Task RespondSafely(string recognizedText)
        {
            if (_isResponding) return;

            _isResponding = true;

            await Task.Run (() =>
            {
                if (recognizedText.Length > 3)
                {
                    Respond(recognizedText);
                }
            });

            _isResponding = false;
        }
        public bool IsDuplicate(string answer)
        {
            var now = DateTime.UtcNow;

            var isDuplicate =
                answer == _lastAnswer &&
                (now - _lastAnswerTime).TotalSeconds < 3;

            if (!isDuplicate)
            {
                _lastAnswer = answer;
                _lastAnswerTime = now;
            }

            return isDuplicate;
        }
        public async Task RespondConditionally(string text)
        {
            ResponseTrigger trigger = DetectTrigger(text);

            switch (trigger)
            {
                case ResponseTrigger.Greeting:
                    await RespondSafely(text);         
                    break;
                case ResponseTrigger.Command:
                    break;                              
                case ResponseTrigger.Unknown:
                    if (_rng.Next(100) < 30)            // 30% шанс
                        await RespondSafely(text);
                    break;
            }
        }
        private ResponseTrigger DetectTrigger(string text)
        {
            var greetings = new[] { "привіт", "до побачення", "добрий" };
            if (greetings.Any(g => text.Contains(g, StringComparison.OrdinalIgnoreCase)))
                return ResponseTrigger.Greeting;

            var commands = new[] { "відкрий", "закрий", "запусти" };
            if (commands.Any(c => text.Contains(c, StringComparison.OrdinalIgnoreCase)))
                return ResponseTrigger.Command;

            return ResponseTrigger.Unknown;
        }
    }
}
