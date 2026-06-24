using System;
using VoiceAssistant.Features.Responses;

namespace VoiceAssistant.Features.ProcessInterop.Interfaces
{
    public interface IGrpcProcessCoordinator
    {
        event Action<string>? OnProcessError;
        event Action? OnProcessExited;
        event Action<string>? OnLogMessage;
        event Action<ServerResponse>? OnProcessOutput;

        void Start();
    }
}
