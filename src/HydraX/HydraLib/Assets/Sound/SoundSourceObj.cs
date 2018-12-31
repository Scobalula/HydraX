using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib
{
    class SoundSourceObj
    {
        /// <summary>
        /// Sound Alias CSV Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Aliases
        /// </summary>
        public Alias[] Aliases { get; set; }

        /// <summary>
        /// Sound Alias
        /// </summary>
        public class Alias
        {
            public Entry[] Entries { get; set; }
            public class Entry
            {
                /// <summary>
                /// Name of this sound alias. A text string to be used throughout the engine to trigger
                /// sounds. Multiple aliases of the same name that are built into a single sound zone will be treated
                /// as a random situation when they are triggered.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Not used
                /// </summary>
                public string Behavior { get; set; }

                /// <summary>
                /// Specifies whether the sound is loaded (specify “loaded”) into RAM or
                /// streamed(specify “streamed”), from disk.
                /// </summary>
                public string Storage { get; set; }

                /// <summary>
                /// The path and filename of the physical wav file for this sound alias
                /// </summary>
                public string FileSpec { get; set; }

                /// <summary>
                /// If this column is filled out with a looping asset then it will be triggered
                /// when the(one shot) asset specified in FileSpec finishes.
                /// </summary>
                public string FileSpecSustain { get; set; }

                /// <summary>
                /// If this column is filled out then this asset will be triggered when the
                /// looping asset specified in FileSpecSustain is stopped.
                /// </summary>
                public string FileSpecRelease { get; set; }

                /// <summary>
                /// , this field points to the name of a template alias defined in
                /// a template CSV located in <game>\share\raw\sound\templates\. 
                /// Only used at compile, template is "merged" with alias.
                /// </summary>
                public string Template { get; set; }

                /// <summary>
                /// A name of a loadspec contained in
                /// <game>\share\raw\sound\globals\loadspec.csv.
                /// </summary>
                public string Loadspec { get; set; }

                /// <summary>
                /// – specify another sound alias here and it will be triggered immediately after
                /// the “primary” sound alias.
                /// </summary>
                public string Secondary { get; set; }

                /// <summary>
                /// Not used
                /// </summary>
                public string SustainAlias { get; set; }

                /// <summary>
                /// Not used
                /// </summary>
                public string ReleaseAlias { get; set; }

                /// <summary>
                /// This is the bus that the sound belongs to.
                /// </summary>
                public string Bus { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string VolumeGroup { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string DuckGroup { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Duck { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string ReverbSend { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string CenterSend { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string VolMin { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string VolMax { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string DistMin { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string DistMaxDry { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string DistMaxWet { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string DryMinCurve { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string DryMaxCurve { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string WetMinCurve { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string WetMaxCurve { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string LimitCount { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string LimitType { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string EntityLimitCount { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string EntityLimitType { get; set; }

                /// <summary>
                /// FOUND BUT TBD
                /// </summary>
                public string PitchMin { get; set; }

                /// <summary>
                /// FOUND BUT TBD
                /// </summary>
                public string PitchMax { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string PriorityMin { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string PriorityMax { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string PriorityThresholdMin { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string PriorityThresholdMax { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string AmplitudePriority { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string PanType { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Pan { get; set; }

                /// <summary>
                /// NOT USED ???
                /// </summary>
                public string Futz { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Looping { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string RandomizeType { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Probability { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string StartDelay { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string EnvelopMin { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string EnvelopMax { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string EnvelopPercent { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string OcclusionLevel { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string IsBig { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string DistanceLpf { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string FluxType { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string FluxTime { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Subtitle { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Doppler { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextType { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextValue { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextType1 { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextValue1 { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextType2 { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextValue2 { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextType3 { get; set; }

                /// <summary>
                /// Used in conjunction with SetSoundContext, etc. to enable this sound
                /// under certain "contexts". (underwater, npc/plr, etc.)
                /// </summary>
                public string ContextValue3 { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Timescale { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string IsMusic { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string IsCinematic { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string FadeIn { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string FadeOut { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string Pauseable { get; set; }

                /// <summary>
                /// FOUND
                /// </summary>
                public string StopOnEntDeath { get; set; }

                /// <summary>
                /// NOT USED ON PC - CONSOLE ONLY
                /// </summary>
                public string Compression { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string StopOnPlay { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string DopplerScale { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FutzPatch { get; set; }

                /// <summary>
                /// NOT USED
                /// </summary>
                public string VoiceLimit { get; set; }

                /// <summary>
                /// DONE
                /// </summary>
                public string IgnoreMaxDist { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string NeverPlayTwice { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string ContinuousPan { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileSource { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileSourceSustain { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileSourceRelease { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileTarget { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileTargetSustain { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FileTargetRelease { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string Platform { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string Language { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string OutputDevices { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string PlatformMask { get; set; }

                /// <summary>
                /// NOT USED - T6 Wii U Specific?
                /// </summary>
                public string WiiUMono { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string StopAlias { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string DistanceLpfMin { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string DistanceLpfMax { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string FacialAnimationName { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string RestartContextLoops { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string SilentInCPZ { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string ContextFailsafe { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string GPAD { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string GPADOnly { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string MuteVoice { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string MuteMusic { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string RowSourceFileName { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string RowSourceShortName { get; set; }

                /// <summary>
                /// TBD
                /// </summary>
                public string RowSourceLineNumber { get; set; }
            }

        }

        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            StringBuilder output = new StringBuilder();

            PropertyInfo[] properties = typeof(Alias.Entry).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                output.Append(String.Format("{0},", property.Name));
            }
            output.AppendLine();
            foreach (var alias in Aliases)
            {
                foreach (var variation in alias.Entries)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        output.Append(String.Format("{0},", property.GetValue(variation)));
                    }
                    output.AppendLine();
                }
            }

            File.WriteAllText(path, output.ToString());
        }
    }
}