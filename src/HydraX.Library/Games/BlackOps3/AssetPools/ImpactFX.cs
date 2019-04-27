using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 ImpactFX Logic
        /// </summary>
        private class ImpactFX : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// ImpactFX Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x28)]
            private struct ImpactFXTableAsset
            {
                #region ImpactFXProperties
                public long NamePointer;
                public long SurfaceFXTablePointer;
                public long EntityFXTablePointer;
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
            public string Name => "impactsfxtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.impactsfxtable;

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
                    var header = instance.Reader.ReadStruct<ImpactFXTableAsset>(StartAddress + (i * AssetSize));

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
                var header = instance.Reader.ReadStruct<ImpactFXTableAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                instance.GDTs["Misc"][asset.Name] = new GameDataTable.Asset(asset.Name, "impactsfxtable");

                if(header.SurfaceFXTablePointer > 0)
                {
                    var surfaceFXTable = instance.Reader.ReadStruct<SurfaceFXTableAsset>(header.SurfaceFXTablePointer);
                    var surfaceFXTableName = instance.Reader.ReadNullTerminatedString(surfaceFXTable.NamePointer);
                    instance.GDTs["Misc"][asset.Name]["surfaceFX"] = surfaceFXTableName;

                    instance.GDTs["Misc"][surfaceFXTableName] = new GameDataTable.Asset(surfaceFXTableName, "surfacefxtable");

                    for (int i = 0; i < surfaceFXTable.SurfaceFXPointers.Length; i++)
                    {
                        var name = instance.Game.CleanAssetName(HydraAssetType.FX, instance.Game.GetAssetName(surfaceFXTable.SurfaceFXPointers[i], instance));
                        instance.GDTs["Misc"][surfaceFXTableName]["Surface" + i.ToString()] = name;
                    }
                }

                if (header.EntityFXTablePointer > 0)
                {
                    var entityFXTable = instance.Reader.ReadStruct<EntityFXTableAsset>(header.EntityFXTablePointer);
                    var entityFXTableName = instance.Reader.ReadNullTerminatedString(entityFXTable.NamePointer);
                    instance.GDTs["Misc"][asset.Name]["entityFXImpacts"] = entityFXTableName;

                    instance.GDTs["Misc"][entityFXTableName] = new GameDataTable.Asset(entityFXTableName, "entityfximpacts");

                    for (int i = 0; i < entityFXTable.EntityFXPointers.Length; i++)
                    {
                        var name = instance.Game.CleanAssetName(HydraAssetType.FX, instance.Game.GetAssetName(entityFXTable.EntityFXPointers[i], instance));
                        instance.GDTs["Misc"][entityFXTableName][EntityFXKeys[i]] = name;
                    }
                }

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
