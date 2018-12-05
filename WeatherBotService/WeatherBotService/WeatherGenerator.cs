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
        public void GenerateWeather()
        {
            var dateTime = DateTime.UtcNow;

            var phase = SetPhase(dateTime);
            var season = SetSeason(dateTime);

            var random = new Random();
            var seasonRoll = random.Next(1, 101);
            var timeRoll = random.Next(1, 101);
            
            if (seasonRoll >= 1 && seasonRoll <= 25) // 25% chance of generic weather with regards to season
            {
                JArray array;
                string selection;

                if (timeRoll >= 1 && timeRoll <= 25) // 25% chance of generic weather with regards to time of day
                {
                    // First create an array out of the JSON string for the appropriate season/phase.
                    array = new JArray(_weatherSet["any_season"]["any_time"]);

                    // Next, select a random entry from the array and convert that JToken to a string.
                    selection = array[random.Next(0, array.Count)].ToString();

                    // Finally, trim the brackets, quotes, and whitespace from the string and set it to Effect.
                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                }
                else
                {
                    switch (phase)
                    {
                        case Phase.Dawn:
                            array = new JArray(_weatherSet["any_season"]["dawn"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Day:
                            array = new JArray(_weatherSet["any_season"]["day"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Dusk:
                            array = new JArray(_weatherSet["any_season"]["dusk"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        case Phase.Night:
                            array = new JArray(_weatherSet["any_season"]["night"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                    }
                }
            }
            else
            {
                JArray array;
                string selection;

                switch (season)
                {
                    case Season.Winter:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["winter"]["any_time"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["winter"]["dawn"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["winter"]["day"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["winter"]["dusk"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["winter"]["night"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Spring:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["spring"]["any_time"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["spring"]["dawn"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["spring"]["day"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["spring"]["dusk"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["spring"]["night"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Summer:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["summer"]["any_time"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["summer"]["dawn"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["summer"]["day"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["summer"]["dusk"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["summer"]["night"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
                            }
                        }
                        break;

                    case Season.Autumn:
                        if (timeRoll >= 1 && timeRoll <= 20)
                        {
                            array = new JArray(_weatherSet["autumn"]["any_time"]);
                            selection = array[random.Next(0, array.Count)].ToString();
                            Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                        }
                        else
                        {
                            switch (phase)
                            {
                                case Phase.Dawn:
                                    array = new JArray(_weatherSet["autumn"]["dawn"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Day:
                                    array = new JArray(_weatherSet["autumn"]["day"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Dusk:
                                    array = new JArray(_weatherSet["autumn"]["dusk"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
                                    break;
                                case Phase.Night:
                                    array = new JArray(_weatherSet["autumn"]["night"]);
                                    selection = array[random.Next(0, array.Count)].ToString();
                                    Effect = selection.Remove(selection.Length - 3).Substring(3).Trim().TrimEnd('"').TrimStart('"');
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