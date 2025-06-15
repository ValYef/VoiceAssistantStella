using System.Diagnostics;

namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class OpenCalculatorCommand:BaseCommand
    {
        public override CommandCategory Category => CommandCategory.LaunchApp;
        protected override List<string> Keywords => new List<string> 
        { 
            "калькулятор", 
            "відкрий калькулятор" ,
            "кальк"
        };
        public override string Execute()
        {
            try
            {
                Process.Start(new ProcessStartInfo("calc")
                {
                    UseShellExecute = true
                });
                return "Відкриваю калькулятор...";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Виникла помилка під час відкривання калькулятора";
            }
        }
    }
}
