using System;
using VoiceAssistant.Features.ProcessInterop.Interfaces;

namespace VoiceAssistant.Features.ProcessInterop
{
    public class GrpcProcessCoordinator:IGrpcProcessCoordinator
    {
        private readonly IProcessManager _processManager;

        public event Action<string>? OnProcessError;
        public event Action? OnProcessExited;
        public event Action<string>? OnLogMessage;
        public event Action<string>? OnProcessOutput;

        public GrpcProcessCoordinator(IProcessManager processManager)
        {
            _processManager = processManager;

            _processManager.OutputReceived += HandleOutputReceived;
            _processManager.ErrorReceived += (s, err) => OnProcessError?.Invoke(err);
            _processManager.ProcessExited += (s, e) => OnProcessExited?.Invoke();
        }

        public void Start() => _processManager.StartProcess();

        private void HandleOutputReceived(object? sender, string text)
        {
            var type = ClassifyMessage(text);
            switch (type)
            {
                case MessageType.Log:
                    OnLogMessage?.Invoke(text);
                    break;
                case MessageType.Error:
                    OnProcessError?.Invoke(text);
                    break;
                default:
                    OnProcessOutput?.Invoke(text);
                    break;
            }
        }
        private MessageType ClassifyMessage(string text)
        {
            
            if (text.StartsWith("[LOG]") || text.Contains("Log", StringComparison.OrdinalIgnoreCase))
                return MessageType.Log;
            
            if (text.StartsWith("[ERROR]") || text.Contains("помилк", StringComparison.OrdinalIgnoreCase))
                return MessageType.Error;

            return MessageType.Output;
        }
    }
}
