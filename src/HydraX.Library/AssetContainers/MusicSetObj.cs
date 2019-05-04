using System.IO;
using Newtonsoft.Json;

namespace HydraX.Library.AssetContainers
{
    /// <summary>
    /// Class to hold a Music Set
    /// </summary>
    public class MusicSetObj
    {
        /// <summary>
        /// Class to hold a Music State
        /// </summary>
        public class StateObj
        {
            /// <summary>
            /// Class to hold a Music State Asset
            /// </summary>
            public class AssetObj
            {
                #region AssetObjProperties
                public string AliasName { get; set; }
                public bool Looping { get; set; }
                public bool CompleteLoop { get; set; }
                public bool RemoveAfterPlay { get; set; }
                public bool PlayAsFirstRandom { get; set; }
                public bool StartSync { get; set; }
                public bool StopSync { get; set; }
                public bool CompleteOnStop { get; set; }
                public int LoopStartOffset { get; set; }
                public int BPM { get; set; }
                public int AssetType { get; set; }
                public int LoopNumber { get; set; }
                public int Order { get; set; }
                public int StartDelayBeats { get; set; }
                public int StartFadeBeats { get; set; }
                public int StopDelayBeats { get; set; }
                public int StopFadeBeats { get; set; }
                public int Meter { get; set; }
                #endregion
            }

            #region MusicStateObjProperties
            public string Name { get; set; }
            public AssetObj IntroAsset { get; set; }
            public AssetObj ExitAsset { get; set; }
            public AssetObj[] LoopAssets { get; set; }
            public int Order { get; set; }
            public bool IsRandom { get; set; }
            public bool IsSequential { get; set; }
            public bool SkipPreviousExit { get; set; }
            #endregion
        }

        #region MusicSetObjProperties
        public string Name { get; set; }
        public StateObj[] StateArray { get; set; }
        public string[] StateNames { get; set; }
        public int LoopIndex { get; set; }
        #endregion

        /// <summary>
        /// Saves XCam to a formatted JSON file
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
