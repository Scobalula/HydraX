using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 ZBarrier Logic
        /// </summary>
        private class ZBarrier : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// ZBarrier Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct ZBarrierAsset
            {
                #region WeaponCamoProperties
                public long NamePointer { get; set; }
                public uint GeneralRepairSoundHash { get; set; }
                public uint SecondGeneralRepairSoundHash { get; set; }
                public uint UpgradedGeneralRepairSoundHash { get; set; }
                public uint SecondUpgradedGeneralRepairSoundHash { get; set; }
                public int DelayBetweenSounds { get; set; }
                public float Delay { get; set; }
                public int EarthquakeOnRepair { get; set; }
                public float EarthquakeMinScale { get; set; }
                public float EarthquakeMaxScale { get; set; }
                public float EarthquakeMinDuration { get; set; }
                public float EarthquakeMaxDuration { get; set; }
                public float EarthquakeRadius { get; set; }
                public int PieceCount { get; set; }
                public int AutoHideOpenPieces { get; set; }
                public int ZombiesTaunt { get; set; }
                public int ZombiesReachThrough { get; set; }
                public int AnimStateStringIndex { get; set; }
                public int AnimSubStateStringIndex { get; set; }
                public int AttackSlotCount { get; set; }
                public float AttackHorizontalOffset { get; set; }
                public long CollisionModelPointer { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                public ZBarrierPiece[] Pieces;
                #endregion
            }

            /// <summary>
            /// ZBarrier Peice Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct ZBarrierPiece
            {
                #region WeaponCamoEntryProperties
                public long PieceModelPointer { get; set; }
                public long AlternatePieceModelPointer { get; set; }
                public long UpgradedPieceModelPointer { get; set; }
                public long OpeningAnimNamePointer { get; set; }
                public long ClosingAnimNamePointer { get; set; }
                public long FirstClosingFXPointer { get; set; }
                public long SecondClosingFXPointer { get; set; }
                public Vector3 FirstClosingFXOffset { get; set; }
                public Vector3 SecondClosingFXOffset { get; set; }
                public uint BoardClosingSoundHash { get; set; }
                public uint BoardClosingHoverSoundHash { get; set; }
                public int PauseAndRepeat { get; set; }
                public float MinPause { get; set; }
                public float MaxPause { get; set; }
                public int AnimStateStringIndex { get; set; }
                public int AnimSubStateStringIndex { get; set; }
                public int LoopAnimRepsCount { get; set; }
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
            public string Name => "zbarrier";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.zbarrier;

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
                    var header = instance.Reader.ReadStruct<ZBarrierAsset>(StartAddress + (i * AssetSize));

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
                var header = instance.Reader.ReadStruct<ZBarrierAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                var zbarrierAsset = new GameDataTable.Asset(asset.Name, "zbarrier");

                zbarrierAsset["generalRepairSound0"]             = GetAliasByHash(header.GeneralRepairSoundHash);
                zbarrierAsset["generalRepairSound1"]             = GetAliasByHash(header.SecondGeneralRepairSoundHash);
                zbarrierAsset["upgradedGeneralRepairSound0"]     = GetAliasByHash(header.UpgradedGeneralRepairSoundHash);
                zbarrierAsset["upgradedGeneralRepairSound1"]     = GetAliasByHash(header.SecondUpgradedGeneralRepairSoundHash);
                zbarrierAsset["useDelayBetweenGeneralRepSounds"] = header.DelayBetweenSounds.ToString();
                zbarrierAsset["delayBetweenGeneralRepSounds"]    = header.Delay.ToString();
                zbarrierAsset["earthquakeOnRepair"]              = header.EarthquakeOnRepair.ToString();
                zbarrierAsset["earthquakeMinScale"]              = header.EarthquakeMinScale.ToString();
                zbarrierAsset["earthquakeMaxScale"]              = header.EarthquakeMaxScale.ToString();
                zbarrierAsset["earthquakeMinDuration"]           = header.EarthquakeMinDuration.ToString();
                zbarrierAsset["earthquakeMaxDuration"]           = header.EarthquakeMaxDuration.ToString();
                zbarrierAsset["earthquakeRadius"]                = header.EarthquakeRadius.ToString();
                zbarrierAsset["autoHideOpenPieces"]              = header.AutoHideOpenPieces.ToString();
                zbarrierAsset["taunts"]                          = header.ZombiesTaunt.ToString();
                zbarrierAsset["reachThroughAttacks"]             = header.ZombiesReachThrough.ToString();
                zbarrierAsset["zombieTauntAnimState"]            = instance.Game.GetString(header.AnimStateStringIndex, instance);
                zbarrierAsset["zombieReachThroughAnimState"]     = instance.Game.GetString(header.AnimSubStateStringIndex, instance);
                zbarrierAsset["numAttackSlots"]                  = header.AttackSlotCount.ToString();
                zbarrierAsset["attackSpotHorzOffset"]            = header.AttackHorizontalOffset.ToString();
                zbarrierAsset["collisionModel"]                  = instance.Game.GetAssetName(header.CollisionModelPointer, instance);

                for(int i = 0; i < header.PieceCount; i++)
                {
                    // Add it to asset
                    zbarrierAsset[string.Format("boardModel{0}", i + 1)]                     = instance.Game.GetAssetName(header.Pieces[i].PieceModelPointer, instance);
                    zbarrierAsset[string.Format("upgradedBoardModel{0}", i + 1)]             = instance.Game.GetAssetName(header.Pieces[i].UpgradedPieceModelPointer, instance);
                    zbarrierAsset[string.Format("alternateBoardModel{0}", i + 1)]            = instance.Game.GetAssetName(header.Pieces[i].AlternatePieceModelPointer, instance);
                    zbarrierAsset[string.Format("boardAnim{0}", i + 1)]                      = instance.Reader.ReadNullTerminatedString(header.Pieces[i].ClosingAnimNamePointer);
                    zbarrierAsset[string.Format("tearAnim{0}", i + 1)]                       = instance.Reader.ReadNullTerminatedString(header.Pieces[i].OpeningAnimNamePointer);
                    zbarrierAsset[string.Format("boardRepairSound{0}", i + 1)]               = GetAliasByHash(header.Pieces[i].BoardClosingSoundHash);
                    zbarrierAsset[string.Format("boardRepairHoverSound{0}", i + 1)]          = GetAliasByHash(header.Pieces[i].BoardClosingHoverSoundHash);
                    zbarrierAsset[string.Format("repairFx{0}0", i + 1)]                      = instance.Game.CleanAssetName(HydraAssetType.FX, instance.Game.GetAssetName(header.Pieces[i].FirstClosingFXPointer, instance));
                    zbarrierAsset[string.Format("repairFx{0}1", i + 1)]                      = instance.Game.CleanAssetName(HydraAssetType.FX, instance.Game.GetAssetName(header.Pieces[i].SecondClosingFXPointer, instance));
                    zbarrierAsset[string.Format("OffsetRepairFxX{0}0", i + 1)]               = header.Pieces[i].FirstClosingFXOffset.X.ToString();
                    zbarrierAsset[string.Format("OffsetRepairFxY{0}0", i + 1)]               = header.Pieces[i].FirstClosingFXOffset.Y.ToString();
                    zbarrierAsset[string.Format("OffsetRepairFxZ{0}0", i + 1)]               = header.Pieces[i].FirstClosingFXOffset.Z.ToString();
                    zbarrierAsset[string.Format("OffsetRepairFxX{0}1", i + 1)]               = header.Pieces[i].SecondClosingFXOffset.X.ToString();
                    zbarrierAsset[string.Format("OffsetRepairFxY{0}1", i + 1)]               = header.Pieces[i].SecondClosingFXOffset.Y.ToString();
                    zbarrierAsset[string.Format("OffsetRepairFxZ{0}1", i + 1)]               = header.Pieces[i].SecondClosingFXOffset.Z.ToString();
                    zbarrierAsset[string.Format("zombieBoardTearAnimState{0}", i + 1)]       = instance.Game.GetString(header.Pieces[i].AnimStateStringIndex, instance);
                    zbarrierAsset[string.Format("zombieBoardTearAnimSubState{0}", i + 1)]    = instance.Game.GetString(header.Pieces[i].AnimSubStateStringIndex, instance);
                    zbarrierAsset[string.Format("pauseAndRepeatBoardRepairSound{0}", i + 1)] = header.Pieces[i].PauseAndRepeat.ToString();
                    zbarrierAsset[string.Format("pauseBetweenRepSoundsMin{0}", i + 1)]       = header.Pieces[i].MinPause.ToString();
                    zbarrierAsset[string.Format("pauseBetweenRepSoundsMax{0}", i + 1)]       = header.Pieces[i].MaxPause.ToString();
                    zbarrierAsset[string.Format("proBoardNumRepsToTear{0}", i + 1)]          = header.Pieces[i].LoopAnimRepsCount.ToString();
                }

                instance.GDTs["Misc"][asset.Name] = zbarrierAsset;

                return HydraStatus.Success;
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
