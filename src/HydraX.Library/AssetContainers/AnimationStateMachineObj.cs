using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// Generic Animation State Machine Object
    /// </summary>
    public class AnimationStateMachineObj
    {
        /// <summary>
        /// Animation State Data
        /// </summary>
        public class StateObj
        {
            [JsonIgnore]
            public string Name { get; set; }
            [JsonProperty("animation_selector")]
            [DefaultValue("")]
            public string AnimationSelector { get; set; }
            [JsonProperty("transition_decorator")]
            [DefaultValue("")]
            public string TransitionDecorator { get; set; }
            [JsonProperty("aim_selector")]
            [DefaultValue("")]
            public string AimSelector { get; set; }
            [JsonProperty("shoot_selector")]
            [DefaultValue("")]
            public string ShootSelector { get; set; }
            [JsonProperty("delta_layer_function")]
            [DefaultValue("")]
            public string DeltaLayerFunction { get; set; }
            [JsonProperty("transdec_layer_function")]
            [DefaultValue("")]
            public string TransDecLayerFunction { get; set; }
            [JsonProperty("asm_client_notify")]
            [DefaultValue("")]
            public string ASMClientNotify { get; set; }
            [JsonProperty("loopsync")]
            [DefaultValue(false)]
            public bool LoopSync { get; set; }
            [JsonProperty("cleanloop")]
            [DefaultValue(false)]
            public bool CleanLoop { get; set; }
            [JsonProperty("multipledelta")]
            [DefaultValue(false)]
            public bool MultipleDelta { get; set; }
            [JsonProperty("terminal")]
            [DefaultValue(false)]
            public bool Terminal { get; set; }
            [JsonProperty("parametric2d")]
            [DefaultValue(false)]
            public bool Parametric2D { get; set; }
            [JsonProperty("animdrivenlocomotion")]
            [DefaultValue(false)]
            public bool AnimDrivenLocmotion { get; set; }
            [JsonProperty("coderate")]
            [DefaultValue(false)]
            public bool Coderate { get; set; }
            [JsonProperty("speedblend")]
            [DefaultValue(false)]
            public bool SpeedBlend { get; set; }
            [JsonProperty("allow_transdec_aim")]
            [DefaultValue(false)]
            public bool AllowTransDecAim { get; set; }
            [JsonProperty("force_fire")]
            [DefaultValue(false)]
            public bool ForceFire { get; set; }
            [JsonProperty("requires_ragdoll_notetrack")]
            [DefaultValue(false)]
            public bool RequiresRagdollNote { get; set; }
            [JsonProperty("delta_requires_translation")]
            [DefaultValue(false)]
            public bool DeltaRequiresTranslation { get; set; }
            [JsonProperty("transitions", Order = 2)]
            public Dictionary<string, StateObj> Transitions;
            [JsonProperty("substates")]
            public Dictionary<string, StateObj> SubStates;
        }

        /// <summary>
        /// Animation States
        /// </summary>
        [JsonProperty("states")]
        public Dictionary<string, StateObj> RootStates = new Dictionary<string, StateObj>();

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
