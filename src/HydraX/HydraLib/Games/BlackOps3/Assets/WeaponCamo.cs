using HydraLib.Assets;
using PhilLibX;
using PhilLibX.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class WeaponCamo
        {
            /// <summary>
            /// Bo3 Raw File Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct WeaponCamoHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the Camo Entries
                /// </summary>
                public long CamosPointer { get; set; }

                /// <summary>
                /// Number of Camos 
                /// </summary>
                public int CamoCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            public struct CamoTypeHeader
            {
                /// <summary>
                /// Number of Material Overrides 
                /// </summary>
                public int MaterialCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Material Overrides
                /// </summary>
                public long MaterialsPointer { get; set; }
            }

            public struct CamoTypeMaterial
            {
                public struct BaseMaterial
                {
                    /// <summary>
                    /// Pointer to the Material Asset
                    /// </summary>
                    public long MaterialPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Camo Mask Image Asset
                    /// </summary>
                    public long CamoMaskImagePointer { get; set; }
                }

                /// <summary>
                /// Number of Base Materials
                /// </summary>
                public int BaseMaterialCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Base Materials
                /// </summary>
                public long BaseMaterialsPointer { get; set; }

                /// <summary>
                /// Constant 1?
                /// </summary>
                public long Unk { get; set; }

                /// <summary>
                /// Flags (Use Normal Map and Use Gloss Map)
                /// </summary>
                public long Flags { get; set; }

                /// <summary>
                /// Pointer to the material that overrides the base materials
                /// </summary>
                public long MaterialPointer { get; set; }

                /// <summary>
                /// Camo Translation/Offset
                /// </summary>
                public Vector2 Translation { get; set; }

                /// <summary>
                /// Camo Scale
                /// </summary>
                public Vector2 Scale { get; set; }

                /// <summary>
                /// Camo Rotation
                /// </summary>
                public float Rotation { get; set; }

                /// <summary>
                /// Gloss Map Blend Amount
                /// </summary>
                public float GlossMapBlend { get; set; }

                /// <summary>
                /// Normal Map Blend Amount
                /// </summary>
                public float NormalMapBlend { get; set; }

                /// <summary>
                /// Pointer to the Detail Normal Image Asset
                /// </summary>
                public long DetailNormalMapPointer { get; set; }

                /// <summary>
                /// Detail Normal Heigh
                /// </summary>
                public float NormalHeight { get; set; }

                /// <summary>
                /// Detail Norml Scale
                /// </summary>
                public Vector2 NormalScale { get; set; }

                /// <summary>
                /// Unused Properties
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 220)]
                public byte[] NullPadding;
            }

            /// <summary>
            /// Loads the Assets for this type from Memory
            /// </summary>
            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Asset
                    var weaponCamoHeader = Hydra.ActiveGameReader.ReadStruct<WeaponCamoHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(weaponCamoHeader.NamePointer))
                        continue;
                    // Create new asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(weaponCamoHeader.NamePointer),
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = String.Format("Camos: {0}", weaponCamoHeader.CamoCount)
                    });
                }
            }

            /// <summary>
            /// Exports the given asset from Memory
            /// </summary>
            public static bool Export(Asset asset)
            {
                // Read Header
                var weaponCamoHeader = Hydra.ActiveGameReader.ReadStruct<WeaponCamoHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(weaponCamoHeader.NamePointer))
                    return false;
                // New Weapon Camo
                var weaponCamo = new WeaponCamoObj()
                {
                    Camos = new WeaponCamoObj.CamoEntry[weaponCamoHeader.CamoCount]
                };
                // Load Camos
                var weaponCamoTypeBuffer = Hydra.ActiveGameReader.ReadBytes(weaponCamoHeader.CamosPointer, weaponCamoHeader.CamoCount * 16);
                // Loop through them
                for(int i = 0; i < weaponCamoHeader.CamoCount; i++)
                {
                    // Load it
                    var weaponCamoType = ByteUtil.BytesToStruct<CamoTypeHeader>(weaponCamoTypeBuffer, i * 16);
                    // Load materials
                    var materialsBuffer = Hydra.ActiveGameReader.ReadBytes(weaponCamoType.MaterialsPointer, weaponCamoType.MaterialCount * 312);
                    // Add base camo type material, clamp to 1 because even if it's an empty slot, our GDTs need it to maintain indices
                    weaponCamo.Camos[i] = new WeaponCamoObj.CamoEntry
                    {
                        CamoMaterials = new WeaponCamoObj.CamoMaterial[MathUtilities.Clamp(weaponCamoType.MaterialCount, 2, 1)]
                    };
                    // Loop through them
                    for (int j = 0; j < weaponCamo.Camos[i].CamoMaterials.Length; j++)
                    {
                        // Create it
                        weaponCamo.Camos[i].CamoMaterials[j] = new WeaponCamoObj.CamoMaterial();
                        // Check for valid entry (we're only adding it if null to maintain indices, we don't need it parse it otherwise)
                        if(weaponCamoType.MaterialCount != 0)
                        {
                            // Load it
                            var material = ByteUtil.BytesToStruct<CamoTypeMaterial>(materialsBuffer, j * 312);
                            // Copy Values
                            weaponCamo.Camos[i].CamoMaterials[j].BaseMaterialCount = material.BaseMaterialCount;
                            weaponCamo.Camos[i].CamoMaterials[j].UseNormalMap      = ByteUtil.GetBit(material.Flags, 1);
                            weaponCamo.Camos[i].CamoMaterials[j].UseGlossMap       = ByteUtil.GetBit(material.Flags, 3);
                            weaponCamo.Camos[i].CamoMaterials[j].Material          = Path.GetFileNameWithoutExtension(GetAssetName(material.MaterialPointer));
                            weaponCamo.Camos[i].CamoMaterials[j].TranslationX      = Math.Round(material.Translation.X, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].TranslationY      = Math.Round(material.Translation.Y, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].ScaleX            = Math.Round(material.Scale.X, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].ScaleY            = Math.Round(material.Scale.Y, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].Rotation          = Math.Round(material.Rotation, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].GlossMapBlend     = Math.Round(material.GlossMapBlend, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].NormalMapBlend    = Math.Round(material.NormalMapBlend, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].DetailNormalMap   = GetAssetName(material.DetailNormalMapPointer, 0xF8);
                            weaponCamo.Camos[i].CamoMaterials[j].NormalHeight      = Math.Round(material.NormalHeight, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].NormalScaleX      = Math.Round(material.NormalScale.X, 2);
                            weaponCamo.Camos[i].CamoMaterials[j].NormalScaleY      = Math.Round(material.NormalScale.Y, 2);
                            // Check for base materials
                            if(material.BaseMaterialCount > 0 && material.BaseMaterialsPointer > 0)
                            {
                                // Load Buffer
                                var baseMaterialsBuffer = Hydra.ActiveGameReader.ReadBytes(material.BaseMaterialsPointer, material.BaseMaterialCount * 16);
                                // Loop through them
                                for(int x = 0; x < material.BaseMaterialCount; x++)
                                {
                                    // Load Material
                                    var baseMaterial = ByteUtil.BytesToStruct<CamoTypeMaterial.BaseMaterial>(baseMaterialsBuffer, x * 16);
                                    // Add Assets
                                    weaponCamo.Camos[i].CamoMaterials[j].BaseMaterials[x] = Path.GetFileNameWithoutExtension(GetAssetName(baseMaterial.MaterialPointer));
                                    weaponCamo.Camos[i].CamoMaterials[j].CamoMasks[x]     = GetAssetName(baseMaterial.CamoMaskImagePointer, 0xF8);
                                }
                            }
                        }
                    }
                }
                // Resulting camo assets
                string[] weaponCamos = new string[10];
                // Calculate the number of assets we need (since by default APE caps us at 76)
                int weaponCamoAssetCount = (int)Math.Ceiling(weaponCamo.Camos.Length / 76.0);
                // Loop through assets
                for(int i = 0; i < weaponCamoAssetCount; i++)
                {
                    // Calculate number of camos in this asset
                    int camoCount = MathUtilities.Clamp(76, weaponCamo.Camos.Length - (i * 76), 0);
                    int baseIndex = i * 76;
                    // Set name (first table is ship, others must be named after their base index)
                    weaponCamos[i] = String.Format("{0}{1}", asset.Name, i == 0 ? "_ship" : String.Format("_base{0}", baseIndex));
                    // New Weapon Camo
                    var weaponCamoAsset = new GameDataTable.Asset(weaponCamos[i], "weaponcamo");
                    // Set Base Properties
                    weaponCamoAsset.Properties["configstringFileType"] = "WEAPONCAMO";
                    weaponCamoAsset.Properties["baseIndex"] = baseIndex.ToString();
                    weaponCamoAsset.Properties["numCamos"] = camoCount.ToString();
                    // Loop through camos
                    for(int camoIndex = 1, j = 0; j < camoCount; j++, camoIndex++)
                    {
                        // Loop through materials
                        for (int materialIndex = 1, k = 0; k < weaponCamo.Camos[baseIndex + j].CamoMaterials.Length; k++, materialIndex++)
                        {
                            // Add Base Materials/Masks
                            for(int l = 0; l < weaponCamo.Camos[baseIndex + j].CamoMaterials[k].BaseMaterialCount; l++)
                            {
                                weaponCamoAsset.Properties[String.Format("material{0}_{1}_base_material_{2}", materialIndex, camoIndex, l + 1)] = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].BaseMaterials[l];
                                weaponCamoAsset.Properties[String.Format("material{0}_{1}_camo_mask_{2}", materialIndex, camoIndex, l + 1)]     = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].CamoMasks[l];
                            }
                            // Add Properties
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_detail_normal_height", materialIndex, camoIndex)]  = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].NormalHeight;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_detail_normal_map", materialIndex, camoIndex)]     = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].DetailNormalMap;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_detail_normal_scale_x", materialIndex, camoIndex)] = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].NormalScaleX;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_detail_normal_scale_y", materialIndex, camoIndex)] = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].NormalScaleY;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_gloss_blend", materialIndex, camoIndex)]           = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].GlossMapBlend;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_normal_amount", materialIndex, camoIndex)]         = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].NormalMapBlend;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_material", materialIndex, camoIndex)]              = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].Material;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_numBaseMaterials", materialIndex, camoIndex)]      = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].BaseMaterialCount;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_rotation", materialIndex, camoIndex)]              = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].Rotation;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_scale_x", materialIndex, camoIndex)]               = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].ScaleX;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_scale_y", materialIndex, camoIndex)]               = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].ScaleY;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_trans_x", materialIndex, camoIndex)]               = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].TranslationX;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_trans_y", materialIndex, camoIndex)]               = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].TranslationY;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_useGlossMap", materialIndex, camoIndex)]           = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].UseGlossMap;
                            weaponCamoAsset.Properties[String.Format("material{0}_{1}_useNormalMap", materialIndex, camoIndex)]          = weaponCamo.Camos[baseIndex + j].CamoMaterials[k].UseNormalMap;
                        }
                    }
                    // Add to GDT
                    Hydra.GDTs["Camo"].AddAsset(weaponCamoAsset.Name, weaponCamoAsset);
                }
                // New Weapon Camo Table
                var weaponCamoTableAsset = new GameDataTable.Asset(asset.Name, "weaponcamotable");
                weaponCamoTableAsset.Properties["configstringFileType"] = "WEAPONCAMO";
                weaponCamoTableAsset.Properties["numCamoTables"] = weaponCamos.Count(x => x != null);
                // Loop through results
                for (int i = 0; i < weaponCamos.Length; i++)
                    weaponCamoTableAsset.Properties[String.Format("table_{0:D2}_name", i + 1)] = weaponCamos[i];
                // Add to GDT
                Hydra.GDTs["Camo"].AddAsset(weaponCamoTableAsset.Name, weaponCamoTableAsset);
                // Done
                return true;
            }
        }
    }
}
