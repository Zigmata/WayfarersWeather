using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherBotService
{
    public class WeatherGenerator
    {
        

        public WeatherGenerator()
        {
            var dateTime = DateTime.UtcNow;

            _phase = GetPhase(dateTime);
            _season = GetSeason(dateTime);

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
        private readonly Random _random = new Random();
        public string Effect { get; private set; }
        private readonly Phase _phase;
        private readonly Season _season;

        private Phase GetPhase(DateTime dateTime)
        {
            var hour = dateTime.Hour;

            // --- Comment this section when dawn/dusk is implemented ---
            if (hour < 6 || hour > 19)
                return Phase.Night;
            
            return Phase.Day;
            // --- End section ---
            
            // Uncomment when bot evolves to had dawn/dusk specific time settings as well
            // This was removed for now due to the bot being on a static 6-hour cycle,
            // preventing these phases from being reached.

            //if (hour < 7 || hour > 21)
            //    return Phase.Night;
            //if (hour == 7)
            //    return Phase.Dawn;
            //if (hour > 7 && hour < 21)
            //    return Phase.Day;
            //if (hour == 21)
            //    return Phase.Dusk;
            //throw new ArgumentOutOfRangeException(nameof(hour), hour, @"Incorrect time reported.");
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

        public void GenerateWeather()
        {
            var seasonRoll = _random.Next(1, 101);
            var timeRoll = _random.Next(1, 101);
            
            if (seasonRoll >= 1 && seasonRoll <= 33) // 33% chance of generic weather with regards to season
            {
                JArray array;
                string selection;

                if (timeRoll >= 1 && timeRoll <= 20) // 20% chance of generic weather with regards to time of day
                {
                    array = new JArray(_weatherSet["any_season"]["any_time"]);
                    selection = array[_random.Next(0, array.Count)].ToString();
                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                }
                else
                {
                    switch (_phase)
                    {
                        case Phase.Dawn:
                            array = new JArray(_weatherSet["any_season"]["dawn"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Day:
                            array = new JArray(_weatherSet["any_season"]["day"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Dusk:
                            array = new JArray(_weatherSet["any_season"]["dusk"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Night:
                            array = new JArray(_weatherSet["any_season"]["night"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_phase), _phase, null);
                    }
                }
            }
            else
            {
                JArray array;
                string selection;

                switch (_season)
                {
                    case Season.Winter:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["winter"]["any_time"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (_phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["winter"]["dawn"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["winter"]["day"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["winter"]["dusk"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["winter"]["night"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_phase), _phase, null);
                            }
                        }

                        break;
                    case Season.Spring:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["spring"]["any_time"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (_phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["spring"]["dawn"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["spring"]["day"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["spring"]["dusk"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["spring"]["night"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_phase), _phase, null);
                            }
                        }

                        break;
                    case Season.Summer:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["summer"]["any_time"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (_phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["summer"]["dawn"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["summer"]["day"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["summer"]["dusk"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["summer"]["night"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_phase), _phase, null);
                            }
                        }

                        break;
                    case Season.Autumn:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["autumn"]["any_time"]);
                            selection = array[_random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (_phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["autumn"]["dawn"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["autumn"]["day"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["autumn"]["dusk"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["autumn"]["night"]);
                                    selection = array[_random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(_phase), _phase, null);
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_season), _season,
                            @"An unexpected value was given.");
                }
            }
        }
    }
}