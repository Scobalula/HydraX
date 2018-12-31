using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Assets
{
    /// <summary>
    /// Generic Weapon Camo Object
    /// </summary>
    class WeaponCamoObj
    {
        /// <summary>
        /// Holds a Camo Materials
        /// </summary>
        public class CamoMaterial
        {
            /// <summary>
            /// Camo's Material Name
            /// </summary>
            public string Material { get; set; }

            /// <summary>
            /// Detail Normal Map for this Camo
            /// </summary>
            public string DetailNormalMap { get; set; }

            /// <summary>
            /// X Axis Translation
            /// </summary>
            [JsonIgnore]
            public double TranslationX { get; set; }

            /// <summary>
            /// Y Axis Translation
            /// </summary>
            [JsonIgnore]
            public double TranslationY { get; set; }

            /// <summary>
            /// X Axis Scale
            /// </summary>
            [JsonIgnore]
            public double ScaleX { get; set; }

            /// <summary>
            /// Y Axis Scale
            /// </summary>
            [JsonIgnore]
            public double ScaleY { get; set; }

            /// <summary>
            /// Rotation
            /// </summary>
            [JsonIgnore]
            public double Rotation { get; set; }

            /// <summary>
            /// Normal Map Blend Amount
            /// </summary>
            public double NormalMapBlend { get; set; }

            /// <summary>
            /// Gloss Map Blend Amount
            /// </summary>
            public double GlossMapBlend { get; set; }

            /// <summary>
            /// Detail Normal Map X Axis Scale
            /// </summary>
            public double NormalScaleX { get; set; }

            /// <summary>
            /// Detail Normal Map Y Axis Scale
            /// </summary>
            public double NormalScaleY { get; set; }

            /// <summary>
            /// Detail Normal Map Height
            /// </summary>
            public double NormalHeight { get; set; }

            /// <summary>
            /// Use Normal Map on this Camo
            /// </summary>
            public int UseNormalMap = 1;

            /// <summary>
            /// Use Gloss Map on this Camo
            /// </summary>
            public int UseGlossMap = 1;

            /// <summary>
            /// Base Material Count
            /// </summary>
            [JsonIgnore]
            public int BaseMaterialCount = 0;

            /// <summary>
            /// Base Materials
            /// </summary>
            [JsonIgnore]
            public string[] BaseMaterials = new string[10];

            /// <summary>
            /// Camo Masks
            /// </summary>
            [JsonIgnore]
            public string[] CamoMasks = new string[10];
        }

        /// <summary>
        /// Holds a Camo Entry
        /// </summary>
        public class CamoEntry
        {
            /// <summary>
            /// Number of Materials
            /// </summary>
            [JsonIgnore]
            public int MaterialCount { get; set; }

            /// <summary>
            /// Materials
            /// </summary>
            public CamoMaterial[] CamoMaterials { get; set; }
        }

        /// <summary>
        /// Weapon Camos
        /// </summary>
        public CamoEntry[] Camos { get; set; }
    }
}
