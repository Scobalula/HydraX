using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Shellshock Logic
        /// </summary>
        private class Shellshock : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Shellshock Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] ShellshockOffsets =
            {
                new Tuple<string, int, int>("screenBlurFadeTime",                                            0x8, 0x9),
                new Tuple<string, int, int>("screenBlurBlendTime",                                           0xc, 0x9),
                new Tuple<string, int, int>("screenFlashWhiteFadeTime",                                      0x10, 0x9),
                new Tuple<string, int, int>("screenFlashShotFadeTime",                                       0x14, 0x9),
                new Tuple<string, int, int>("screenType",                                                    0x18, 0x33),
                new Tuple<string, int, int>("viewKickFadeTime",                                              0x1c, 0x9),
                new Tuple<string, int, int>("viewKickPeriod",                                                0x20, 0x9),
                new Tuple<string, int, int>("viewKickRadius",                                                0x24, 0x8),
                new Tuple<string, int, int>("sound",                                                         0x28, 0x7),
                new Tuple<string, int, int>("soundLoop",                                                     0x2c, 0x18),
                new Tuple<string, int, int>("soundLoopSilent",                                               0x30, 0x18),
                new Tuple<string, int, int>("soundEnd",                                                      0x34, 0x18),
                new Tuple<string, int, int>("soundEndAbort",                                                 0x38, 0x18),
                new Tuple<string, int, int>("soundRoomType",                                                 0x40, 0x0),
                new Tuple<string, int, int>("soundSnapshot",                                                 0x48, 0x0),
                new Tuple<string, int, int>("soundFadeInTime",                                               0x50, 0x9),
                new Tuple<string, int, int>("soundFadeOutTime",                                              0x54, 0x9),
                new Tuple<string, int, int>("soundDryLevel",                                                 0x58, 0x8),
                new Tuple<string, int, int>("soundWetLevel",                                                 0x5c, 0x8),
                new Tuple<string, int, int>("soundModEndDelay",                                              0x60, 0x9),
                new Tuple<string, int, int>("soundLoopFadeTime",                                             0x64, 0x9),
                new Tuple<string, int, int>("soundLoopEndDelay",                                             0x68, 0x9),
                new Tuple<string, int, int>("lookControl",                                                   0x70, 0x7),
                new Tuple<string, int, int>("lookDisableAimAssistLockOn",                                    0x74, 0x7),
                new Tuple<string, int, int>("lookDisableAimAssistSlowDown",                                  0x78, 0x7),
                new Tuple<string, int, int>("lookControlFadeTime",                                           0x7c, 0x9),
                new Tuple<string, int, int>("lookControlMouseSensitivity",                                   0x80, 0x8),
                new Tuple<string, int, int>("lookControlMaxPitchSpeed",                                      0x84, 0x8),
                new Tuple<string, int, int>("lookControlMaxYawSpeed",                                        0x88, 0x8),
                new Tuple<string, int, int>("visionSetName",                                                 0x90, 0x0),
                new Tuple<string, int, int>("visionSetFadeInTime",                                           0x98, 0x9),
                new Tuple<string, int, int>("visionSetFadeOutTime",                                          0x9c, 0x9),
                new Tuple<string, int, int>("movement",                                                      0xa0, 0x8),
                new Tuple<string, int, int>("cancelsMovement",                                               0xa4, 0x7),
                new Tuple<string, int, int>("animation",                                                     0xa8, 0x7),
            };

            /// <summary>
            /// Shellshock Screen Types
            /// </summary>
            private static readonly string[] ShellshockScreenType =
            {
                "blurred",
                "flashed",
                "concussed",
                "shocked",
                "none",
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
            public string Name => "shellshock";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Physics";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.shellshock;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, ShellshockOffsets, instance, HandleShellshockSettings);

                result.Type = "shellshock";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Shellshock Specific Settings
            /// </summary>
            private static object HandleShellshockSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return ShellshockScreenType[BitConverter.ToInt32(assetBuffer, offset)];
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
