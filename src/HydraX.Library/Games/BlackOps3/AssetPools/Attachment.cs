using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Attachment Logic
        /// </summary>
        private class Attachment : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Attachment Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] AttachmentOffsets =
            {
                new Tuple<string, int, int>("displayName",                                                   0x8, 0x0),
                new Tuple<string, int, int>("attachmentType",                                                0x10, 0x33),
                new Tuple<string, int, int>("penetrateType",                                                 0x18, 0x38),
                new Tuple<string, int, int>("firstRaisePriority",                                            0x20, 0x4),
                new Tuple<string, int, int>("hipIdleAmount",                                                 0x24, 0x8),
                new Tuple<string, int, int>("fireType",                                                      0x1c, 0x39),
                new Tuple<string, int, int>("damageRangeScale",                                              0x3c, 0x8),
                new Tuple<string, int, int>("damageScale1",                                                  0x40, 0x8),
                new Tuple<string, int, int>("damageScale2",                                                  0x44, 0x8),
                new Tuple<string, int, int>("damageScale3",                                                  0x48, 0x8),
                new Tuple<string, int, int>("damageScale4",                                                  0x4c, 0x8),
                new Tuple<string, int, int>("damageScale5",                                                  0x50, 0x8),
                new Tuple<string, int, int>("damageScale6",                                                  0x54, 0x8),
                new Tuple<string, int, int>("damageRangeScale1",                                             0x58, 0x8),
                new Tuple<string, int, int>("damageRangeScale2",                                             0x5c, 0x8),
                new Tuple<string, int, int>("damageRangeScale3",                                             0x60, 0x8),
                new Tuple<string, int, int>("damageRangeScale4",                                             0x64, 0x8),
                new Tuple<string, int, int>("damageRangeScale5",                                             0x68, 0x8),
                new Tuple<string, int, int>("damageRangeScale6",                                             0x6c, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale1",                                     0x70, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale2",                                     0x74, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale3",                                     0x78, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale4",                                     0x7c, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale5",                                     0x80, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageScale6",                                     0x84, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale1",                                0x88, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale2",                                0x8c, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale3",                                0x90, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale4",                                0x94, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale5",                                0x98, 0x8),
                new Tuple<string, int, int>("multishotBaseDamageRangeScale6",                                0x9c, 0x8),
                new Tuple<string, int, int>("aimAssistRangeScale",                                           0xa0, 0x8),
                new Tuple<string, int, int>("aimAssistRangeAdsScale",                                        0xa4, 0x8),
                new Tuple<string, int, int>("adsFlinchScalar",                                               0xa8, 0x8),
                new Tuple<string, int, int>("adsFiringFlinchScalar",                                         0xac, 0x8),
                new Tuple<string, int, int>("adsTurnRateScalar",                                             0xb0, 0x8),
                new Tuple<string, int, int>("adsWallRunBobScalar",                                           0xb4, 0x8),
                new Tuple<string, int, int>("adsAdditiveFallScalar",                                         0xb8, 0x8),
                new Tuple<string, int, int>("adsAdditiveJumpScalar",                                         0xbc, 0x8),
                new Tuple<string, int, int>("adsAdditiveJumpLandScalar",                                     0xc0, 0x8),
                new Tuple<string, int, int>("adsZoom1_focalLength",                                          0xc4, 0x8),
                new Tuple<string, int, int>("adsZoom2_focalLength",                                          0xcc, 0x8),
                new Tuple<string, int, int>("adsZoom3_focalLength",                                          0xd4, 0x8),
                new Tuple<string, int, int>("adsZoom1_fStop",                                                0xc8, 0x8),
                new Tuple<string, int, int>("adsZoom2_fStop",                                                0xd0, 0x8),
                new Tuple<string, int, int>("adsZoom3_fStop",                                                0xd8, 0x8),
                new Tuple<string, int, int>("adsZoomFov1",                                                   0xdc, 0x8),
                new Tuple<string, int, int>("adsZoomFov2",                                                   0xe0, 0x8),
                new Tuple<string, int, int>("adsZoomFov3",                                                   0xe4, 0x8),
                new Tuple<string, int, int>("adsZoomInFrac",                                                 0xe8, 0x8),
                new Tuple<string, int, int>("adsZoomOutFrac",                                                0xec, 0x8),
                new Tuple<string, int, int>("adsTransInTimeScale",                                           0xf0, 0x8),
                new Tuple<string, int, int>("adsTransOutTimeScale",                                          0xf4, 0x8),
                new Tuple<string, int, int>("adsRecoilReductionRate",                                        0xf8, 0x8),
                new Tuple<string, int, int>("adsRecoilReductionLimit",                                       0xfc, 0x8),
                new Tuple<string, int, int>("adsViewKickCenterSpeedScale",                                   0x100, 0x8),
                new Tuple<string, int, int>("adsIdleAmountScale",                                            0x104, 0x8),
                new Tuple<string, int, int>("idleSpeedFromFireTransitionTime",                               0x108, 0x9),
                new Tuple<string, int, int>("adsScopeBlurAmount",                                            0x10c, 0x8),
                new Tuple<string, int, int>("adsScopeBlurStart",                                             0x110, 0x8),
                new Tuple<string, int, int>("adsSwayViewInsteadOfGun",                                       0x114, 0x6),
                new Tuple<string, int, int>("kickAlignedInputScalar",                                        0x118, 0x8),
                new Tuple<string, int, int>("kickOpposedInputScalar",                                        0x11c, 0x8),
                new Tuple<string, int, int>("swayOverride",                                                  0x120, 0x6),
                new Tuple<string, int, int>("swayMaxAngle",                                                  0x124, 0x8),
                new Tuple<string, int, int>("swayLerpSpeed",                                                 0x128, 0x8),
                new Tuple<string, int, int>("swayPitchScale",                                                0x12c, 0x8),
                new Tuple<string, int, int>("swayYawScale",                                                  0x130, 0x8),
                new Tuple<string, int, int>("swayHorizScale",                                                0x134, 0x8),
                new Tuple<string, int, int>("swayVertScale",                                                 0x138, 0x8),
                new Tuple<string, int, int>("adsSwayOverride",                                               0x13c, 0x6),
                new Tuple<string, int, int>("adsSwayMaxAngle",                                               0x140, 0x8),
                new Tuple<string, int, int>("adsSwayLerpSpeed",                                              0x144, 0x8),
                new Tuple<string, int, int>("adsSwayTransitionLerpSpeed",                                    0x148, 0x8),
                new Tuple<string, int, int>("adsSwayPitchScale",                                             0x14c, 0x8),
                new Tuple<string, int, int>("adsSwayYawScale",                                               0x150, 0x8),
                new Tuple<string, int, int>("adsSwayHorizScale",                                             0x154, 0x8),
                new Tuple<string, int, int>("adsSwayVertScale",                                              0x158, 0x8),
                new Tuple<string, int, int>("adsMoveSpeedScale",                                             0x15c, 0x8),
                new Tuple<string, int, int>("adsFiringSpeedScale",                                           0x160, 0x8),
                new Tuple<string, int, int>("adsMoveSpeedTransitionTimeIn",                                  0x164, 0x9),
                new Tuple<string, int, int>("adsMoveSpeedTransitionTimeOut",                                 0x168, 0x9),
                new Tuple<string, int, int>("hipSpreadMinScale",                                             0x16c, 0x8),
                new Tuple<string, int, int>("hipSpreadMaxScale",                                             0x170, 0x8),
                new Tuple<string, int, int>("strafeRotR",                                                    0x174, 0x8),
                new Tuple<string, int, int>("standMoveF",                                                    0x178, 0x8),
                new Tuple<string, int, int>("standRotP",                                                     0x17c, 0x8),
                new Tuple<string, int, int>("standRotY",                                                     0x180, 0x8),
                new Tuple<string, int, int>("standRotR",                                                     0x184, 0x8),
                new Tuple<string, int, int>("fireTimeScale",                                                 0x188, 0x8),
                new Tuple<string, int, int>("burstDelayTimeScale",                                           0x18c, 0x8),
                new Tuple<string, int, int>("reloadTimeScale",                                               0x190, 0x8),
                new Tuple<string, int, int>("reloadEmptyTimeScale",                                          0x194, 0x8),
                new Tuple<string, int, int>("reloadAddTimeScale",                                            0x198, 0x8),
                new Tuple<string, int, int>("reloadEmptyAddTimeScale",                                       0x19c, 0x8),
                new Tuple<string, int, int>("reloadQuickTimeScale",                                          0x1a0, 0x8),
                new Tuple<string, int, int>("reloadQuickEmptyTimeScale",                                     0x1a4, 0x8),
                new Tuple<string, int, int>("reloadQuickAddTimeScale",                                       0x1a8, 0x8),
                new Tuple<string, int, int>("reloadQuickEmptyAddTimeScale",                                  0x1ac, 0x8),
                new Tuple<string, int, int>("perks2",                                                        0x1e0, 0x5),
                new Tuple<string, int, int>("perks1",                                                        0x1e4, 0x5),
                new Tuple<string, int, int>("perks0",                                                        0x1e8, 0x5),
                new Tuple<string, int, int>("altWeaponAdsOnly",                                              0x28, 0x6),
                new Tuple<string, int, int>("altWeaponDisableSwitching",                                     0x29, 0x6),
                new Tuple<string, int, int>("altScopeADSTransInTime",                                        0x2c, 0x8),
                new Tuple<string, int, int>("altScopeADSTransOutTime",                                       0x30, 0x8),
                new Tuple<string, int, int>("silenced",                                                      0x34, 0x6),
                new Tuple<string, int, int>("dualMag",                                                       0x35, 0x6),
                new Tuple<string, int, int>("laserSight",                                                    0x36, 0x6),
                new Tuple<string, int, int>("infrared",                                                      0x37, 0x6),
                new Tuple<string, int, int>("useAsMelee",                                                    0x38, 0x6),
                new Tuple<string, int, int>("dualWield",                                                     0x39, 0x6),
                new Tuple<string, int, int>("sharedAmmo",                                                    0x3a, 0x6),
                new Tuple<string, int, int>("mmsWeapon",                                                     0x1b0, 0x6),
                new Tuple<string, int, int>("mmsInScope",                                                    0x1b1, 0x6),
                new Tuple<string, int, int>("mmsFOV",                                                        0x1b4, 0x8),
                new Tuple<string, int, int>("mmsAspect",                                                     0x1b8, 0x8),
                new Tuple<string, int, int>("mmsMaxDist",                                                    0x1bc, 0x8),
                new Tuple<string, int, int>("clipSizeScale",                                                 0x1c0, 0x8),
                new Tuple<string, int, int>("clipSize",                                                      0x1c4, 0x4),
                new Tuple<string, int, int>("stackFire",                                                     0x1c8, 0x8),
                new Tuple<string, int, int>("stackFireSpread",                                               0x1cc, 0x8),
                new Tuple<string, int, int>("stackFireAccuracyDecay",                                        0x1d0, 0x8),
                new Tuple<string, int, int>("luiReticle",                                                    0x1d8, 0x0),
                new Tuple<string, int, int>("mods",                                                          0x1e0, 0x3a),
                new Tuple<string, int, int>("mods",                                                          0x1e4, 0x3b),
                new Tuple<string, int, int>("mods",                                                          0x1e8, 0x3c),
                new Tuple<string, int, int>("customFloat0",                                                  0x1f0, 0x8),
                new Tuple<string, int, int>("customFloat1",                                                  0x1f4, 0x8),
                new Tuple<string, int, int>("customFloat2",                                                  0x1f8, 0x8),
                new Tuple<string, int, int>("customFloat3",                                                  0x1fc, 0x8),
                new Tuple<string, int, int>("customFloat4",                                                  0x200, 0x8),
                new Tuple<string, int, int>("customFloat5",                                                  0x204, 0x8),
                new Tuple<string, int, int>("customBool0",                                                   0x208, 0x6),
                new Tuple<string, int, int>("customBool1",                                                   0x20c, 0x6),
                new Tuple<string, int, int>("customBool2",                                                   0x210, 0x6),
                new Tuple<string, int, int>("customBool3",                                                   0x214, 0x6),
                new Tuple<string, int, int>("customBool4",                                                   0x218, 0x6),
                new Tuple<string, int, int>("customBool5",                                                   0x21c, 0x6),
            };

            /// <summary>
            /// Attachment Types
            /// </summary>
            private static readonly string[] AttachmentTypes =
            {
                "none",
                "acog",
                "damage",
                "dualclip",
                "dualoptic",
                "dw",
                "dynzoom",
                "extbarrel",
                "extclip",
                "fastads",
                "fastreload",
                "fmj",
                "gmod0",
                "gmod1",
                "gmod2",
                "gmod3",
                "gmod4",
                "gmod5",
                "gmod6",
                "gmod7",
                "gl",
                "grip",
                "holo",
                "ir",
                "is",
                "mk",
                "mms",
                "notracer",
                "precision",
                "quickdraw",
                "rangefinder",
                "recon",
                "reddot",
                "reflex",
                "rf",
                "sf",
                "stackfire",
                "stalker",
                "steadyaim",
                "supply",
                "suppressed",
                "swayreduc",
                "tacknife",
                "vzoom"
            };

            /// <summary>
            /// Penetration Types
            /// </summary>
            private static readonly string[] PenetrationTypes =
            {
                "none",
                "small",
                "medium",
                "large"
            };

            /// <summary>
            /// Attachment Fire Types
            /// </summary>
            private static readonly string[] AttachmentFireTypes =
            {
                "Full Auto",
                "Single Shot",
                "Burst",
                "Auto Burst",
                "Stacked Fire"
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
            public string Name => "attachment";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Attachment";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.attachment;

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
                        Name = instance.Reader.ReadNullTerminatedString(namePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = "Type: " + AttachmentTypes[instance.Reader.ReadInt32(address + 0x10)]
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

                var result = ConvertAssetBufferToGDTAsset(buffer, AttachmentOffsets, instance, HandleAttachmentSettings);

                result.Type = "attachment";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Attachment Specific Settings
            /// </summary>
            private static object HandleAttachmentSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return AttachmentTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x38:
                        return PenetrationTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x39:
                        return AttachmentFireTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x3A:
                    case 0x3B:
                    case 0x3C:
                        return instance.Game.GetString(BitConverter.ToInt32(assetBuffer, offset), instance);
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
