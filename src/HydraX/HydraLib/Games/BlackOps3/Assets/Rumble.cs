using HydraLib.Assets;
using PhilLibX.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class Rumble
        {
            /// <summary>
            /// Bo3 Raw File Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct RumbleHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Duration in Milliseconds
                /// </summary>
                public int Duration { get; set; }

                /// <summary>
                /// Object Mass in Pounds
                /// </summary>
                public float Range { get; set; }

                /// <summary>
                /// High Rumble Pointer
                /// </summary>
                public long HighRumblePointer { get; set; }

                /// <summary>
                /// Low Rumble Pointer
                /// </summary>
                public long LowRumblePointer { get; set; }

                public int FadeWithDistance { get; set; }
                public int Broadcast { get; set; }
                public float CamShakeRange { get; set; }
                public int CamShakeDuration { get; set; }
                public float CamShakeScale { get; set; }
                public float PulseRadiusOuter { get; set; }
                public float PulseScale { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            public struct RumbleFileHeader
            {
                /// <summary>
                /// Pointer to the name of this Rumble file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Number of Rumble Entries
                /// </summary>
                public int EntryCount { get; set; }
            }

            /// <summary>
            /// Loads the Assets for this type from Memory
            /// </summary>
            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Asset
                    var physPresetHeader = Hydra.ActiveGameReader.ReadStruct<RumbleHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(physPresetHeader.NamePointer))
                        continue;
                    // Create new asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(physPresetHeader.NamePointer),
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = "N/A"
                    });
                }
            }

            /// <summary>
            /// Exports the given asset from Memory
            /// </summary>
            public static bool Export(Asset asset)
            {
                // Read Header
                var rumbleHeader = Hydra.ActiveGameReader.ReadStruct<RumbleHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(rumbleHeader.NamePointer))
                    return false;
                // Add it to our GDTs
                Hydra.GDTs["Physic"].AddAsset(asset.Name, "rumble", new RumbleObj()
                {
                    HighRumble       = ExportRumbleFile(rumbleHeader.HighRumblePointer),
                    LowRumble        = ExportRumbleFile(rumbleHeader.LowRumblePointer),
                    Duration         = rumbleHeader.Duration / 1000.0f,
                    CamShakeDuration = rumbleHeader.CamShakeDuration / 1000.0f,
                    CamShakeRange    = rumbleHeader.CamShakeRange,
                    CamShakeScale    = rumbleHeader.CamShakeScale,
                    Broadcast        = rumbleHeader.Broadcast,
                    FadeWithDistance = rumbleHeader.FadeWithDistance,
                    PulseScale       = rumbleHeader.PulseScale,
                    PulseRadiusOuter = rumbleHeader.PulseRadiusOuter,
                    Range            = rumbleHeader.Range,
                });
                // Done
                return true;
            }

            /// <summary>
            /// Exports a Rumble File from Memory
            /// </summary>
            public static string ExportRumbleFile(long pointer)
            {
                // Check if there is a rumble
                if (pointer == 0)
                    return "";
                // Read rumble
                var rumbleFile = Hydra.ActiveGameReader.ReadStruct<RumbleFileHeader>(pointer);
                // Path Result
                string rumbleName = Hydra.ActiveGameReader.ReadNullTerminatedString(rumbleFile.NamePointer);
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "rumble", rumbleName);
                // Create Directory
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                // Create reslting File
                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    // Write Header
                    streamWriter.WriteLine("RUMBLEGRAPHFILE");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine(rumbleFile.EntryCount);
                    // Convert to a stream
                    using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(Hydra.ActiveGameReader.ReadBytes(pointer + 12, 8 * rumbleFile.EntryCount))))
                    {
                        // Loop through entries
                        for (int i = 0; i < rumbleFile.EntryCount; i++)
                        {
                            // Read the floats
                            float float1 = binaryReader.ReadSingle();
                            float float2 = binaryReader.ReadSingle();
                            // Write them
                            streamWriter.WriteLine("{0:0.0000} {1:0.0000}", float1, float2);
                        }
                    }
                }
                // Done
                return rumbleName;
            }
        }
    }
}
