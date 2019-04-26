using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;

namespace HydraX.Library
{
    partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Rumble Logic
        /// </summary>
        private class Rumble : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Rumble Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct RumbleAsset
            {
                #region RumbleAssetProperties
                public long NamePointer;
                public int Duration;
                public float Range;
                public long HighRumbleFilePointer;
                public long LowRumbleFilePointer;
                public int FadeWithDistance;
                public int Broadcast;
                public float CamShakeScale;
                public int CamShakeDuration;
                public float CamShakeRange;
                public float PulseRadiusOuter;
                public float PulseScale;
                public int PulseBoneTagStringIndex;
                #endregion
            }

            /// <summary>
            /// Rumble File Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct RumbleFile
            {
                #region RawFileAssetProperties
                public long NamePointer;
                public int EntryCount;
                #endregion
            }
            #endregion

            /// <summary>
            /// Size of each asset
            /// </summary>
            public int AssetSize { get; set; }

            /// <summary>
            /// Gets or Sets the number of Assets 
            /// </summary>
            public int AssetCount { get; set; }

            /// <summary>
            /// Gets or Sets the Start Address
            /// </summary>
            public long StartAddress { get; set; }

            /// <summary>
            /// Gets or Sets the End Address
            /// </summary>
            public long EndAddress { get { return StartAddress + (AssetCount * AssetSize); } set => throw new NotImplementedException(); }

            /// <summary>
            /// Gets the Name of this Pool
            /// </summary>
            public string Name => "rumble";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.rumble;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public List<GameAsset> Load(HydraInstance instance)
            {
                var results = new List<GameAsset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.BaseAddress + instance.Game.AssetPoolsAddresses[instance.Game.ProcessIndex] + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<RumbleAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Type = Name,
                        Information = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<RumbleAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                var rumbleAsset = new GameDataTable.Asset(asset.Name, "rumble");

                rumbleAsset["broadcast"]           = header.Broadcast;
                rumbleAsset["camShakeDuration"]    = header.CamShakeDuration;
                rumbleAsset["camShakeRange"]       = header.CamShakeRange;
                rumbleAsset["camShakeScale"]       = header.CamShakeScale;
                rumbleAsset["duration"]            = header.Duration;
                rumbleAsset["fadeWithDistance"]    = header.FadeWithDistance;
                rumbleAsset["pulseBoneTagPointer"] = instance.Game.GetString(header.PulseBoneTagStringIndex, instance);
                rumbleAsset["pulseRadiusOuter"]    = header.PulseRadiusOuter;
                rumbleAsset["pulseScale"]          = header.PulseScale;
                rumbleAsset["range"]               = header.Range;
                rumbleAsset["lowrumblefile"]       = WriteRumbleFile(header.LowRumbleFilePointer, instance);
                rumbleAsset["highrumblefile"]      = WriteRumbleFile(header.HighRumbleFilePointer, instance);

                instance.GDTs["Misc"][rumbleAsset.Name] = rumbleAsset;

                return HydraStatus.Success;
            }

            public string WriteRumbleFile(long address, HydraInstance instance)
            {
                var rumbleFile = instance.Reader.ReadStruct<RumbleFile>(address);
                var rumbleFileName = instance.Reader.ReadNullTerminatedString(rumbleFile.NamePointer);

                if(address > 0)
                {
                    var path = Path.Combine(instance.ExportFolder, "rumble", rumbleFileName);

                    Directory.CreateDirectory(Path.Combine(instance.ExportFolder, "rumble"));

                    using (StreamWriter streamWriter = new StreamWriter(path))
                    {
                        streamWriter.WriteLine("RUMBLEGRAPHFILE");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine(rumbleFile.EntryCount);

                        using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(instance.Reader.ReadBytes(address + 12, 8 * rumbleFile.EntryCount))))
                        {

                            for (int i = 0; i < rumbleFile.EntryCount; i++)
                            {
                                streamWriter.WriteLine("{0:0.0000} {1:0.0000}", binaryReader.ReadSingle(), binaryReader.ReadSingle());
                            }
                        }
                    }
                }

                return rumbleFileName;
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(GameAsset asset)
            {
                return IsNullAsset(asset.NameLocation);
            }

            /// <summary>
            /// Checks if the given asset is a null slot
            /// </summary>
            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }
        }
    }
}
