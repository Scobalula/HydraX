using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Attachment Cosmetic Variant Logic
        /// </summary>
        private class AttachmentCosmeticVariant : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Sub State Flags
            /// </summary>
            private enum Flags : int
            {
                Terminal            = 0x1,
                LoopSync            = 0x2,
                MultipleDelta       = 0x4,
                Parametric2D        = 0x8,
                Coderate            = 0x10,
                AllowTransDecAim    = 0x20,
                ForceFire           = 0x40,
                CleanLoop           = 0x80,
                AnimDrivenLocmotion = 0x100,
                SpeedBlend          = 0x200,
            }

            /// <summary>
            /// Sub State Requirements
            /// </summary>
            private enum Requirements : int
            {
                DeltaRequiresTranslation = 0x1,
                RequiresRagdollNotetrack = 0x2,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Attachment Cosmetic Variant Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x18)]
            private struct AttachmentCosmeticVariantAsset
            {
                #region AnimationStateMachineProperties
                public long NamePointer;
                public long VariantsPointer;
                public int VariantCount;
                #endregion
            }

            /// <summary>
            /// Variant Model
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct VariantModel
            {
                #region AnimationStateMachineProperties
                public long ModelPointer;
                public long ADSModelPointer;
                public long TagPointer;
                public Vector3 Position;
                public Vector3 Rotation;
                #endregion
            }

            /// <summary>
            /// Variant Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct Variant
            {
                #region AnimationStateMachineProperties
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public VariantModel[] VariantModels;
                public long ShortDisplayNamePointer;
                public long LongDisplayNamePointer;
                public long DescriptionPointer;
                public long MaterialPointer;
                #endregion
            }
            #endregion

            #region Tables
            /// <summary>
            /// Variant Model Names
            /// </summary>
            private static readonly string[] ModelNames =
            {
                "viewModel",
                "worldModel",
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
            public string Name => "attachmentcosmeticvariant";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Weapon";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.attachmentcosmeticvariant;

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
                    var header = instance.Reader.ReadStruct<AttachmentCosmeticVariantAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type = Name,
                        Status = "Loaded",
                        Data = address,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        LoadMethod = ExportAsset,
                        Information = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<AttachmentCosmeticVariantAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var result = new GameDataTable.Asset(asset.Name, "attachmentcosmeticvariant");

                var variants = instance.Reader.ReadArray<Variant>(header.VariantsPointer, header.VariantCount);

                for(int i = 0; i < variants.Length; i++)
                {
                    for(int j = 0; j < variants[i].VariantModels.Length; j++)
                    {
                        result[string.Format("acv{0}_{1}_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]            = instance.Game.GetAssetName(variants[i].VariantModels[j].ModelPointer, instance);
                        result[string.Format("acv{0}_{1}ADS_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]         = instance.Game.GetAssetName(variants[i].VariantModels[j].ADSModelPointer, instance);
                        result[string.Format("acv{0}_{1}Tag_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]         = instance.Reader.ReadNullTerminatedString(variants[i].VariantModels[j].TagPointer);
                        result[string.Format("acv{0}_{1}OffsetX_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]     = variants[i].VariantModels[j].Position.X;
                        result[string.Format("acv{0}_{1}OffsetY_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]     = variants[i].VariantModels[j].Position.Y;
                        result[string.Format("acv{0}_{1}OffsetZ_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]     = variants[i].VariantModels[j].Position.Z;
                        result[string.Format("acv{0}_{1}OffsetPitch_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)] = variants[i].VariantModels[j].Rotation.X;
                        result[string.Format("acv{0}_{1}OffsetYaw_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]   = variants[i].VariantModels[j].Rotation.Y;
                        result[string.Format("acv{0}_{1}OffsetRoll_model{2}", i, ModelNames[j < 2 ? 0 : 1], j % 2)]  = variants[i].VariantModels[j].Rotation.Z;
                    }

                    result[string.Format("acv{0}_description", i)]      = instance.Reader.ReadNullTerminatedString(variants[i].DescriptionPointer);
                    result[string.Format("acv{0}_displayNameShort", i)] = instance.Reader.ReadNullTerminatedString(variants[i].ShortDisplayNamePointer);
                    result[string.Format("acv{0}_displayNameLong", i)]  = instance.Reader.ReadNullTerminatedString(variants[i].LongDisplayNamePointer);
                    result[string.Format("acv{0}_uiMaterial", i)]       = instance.Game.GetAssetName(variants[i].MaterialPointer, instance, 0xF8);
                }

                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                // Done
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
