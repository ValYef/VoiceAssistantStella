using NAudio.Wave;
using System;
using System.Diagnostics;
using VoiceAssistant.Features.Audio.Recorder;

namespace VoiceAssistant.Features.Audio.AudioRecorder
{
    public class AudioRecorder : IRecorder, IDisposable
    {
        private WaveInEvent? waveIn;
        public event Action<byte[]>? AudioCaptured;

        private bool _isRecording = false;

        public void StartRecording()
        {
            if (_isRecording) return;
            _isRecording = true;

            var format = new WaveFormat(16000, 16, 1);

            waveIn = new WaveInEvent() 
            {
                WaveFormat = format
            };
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();
        }

        public void StopRecording()
        {
            if (!_isRecording) return;
            _isRecording = false;

            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveIn = null;
        }

        public void Dispose() => StopRecording();
        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                byte[] bufferCopy = new byte[e.BytesRecorded];
                Array.Copy(e.Buffer, 0, bufferCopy, 0, e.BytesRecorded);
                AudioCaptured?.Invoke(bufferCopy);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Audio Error] {ex}");
            }
        }

        public double CalculateRMSVolume(byte[] audioData)
        {
            int bytesPerSample = 2; // 16-bit audio
            int sampleCount = audioData.Length / bytesPerSample;
            double sum = 0.0;

            for (int i = 0; i < audioData.Length; i+=2)
            {
                short sample = BitConverter.ToInt16(audioData, i);
                double normalized = sample / 32768.0; // Normalize to [-1, 1]
                sum += normalized * normalized;
            }

            return Math.Sqrt(sum / sampleCount); // RMS
        }
    }
}
