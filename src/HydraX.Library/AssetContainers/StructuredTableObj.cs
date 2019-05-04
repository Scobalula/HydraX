using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// Structured Table Object
    /// </summary>
    public class StructuredTableObj
    {
        /// <summary>
        /// Meta Data (Not used by Linker)
        /// </summary>
        [JsonProperty(PropertyName = "_meta")]
        public Dictionary<string, object> Meta = new Dictionary<string, object>()
        {
            { "Exported via", "HydraX by Scobalula" }
        };

        /// <summary>
        /// Structured Data
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public Dictionary<string, object>[] Data { get; set; }

        /// <summary>
        /// Initializes Structured Table Obj and preallocates Entries
        /// </summary>
        /// <param name="entryCount">Number of Entries</param>
        public StructuredTableObj(int entryCount)
        {
            Data = new Dictionary<string, object>[entryCount];
        }

        /// <summary>
        /// Saves Structured Table to a JSON file
        /// </summary>
        /// <param name="path">File Path</param>
        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (JsonTextWriter output = new JsonTextWriter(new StreamWriter(path)))
            {
                output.Formatting = Formatting.Indented;
                output.Indentation = 4;
                output.IndentChar = ' ';

                JsonSerializer serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                serializer.Serialize(output, this);
            }
        }
    }
}
