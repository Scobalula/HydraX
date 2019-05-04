using System.IO;
using System.Reflection;
using System.Text;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// A class to hold a sound source (CSV File containing aliases)
    /// </summary>
    public class SoundSourceObj
    {
        #region SoundSourceObjProperties
        public object Name { get; set; }
        public object Language { get; set; }
        public Alias[] Aliases { get; set; }
        #endregion

        /// <summary>
        /// A Class to hold a sound alias
        /// </summary>
        public class Alias
        {
            #region SoundAliasProperties
            public Entry[] Entries { get; set; }
            #endregion

            /// <summary>
            /// A class to hold a sound alias entry
            /// </summary>
            public class Entry
            {
                #region SoundAliasEntryProperties
                public object Name { get; set; }
                public object Behavior { get; set; }
                public object Storage { get; set; }
                public object FileSpec { get; set; }
                public object FileSpecSustain { get; set; }
                public object FileSpecRelease { get; set; }
                public object Template { get; set; }
                public object Loadspec { get; set; }
                public object Secondary { get; set; }
                public object SustainAlias { get; set; }
                public object ReleaseAlias { get; set; }
                public object Bus { get; set; }
                public object VolumeGroup { get; set; }
                public object DuckGroup { get; set; }
                public object Duck { get; set; }
                public object ReverbSend { get; set; }
                public object CenterSend { get; set; }
                public object VolMin { get; set; }
                public object VolMax { get; set; }
                public object DistMin { get; set; }
                public object DistMaxDry { get; set; }
                public object DistMaxWet { get; set; }
                public object DryMinCurve { get; set; }
                public object DryMaxCurve { get; set; }
                public object WetMinCurve { get; set; }
                public object WetMaxCurve { get; set; }
                public object LimitCount { get; set; }
                public object LimitType { get; set; }
                public object EntityLimitCount { get; set; }
                public object EntityLimitType { get; set; }
                public object PitchMin { get; set; }
                public object PitchMax { get; set; }
                public object PriorityMin { get; set; }
                public object PriorityMax { get; set; }
                public object PriorityThresholdMin { get; set; }
                public object PriorityThresholdMax { get; set; }
                public object AmplitudePriority { get; set; }
                public object PanType { get; set; }
                public object Pan { get; set; }
                public object Futz { get; set; }
                public object Looping { get; set; }
                public object RandomizeType { get; set; }
                public object Probability { get; set; }
                public object StartDelay { get; set; }
                public object EnvelopMin { get; set; }
                public object EnvelopMax { get; set; }
                public object EnvelopPercent { get; set; }
                public object OcclusionLevel { get; set; }
                public object IsBig { get; set; }
                public object DistanceLpf { get; set; }
                public object FluxType { get; set; }
                public object FluxTime { get; set; }
                public object Subtitle { get; set; }
                public object Doppler { get; set; }
                public object ContextType { get; set; }
                public object ContextValue { get; set; }
                public object ContextType1 { get; set; }
                public object ContextValue1 { get; set; }
                public object ContextType2 { get; set; }
                public object ContextValue2 { get; set; }
                public object ContextType3 { get; set; }
                public object ContextValue3 { get; set; }
                public object Timescale { get; set; }
                public object IsMusic { get; set; }
                public object IsCinematic { get; set; }
                public object FadeIn { get; set; }
                public object FadeOut { get; set; }
                public object Pauseable { get; set; }
                public object StopOnEntDeath { get; set; }
                public object Compression { get; set; }
                public object StopOnPlay { get; set; }
                public object DopplerScale { get; set; }
                public object FutzPatch { get; set; }
                public object VoiceLimit { get; set; }
                public object IgnoreMaxDist { get; set; }
                public object NeverPlayTwice { get; set; }
                public object ContinuousPan { get; set; }
                public object FileSource { get; set; }
                public object FileSourceSustain { get; set; }
                public object FileSourceRelease { get; set; }
                public object FileTarget { get; set; }
                public object FileTargetSustain { get; set; }
                public object FileTargetRelease { get; set; }
                public object Platform { get; set; }
                public object Language { get; set; }
                public object OutputDevices { get; set; }
                public object PlatformMask { get; set; }
                public object WiiUMono { get; set; }
                public object StopAlias { get; set; }
                public object DistanceLpfMin { get; set; }
                public object DistanceLpfMax { get; set; }
                public object FacialAnimationName { get; set; }
                public object RestartContextLoops { get; set; }
                public object SilentInCPZ { get; set; }
                public object ContextFailsafe { get; set; }
                public object GPAD { get; set; }
                public object GPADOnly { get; set; }
                public object MuteVoice { get; set; }
                public object MuteMusic { get; set; }
                public object RowSourceFileName { get; set; }
                public object RowSourceShortName { get; set; }
                public object RowSourceLineNumber { get; set; }
                #endregion
            }

        }

        /// <summary>
        /// Saves the sound source file to the given path
        /// </summary>
        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            StringBuilder output = new StringBuilder();

            PropertyInfo[] properties = typeof(Alias.Entry).GetProperties();

            foreach (PropertyInfo property in properties)
                output.Append(property.Name + ",");
            output.AppendLine();

            foreach (var alias in Aliases)
            {
                foreach (var variation in alias.Entries)
                {
                    foreach (PropertyInfo property in properties)
                    {
                        output.Append(string.Format("{0},", property.GetValue(variation)));
                    }
                    output.AppendLine();
                }
            }

            File.WriteAllText(path, output.ToString());
        }
    }
}
