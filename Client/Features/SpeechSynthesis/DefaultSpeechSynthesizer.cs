using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Media;

namespace VoiceAssistant.Features.SpeechSynthesis
{
    public class DefaultSpeechSynthesizer:ISpeechSynthesizer
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUrl;
        public DefaultSpeechSynthesizer(string serviceUrl)
        {
            _httpClient = new HttpClient();
            _serviceUrl = serviceUrl;
        }
        public async Task SpeakAsync(string text)
        {
            try
            {
                var url = $"{_serviceUrl}?text={Uri.EscapeDataString(text)}";
                var response = await _httpClient.PostAsync(url,null);

                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Error: {response.StatusCode}");
                    return;
                }

                var audioData = await response.Content.ReadAsByteArrayAsync();
                var tempFilePath = Path.GetTempFileName()+".wav";
                await File.WriteAllBytesAsync(tempFilePath, audioData);

                using var player=new SoundPlayer(tempFilePath);
                player.Play();

                // удаляем позже, чтобы не мешало
                _ = Task.Delay(5000).ContinueWith(_ => File.Delete(tempFilePath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SpeakAsync: {ex.Message}");
            }
        }
        public void Stop()
        {
            
        }
    }
}
