using System;

namespace VoiceAssistant.Features.ErrorHandling
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
        void HandleProcessError(string errorData);
    }
}
