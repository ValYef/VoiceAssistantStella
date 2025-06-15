using System;
using System.Collections.Generic;

namespace VoiceAssistant.Features.Audio.AudioPlayer
{
    public interface IAudioPlayer
    {
        void Play(string filePath);
        void PlayRandom(List<string> soundFiles);
    }
}
