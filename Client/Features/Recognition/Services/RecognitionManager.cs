using System;
using VoiceAssistant.Features.Audio.AudioRecorder;
using VoiceAssistant.Features.Audio.Recorder;
using VoiceAssistant.Features.ProcessInterop.Interfaces;
using VoiceAssistant.Features.Recognition.Interfaces;

namespace VoiceAssistant.Features.Recognition.Services
{
    public class RecognitionManager : IDisposable
    {
        private readonly IRecorder _recorder;
        private readonly IProcessManager _processManager;
        private readonly IRecognitionStateManager _stateManager;
        private bool _disposed;

        public event Action<string>? TextRecognized;
        public event Action<double>? VolumeChanged;

        public bool CanStartRecognition => _stateManager.CurrentState == RecognitionState.Idle;
        public RecognitionManager(
            IProcessManager processManager,
            IRecorder? recorder = null,
            IRecognitionStateManager? stateManager = null)
        {
            _recorder = recorder ?? new AudioRecorder();
            _processManager = processManager;
            _stateManager = stateManager ?? new DefaultRecognitionStateManager();

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _recorder.AudioCaptured += HandleAudioCaptured;
            _stateManager.StateChanged += HandleStateChange;
            _processManager.ProcessExited += HandleProcessExit;
        }

        private void UnsubscribeFromEvents()
        {
            _recorder.AudioCaptured -= HandleAudioCaptured;
            _stateManager.StateChanged -= HandleStateChange;
            _processManager.ProcessExited -= HandleProcessExit;
        }

        public void StartRecognition()
        {
            if (_stateManager.CurrentState != RecognitionState.Idle) return;

            try
            {
                _processManager.StartProcess();
                _recorder.StartRecording();
                _stateManager.TransitionTo(RecognitionState.Listening);
            }
            catch (Exception ex)
            {
                _stateManager.TransitionTo(RecognitionState.Idle);
                Console.WriteLine($"Recognition start failed: {ex.Message}");
            }
        }

        public void ResumeRecognition()
        {
            if (_stateManager.CurrentState != RecognitionState.Paused) return;

            _recorder.StartRecording();
            _stateManager.TransitionTo(RecognitionState.Listening);
        }

        public void TerminateRecognition()
        {
            _recorder.StopRecording();
            _processManager.StopProcess();
            _stateManager.Reset();
        }

        private void HandleStateChange(object? sender, RecognitionState newState)
        {
            switch (newState)
            {
                case RecognitionState.Processing:
                    _recorder.StopRecording();
                    break;
                case RecognitionState.Listening:
                    _recorder.StartRecording();
                    break;
            }
        }

        private void HandleProcessExit(object? sender, EventArgs e)
        {
            TerminateRecognition();
        }

        private async void HandleAudioCaptured(byte[] audioData)
        {
            try
            {
                if (!_processManager.IsPaused)
                {
                    await _processManager.SendAudioAsync(audioData);
                    double volume = _recorder.CalculateRMSVolume(audioData);
                    VolumeChanged?.Invoke(volume);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending audio: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            UnsubscribeFromEvents();
            _recorder.StopRecording();
            (_processManager as IDisposable)?.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
