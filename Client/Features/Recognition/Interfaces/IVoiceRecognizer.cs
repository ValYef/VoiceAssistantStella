using System;

namespace VoiceAssistant.Features.Recognition.Interfaces
{
    public interface IVoiceRecognizer
    {
        event Action<string> OnSpeechRecognized;
        event Action<string> OnRecognitionStopped;
        event Action<double> OnVolumeChanged;
        event Action<string>? OnCommandResult;

        bool IsModelReady { get; }

        void StartRecognition();
        void PauseRecognition();
        void TerminateRecognition();

    }
}
