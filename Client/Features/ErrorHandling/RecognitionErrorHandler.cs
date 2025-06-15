using System;
using System.Diagnostics;

namespace VoiceAssistant.Features.ErrorHandling
{
    public class RecognitionErrorHandler : IErrorHandler
    {
        public void HandleError(Exception ex)
        {
            Debug.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} - {ex.GetType().Name}: {ex.Message}");
        }

        public void HandleProcessError(string errorData)
        {
            Debug.WriteLine($"[PROCESS ERROR] {DateTime.Now:HH:mm:ss} - {errorData}");
        }
    }
}
