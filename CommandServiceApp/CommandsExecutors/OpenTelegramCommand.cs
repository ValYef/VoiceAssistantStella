using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class OpenTelegramCommand:BaseCommand
    {
        public override CommandCategory Category => CommandCategory.LaunchApp;
        protected override List<string> Keywords => new List<string>
        {
            "відкрий телеграм",
            "телег",
            "запусти телеграм"
        };

        private string userPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Telegram Desktop",
            "Telegram.exe"
        );

        private string programFilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "Telegram Desktop",
            "Telegram.exe"
        );
        public override string Execute()
        {
            try
            {
                var packageFamilyName = "TelegramMessengerLLP.TelegramDesktop_t4vj0pshhgkwm";
                var appId = "TelegramDesktop";

                string arguments = $"shell:appsFolder\\{packageFamilyName}!{appId}";

                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = arguments,
                    UseShellExecute = true
                });

                return "Відкриваю телеграм...";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Виникла помилка під час відкривання телеграму";
            }
            
        }
    }
}
