using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherBotService
{
    public class WeatherGenerator
    {
        

        public WeatherGenerator()
        {
            var phase = GetPhase(DateTime.UtcNow);
            var season = GetSeason(DateTime.UtcNow);
            Effect = GenerateWeather(phase, season);
            _random = new Random();

            // Read JSON from weather.txt and store the JObject in _weatherSet.
            using (var file = File.OpenText(@"C:\WayfarersWeather\weather.txt"))
            {
                using (var reader = new JsonTextReader(file))
                {
                    _weatherSet = (JObject) JToken.ReadFrom(reader);
                }
            }
        }

        private readonly JObject _weatherSet;
        private readonly Random _random;
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
            var seasonRoll = _random.Next(1, 3);
            var timeRoll = _random.Next(1, 3);

            if (seasonRoll == 1)
            {
                if (timeRoll == 1)
                    return (string) _weatherSet["any_season"]["any_time"][
                        _random.Next(0, _weatherSet["any_season"]["any_time"].Count())];

                switch (phase)
                {
                    case Phase.Dawn:
                        return (string) _weatherSet["any_season"]["dawn"][
                            _random.Next(0, _weatherSet["any_season"]["dawn"].Count())];
                    case Phase.Day:
                        return (string) _weatherSet["any_season"]["day"][
                            _random.Next(0, _weatherSet["any_season"]["day"].Count())];
                    case Phase.Dusk:
                        return (string) _weatherSet["any_season"]["dusk"][
                            _random.Next(0, _weatherSet["any_season"]["dusk"].Count())];
                    case Phase.Night:
                        return (string) _weatherSet["any_season"]["night"][
                            _random.Next(0, _weatherSet["any_season"]["night"].Count())];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                }
            }

            switch (season)
            {
                case Season.Winter:
                    if (timeRoll == 1)
                        return (string) _weatherSet["winter"]["any_time"][
                            _random.Next(0, _weatherSet["winter"]["any_time"].Count())];

                    switch (phase)
                    {
                        case Phase.Dawn:
                            return (string) _weatherSet["winter"]["dawn"][
                                _random.Next(0, _weatherSet["winter"]["dawn"].Count())];
                        case Phase.Day:
                            return (string) _weatherSet["winter"]["day"][
                                _random.Next(0, _weatherSet["winter"]["day"].Count())];
                        case Phase.Dusk:
                            return (string) _weatherSet["winter"]["dusk"][
                                _random.Next(0, _weatherSet["winter"]["dusk"].Count())];
                        case Phase.Night:
                            return (string) _weatherSet["winter"]["night"][
                                _random.Next(0, _weatherSet["winter"]["night"].Count())];
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                case Season.Spring:
                    if (timeRoll == 1)
                        return (string) _weatherSet["spring"]["any_time"][
                            _random.Next(0, _weatherSet["spring"]["any_time"].Count())];

                    switch (phase)
                    {
                        case Phase.Dawn:
                            return (string) _weatherSet["spring"]["dawn"][
                                _random.Next(0, _weatherSet["spring"]["dawn"].Count())];
                        case Phase.Day:
                            return (string) _weatherSet["spring"]["day"][
                                _random.Next(0, _weatherSet["spring"]["day"].Count())];
                        case Phase.Dusk:
                            return (string) _weatherSet["spring"]["dusk"][
                                _random.Next(0, _weatherSet["spring"]["dusk"].Count())];
                        case Phase.Night:
                            return (string) _weatherSet["spring"]["night"][
                                _random.Next(0, _weatherSet["spring"]["night"].Count())];
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                case Season.Summer:
                    if (timeRoll == 1)
                        return (string) _weatherSet["summer"]["any_time"][
                            _random.Next(0, _weatherSet["summer"]["any_time"].Count())];

                    switch (phase)
                    {
                        case Phase.Dawn:
                            return (string) _weatherSet["summer"]["dawn"][
                                _random.Next(0, _weatherSet["summer"]["dawn"].Count())];
                        case Phase.Day:
                            return (string) _weatherSet["summer"]["day"][
                                _random.Next(0, _weatherSet["summer"]["day"].Count())];
                        case Phase.Dusk:
                            return (string) _weatherSet["summer"]["dusk"][
                                _random.Next(0, _weatherSet["summer"]["dusk"].Count())];
                        case Phase.Night:
                            return (string) _weatherSet["summer"]["night"][
                                _random.Next(0, _weatherSet["summer"]["night"].Count())];
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                case Season.Autumn:
                    if (timeRoll == 1)
                        return (string) _weatherSet["autumn"]["any_time"][
                            _random.Next(0, _weatherSet["autumn"]["any_time"].Count())];

                    switch (phase)
                    {
                        case Phase.Dawn:
                            return (string) _weatherSet["autumn"]["dawn"][
                                _random.Next(0, _weatherSet["autumn"]["dawn"].Count())];
                        case Phase.Day:
                            return (string) _weatherSet["autumn"]["day"][
                                _random.Next(0, _weatherSet["autumn"]["day"].Count())];
                        case Phase.Dusk:
                            return (string) _weatherSet["autumn"]["dusk"][
                                _random.Next(0, _weatherSet["autumn"]["dusk"].Count())];
                        case Phase.Night:
                            return (string) _weatherSet["autumn"]["night"][
                                _random.Next(0, _weatherSet["autumn"]["night"].Count())];
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(season), season,
                        @"An unexpected value was given.");
            }
        }
    }
}