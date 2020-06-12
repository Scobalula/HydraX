using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;
using PhilLibX;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Weapon Camo Logic
        /// </summary>
        private class WeaponCamo : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Material Flags
            /// </summary>
            private enum MaterialUsageFlags : byte
            {
                ColorMap         = 0x1,
                NormalMap        = 0x2,
                SpecularMap      = 0x4,
                GlossMap         = 0x8,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Weapon Camo Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x28)]
            private struct WeaponCamoAsset
            {
                #region WeaponCamoProperties
                public long NamePointer;
                public long CamosPointer;
                public int CamoCount;
                #endregion
            }

            /// <summary>
            /// Weapon Camo Entry Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct WeaponCamoEntry
            {
                #region WeaponCamoEntryProperties
                public int MaterialsCount;
                public long MaterialsPointer;
                #endregion
            }

            /// <summary>
            /// Weapon Camo Material Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x138)]
            private struct WeaponCamoMaterial
            {
                #region WeaponCamoEntryProperties
                public int BaseMaterialCount;
                public long BaseMaterialsPointer;
                public long Unk;
                public MaterialUsageFlags Flags;
                public long MaterialPointer;
                public Vector2 Translation;
                public Vector2 Scale;
                public float Rotation;
                public float GlossMapBlend;
                public float NormalBlend;
                public long DetailNormalMapPointer;
                public float NormalHeight;
                public Vector2 NormalScale;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 220)]
                public byte[] NullPadding;
                #endregion
            }

            /// <summary>
            /// Weapon Camo Base Material Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct WeaponCamoBaseMaterial
            {
                #region WeaponCamoEntryProperties
                public long MaterialPointer;
                public long CamoMaskPointer;
                #endregion
            }

            /// <summary>
            /// SurfaceFXTable Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x28)]
            private struct SurfaceFXTableAsset
            {
                #region SurfaceFXTableProperties
                public long NamePointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x28)]
                public long[] SurfaceFXPointers;
                #endregion
            }

            /// <summary>
            /// EntityFXTable Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x28)]
            private struct EntityFXTableAsset
            {
                #region EntityFXTableProperties
                public long NamePointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30)]
                public byte[] NullBytes;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3C)]
                public long[] EntityFXPointers;
                #endregion
            }
            #endregion

            #region Tables
            /// <summary>
            /// Entity FX GDT Keys
            /// </summary>
            private static readonly string[] EntityFXKeys =
            {
                "fleshBody",
                "fleshBodyFatal",
                "fleshHead",
                "fleshHeadFatal",
                "fleshLimb",
                "fleshLimbFatal",
                "zombieBody",
                "zombieBodyFatal",
                "zombieHead",
                "zombieHeadFatal",
                "zombieLimb",
                "zombieLimbFatal",
                "armorLightBody",
                "armorLightBodyFatal",
                "armorLightHead",
                "armorLightHeadFatal",
                "armorLightLimb",
                "armorLightLimbFatal",
                "armorHeavyBody",
                "armorHeavyBodyFatal",
                "armorHeavyHead",
                "armorHeavyHeadFatal",
                "armorHeavyLimb",
                "armorHeavyLimbFatal",
                "robotLightBody",
                "robotLightBodyFatal",
                "robotLightHead",
                "robotLightHeadFatal",
                "robotLightLimb",
                "robotLightLimbFatal",
                "robotHeavyBody",
                "robotHeavyBodyFatal",
                "robotHeavyHead",
                "robotHeavyHeadFatal",
                "robotHeavyLimb",
                "robotHeavyLimbFatal",
                "robotBossBody",
                "robotBossBodyFatal",
                "robotBossHead",
                "robotBossHeadFatal",
                "robotBossLimb",
                "robotBossLimbFatal",
                "powerArmorBody",
                "powerArmorBodyFatal",
                "powerArmorHead",
                "powerArmorHeadFatal",
                "powerArmorLimb",
                "powerArmorLimbFatal",
                "skeletonBody",
                "skeletonBodyFatal",
                "skeletonHead",
                "skeletonHeadFatal",
                "skeletonLimb",
                "skeletonLimbFatal",
                "fleshCorpseBody",
                "fleshCorpseBodyFatal",
                "fleshCorpseHead",
                "fleshCorpseHeadFatal",
                "fleshCorpseLimb",
                "fleshCorpseLimbFatal",
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
            public string Name => "weaponcamo";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Weapon";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.weaponcamo;

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
                    var header = instance.Reader.ReadStruct<WeaponCamoAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format("Camos: {0}", header.CamoCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<WeaponCamoAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var weaponCamos = instance.Reader.ReadArray<WeaponCamoEntry>(header.CamosPointer, header.CamoCount);
                var weaponCamoAssetNames = new string[10];
                var weaponCamoAssetCount = (int)Math.Ceiling(weaponCamos.Length / 75.0);

                for(int i = 0; i < weaponCamoAssetCount; i++)
                {
                    int camoCount = MathUtilities.Clamp(weaponCamos.Length - (i * 75), 75, 0);

                    int baseIndex = i * 75;

                    weaponCamoAssetNames[i] = string.Format("{0}{1}", asset.Name, i == 0 ? "_ship" : "_base" + (baseIndex + 1).ToString());

                    var weaponCamoAsset = new GameDataTable.Asset(weaponCamoAssetNames[i], "weaponcamo");

                    weaponCamoAsset.Properties["configstringFileType"] = "WEAPONCAMO";
                    weaponCamoAsset.Properties["baseIndex"] = baseIndex + 1;
                    weaponCamoAsset.Properties["numCamos"] = camoCount;

                    for (int j = 0; j < camoCount; j++)
                    {
                        var weaponCamoMaterial = weaponCamos[baseIndex + j];

                        var materials = instance.Reader.ReadArray<WeaponCamoMaterial>(weaponCamoMaterial.MaterialsCount == 0 ? 0 : weaponCamoMaterial.MaterialsPointer, weaponCamoMaterial.MaterialsCount == 0 ? 1 : weaponCamoMaterial.MaterialsCount);

                        for(int k = 0; k < materials.Length; k++)
                        {
                            var weaponCamoBaseMaterials = instance.Reader.ReadArray<WeaponCamoBaseMaterial>(materials[k].BaseMaterialsPointer, materials[k].BaseMaterialCount);

                            for(int l = 0; l < weaponCamoBaseMaterials.Length; l++)
                            {
                                weaponCamoAsset[string.Format("material{0}_{1}_base_material_{2}", k + 1, j + 1, l + 1)] = Path.GetFileNameWithoutExtension(instance.Game.GetAssetName(weaponCamoBaseMaterials[l].MaterialPointer, instance));
                                weaponCamoAsset[string.Format("material{0}_{1}_camo_mask_{2}", k + 1, j + 1, l + 1)] = instance.Game.GetAssetName(weaponCamoBaseMaterials[l].CamoMaskPointer, instance, 0xF8);
                            }

                            weaponCamoAsset[string.Format("material{0}_{1}_detail_normal_height", k + 1, j + 1)]  = materials[k].NormalHeight;
                            weaponCamoAsset[string.Format("material{0}_{1}_detail_normal_map", k + 1, j + 1)]     = instance.Game.GetAssetName(materials[k].DetailNormalMapPointer, instance, 0xF8);
                            weaponCamoAsset[string.Format("material{0}_{1}_detail_normal_scale_x", k + 1, j + 1)] = materials[k].NormalScale.X;
                            weaponCamoAsset[string.Format("material{0}_{1}_detail_normal_scale_y", k + 1, j + 1)] = materials[k].NormalScale.Y;
                            weaponCamoAsset[string.Format("material{0}_{1}_gloss_blend", k + 1, j + 1)]           = materials[k].GlossMapBlend;
                            weaponCamoAsset[string.Format("material{0}_{1}_normal_amount", k + 1, j + 1)]         = materials[k].NormalBlend;
                            weaponCamoAsset[string.Format("material{0}_{1}_material", k + 1, j + 1)]              = Path.GetFileNameWithoutExtension(instance.Game.GetAssetName(materials[k].MaterialPointer, instance));
                            weaponCamoAsset[string.Format("material{0}_{1}_numBaseMaterials", k + 1, j + 1)]      = materials[k].BaseMaterialCount;
                            weaponCamoAsset[string.Format("material{0}_{1}_rotation", k + 1, j + 1)]              = materials[k].Rotation;
                            weaponCamoAsset[string.Format("material{0}_{1}_scale_x", k + 1, j + 1)]               = materials[k].Scale.X;
                            weaponCamoAsset[string.Format("material{0}_{1}_scale_y", k + 1, j + 1)]               = materials[k].Scale.Y;
                            weaponCamoAsset[string.Format("material{0}_{1}_trans_x", k + 1, j + 1)]               = materials[k].Translation.X;
                            weaponCamoAsset[string.Format("material{0}_{1}_trans_y", k + 1, j + 1)]               = materials[k].Translation.Y;
                            weaponCamoAsset[string.Format("material{0}_{1}_useGlossMap", k + 1, j + 1)]           = materials[k].Flags.HasFlag(MaterialUsageFlags.GlossMap);
                            weaponCamoAsset[string.Format("material{0}_{1}_useNormalMap", k + 1, j + 1)]          = materials[k].Flags.HasFlag(MaterialUsageFlags.NormalMap);
                        }

                    }

                    instance.AddGDTAsset(weaponCamoAsset, weaponCamoAsset.Type, weaponCamoAsset.Name);
                }

                var weaponCamoTableAsset = new GameDataTable.Asset(asset.Name, "weaponcamotable");
                weaponCamoTableAsset["configstringFileType"] = "WEAPONCAMO";
                weaponCamoTableAsset["numCamoTables"] = weaponCamoAssetNames.Count(x => x != null);


                for (int i = 0; i < weaponCamoAssetNames.Length; i++)
                    weaponCamoTableAsset[string.Format("table_{0:D2}_name", i + 1)] = weaponCamoAssetNames[i];

                instance.AddGDTAsset(weaponCamoTableAsset, weaponCamoTableAsset.Type, weaponCamoTableAsset.Name);

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
