using PhilLibX.IO;
using PhilLibX.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Raw File Logic
        /// </summary>
        private class XModel : IAssetPool
        {
            [StructLayout(LayoutKind.Sequential, Pack=8, Size = 392)]
            private unsafe struct XModelAsset
            {
                public long NamePointer;
                public byte BoneCount;
                public byte RootCount;
                public ushort BoneCosmeticCount;
                public long BoneNamesPointer;
                public long ParentListPointer;
                public long QuaternionsPointer;
                public long TranslationsPointer;
                public long PartClassificationPointer;
                public long BaseMatPointer;
                public int LODCount;
                public fixed byte CustomLODParams[68];
                public fixed long LODPointers[8];
                public long MeshMaterialsPointer;
                public long UsageFlags;
                public long UnknownPointer01;
                public int Unknown01;
                public int Unknown02;
                public long UnknownPointer02;
                public float Radius;
                public Vector3 Min;
                public Vector3 Max;
                public long BulletCollisionsPointer;
                public int BulletCollisionMeshCount;
                public int Flags;
                public long PhysicsConstraintsPointer;
                public long PhysicsPresetPointer;
                public long Padding;
                public int SubModelCount;
                public long SubModelsPointer;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            struct XModelSubmodel
            {
                public long ModelPointer;
                public int TagIndex;
                public Vector3 Offset;
                public Vector3 Angle;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            struct XModelMaterials
            {
                public int MaterialCount;
                public long MaterialsPointer;
                public long HimipInvSqRadii;
            }

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
            public long EndAddress { get; set; }

            /// <summary>
            /// Gets the Name of this Pool
            /// </summary>
            public string Name => "xmodel";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.xmodel;

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
                EndAddress = StartAddress + (AssetCount * AssetSize);

                var headers = instance.Reader.ReadArrayUnsafe<XModelAsset>(StartAddress, AssetCount);

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = headers[i];

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name        = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Zone        = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format("Bones: {0} LODs: {1}", header.BoneCosmeticCount + header.BoneCount, header.LODCount),
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
                var header = instance.Reader.ReadStruct<XModelAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var mtlInfos = instance.Reader.ReadArray<XModelMaterials>(header.MeshMaterialsPointer, header.LODCount);

                var gdt = new GameDataTable();

                foreach (var mtlInfo in mtlInfos)
                {
                    var mtlPointers = instance.Reader.ReadArray<long>(mtlInfo.MaterialsPointer, mtlInfo.MaterialCount);

                    foreach (var mtlPointer in mtlPointers)
                    {
                        foreach (var result in Material.ExportMTL(instance.Reader.ReadStruct<Material.MaterialAsset>(mtlPointer), instance))
                        {
                            if (instance.ExistsInGDTDB(result.Type, result.Name))
                                continue;

                            gdt[result.Type, result.Name] = result;
                        }
                    }
                }

                var xmodelAsset = new GameDataTable.Asset(asset.Name, "xmodel");

                // Set initial LOD Values
                foreach (var lodGDTKeys in GameDataTable.LODGDTKeys)
                {
                    xmodelAsset.Properties[lodGDTKeys.Item1] = "";
                    xmodelAsset.Properties[lodGDTKeys.Item2] = "";
                    xmodelAsset.Properties[lodGDTKeys.Item3] = "0";
                }

                xmodelAsset.Properties["skinOverride"] = "";
                xmodelAsset.Properties["BulletCollisionLOD"] = "High";
                xmodelAsset.Properties["type"] = (header.BoneCount + header.BoneCosmeticCount) > 2 ? "animated" : "rigid";

                xmodelAsset.Properties["physicsPreset"] = instance.Game.GetAssetName(header.PhysicsPresetPointer, instance);
                xmodelAsset.Properties["physicsConstraints"] = instance.Game.GetAssetName(header.PhysicsConstraintsPointer, instance);

                for (int i = 0; i < header.LODCount; i++)
                {
                    // Set the LOD path and distance, and disable generated LODs
                    xmodelAsset.Properties[GameDataTable.LODGDTKeys[i].Item1] = string.Format("<EXPORT_DIRECTORY_FIND_REPLACE_ME_OwO>\\\\{0}_lod{1}.xmodel_bin", asset.Name, header.LODCount - (i + 1));
                    xmodelAsset.Properties[GameDataTable.LODGDTKeys[i].Item2] = 175 * (i + 1);
                    xmodelAsset.Properties[GameDataTable.LODGDTKeys[i].Item3] = "0";
                }

                xmodelAsset.Properties["hitBoxModel"] = string.Format("<EXPORT_DIRECTORY_FIND_REPLACE_ME_OwO>\\\\{0}_hitbox.xmodel_bin", asset.Name);

                var subModels = instance.Reader.ReadArray<XModelSubmodel>(header.SubModelsPointer, header.SubModelCount);

                for (int i = 0; i < subModels.Length; i++)
                {
                    xmodelAsset.Properties[string.Format("submodel{0}_Name", i)] = instance.Game.GetAssetName(subModels[i].ModelPointer, instance);
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetPitch", i)] = subModels[i].Angle.X;
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetRoll", i)] = subModels[i].Angle.Y;
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetYaw", i)] = subModels[i].Angle.Z;
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetX", i)] = subModels[i].Offset.X;
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetY", i)] = subModels[i].Offset.Y;
                    xmodelAsset.Properties[string.Format("submodel{0}_OffsetZ", i)] = subModels[i].Offset.Z;
                    xmodelAsset.Properties[string.Format("submodel{0}_ParentTag", i)] = instance.Game.GetString(subModels[i].TagIndex, instance);
                }

                if (instance.ExistsInGDTDB(xmodelAsset.Type, xmodelAsset.Name))
                {
                    gdt[xmodelAsset.Type, xmodelAsset.Name] = xmodelAsset;
                }
                var path = Path.Combine(instance.ExportFolder, "model_export", "hydra_model_gdts", asset.Name + ".gdt");
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                gdt.Save(path);
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
