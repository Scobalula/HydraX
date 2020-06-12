using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Physics Constraints Logic
        /// </summary>
        private class PhysicsConstraints : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Physics Constraints Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] PhysicsConstraintsOffsets =
            {
                new Tuple<string, int, int>("c1_type",                                                       0x14, 0x33),
                new Tuple<string, int, int>("c1_bone1_name",                                                 0x24, 0x15),
                new Tuple<string, int, int>("c1_bone2_name",                                                 0x34, 0x15),
                new Tuple<string, int, int>("c1_offsetX",                                                    0x38, 0x8),
                new Tuple<string, int, int>("c1_offsetY",                                                    0x3c, 0x8),
                new Tuple<string, int, int>("c1_offsetZ",                                                    0x40, 0x8),
                new Tuple<string, int, int>("c1_timeout",                                                    0x6c, 0x4),
                new Tuple<string, int, int>("c1_min_health",                                                 0x70, 0x4),
                new Tuple<string, int, int>("c1_max_health",                                                 0x74, 0x4),
                new Tuple<string, int, int>("c1_length",                                                     0x78, 0x8),
                new Tuple<string, int, int>("c1_damp",                                                       0x7c, 0x8),
                new Tuple<string, int, int>("c1_power",                                                      0x80, 0x8),
                new Tuple<string, int, int>("c1_pitch",                                                      0x84, 0x8),
                new Tuple<string, int, int>("c1_yaw",                                                        0x88, 0x8),
                new Tuple<string, int, int>("c1_shakescalez",                                                0x8c, 0x8),
                new Tuple<string, int, int>("c1_spin_scale",                                                 0x90, 0x8),
                new Tuple<string, int, int>("c1_min_angle",                                                  0x94, 0x8),
                new Tuple<string, int, int>("c1_max_angle",                                                  0x98, 0x8),
                new Tuple<string, int, int>("c1_min_angle_yaw",                                              0x9c, 0x8),
                new Tuple<string, int, int>("c1_max_angle_yaw",                                              0xa0, 0x8),
                new Tuple<string, int, int>("c1_gravity",                                                    0xb8, 0x8),
                new Tuple<string, int, int>("c1_useantenna_xaxis",                                           0xbc, 0x6),
                new Tuple<string, int, int>("c2_type",                                                       0xe4, 0x33),
                new Tuple<string, int, int>("c2_bone1_name",                                                 0xf4, 0x15),
                new Tuple<string, int, int>("c2_bone2_name",                                                 0x104, 0x15),
                new Tuple<string, int, int>("c2_offsetX",                                                    0x108, 0x8),
                new Tuple<string, int, int>("c2_offsetY",                                                    0x10c, 0x8),
                new Tuple<string, int, int>("c2_offsetZ",                                                    0x110, 0x8),
                new Tuple<string, int, int>("c2_timeout",                                                    0x13c, 0x4),
                new Tuple<string, int, int>("c2_min_health",                                                 0x140, 0x4),
                new Tuple<string, int, int>("c2_max_health",                                                 0x144, 0x4),
                new Tuple<string, int, int>("c2_length",                                                     0x148, 0x8),
                new Tuple<string, int, int>("c2_damp",                                                       0x14c, 0x8),
                new Tuple<string, int, int>("c2_power",                                                      0x150, 0x8),
                new Tuple<string, int, int>("c2_min_angle",                                                  0x164, 0x8),
                new Tuple<string, int, int>("c2_max_angle",                                                  0x168, 0x8),
                new Tuple<string, int, int>("c2_min_angle_yaw",                                              0x16c, 0x8),
                new Tuple<string, int, int>("c2_max_angle_yaw",                                              0x170, 0x8),
                new Tuple<string, int, int>("c2_gravity",                                                    0x188, 0x8),
                new Tuple<string, int, int>("c2_useantenna_xaxis",                                           0x18c, 0x6),
                new Tuple<string, int, int>("c3_type",                                                       0x1b4, 0x33),
                new Tuple<string, int, int>("c3_bone1_name",                                                 0x1c4, 0x15),
                new Tuple<string, int, int>("c3_bone2_name",                                                 0x1d4, 0x15),
                new Tuple<string, int, int>("c3_offsetX",                                                    0x1d8, 0x8),
                new Tuple<string, int, int>("c3_offsetY",                                                    0x1dc, 0x8),
                new Tuple<string, int, int>("c3_offsetZ",                                                    0x1e0, 0x8),
                new Tuple<string, int, int>("c3_timeout",                                                    0x20c, 0x4),
                new Tuple<string, int, int>("c3_min_health",                                                 0x210, 0x4),
                new Tuple<string, int, int>("c3_max_health",                                                 0x214, 0x4),
                new Tuple<string, int, int>("c3_length",                                                     0x218, 0x8),
                new Tuple<string, int, int>("c3_damp",                                                       0x21c, 0x8),
                new Tuple<string, int, int>("c3_power",                                                      0x220, 0x8),
                new Tuple<string, int, int>("c3_min_angle",                                                  0x234, 0x8),
                new Tuple<string, int, int>("c3_max_angle",                                                  0x238, 0x8),
                new Tuple<string, int, int>("c3_min_angle_yaw",                                              0x23c, 0x8),
                new Tuple<string, int, int>("c3_max_angle_yaw",                                              0x240, 0x8),
                new Tuple<string, int, int>("c3_gravity",                                                    0x258, 0x8),
                new Tuple<string, int, int>("c3_useantenna_xaxis",                                           0x25c, 0x6),
                new Tuple<string, int, int>("c4_type",                                                       0x284, 0x33),
                new Tuple<string, int, int>("c4_bone1_name",                                                 0x294, 0x15),
                new Tuple<string, int, int>("c4_bone2_name",                                                 0x2a4, 0x15),
                new Tuple<string, int, int>("c4_offsetX",                                                    0x2a8, 0x8),
                new Tuple<string, int, int>("c4_offsetY",                                                    0x2ac, 0x8),
                new Tuple<string, int, int>("c4_offsetZ",                                                    0x2b0, 0x8),
                new Tuple<string, int, int>("c4_timeout",                                                    0x2dc, 0x4),
                new Tuple<string, int, int>("c4_min_health",                                                 0x2e0, 0x4),
                new Tuple<string, int, int>("c4_max_health",                                                 0x2e4, 0x4),
                new Tuple<string, int, int>("c4_length",                                                     0x2e8, 0x8),
                new Tuple<string, int, int>("c4_damp",                                                       0x2ec, 0x8),
                new Tuple<string, int, int>("c4_power",                                                      0x2f0, 0x8),
                new Tuple<string, int, int>("c4_min_angle",                                                  0x304, 0x8),
                new Tuple<string, int, int>("c4_max_angle",                                                  0x308, 0x8),
                new Tuple<string, int, int>("c4_min_angle_yaw",                                              0x30c, 0x8),
                new Tuple<string, int, int>("c4_max_angle_yaw",                                              0x310, 0x8),
                new Tuple<string, int, int>("c4_gravity",                                                    0x328, 0x8),
                new Tuple<string, int, int>("c4_useantenna_xaxis",                                           0x32c, 0x6),
            };

            /// <summary>
            /// Constraint Types
            /// </summary>
            private static readonly string[] ConstraintTypes =
            {
                "none",
                "point",
                "hinge",
                "actuator",
                "fake_shake",
                "launch",
                "antenna",
                "rope",
                "light",
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
            public string Name => "physconstraints";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Physics";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.physconstraints;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, PhysicsConstraintsOffsets, instance, HandlePhysicsConstraintsSettings);

                result.Type = "physconstraints";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);
            }

            /// <summary>
            /// Handles Physics Constrain Specific Settings
            /// </summary>
            private static object HandlePhysicsConstraintsSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return ConstraintTypes[BitConverter.ToInt32(assetBuffer, offset)];
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
