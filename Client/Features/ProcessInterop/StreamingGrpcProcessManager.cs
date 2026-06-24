using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using VoiceAssistant.Features.ProcessInterop.Interfaces;
using VoiceAssistant.Features.Responses;
using VoiceAssistant.Grps;

namespace VoiceAssistant.Features.ProcessInterop
{
    public class StreamingGrpcProcessManager : IProcessManager
    {
        private readonly SpeechRecognizerGrpcClient _grpcClient;
        private readonly SynchronizationContext _syncContext;
        private Channel<byte[]> _audioChannel;
        private CancellationTokenSource? _cts;
        private Task? _streamingTask;
        private bool _isPaused;

        public bool IsPaused => _isPaused;

        public event EventHandler<ServerResponse> OutputReceived;
        public event EventHandler<string>? ErrorReceived;
        public event EventHandler? ProcessExited;

        public StreamingGrpcProcessManager(SpeechRecognizerGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
        }
        
        public async Task StartContinuousStreamingAsync(CancellationToken token)
        {
            try
            {
                await _grpcClient.StreamRecognizeAsync(
                    ReadAudioChunksAsync(token),
                    (response) => 
                    {
                        var serverResponse = new ServerResponse
                        {
                            Text = response.Text,
                            Answer = response.Answer,
                            Intent = response.Intent,
                            Confidence = response.Confidence
                        };

                        if (_syncContext != null)
                        {
                            _syncContext.Post(_ => OutputReceived?.Invoke(this, serverResponse), null);
                        }
                        else
                        {
                            OutputReceived?.Invoke(this, serverResponse);
                        }
                    },
                    token
                );
            }
            catch (OperationCanceledException ex)
            {
                ErrorReceived?.Invoke(this, $"Помилка операції: {ex.Message}");
            }
            catch (Exception ex)
            {
                ErrorReceived?.Invoke(this, $"Помилка потоку: {ex.Message}");
            }
        }
        private async IAsyncEnumerable<byte[]> ReadAudioChunksAsync(
            [EnumeratorCancellation] CancellationToken token)
        {
            while (await _audioChannel.Reader.WaitToReadAsync(token))
            {
                while (_audioChannel.Reader.TryRead(out var chunk))
                {
                    yield return chunk;
                }
            }
        }
        public void StartProcess()
        {
            if (_streamingTask != null && !_streamingTask.IsCompleted) return;

            _audioChannel = Channel.CreateUnbounded<byte[]>();
            _cts = new CancellationTokenSource();
            _streamingTask = StartContinuousStreamingAsync(_cts.Token);
        }
        public void StopProcess()
        {
            _cts?.Cancel();
            _audioChannel.Writer.TryComplete();

            _cts?.Dispose();
            _cts = null;
            _streamingTask = null;
        }
        public async Task SendAudioAsync(byte[] audioData)
        {
            await _audioChannel.Writer.WriteAsync(audioData);
        }
        public void Dispose()
        {
            StopProcess();
            GC.SuppressFinalize(this);
        }

    }
}
