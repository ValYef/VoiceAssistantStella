using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Features.Audio.AudioPlayer
{
    public class AudioPlayer:IAudioPlayer
    {
        public static string AudioFileFolder { get; } = Path.Combine(AppContext.BaseDirectory, "Features", "Responses","Files");
        private readonly Random _random = new Random();
        public void Play(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine($"Audio file {filePath} didn`t found ");
                }

                using var player = new SoundPlayer(filePath);
                player.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing audio file: {ex.Message}");
            }
        }

        public void PlayRandom(List<string> soundFiles)
        {
            var selectedFile = soundFiles[_random.Next(soundFiles.Count)];
            var filePath = Path.Combine(AudioFileFolder, selectedFile);
            Play(filePath);
        }
    }
}
