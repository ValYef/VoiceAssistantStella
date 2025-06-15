using VoiceAssistant.Classes.Commands;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        var commandExecutor = new CommandExecutor();

        if (args.Length == 0)
        {
            Console.WriteLine("Будь ласка, введіть команду як аргумент.");
            return;
        }

        string input = string.Join(" ", args);
        var result = commandExecutor.ExecuteCommand(input);

        if (!string.IsNullOrWhiteSpace(result))
        {
            Console.WriteLine(result);
        }
    }
}