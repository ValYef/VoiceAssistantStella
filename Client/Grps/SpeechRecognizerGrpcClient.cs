using Grpc.Net.Client;
using System.Threading.Tasks;
using Grps.Proto;
using Grpc.Core;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Diagnostics;

namespace VoiceAssistant.Grps
{
    public class SpeechRecognizerGrpcClient
    {
        private readonly SpeechRecognizer.SpeechRecognizerClient _client;

        public SpeechRecognizerGrpcClient(string serverAddress = "http://localhost:50051")
        {
            var channel=GrpcChannel.ForAddress(serverAddress);
            _client=new SpeechRecognizer.SpeechRecognizerClient(channel);
        }
        public async Task StreamRecognizeAsync(IAsyncEnumerable<byte[]> audioChunks, Action<string> onTextReceived, CancellationToken cancellationToken = default)
        {
            using var call = _client.StreamRecognize();

            Debug.WriteLine("[GRPC] Stream started");

            var responseReaderTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
                    {

                        if (!string.IsNullOrEmpty(response.Text))
                        {
                            onTextReceived(response.Text);
                        }
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled && cancellationToken.IsCancellationRequested)
                {
                    Debug.WriteLine("[GRPC] Потік відмінено по запросу користувача(-ки) (очікувано)");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Помилка читання відповіді: {ex.Message}");
                }
            }, cancellationToken);

            try
            {
                await foreach (var chunk in audioChunks.WithCancellation(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    await call.RequestStream.WriteAsync(new AudioChunk
                    {
                        AudioData = Google.Protobuf.ByteString.CopyFrom(chunk)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка відправки аудіо: {ex.Message}");
            }

            try
            {
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка при завершені потока: {ex.Message}");
            }

            await responseReaderTask;
        }
    }
}
