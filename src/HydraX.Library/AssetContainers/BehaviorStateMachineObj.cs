using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// Generic Behavior State Machine Object
    /// </summary>
    public class BehaviorStateMachineObj
    {
        /// <summary>
        /// Connection Obj
        /// </summary>
        public class ConnectionObj
        {
            [JsonProperty("toState")]
            [DefaultValue("")]
            public string ToState { get; set; }
            [JsonProperty("conditionFunction")]
            [DefaultValue("")]
            public string ConditionFunction { get; set; }
            [JsonProperty("minRunningStateTime")]
            [DefaultValue(0)]
            public int MinRunningStateTime { get; set; }
            [JsonProperty("waitTillStateFinish")]
            [DefaultValue(false)]
            public bool WaitTillStateFinish { get; set; }
            [JsonProperty("evaluateOnStateFailure")]
            [DefaultValue(false)]
            public bool EvaluateOnStateFailure { get; set; }
            [JsonProperty("evaluateDuringTransition")]
            [DefaultValue(false)]
            public bool EvaluateDuringTransition { get; set; }
        }

        /// <summary>
        /// Behavior State Data
        /// </summary>
        public class StateObj
        {
            [JsonProperty("name")]
            [DefaultValue("")]
            public string Name { get; set; }
            [JsonProperty("asmStateName")]
            [DefaultValue("")]
            public string AnimationStateName { get; set; }
            [JsonProperty("entryPointCondition")]
            [DefaultValue("")]
            public string EntryPointCondition { get; set; }
            [JsonProperty("startFunction")]
            [DefaultValue("")]
            public string StartFunction { get; set; }
            [JsonProperty("updateFunction")]
            [DefaultValue("")]
            public string UpdateFunction { get; set; }
            [JsonProperty("terminateFunction")]
            [DefaultValue("")]
            public string TerminateFunction { get; set; }
            [JsonProperty("plannerFunction")]
            [DefaultValue("")]
            public string PlannerFunction { get; set; }
            [JsonProperty("actionNotify")]
            [DefaultValue("")]
            public string ActionNotify { get; set; }

            [JsonProperty("wildcard")]
            [DefaultValue(false)]
            public bool Wildcard { get; set; }
            [JsonProperty("evaluateStateForASM")]
            [DefaultValue(false)]
            public bool EvaluateStateForASM { get; set; }
            [JsonProperty("plannerstate")]
            [DefaultValue(false)]
            public bool PlannerState { get; set; }
            [JsonProperty("terminalState")]
            [DefaultValue(false)]
            public bool TerminalState { get; set; }
            [JsonProperty("entryState")]
            [DefaultValue(false)]
            public bool EntryState { get; set; }

            [JsonProperty("connections", Order = 2)]
            public ConnectionObj[] Connections;
        }

        /// <summary>
        /// Animation States
        /// </summary>
        [JsonProperty("states")]
        public StateObj[] States { get; set; }

        /// <summary>
        /// Saves the Animation State Machine to a formatted JSON file
        /// </summary>
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
