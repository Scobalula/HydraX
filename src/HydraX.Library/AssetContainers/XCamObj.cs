using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HydraX.Library.AssetContainers
{
    public class XCamObj
    {
        /// <summary>
        /// XCam Notetrack
        /// </summary>
        public class Notetrack
        {
            [JsonProperty("name")]
            public string Name = "";

            [JsonProperty("frame")]
            public int Frame = 0;

            public Notetrack(string name, int frame)
            {
                Name = name;
                Frame = frame;
            }
        }

        /// <summary>
        /// XCam Camera Switch
        /// </summary>
        public class CameraSwitch
        {
            [JsonProperty("frame")]
            public int Frame { get; set; }

            [JsonProperty("dissolve")]
            public double Dissolve { get; set; }

            [JsonProperty("cameras")]
            public int[] Cameras { get; set; }
        }

        /// <summary>
        /// XCam Align Node
        /// </summary>
        public class Align
        {
            [JsonProperty("tag")]
            public string Tag = "tag_align";

            [JsonProperty("offset")]
            public double[] Offset = new double[3];

            [JsonProperty("axis")]
            public Dictionary<string, double[]> Axis = new Dictionary<string, double[]>()
            {
                { "x", new double[3] },
                { "y", new double[3] },
                { "z", new double[3] },
            };
        }

        /// <summary>
        /// Target Model Bone Root Frame Data
        /// </summary>
        public class TargetModelBoneRootFrame
        {
            [JsonProperty("frame")]
            public int Frame = 0;

            [JsonProperty("offset")]
            public double[] Offset = new double[3];

            [JsonProperty("axis")]
            public Dictionary<string, double[]> Axis = new Dictionary<string, double[]>()
            {
                { "x", new double[3] },
                { "y", new double[3] },
                { "z", new double[3] },
            };
        }

        /// <summary>
        /// Target Model Bone Root Data
        /// </summary>
        public class TargetModelBoneRoot
        {
            [JsonProperty("name", Order = 1)]
            public string Name { get; set; }

            [JsonProperty("animation", Order = 3)]
            public TargetModelBoneRootFrame[] Animation { get; set; }

            [JsonProperty("axis", Order = 2)]
            public Dictionary<string, double[]> Axis = new Dictionary<string, double[]>()
            {
                { "x", new double[3] },
                { "y", new double[3] },
                { "z", new double[3] },
            };
        }

        /// <summary>
        /// Camera Animation Data
        /// </summary>
        public class CameraAnimation
        {
            [JsonProperty("frame", Order = -2)]
            public int Frame { get; set; }

            [JsonProperty("origin")]
            public double[] Origin = new double[3];

            [JsonProperty("dir")]
            public double[] Dir = new double[3];

            [JsonProperty("up")]
            public double[] Up = new double[3];

            [JsonProperty("right")]
            public double[] Right = new double[3];

            [JsonProperty("flen")]
            public double FocalLength = 27.0000;

            [JsonProperty("fov")]
            public double FieldOfView = 28.7985;

            [JsonProperty("fdist")]
            public double FDist = 109.4973;

            [JsonProperty("fstop")]
            public double FStop = 1.2000;

            [JsonProperty("lense")]
            public int Lense = 10;
        }

        /// <summary>
        /// Camera Data
        /// </summary>
        public class Camera
        {
            [JsonProperty("name", Order = -3)]
            public string Name { get; set; }

            [JsonProperty("index", Order = -2)]
            public int Index { get; set; }

            [JsonProperty("type")]
            public string Type = "Perspective";

            [JsonProperty("aperture")]
            public string Aperture = "FOCAL_LENGTH";

            [JsonProperty("origin")]
            public double[] Origin = new double[3];

            [JsonProperty("dir")]
            public double[] Dir = new double[3];

            [JsonProperty("up")]
            public double[] Up = new double[3];

            [JsonProperty("right")]
            public double[] Right = new double[3];

            [JsonProperty("flen")]
            public double FocalLength = 27.0000;

            [JsonProperty("fov")]
            public double FieldOfView = 28.7985;

            [JsonProperty("fdist")]
            public double FDist = 109.4973;

            [JsonProperty("fstop")]
            public double FStop = 1.2000;

            [JsonProperty("lense")]
            public int Lense = 10;

            [JsonProperty("aspectratio")]
            public double AspectRatio = 1.7786;

            [JsonProperty("nearz")]
            public double Nearz = 3.9370;

            [JsonProperty("farz")]
            public double Farz = 3937.0079;

            [JsonProperty("animation")]
            public CameraAnimation[] Animations { get; set; }
        }

        [JsonProperty("version")]
        public int Version = 1;
        [JsonProperty("scene")]
        public string Scene = "exported_via_HydraX_by_Scobalula.fbx";
        [JsonProperty("align")]
        public Align AlignNode = new Align();
        [JsonProperty("framerate")]
        public int FrameRate = 30;
        [JsonProperty("numframes")]
        public int FrameCount { get; set; }
        [JsonProperty("targetModelBoneRoots", Order = 1)]
        public TargetModelBoneRoot[] TargetModelBoneRoots = new TargetModelBoneRoot[0];
        [JsonProperty("cameras", Order = 2)]
        public Camera[] Cameras = new Camera[0];
        [JsonProperty("notetracks", Order = 4)]
        public Notetrack[] Notetracks = new Notetrack[0];
        [JsonProperty("cameraSwitch", Order = 3)]
        public CameraSwitch[] CameraSwitches = new CameraSwitch[0];

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
