using System;

namespace VoiceAssistant.Features.ProcessInterop
{
    public enum MessageType
    {
        Output,
        Error,
        Status,
        Log
    }
    public class ProcessMessageEventArgs:EventArgs
    {
        public string Message { get; }

        public DateTime Timestamp { get; }

        public MessageType Type { get; }

        public ProcessMessageEventArgs(string message, MessageType type = MessageType.Output)
        {
            Message = message;
            Timestamp = DateTime.Now;
            Type = type;
        }
    }
}
