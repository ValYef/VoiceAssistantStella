using System;
using System.Diagnostics;
using VoiceAssistant.Classes.Recognition;
using VoiceAssistant.Features.ProcessInterop.Interfaces;
using VoiceAssistant.Features.Recognition.Interfaces;
using VoiceAssistant.Features.Responses;

namespace VoiceAssistant.Features.Recognition.Services
{
    public class VoiceRecognitionService : IVoiceRecognizer, IDisposable
    {
        private readonly RecognitionManager _recognitionManager;
        private readonly IGrpcProcessCoordinator _grpcCoordinator;
        private readonly SpeechResultHandler _speechHandler;

        private bool _disposed;

        public event Action<string>? OnSpeechRecognized;
        public event Action<string>? OnRecognitionStopped;
        public event Action<double>? OnVolumeChanged;
        public event Action<string>? OnCommandResult;
        public event Action<string>? OnAnswerReceived;
        public event Action<string, float>? OnIntentRecognized;

        public bool IsModelReady { get; private set; }

        public VoiceRecognitionService(
            RecognitionManager recognitionManager,
            IGrpcProcessCoordinator grpcCoordinator,
            SpeechResultHandler speechHandler)
        {
            _recognitionManager = recognitionManager;
            _grpcCoordinator = grpcCoordinator;
            _speechHandler = speechHandler;

            SubscribeToEvents();
        }
        private void SubscribeToEvents()
        {
            _recognitionManager.TextRecognized += HandleTextRecognized;
            _recognitionManager.VolumeChanged += volume => OnVolumeChanged?.Invoke(volume);

            _grpcCoordinator.OnLogMessage += msg => Debug.WriteLine($"[GRPC LOG] {msg}");
            _grpcCoordinator.OnProcessOutput += HandleOutput;
            _grpcCoordinator.OnProcessError += HandleProcessError;
            _grpcCoordinator.OnProcessExited += HandleProcessExit;
        }

        private void UnsubscribeFromEvents()
        {
            _recognitionManager.TextRecognized -= HandleTextRecognized;
            _grpcCoordinator.OnProcessOutput -= HandleOutput;
            _grpcCoordinator.OnProcessError -= HandleProcessError;
            _grpcCoordinator.OnProcessExited -= HandleProcessExit;
        }
        public void StartRecognition()
        {
            if (!_recognitionManager.CanStartRecognition) return;

            IsModelReady = true;
            _grpcCoordinator.Start();
            _recognitionManager.StartRecognition();
            Debug.WriteLine("[Service] Recognition started");
        }

        public void PauseRecognition()
        {
            _recognitionManager.TerminateRecognition();
            Debug.WriteLine("[Service] Recognition paused");
            OnRecognitionStopped?.Invoke("Paused");
        }

        public void TerminateRecognition()
        {
            _recognitionManager.TerminateRecognition();
            Debug.WriteLine("[Service] Recognition terminated");
            OnRecognitionStopped?.Invoke("Terminated");
        }

        public void Dispose()
        {
            if (_disposed) return;

            UnsubscribeFromEvents();
            _recognitionManager.Dispose();
            _disposed = true;
        }

        #region Event Handlers

        private async void HandleTextRecognized(string text)
        {
            try
            {
                OnSpeechRecognized?.Invoke(text);
            }
            catch (Exception ex)
            {
                HandleProcessError($"Exception: {ex.Message}");
            }
        }

        private async void HandleOutput(ServerResponse response)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(response.Text))
                {
                    OnSpeechRecognized?.Invoke(response.Text);
                }

                OnIntentRecognized?.Invoke(response.Intent, response.Confidence);

                

                await _speechHandler.HandleAsync(
                    response.Text,
                    response.Answer,
                    OnSpeechRecognized,
                    OnCommandResult,
                    OnAnswerReceived
                );
            }
            catch (Exception ex)
            {
                HandleProcessError($"Exception: {ex.Message}");
            }
        }

        private void HandleProcessError(string error)
        {
            Debug.WriteLine($"[Process Error] {error}");
            OnRecognitionStopped?.Invoke($"Error: {error}");
            TerminateRecognition();
        }

        private void HandleProcessExit()
        {
            OnRecognitionStopped?.Invoke("Process exited");
            TerminateRecognition();
        }

        #endregion
    }
}
