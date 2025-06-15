using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Features.Commands
{
    public class CommandServiceProcessAdapter : ICommandServiceAdapter
    {
        private readonly string _commandServicePath = "\"C:\\Users\\lera6\\Documents\\Навчання\\3 курс\\ТП\\Курсова\\VoiceAssistant Stella\\CommandServiceApp\\bin\\Debug\\net8.0\\CommandServiceApp.exe\"";
        public async Task<string?> ExecuteCommandAsync(string recognizedText)
        {
            return await SendCommandAsync(recognizedText);
        }
        private async Task<string?> SendCommandAsync(string command)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _commandServicePath,
                    Arguments = $"\"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };

                using var process = new Process { StartInfo = startInfo };
                var outputBuilder = new StringBuilder();
                bool isUnrecognized = false;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        if(args.Data.Contains("Команда не розпізнана", StringComparison.OrdinalIgnoreCase))
                        {
                            isUnrecognized = true;
                        }
                        else outputBuilder.AppendLine(args.Data);
                    }
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        Debug.WriteLine($"[STDERR]: {args.Data}");
                };

                process.Start();
                process.BeginOutputReadLine();

                await process.WaitForExitAsync();

                if (isUnrecognized) return null;

                string result = outputBuilder.ToString().Trim();
                Debug.WriteLine($"[CommandService Execute] {command}");
                Debug.WriteLine($"[CommandService Output] {result}");

                return result;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"[CommandService Error] {ex.Message}");
                return "Помилка виконання команди.";
            }
        }
    }
}
