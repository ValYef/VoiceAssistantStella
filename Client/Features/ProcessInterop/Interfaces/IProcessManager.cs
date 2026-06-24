using System;
using System.Threading.Tasks;
using VoiceAssistant.Features.Responses;

namespace VoiceAssistant.Features.ProcessInterop.Interfaces
{
    public interface IProcessManager:IDisposable
    {
        event EventHandler<ServerResponse> OutputReceived;
        event EventHandler<string> ErrorReceived;
        event EventHandler ProcessExited;

        void StartProcess();
        void StopProcess();
        Task SendAudioAsync(byte[] audioData);
        bool IsPaused { get; }
    }
}
