using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HydraLib.Assets;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class AnimationMappingTable
        {
            /// <summary>
            /// Bo3 AnimMappingTable Header
            /// </summary>
            public struct AnimationMappingTableHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the maps
                /// </summary>
                public long MapPointer { get; set; }

                /// <summary>
                /// Number of Maps
                /// </summary>
                public int MapCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            /// <summary>
            /// Bo3 Animation Map Header
            /// </summary>
            public struct AnimationMapHeader
            {
                /// <summary>
                /// Index of the Name of this Map in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Animation String Indices
                /// </summary>
                public long AnimationStringIndicesPointer { get; set; }

                /// <summary>
                /// Number of Animations
                /// </summary>
                public int AnimationCount { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                // Loop through entire pool, and add what valid assets
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var animSelectorTable = Hydra.ActiveGameReader.ReadStruct<AnimationMappingTableHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(animSelectorTable.NamePointer))
                        continue;
                    // Add Asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(animSelectorTable.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = String.Format("Maps - {0}", animSelectorTable.MapCount),
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var animMappingTableHeader = Hydra.ActiveGameReader.ReadStruct<AnimationMappingTableHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(animMappingTableHeader.NamePointer))
                    return false;
                // Create output path
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "animtables", asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Preallocate and read buffer (buffer = MapCount * sizeof(AnimationMapHeader) (which is 24))
                AnimationMapObj[] animationMaps = new AnimationMapObj[animMappingTableHeader.MapCount];
                byte[] buffer = Hydra.ActiveGameReader.ReadBytes(animMappingTableHeader.MapPointer, animMappingTableHeader.MapCount * 24);
                // Loop selectors
                for (int i = 0; i < animMappingTableHeader.MapCount; i++)
                {
                    // Grab
                    var animationMap = ByteUtil.BytesToStruct<AnimationMapHeader>(buffer, i * 24);
                    // Create the selector
                    animationMaps[i] = new AnimationMapObj(GetString(animationMap.NameStringIndex), animationMap.AnimationCount);
                    // Read Buffer
                    byte[] indicesBuffer = Hydra.ActiveGameReader.ReadBytes(animationMap.AnimationStringIndicesPointer, animationMap.AnimationCount * 4);
                    // Loop animations
                    for (int j = 0; j < animationMap.AnimationCount; j++)
                        // Add to map
                        animationMaps[i].Animations[j] = GetString(BitConverter.ToInt32(indicesBuffer, j * 4));
                }
                // Output Stream
                using (StreamWriter writer = new StreamWriter(path))
                {
                    // Write number symbol
                    writer.WriteLine("#");
                    // Write Animation Maps
                    foreach(var animMap in animationMaps)
                    {
                        // Write Name
                        writer.Write("{0},", animMap.Name);
                        // Write Animations
                        foreach (var animation in animMap.Animations)
                            writer.Write("{0},", animation);
                        // New line for next map
                        writer.WriteLine();
                    }
                }
                // Done
                return true;
            }
        }
    }
}
