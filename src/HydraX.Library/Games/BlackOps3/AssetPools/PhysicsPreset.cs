using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Physics Preset Logic
        /// </summary>
        private class PhysicsPreset : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Physics Preset Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] PhysicsPresetOffsets =
            {
                new Tuple<string, int, int>("name",                                                          0x0, 0x0),
                new Tuple<string, int, int>("flags",                                                         0x8, 0x4),
                new Tuple<string, int, int>("mass",                                                          0xc, 0x34),
                new Tuple<string, int, int>("bounce",                                                        0x10, 0x8),
                new Tuple<string, int, int>("friction",                                                      0x14, 0x8),
                new Tuple<string, int, int>("damping_linear",                                                0x18, 0x8),
                new Tuple<string, int, int>("damping_angular",                                               0x1c, 0x8),
                new Tuple<string, int, int>("bulletForceScale",                                              0x20, 0x8),
                new Tuple<string, int, int>("explosiveForceScale",                                           0x24, 0x8),
                new Tuple<string, int, int>("sndAliasPrefix",                                                0x28, 0x0),
                new Tuple<string, int, int>("canFloat",                                                      0x30, 0x4),
                new Tuple<string, int, int>("gravityScale",                                                  0x34, 0x8),
                new Tuple<string, int, int>("massOffsetX",                                                   0x38, 0x8),
                new Tuple<string, int, int>("massOffsetY",                                                   0x3c, 0x8),
                new Tuple<string, int, int>("massOffsetZ",                                                   0x40, 0x8),
                new Tuple<string, int, int>("buoyancyMinX",                                                  0x44, 0x8),
                new Tuple<string, int, int>("buoyancyMinY",                                                  0x48, 0x8),
                new Tuple<string, int, int>("buoyancyMinZ",                                                  0x4c, 0x8),
                new Tuple<string, int, int>("buoyancyMaxX",                                                  0x50, 0x8),
                new Tuple<string, int, int>("buoyancyMaxY",                                                  0x54, 0x8),
                new Tuple<string, int, int>("buoyancyMaxZ",                                                  0x58, 0x8),
                new Tuple<string, int, int>("trailFX",                                                       0x60, 0xa),
                new Tuple<string, int, int>("impactsFXTable",                                                0x68, 0x23),
                new Tuple<string, int, int>("impactsSoundsTable",                                            0x70, 0x22),
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
            public string Name => "physpreset";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Physics";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.physpreset;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, PhysicsPresetOffsets, instance, HandlePhysicsPresetSettings);

                result.Type = "physpreset";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);
            }

            /// <summary>
            /// Handles Physics Preset Specific Settings
            /// </summary>
            private static object HandlePhysicsPresetSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x34:
                        return BitConverter.ToSingle(assetBuffer, offset) / 0.001;
                    default:
                        {
                            return null;
                        }
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
