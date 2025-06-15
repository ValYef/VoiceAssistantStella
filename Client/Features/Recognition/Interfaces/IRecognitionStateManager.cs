using System;
namespace VoiceAssistant.Features.Recognition.Interfaces
{
    public enum RecognitionState
    {
        Idle,
        Listening,
        Paused,
        Processing
    }
    public interface IRecognitionStateManager
    {
        RecognitionState CurrentState { get; }
        event EventHandler<RecognitionState>? StateChanged;

        void TransitionTo(RecognitionState newState);
        void Reset();
    }
}
