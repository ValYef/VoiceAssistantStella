using System;
using VoiceAssistant.Features.ProcessInterop.Interfaces;
using VoiceAssistant.Features.Responses;

namespace VoiceAssistant.Features.ProcessInterop
{
    public class GrpcProcessCoordinator:IGrpcProcessCoordinator
    {
        private readonly IProcessManager _processManager;

        public event Action<string>? OnProcessError;
        public event Action? OnProcessExited;
        public event Action<string>? OnLogMessage;
        public event Action<ServerResponse>? OnProcessOutput;

        public GrpcProcessCoordinator(IProcessManager processManager)
        {
            _processManager = processManager;

            _processManager.OutputReceived += HandleOutputReceived;
            _processManager.ErrorReceived += (s, err) => OnProcessError?.Invoke(err);
            _processManager.ProcessExited += (s, e) => OnProcessExited?.Invoke();
        }

        public void Start() => _processManager.StartProcess();

        private void HandleOutputReceived(object? sender, ServerResponse response)
        {
            var type = ClassifyMessage(response.Text);

            switch (type)
            {
                case MessageType.Log:
                    OnLogMessage?.Invoke(response.Text);
                    break;
                case MessageType.Error:
                    OnProcessError?.Invoke(response.Text);
                    break;
                default:
                    OnProcessOutput?.Invoke(response);
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
