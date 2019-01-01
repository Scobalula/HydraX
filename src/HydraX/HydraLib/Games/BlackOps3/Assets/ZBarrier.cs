using PhilLibX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HydraLib.Assets;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class ZBarrier
        {
            /// <summary>
            /// Bo3 AnimMappingTable Header
            /// </summary>
            public struct ZBarrierPiece
            {
                /// <summary>
                /// Pointer to the Piece Model Asset
                /// </summary>
                public long PieceModelPointer { get; set; }

                /// <summary>
                /// Pointer to the Piece Model Asset
                /// </summary>
                public long AlternatePieceModelPointer { get; set; }

                /// <summary>
                /// Pointer to the Piece Model Asset
                /// </summary>
                public long UpgradedPieceModelPointer { get; set; }

                /// <summary>
                /// Pointer to the name of the opening anim
                /// </summary>
                public long OpeningAnimNamePointer { get; set; }

                /// <summary>
                /// Pointer to the name of the closing anim
                /// </summary>
                public long ClosingAnimNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Closing FX Asset
                /// </summary>
                public long FirstClosingFXPointer { get; set; }

                /// <summary>
                /// Pointer to the Closing FX Asset
                /// </summary>
                public long SecondClosingFXPointer { get; set; }

                /// <summary>
                /// Offset of the Closing FX
                /// </summary>
                public Vector3 FirstClosingFXOffset { get; set; }

                /// <summary>
                /// Offset of the Closing FX
                /// </summary>
                public Vector3 SecondClosingFXOffset { get; set; }

                /// <summary>
                /// Hash of the Board Closing Sound
                /// </summary>
                public uint BoardClosingSoundHash { get; set; }

                /// <summary>
                /// Hash of the Board Closing Sound
                /// </summary>
                public uint BoardClosingHoverSoundHash { get; set; }

                /// <summary>
                /// Whether or not to repeat the board closing sound
                /// </summary>
                public int PauseAndRepeat { get; set; }

                /// <summary>
                /// Minimum pause between FX and sounds
                /// </summary>
                public float MinPause { get; set; }

                /// <summary>
                /// Maximum pause between FX and sounds
                /// </summary>
                public float MaxPause { get; set; }

                /// <summary>
                /// Animation State String Index
                /// </summary>
                public int AnimStateStringIndex { get; set; }

                /// <summary>
                /// Animation Sub-State String Index
                /// </summary>
                public int AnimSubStateStringIndex { get; set; }

                /// <summary>
                /// Number of loop anim repeats
                /// </summary>
                public int LoopAnimRepsCount { get; set; }
            }

            /// <summary>
            /// Bo3 AnimMappingTable Header
            /// </summary>
            public struct ZBarrierAsset
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Hash of the General Repair Sound
                /// </summary>
                public uint GeneralRepairSoundHash { get; set; }

                /// <summary>
                /// Hash of the General Repair Sound
                /// </summary>
                public uint SecondGeneralRepairSoundHash { get; set; }

                /// <summary>
                /// Hash of the General Repair Sound
                /// </summary>
                public uint UpgradedGeneralRepairSoundHash { get; set; }

                /// <summary>
                /// Hash of the General Repair Sound
                /// </summary>
                public uint SecondUpgradedGeneralRepairSoundHash { get; set; }

                /// <summary>
                /// Whether or not we delay between the general repair sounds
                /// </summary>
                public int DelayBetweenSounds { get;set; }

                /// <summary>
                /// The delay between each sound
                /// </summary>
                public float Delay { get; set; }

                /// <summary>
                /// Whether or not we execute an earthquake/shock on repair
                /// </summary>
                public int EarthquakeOnRepair { get; set; }

                /// <summary>
                /// Minimum Earthquake Scale
                /// </summary>
                public float EarthquakeMinScale { get; set; }

                /// <summary>
                /// Maximum Earthquake Scale
                /// </summary>
                public float EarthquakeMaxScale { get; set; }

                /// <summary>
                /// Minimum Earthquake Duration
                /// </summary>
                public float EarthquakeMinDuration { get; set; }

                /// <summary>
                /// Maximum Earthquake Duration
                /// </summary>
                public float EarthquakeMaxDuration { get; set; }

                /// <summary>
                /// Earthquake Radius
                /// </summary>
                public float EarthquakeRadius { get; set; }

                /// <summary>
                /// Number of Pieces (6 max)
                /// </summary>
                public int PieceCount { get; set; }

                /// <summary>
                /// Whether or not we should hide open peices
                /// </summary>
                public int AutoHideOpenPieces { get; set; }

                /// <summary>
                /// Whether or not the zombies should do taunts
                /// </summary>
                public int ZombiesTaunt { get; set; }

                /// <summary>
                /// Whether or not the zombies should reach through the barrier
                /// </summary>
                public int ZombiesReachThrough { get; set; }

                /// <summary>
                /// Animation State String Index
                /// </summary>
                public int AnimStateStringIndex { get; set; }

                /// <summary>
                /// Animation Sub-State String Index
                /// </summary>
                public int AnimSubStateStringIndex { get; set; }

                /// <summary>
                /// Number of attack slots
                /// </summary>
                public int AttackSlotCount { get; set; }

                /// <summary>
                /// Horizontal Attack Offset
                /// </summary>
                public float AttackHorizontalOffset { get; set; }

                /// <summary>
                /// Pointer to the Piece Model Asset
                /// </summary>
                public long CollisionModelPointer { get; set; }

                /// <summary>
                /// ZBarrier Pieces
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
                public ZBarrierPiece[] Pieces;
            }

            public static void Load(AssetPool assetPool)
            {
                // Loop through entire pool, and add what valid assets
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Rawfile
                    var animSelectorTable = Hydra.ActiveGameReader.ReadStruct<ZBarrierAsset>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(animSelectorTable.NamePointer))
                        continue;
                    // Add Asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(animSelectorTable.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = "N/A",
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var zbarrier = Hydra.ActiveGameReader.ReadStruct<ZBarrierAsset>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(zbarrier.NamePointer))
                    return false;
                // Convert to generic ZBarrier and add base settings for GDT
                Hydra.GDTs["Misc"].AddAsset(asset.Name, "zbarrier", new ZBarrierObj()
                {
                    GeneralRepairSound0             = Sound.GetAliasByHash(zbarrier.GeneralRepairSoundHash),
                    GeneralRepairSound1             = Sound.GetAliasByHash(zbarrier.SecondGeneralRepairSoundHash),
                    UpgradedGeneralRepairSound0     = Sound.GetAliasByHash(zbarrier.UpgradedGeneralRepairSoundHash),
                    UpgradedGeneralRepairSound1     = Sound.GetAliasByHash(zbarrier.SecondUpgradedGeneralRepairSoundHash),
                    UseDelayBetweenGeneralRepSounds = zbarrier.DelayBetweenSounds.ToString(),
                    DelayBetweenGeneralRepSounds    = zbarrier.Delay.ToString(),
                    EarthquakeOnRepair              = zbarrier.EarthquakeOnRepair.ToString(),
                    EarthquakeMinScale              = zbarrier.EarthquakeMinScale.ToString(),
                    EarthquakeMaxScale              = zbarrier.EarthquakeMaxScale.ToString(),
                    EarthquakeMinDuration           = zbarrier.EarthquakeMinDuration.ToString(),
                    EarthquakeMaxDuration           = zbarrier.EarthquakeMaxDuration.ToString(),
                    EarthquakeRadius                = zbarrier.EarthquakeRadius.ToString(),
                    AutoHideOpenPieces              = zbarrier.AutoHideOpenPieces.ToString(),
                    Taunts                          = zbarrier.ZombiesTaunt.ToString(),
                    ReachThroughAttacks             = zbarrier.ZombiesReachThrough.ToString(),
                    ZombieTauntAnimState            = GetString(zbarrier.AnimStateStringIndex),
                    ZombieReachThroughAnimState     = GetString(zbarrier.AnimSubStateStringIndex),
                    NumAttackSlots                  = zbarrier.AttackSlotCount.ToString(),
                    AttackSpotHorzOffset            = zbarrier.AttackHorizontalOffset.ToString(),
                    CollisionModel                  = GetAssetName(zbarrier.CollisionModelPointer),
                });
                // Loop through pieces
                for(int i = 0; i < zbarrier.PieceCount; i++)
                {
                    // Add it to asset
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("boardModel{0}", i + 1)]                     = GetAssetName(zbarrier.Pieces[i].PieceModelPointer);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("upgradedBoardModel{0}", i + 1)]             = GetAssetName(zbarrier.Pieces[i].UpgradedPieceModelPointer);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("alternateBoardModel{0}", i + 1)]            = GetAssetName(zbarrier.Pieces[i].AlternatePieceModelPointer);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("boardAnim{0}", i + 1)]                      = Hydra.ActiveGameReader.ReadNullTerminatedString(zbarrier.Pieces[i].ClosingAnimNamePointer);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("tearAnim{0}", i + 1)]                       = Hydra.ActiveGameReader.ReadNullTerminatedString(zbarrier.Pieces[i].OpeningAnimNamePointer);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("boardRepairSound{0}", i + 1)]               = Sound.GetAliasByHash(zbarrier.Pieces[i].BoardClosingSoundHash);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("boardRepairHoverSound{0}", i + 1)]          = Sound.GetAliasByHash(zbarrier.Pieces[i].BoardClosingHoverSoundHash);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("repairFx{0}0", i + 1)]                      = Hydra.CleanFXName(GetAssetName(zbarrier.Pieces[i].FirstClosingFXPointer));
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("repairFx{0}1", i + 1)]                      = Hydra.CleanFXName(GetAssetName(zbarrier.Pieces[i].SecondClosingFXPointer));
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxX{0}0", i + 1)]               = zbarrier.Pieces[i].FirstClosingFXOffset.X.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxY{0}0", i + 1)]               = zbarrier.Pieces[i].FirstClosingFXOffset.Y.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxZ{0}0", i + 1)]               = zbarrier.Pieces[i].FirstClosingFXOffset.Z.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxX{0}1", i + 1)]               = zbarrier.Pieces[i].SecondClosingFXOffset.X.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxY{0}1", i + 1)]               = zbarrier.Pieces[i].SecondClosingFXOffset.Y.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("OffsetRepairFxZ{0}1", i + 1)]               = zbarrier.Pieces[i].SecondClosingFXOffset.Z.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("zombieBoardTearAnimState{0}", i + 1)]       = GetString(zbarrier.Pieces[i].AnimStateStringIndex);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("zombieBoardTearAnimSubState{0}", i + 1)]    = GetString(zbarrier.Pieces[i].AnimSubStateStringIndex);
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("pauseAndRepeatBoardRepairSound{0}", i + 1)] = zbarrier.Pieces[i].PauseAndRepeat.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("pauseBetweenRepSoundsMin{0}", i + 1)]       = zbarrier.Pieces[i].MinPause.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("pauseBetweenRepSoundsMax{0}", i + 1)]       = zbarrier.Pieces[i].MaxPause.ToString();
                    Hydra.GDTs["Misc"].Assets[asset.Name].Properties[String.Format("proBoardNumRepsToTear{0}", i + 1)]          = zbarrier.Pieces[i].LoopAnimRepsCount.ToString();
                }
                // Done
                return true;
            }
        }
    }
}
