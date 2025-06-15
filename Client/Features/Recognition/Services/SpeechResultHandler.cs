using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VoiceAssistant.Features.Audio.AudioPlayer;
using VoiceAssistant.Features.Commands;
using VoiceAssistant.Features.Responses;
using VoiceAssistant.Shared.Interfaces;
using VoiceAssistant.Shared.Parsers;

namespace VoiceAssistant.Classes.Recognition
{
    public class SpeechResultHandler
    {
        private readonly IOutputParser _parser;
        private readonly ICommandServiceAdapter _adapter;
        private readonly IVoiceResponseService _responseService;
        private readonly IAudioPlayer _audioPlayer;

        public SpeechResultHandler(
            IOutputParser? parser=null,
            ICommandServiceAdapter? adapter = null)
        {
            _parser = parser ?? new VoiceOutputParser();
            _adapter = adapter ?? new CommandServiceProcessAdapter();

            _audioPlayer = new AudioPlayer();
            _responseService = new VoiceResponseService(_audioPlayer);
        }

        public async Task<bool> HandleAsync(
            string message,
            Action<string> onRecognized,
            Action<string> onCommandResult)
        {
            if (!_parser.TryParseText(message, out var parsedText))
                return false;

            if (_parser.IsDuplicate(parsedText))
                return false;

            Debug.WriteLine($"[Handle] {message}");

            try
            {
                onRecognized?.Invoke(parsedText);

                _responseService.RespondSafely(parsedText);

                var result = await _adapter.ExecuteCommandAsync(parsedText);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    onCommandResult?.Invoke(result);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Command error: {ex.Message}");
                return false;
            }
        }
    }
}
