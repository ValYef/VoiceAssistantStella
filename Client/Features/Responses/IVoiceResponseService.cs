using System.Threading.Tasks;

namespace VoiceAssistant.Features.Responses
{
    public interface IVoiceResponseService
    {
        void Respond(string recognizedText);
        Task RespondSafely(string recognizedText);
    }
}
