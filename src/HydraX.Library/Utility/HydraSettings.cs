using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HydraX.Library
{
    public class HydraSettings
    {
        /// <summary>
        /// Setting Values
        /// </summary>
        private Dictionary<string, string> Values = new Dictionary<string, string>();

        /// <summary>
        /// Gets the setting with the given name, if not found, returns defaultVal
        /// </summary>
        public string this[string key, string defaultVal]
        {
            get
            {
                if(!Values.TryGetValue(key, out var val))
                {
                    val = defaultVal;
                    Values[key] = val;
                }

                return val;
            }
        }

        /// <summary>
        /// Sets the setting with the given name
        /// </summary>
        public string this[string key]
        {
            set
            {
                Values[key] = value;
            }
        }

        /// <summary>
        /// Initializes an instance of the Settings Class
        /// </summary>
        public HydraSettings() { }

        /// <summary>
        /// Initializes an instance of the Settings Class and loads the settings
        /// </summary>
        /// <param name="fileName">File Name</param>
        public HydraSettings(string fileName)
        {
            Load(fileName);
        }

        /// <summary>
        /// Loads Settings from a file
        /// </summary>
        /// <param name="fileName">File Name</param>
        public void Load(string fileName)
        {
            try
            {
                Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(fileName));
            }
            catch
            {
                Save(fileName);
            }
        }

        /// <summary>
        /// Saves all settings to a file
        /// </summary>
        /// <param name="fileName">File Name</param>
        public void Save(string fileName)
        {
            try
            {
                using (JsonTextWriter output = new JsonTextWriter(new StreamWriter(fileName)))
                {
                    output.Formatting = Formatting.Indented;
                    output.Indentation = 4;
                    output.IndentChar = ' ';

                    JsonSerializer serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };

                    serializer.Serialize(output, Values);

                }
            }
            catch
            {
                return;
            }
        }
    }
}
