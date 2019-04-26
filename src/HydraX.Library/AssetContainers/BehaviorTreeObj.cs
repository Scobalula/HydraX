using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// Generic Behavior Tree Object
    /// </summary>
    public class BehaviorObj
    {
        [JsonProperty("id")]
        [DefaultValue("")]
        public string Name { get; set; }
        [JsonProperty("type")]
        [DefaultValue("")]
        public string Type { get; set; }
        [JsonProperty("ActionName")]
        [DefaultValue("")]
        public string ActionName { get; set; }
        [JsonProperty("ASMStateName")]
        [DefaultValue("")]
        public string ASMStateName { get; set; }
        [JsonProperty("actionNotify")]
        [DefaultValue("")]
        public string ActionNotify { get; set; }
        [JsonProperty("StartFunction")]
        [DefaultValue("")]
        public string StartFunction { get; set; }
        [JsonProperty("TerminateFunction")]
        [DefaultValue("")]
        public string TerminateFunction { get; set; }
        [JsonProperty("UpdateFunction")]
        [DefaultValue("")]
        public string UpdateFunction { get; set; }
        [JsonProperty("loopingAction")]
        [DefaultValue(0)]
        public int? LoopingAction { get; set; }
        [JsonProperty("actionTimeMax")]
        [DefaultValue(0)]
        public int? ActionTimeMax { get; set; }
        [JsonProperty("scriptFunction")]
        [DefaultValue("")]
        public string ScriptFunction { get; set; }
        [JsonProperty("interruptName")]
        [DefaultValue("")]
        public string InterruptName { get; set; }
        [JsonProperty("percentChance")]
        [DefaultValue(0.0f)]
        public float? PercentChance { get; set; }
        [JsonProperty("cooldownMin")]
        [DefaultValue(0)]
        public int? CoolDownMin { get; set; }
        [JsonProperty("cooldownMax")]
        [DefaultValue(0)]
        public int? CooldDownMax { get; set; }
        [JsonProperty("children", Order = 1)]
        [DefaultValue(null)]
        public BehaviorObj[] Children;
        [JsonIgnore]
        public int[] ChildIndices { get; set; }

        /// <summary>
        /// Saves Behavior to a formatted JSON file
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
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };

                serializer.Serialize(output, this);

            }
        }
    }
}
