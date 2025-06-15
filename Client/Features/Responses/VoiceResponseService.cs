using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoiceAssistant.Features.Audio.AudioPlayer;

namespace VoiceAssistant.Features.Responses
{
    public class VoiceResponseService : IVoiceResponseService
    {
        public bool _isResponding { get; set; } = false;
        private readonly IAudioPlayer _audioPlayer;

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
        public void Respond(string recognizedText)
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

        public async Task RespondSafely(string recognizedText)
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
    }
}
