using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherBotService
{
    public sealed class WeatherGenerator : IDisposable
    {
        /// <summary>Field containing the JSON string of weather descriptions.</summary>
        private JObject _weatherSet;
        /// <summary>Gets the string of the generated weather description.</summary>
        public string Effect { get; private set; }
        /// <summary>Bool field determining whether or not the disposer has been called.</summary>
        private bool _disposed;
        /// <summary>SafeHandle for disposal.</summary>
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// Default constructor. Loads JSON with weather descriptions.
        /// </summary>
        public WeatherGenerator()
        {
            // Read JSON from C:\WayfarersWeather\weather.txt and store the JObject in _weatherSet.
            using (var file = File.OpenText(@"C:\WayfarersWeather\weather.txt"))
            {
                using (var reader = new JsonTextReader(file))
                {
                    _weatherSet = (JObject) JToken.ReadFrom(reader);
                }
            }
        }

        // Set the phase of day for the weather.
        /// <summary>
        /// Sets the phase of day for the weather.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> object for determining phase of day.</param>
        /// <returns></returns>
        private Phase SetPhase(DateTime dateTime)
        {
            var hour = dateTime.Hour;

            //// --- Comment this section when dawn/dusk is implemented ---
            //if (hour < 6 || hour > 19)
            //    return Phase.Night;

            //return Phase.Day;
            //// --- End section ---

            if (hour < 6 || hour >= 20)
                return Phase.Night;
            if (hour == 6)
                return Phase.Dawn;
            if (hour >= 7 && hour < 18)
                return Phase.Day;
            if (hour == 18 || hour == 19)
                return Phase.Dusk;
            throw new ArgumentOutOfRangeException(nameof(hour), hour, @"Incorrect time reported.");
        }

        // Set the season for the weather.
        /// <summary>
        /// Sets the season for the weather.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> object for determining season.</param>
        /// <returns></returns>
        private Season SetSeason(DateTime dateTime)
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

        // Randomly generates a new weather effect.
        /// <summary>
        /// Randomly generates a new weather effect.
        /// </summary>
        public void GenerateWeather(DateTime time)
        {
            var phase = SetPhase(time);
            var season = SetSeason(time);

            var random = new Random();
            var seasonRoll = random.Next(1, 101);
            var timeRoll = random.Next(1, 101);
            
            if (seasonRoll >= 1 && seasonRoll <= 25) // 25% chance of generic weather with regards to season
            {
                JArray array;

                if (timeRoll >= 1 && timeRoll <= 25) // 25% chance of generic weather with regards to time of day
                {
                    // Create an array out of the JSON string for the appropriate season/phase.
                    array = (JArray) _weatherSet["any_season"]["any_time"];

                    // Select a random entry from the array, convert that JToken to a string, and set it as the current Effect.
                    Effect = array[random.Next(0, array.Count)].ToString();
                }
                else
                {
                    switch (phase)
                    {
                        case Phase.Dawn:
                            array = (JArray) _weatherSet["any_season"]["dawn"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                            break;
                        case Phase.Day:
                            array = (JArray) _weatherSet["any_season"]["day"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                            break;
                        case Phase.Dusk:
                            array = (JArray) _weatherSet["any_season"]["dusk"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                            break;
                        case Phase.Night:
                            array = (JArray) _weatherSet["any_season"]["night"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                }
            }
            else
            {
                JArray array;

                switch (season)
                {
                    case Season.Winter:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = (JArray) _weatherSet["winter"]["any_time"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = (JArray) _weatherSet["winter"]["dawn"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Day:
                                    array = (JArray) _weatherSet["winter"]["day"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Dusk:
                                    array = (JArray) _weatherSet["winter"]["dusk"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Night:
                                    array = (JArray) _weatherSet["winter"]["night"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Spring:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = (JArray) _weatherSet["spring"]["any_time"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = (JArray) _weatherSet["spring"]["dawn"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Day:
                                    array = (JArray) _weatherSet["spring"]["day"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Dusk:
                                    array = (JArray) _weatherSet["spring"]["dusk"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Night:
                                    array = (JArray) _weatherSet["spring"]["night"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Summer:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = (JArray) _weatherSet["summer"]["any_time"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = (JArray) _weatherSet["summer"]["dawn"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Day:
                                    array = (JArray) _weatherSet["summer"]["day"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Dusk:
                                    array = (JArray) _weatherSet["summer"]["dusk"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Night:
                                    array = (JArray) _weatherSet["summer"]["night"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Autumn:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = (JArray) _weatherSet["autumn"]["any_time"];
                            Effect = array[random.Next(0, array.Count)].ToString();
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = (JArray) _weatherSet["autumn"]["dawn"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Day:
                                    array = (JArray) _weatherSet["autumn"]["day"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Dusk:
                                    array = (JArray) _weatherSet["autumn"]["dusk"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                case Phase.Night:
                                    array = (JArray) _weatherSet["autumn"]["night"];
                                    Effect = array[random.Next(0, array.Count)].ToString();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(season), season,
                            @"An unexpected value was given.");
                }
            }
        }

        // Disposal method for unmanaged objects.
        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            _handle.Dispose();
            _weatherSet = null;
            Effect = null;

            _disposed = true;
        }
    }
}