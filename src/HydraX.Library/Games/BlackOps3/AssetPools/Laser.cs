using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Laser Logic
        /// </summary>
        private class Laser : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Laser Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] LaserOffsets =
            {
                new Tuple<string, int, int>("material",                                                      0x8, 0x33),
                new Tuple<string, int, int>("width",                                                         0x10, 0x8),
                new Tuple<string, int, int>("uvRepeatLength",                                                0x14, 0x8),
                new Tuple<string, int, int>("firstSegmentLength",                                            0x18, 0x8),
                new Tuple<string, int, int>("flarePct",                                                      0x1c, 0x8),
                new Tuple<string, int, int>("flarePctAlt",                                                   0x20, 0x8),
                new Tuple<string, int, int>("range",                                                         0x24, 0x8),
                new Tuple<string, int, int>("rangeAlt",                                                      0x28, 0x8),
                new Tuple<string, int, int>("rangePlayer",                                                   0x2c, 0x8),
                new Tuple<string, int, int>("endOffset",                                                     0x30, 0x8),
                new Tuple<string, int, int>("noCollision",                                                   0x34, 0x6),
                new Tuple<string, int, int>("impactEffect",                                                  0x38, 0xa),
                new Tuple<string, int, int>("colorR0",                                                       0x40, 0x8),
                new Tuple<string, int, int>("colorG0",                                                       0x44, 0x8),
                new Tuple<string, int, int>("colorB0",                                                       0x48, 0x8),
                new Tuple<string, int, int>("colorA0",                                                       0x4c, 0x8),
                new Tuple<string, int, int>("colorR1",                                                       0x50, 0x8),
                new Tuple<string, int, int>("colorG1",                                                       0x54, 0x8),
                new Tuple<string, int, int>("colorB1",                                                       0x58, 0x8),
                new Tuple<string, int, int>("colorA1",                                                       0x5c, 0x8),
                new Tuple<string, int, int>("colorR2",                                                       0x60, 0x8),
                new Tuple<string, int, int>("colorG2",                                                       0x64, 0x8),
                new Tuple<string, int, int>("colorB2",                                                       0x68, 0x8),
                new Tuple<string, int, int>("colorA2",                                                       0x6c, 0x8),
            };

            /// <summary>
            /// AI Species Types
            /// </summary>
            private static readonly string[] AISpecies =
            {
               "human",
               "dog",
               "zombie",
               "zombie_dog",
               "robot",
            };

            /// <summary>
            /// AI Fire Modes
            /// </summary>
            private static readonly string[] AIFireModes =
            {
               "fullauto",
               "burst",
               "singleshot",
            };
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
            public string Name => "laser";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.laser;

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
                var buffer = instance.Reader.ReadBytes((long)asset.Data, AssetSize);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(buffer, 0)))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var result = ConvertAssetBufferToGDTAsset(buffer, LaserOffsets, instance, HandleLaserSettings);

                result.Type = "flametable";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Laser Specific Settings
            /// </summary>
            private static object HandleLaserSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, offset), instance);
                    default:
                        return null;
                }
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
