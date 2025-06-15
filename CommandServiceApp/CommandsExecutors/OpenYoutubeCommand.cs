using System.Diagnostics;

namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class OpenYoutubeCommand: BaseCommand
    {
        public override CommandCategory Category => CommandCategory.LaunchApp;
        protected override List<string> Keywords => new List<string>
        {
            "відкрий ют",
            "ютуб"
        };
        public override string Execute()
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://www.youtube.com")
                {
                    UseShellExecute = true
                });
                return "Відкриваю ютуб...";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Виникла помилка під час відкривання ютубу";
            }        
        }
    }
}
