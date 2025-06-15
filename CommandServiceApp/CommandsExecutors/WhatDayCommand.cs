using System.Globalization;

namespace VoiceAssistant.Classes.Commands.CommandsExecutors
{
    public class WhatDayCommand:BaseCommand
    {
        public override CommandCategory Category => CommandCategory.DateTime;
        protected override List<string> Keywords => new List<string>
        {
            "який сьогодні день",
            "яка сьогодні дата",
            "дата",
            "день",
            "сьогодні",
            "рік",
            "місяць",
        };
        public override string Execute()
        {
            var result = DateTime.Now.ToString("dddd, dd MMMM yyyy", new CultureInfo("uk-UA"));
            return result;
        }
    }
}
