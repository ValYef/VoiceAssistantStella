using System;

namespace VoiceAssistant.Features.Audio.Recorder
{
    public interface IRecorder
    {
        event Action<byte[]> AudioCaptured;
        void StartRecording();
        void StopRecording();
        double CalculateRMSVolume(byte[] audioData);
    }
}
