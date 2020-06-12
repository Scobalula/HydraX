using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
            public string SettingGroup => "Physics";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.rumble;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var address = StartAddress + (i * AssetSize);
                    var namePointer = instance.Reader.ReadInt64(address);

                    if (IsNullAsset(namePointer))
                        continue;

                    results.Add(new Asset()
                    {
                        Name        = instance.Reader.ReadNullTerminatedString(namePointer),
                        Type        = Name,
                        Zone        = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = "N/A",
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<RumbleAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

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

                instance.AddGDTAsset(rumbleAsset, rumbleAsset.Type, rumbleAsset.Name);

                return;
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
            public bool IsNullAsset(Asset asset)
            {
                return IsNullAsset((long)asset.Data);
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
