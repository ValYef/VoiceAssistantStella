
namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class CommandList : BaseCommand
    {
        public override CommandCategory Category => CommandCategory.Info;

        protected override List<string> Keywords => new List<string>()
        {
            "які команд",
            "що ти вмієш",
            "список команд",
            "що ти можеш зробити",
            "доступні команд",
            "які є команд",
            "покажи команд",
            "вмієш робити",
        };

        public override string Execute()
        {
            return
                "Доступні голосові команди:\r\n• Відкрий браузер\r\n• Відкрий калькулятор\r\n• Відкрий Телеграм\r\n• Відкрий Ютуб\r\n• Який сьогодні день?\r\n\r\nСкажіть одну з фраз — і я виконаю команду ";
        }
    }
}
