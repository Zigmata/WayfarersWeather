using System;

namespace WeatherBotService
{
    public class WeatherPattern
    {
        public WeatherPattern()
        {
            Phase = GetPhase(DateTime);
            Season = GetSeason(DateTime);
            WeatherEffect = GenerateWeather(Season);
        }

        // private readonly Random _random;

        public Season Season { get; }
        public DateTime DateTime = DateTime.UtcNow;
        public Phase Phase { get; }
        public string WeatherEffect { get; }

        private Phase GetPhase(DateTime dateTime)
        {
            var hour = dateTime.Hour;

            if (hour < 6 || hour > 20)
                return Phase.Night;
            if (hour == 6)
                return Phase.Dawn;
            if (hour > 6 && hour < 20)
                return Phase.Day;
            if (hour == 20)
                return Phase.Dusk;
            throw new ArgumentOutOfRangeException(nameof(hour), hour, @"Incorrect time reported.");
        }

        private Season GetSeason(DateTime dateTime)
        {
            var day = dateTime.DayOfYear;

            if (day <= 78 || day >= 355)
                return Season.Winter;
            if (day >= 79 && day <= 171)
                return Season.Spring;
            if (day >= 172 && day <= 265)
                return Season.Summer;
            if (day >= 266 && day <= 354)
                return Season.Autumn;

            throw new ArgumentOutOfRangeException(nameof(day), day, @"Incorrect date reported.");
        }

        private string GenerateWeather(Season season)
        {
            switch (season)
            {
                case Season.Winter:
                    return "Fuck, it's cold!";
                case Season.Spring:
                    return "Fuck, it's nice!";
                case Season.Summer:
                    return "Fuck, it's hot!";
                case Season.Autumn:
                    return "Fuck, it's windy!";
                default:
                    throw new ArgumentOutOfRangeException(nameof(season), season, @"An unexpected value was given.");
            }
        }
    }
}