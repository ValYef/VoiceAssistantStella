using System.Threading.Tasks;

namespace VoiceAssistant.Features.Commands
{
    public interface ICommandServiceAdapter
    {
        Task<string?> ExecuteCommandAsync(string recognizedText);
    }
}
