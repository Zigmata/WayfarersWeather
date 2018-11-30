using System;

namespace WeatherBotService
{
    public class WeatherGenerator
    {
        public WeatherGenerator()
        {
            var phase = GetPhase(DateTime.UtcNow);
            var season = GetSeason(DateTime.UtcNow);
            Effect = GenerateWeather(phase, season);
        }

        // private readonly Random _random;

        public string Effect { get; }

        private Phase GetPhase(DateTime dateTime)
        {
            var hour = dateTime.Hour;

            if (hour < 7 || hour > 21)
                return Phase.Night;
            if (hour == 7)
                return Phase.Dawn;
            if (hour > 7 && hour < 21)
                return Phase.Day;
            if (hour == 21)
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

        private string GenerateWeather(Phase phase, Season season)
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