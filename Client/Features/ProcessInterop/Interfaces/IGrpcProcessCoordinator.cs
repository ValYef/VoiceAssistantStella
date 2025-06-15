using System;

namespace VoiceAssistant.Features.ProcessInterop.Interfaces
{
    public interface IGrpcProcessCoordinator
    {
        event Action<string>? OnProcessError;
        event Action? OnProcessExited;
        event Action<string>? OnLogMessage;
        event Action<string>? OnProcessOutput;

        void Start();
    }
}
