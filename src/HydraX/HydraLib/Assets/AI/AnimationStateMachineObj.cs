using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HydraLib.Assets
{
    /// <summary>
    /// Generic Animation State Machine Object
    /// </summary>
    public class AnimationStateMachineObj
    {
        /// <summary>
        /// Animation State Data
        /// </summary>
        public class AnimationStateObj
        {
            /// <summary>
            /// Transition Data
            /// </summary>
            public class TransitionObj
            {
                /// <summary>
                /// Animation Selector
                /// </summary>
                [JsonIgnore]
                public string Name { get; set; }

                /// <summary>
                /// Animation Selector
                /// </summary>
                [JsonProperty("animation_selector")]
                [DefaultValue("")]
                public string AnimationSelector { get; set; }

                /// <summary>
                /// Creates a Transition Object
                /// </summary>
                /// <param name="name">Transition Name</param>
                /// <param name="animSelector">Transition Animation Selector</param>
                public TransitionObj(string name, string animSelector)
                {
                    Name = name;
                    AnimationSelector = animSelector;
                }
            }

            /// <summary>
            /// Animation Selector
            /// </summary>
            [JsonIgnore]
            public string Name { get; set; }

            /// <summary>
            /// Animation Selector
            /// </summary>
            [JsonProperty("animation_selector")]
            [DefaultValue("")]
            public string AnimationSelector { get; set; }

            /// <summary>
            /// Transition Decorator
            /// </summary>
            [JsonProperty("transition_decorator")]
            [DefaultValue("")]
            public string TransitionDecorator { get; set; }

            /// <summary>
            /// Aim Selector
            /// </summary>
            [JsonProperty("aim_selector")]
            [DefaultValue("")]
            public string AimSelector { get; set; }

            /// <summary>
            /// Shoot Selector
            /// </summary>
            [JsonProperty("shoot_selector")]
            [DefaultValue("")]
            public string ShootSelector { get; set; }

            /// <summary>
            /// Delta Layer Function
            /// </summary>
            [JsonProperty("delta_layer_function")]
            [DefaultValue("")]
            public string DeltaLayerFunction { get; set; }

            /// <summary>
            /// Transition Decorator Layer Function
            /// </summary>
            [JsonProperty("transdec_layer_function")]
            [DefaultValue("")]
            public string TransDecLayerFunction { get; set; }

            /// <summary>
            /// AnimStateMachine Client Notify
            /// </summary>
            [JsonProperty("asm_client_notify")]
            [DefaultValue("")]
            public string ASMClientNotify { get; set; }

            /// <summary>
            /// Whether this ASM is Sync Loop or not
            /// </summary>
            [JsonProperty("loopsync")]
            [DefaultValue(false)]
            public bool LoopSync { get; set; }

            /// <summary>
            /// Whether this ASM is Clean Loop or not
            /// </summary>
            [JsonProperty("cleanloop")]
            [DefaultValue(false)]
            public bool CleanLoop { get; set; }

            /// <summary>
            /// Whether this ASM is Multiple Delta or not
            /// </summary>
            [JsonProperty("multipledelta")]
            [DefaultValue(false)]
            public bool MultipleDelta { get; set; }

            /// <summary>
            /// Whether this ASM is Terminal or not
            /// </summary>
            [JsonProperty("terminal")]
            [DefaultValue(false)]
            public bool Terminal { get; set; }

            /// <summary>
            /// Whether this ASM is Parameric 2D or not
            /// </summary>
            [JsonProperty("parametric2d")]
            [DefaultValue(false)]
            public bool Parametric2D { get; set; }

            /// <summary>
            /// Whether this ASM is Animation Driven LocMotion or not
            /// </summary>
            [JsonProperty("animdrivenlocomotion")]
            [DefaultValue(false)]
            public bool AnimDrivenLocmotion { get; set; }

            /// <summary>
            /// Whether this ASM is Coderate or not
            /// </summary>
            [JsonProperty("coderate")]
            [DefaultValue(false)]
            public bool Coderate { get; set; }

            /// <summary>
            /// Whether this ASM is  or not
            /// </summary>
            [JsonProperty("speedblend")]
            [DefaultValue(false)]
            public bool SpeedBlend { get; set; }

            /// <summary>
            /// Whether this ASM allows Transition Decorator Aim  or not
            /// </summary>
            [JsonProperty("allow_transdec_aim")]
            [DefaultValue(false)]
            public bool AllowTransDecAim { get; set; }

            /// <summary>
            /// Whether this ASM is Force Fire or not
            /// </summary>
            [JsonProperty("force_fire")]
            [DefaultValue(false)]
            public bool ForceFire { get; set; }

            /// <summary>
            /// Whether this ASM requires a Ragdoll Notetrack  or not
            /// </summary>
            [JsonProperty("requires_ragdoll_notetrack")]
            [DefaultValue(false)]
            public bool RequiresRagdollNote { get; set; }

            /// <summary>
            /// Sub Anim States
            /// </summary>
            [JsonProperty("substates", Order = 1)]
            public Dictionary<string, AnimationStateObj> SubStates;

            /// <summary>
            /// Transitions
            /// </summary>
            [JsonProperty("transitions", Order = 2)]
            public Dictionary<string, TransitionObj> Transitions;
        }

        /// <summary>
        /// Animation States
        /// </summary>
        [JsonProperty("states")]
        public Dictionary<string, Dictionary<string, AnimationStateObj>> RootStates = new Dictionary<string, Dictionary<string, AnimationStateObj>>();

        /// <summary>
        /// Saves the Animation State Machine to a formatted JSON file
        /// </summary>
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
