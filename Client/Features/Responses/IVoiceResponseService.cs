using System.Threading.Tasks;

namespace VoiceAssistant.Features.Responses
{
    public interface IVoiceResponseService
    {
        bool IsDuplicate(string answer);

        Task RespondConditionally(string text);
    }
}
