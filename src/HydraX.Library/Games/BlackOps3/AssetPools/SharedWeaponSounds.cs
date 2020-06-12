using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 SharedWeaponSounds Logic
        /// </summary>
        private class SharedWeaponSounds : IAssetPool
        {
            #region Tables
            /// <summary>
            /// SharedWeaponSounds Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] SharedWeaponSoundsOffsets =
            {
                new Tuple<string, int, int>("meleeSurfaceSound",                                             0x8, 0x1f),
                new Tuple<string, int, int>("meleeSurfaceSoundPlayer",                                       0x10, 0x1f),
                new Tuple<string, int, int>("meleeEntityImpactSwipe",                                        0x18, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactSwipeVictim",                                  0x20, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactSwipePlayer",                                  0x28, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactPower",                                        0x30, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactPowerVictim",                                  0x38, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactPowerPlayer",                                  0x40, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactLunge",                                        0x48, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactLungeVictim",                                  0x50, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactLungePlayer",                                  0x58, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactChainMid",                                     0x60, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactChainMidPlayer",                               0x68, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactChainFinish",                                  0x70, 0x21),
                new Tuple<string, int, int>("meleeEntityImpactChainFinishPlayer",                            0x78, 0x21),
                new Tuple<string, int, int>("meleeWhoosh",                                                   0x80, 0x18),
                new Tuple<string, int, int>("meleeWhooshPlayer",                                             0x84, 0x18),
                new Tuple<string, int, int>("meleeLungeInWhoosh",                                            0x88, 0x18),
                new Tuple<string, int, int>("meleeLungeInWhooshPlayer",                                      0x8c, 0x18),
                new Tuple<string, int, int>("meleePowerWhoosh",                                              0x90, 0x18),
                new Tuple<string, int, int>("meleePowerWhooshLeft",                                          0x94, 0x18),
                new Tuple<string, int, int>("meleePowerWhooshPlayer",                                        0x98, 0x18),
                new Tuple<string, int, int>("meleePowerWhooshPlayerLeft",                                    0x9c, 0x18),
                new Tuple<string, int, int>("meleeHitShield",                                                0xa0, 0x18),
                new Tuple<string, int, int>("meleeHitShieldVictim",                                          0xa4, 0x18),
                new Tuple<string, int, int>("meleeHitShieldPlayer",                                          0xa8, 0x18),
                new Tuple<string, int, int>("meleeFromBehind",                                               0xac, 0x18),
                new Tuple<string, int, int>("meleeFromBehindVictim",                                         0xb0, 0x18),
                new Tuple<string, int, int>("meleeFromBehindPlayer",                                         0xb4, 0x18),
                new Tuple<string, int, int>("pickupWeapon",                                                  0xb8, 0x18),
                new Tuple<string, int, int>("pickupWeaponPlayer",                                            0xbc, 0x18),
                new Tuple<string, int, int>("pickupAmmo",                                                    0xc0, 0x18),
                new Tuple<string, int, int>("pickupAmmoPlayer",                                              0xc4, 0x18),
                new Tuple<string, int, int>("raiseWeapon",                                                   0xc8, 0x18),
                new Tuple<string, int, int>("raiseWeaponPlayer",                                             0xcc, 0x18),
                new Tuple<string, int, int>("firstRaiseWeapon",                                              0xd0, 0x18),
                new Tuple<string, int, int>("firstRaiseWeaponPlayer",                                        0xd4, 0x18),
                new Tuple<string, int, int>("raiseADSWeaponPlayer",                                          0xd8, 0x18),
                new Tuple<string, int, int>("lowerADSWeaponPlayer",                                          0xdc, 0x18),
                new Tuple<string, int, int>("putAway",                                                       0xe0, 0x18),
                new Tuple<string, int, int>("putAwayPlayer",                                                 0xe4, 0x18),
                new Tuple<string, int, int>("altSwitch",                                                     0xe8, 0x18),
                new Tuple<string, int, int>("altSwitchPlayer",                                               0xec, 0x18),
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
            public string Name => "sharedweaponsounds";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.sharedweaponsounds;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, SharedWeaponSoundsOffsets, instance);

                result.Type = "sharedweaponsounds";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
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
