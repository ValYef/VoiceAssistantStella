using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.Features.Recognition.Interfaces;

namespace VoiceAssistant.Features.Recognition.Services
{
    public class DefaultRecognitionStateManager:IRecognitionStateManager
    {
        private RecognitionState _currentState = RecognitionState.Idle;

        public RecognitionState CurrentState => _currentState;
        public event EventHandler<RecognitionState>? StateChanged;

        public void TransitionTo(RecognitionState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            StateChanged?.Invoke(this, newState);
        }

        public void Reset() => TransitionTo(RecognitionState.Idle);
    }
}
