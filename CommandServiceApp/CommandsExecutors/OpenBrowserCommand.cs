using System.Diagnostics;

namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class OpenBrowserCommand: BaseCommand
    {
        public override CommandCategory Category => CommandCategory.LaunchApp;
        protected override List<string> Keywords => new List<string>
        {
            "відкрий брау",
            "запусти брау",
            "відкрий інтернет",
            "брау",
        };
        public override string Execute()
        {
            try
            {
                Process.Start(
                     new ProcessStartInfo("https://www.google.com")
                     {
                        UseShellExecute = true
                     }
                );
                return "Відкриваю браузер...";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Виникла помилка під час відкривання браузера";
            }
        }
    }
}
