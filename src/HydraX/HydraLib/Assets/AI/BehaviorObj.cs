using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    /// <summary>
    /// Generic Behavior Tree Object
    /// </summary>
    public class BehaviorObj
    {
        /// <summary>
        /// Animation Selector
        /// </summary>
        [JsonProperty("id")]
        [DefaultValue("")]
        public string Name { get; set; }

        /// <summary>
        /// Behavior Type
        /// </summary>
        [JsonProperty("type")]
        [DefaultValue("")]
        public string Type { get; set; }

        /// <summary>
        /// Action Name
        /// </summary>
        [JsonProperty("ActionName")]
        [DefaultValue("")]
        public string ActionName { get; set; }

        /// <summary>
        /// Animation State Machine State Name
        /// </summary>
        [JsonProperty("ASMStateName")]
        [DefaultValue("")]
        public string ASMStateName { get; set; }

        /// <summary>
        /// Action Notification
        /// </summary>
        [JsonProperty("actionNotify")]
        [DefaultValue("")]
        public string ActionNotify { get; set; }

        /// <summary>
        /// Start Function
        /// </summary>
        [JsonProperty("StartFunction")]
        [DefaultValue("")]
        public string StartFunction { get; set; }

        /// <summary>
        /// Terminate Function
        /// </summary>
        [JsonProperty("TerminateFunction")]
        [DefaultValue("")]
        public string TerminateFunction { get; set; }

        /// <summary>
        /// Update Function
        /// </summary>
        [JsonProperty("UpdateFunction")]
        [DefaultValue("")]
        public string UpdateFunction { get; set; }

        /// <summary>
        /// Looping Action
        /// </summary>
        [JsonProperty("loopingAction")]
        [DefaultValue(0)]
        public int? LoopingAction { get; set; }

        /// <summary>
        /// Max Action Time
        /// </summary>
        [JsonProperty("actionTimeMax")]
        [DefaultValue(0)]
        public int? ActionTimeMax { get; set; }

        /// <summary>
        /// Script Function
        /// </summary>
        [JsonProperty("scriptFunction")]
        [DefaultValue("")]
        public string ScriptFunction { get; set; }

        /// <summary>
        /// Interruption Name
        /// </summary>
        [JsonProperty("interruptName")]
        [DefaultValue("")]
        public string InterruptName { get; set; }

        /// <summary>
        /// Chance Percentage
        /// </summary>
        [JsonProperty("percentChance")]
        [DefaultValue(0.0f)]
        public float? PercentChance { get; set; }

        /// <summary>
        /// Min Cool Down
        /// </summary>
        [JsonProperty("cooldownMin")]
        [DefaultValue(0)]
        public int? CoolDownMin { get; set; }

        /// <summary>
        /// Max Cool Down
        /// </summary>
        [JsonProperty("cooldownMax")]
        [DefaultValue(0)]
        public int? CooldDownMax { get; set; }

        /// <summary>
        /// Child Behaviors
        /// </summary>
        [JsonProperty("children", Order = 1)]
        [DefaultValue(null)]
        public BehaviorObj[] Children;

        /// <summary>
        /// Child Indices
        /// </summary>
        [JsonIgnore]
        public int[] ChildIndices { get; set; }

        /// <summary>
        /// Saves Behavior to a formatted JSON file
        /// </summary>
        /// <param name="path">File Path</param>
        public void Save(string path)
        {
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
