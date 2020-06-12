using System;
using System.Collections.Generic;
using System.IO;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Character Logic
        /// </summary>
        private class Character : IAssetPool
        {
            #region Tables
            private static readonly Tuple<string, int, int>[] CharacterOffsets =
            {
                new Tuple<string, int, int>("skeleton", 0x8, 0x33),
                new Tuple<string, int, int>("voice", 0xc, 0x34),
                new Tuple<string, int, int>("destructDef", 0x10, 0x1b),
                new Tuple<string, int, int>("gibDef", 0x18, 0x1b),
                new Tuple<string, int, int>("fxDef", 0x20, 0x1b),
                new Tuple<string, int, int>("deathFxDef", 0x28, 0x1b),
                new Tuple<string, int, int>("altFxDef1", 0x30, 0x1b),
                new Tuple<string, int, int>("altFxDef2", 0x38, 0x1b),
                new Tuple<string, int, int>("altFxDef3", 0x40, 0x1b),
                new Tuple<string, int, int>("altFxDef4", 0x48, 0x1b),
                new Tuple<string, int, int>("altFxDef5", 0x50, 0x1b),
                new Tuple<string, int, int>("altFxDef6", 0x58, 0x1b),
                new Tuple<string, int, int>("body", 0x60, 0xd),
                new Tuple<string, int, int>("bodyAlias", 0x68, 0xe),
                new Tuple<string, int, int>("eyeGlowBone", 0x260, 0x0),
                new Tuple<string, int, int>("eyeGlowFX", 0x258, 0x35),
                new Tuple<string, int, int>("head", 0x70, 0xd),
                new Tuple<string, int, int>("headAlias", 0x78, 0xe),
                new Tuple<string, int, int>("headAliasZombie", 0x80, 0xe),
                new Tuple<string, int, int>("hat", 0x88, 0xd),
                new Tuple<string, int, int>("hatAlias", 0x90, 0xe),
                new Tuple<string, int, int>("gear", 0x98, 0xd),
                new Tuple<string, int, int>("gearAlias", 0xa0, 0xe),
                new Tuple<string, int, int>("viewmodel", 0xa8, 0xd),
                new Tuple<string, int, int>("shadowCharacter", 0xb0, 0xd),
                new Tuple<string, int, int>("torsoDmg1", 0xb8, 0xd),
                new Tuple<string, int, int>("torsoDmg1_Alias", 0xc0, 0xe),
                new Tuple<string, int, int>("torsoDmg2", 0xc8, 0xd),
                new Tuple<string, int, int>("torsoDmg2_Alias", 0xd0, 0xe),
                new Tuple<string, int, int>("torsoDmg3", 0xd8, 0xd),
                new Tuple<string, int, int>("torsoDmg3_Alias", 0xe0, 0xe),
                new Tuple<string, int, int>("torsoDmg4", 0xe8, 0xd),
                new Tuple<string, int, int>("torsoDmg4_Alias", 0xf0, 0xe),
                new Tuple<string, int, int>("torsoDmg5", 0xf8, 0xd),
                new Tuple<string, int, int>("torsoDmg5_Alias", 0x100, 0xe),
                new Tuple<string, int, int>("legDmg1", 0x108, 0xd),
                new Tuple<string, int, int>("legDmg1_Alias", 0x110, 0xe),
                new Tuple<string, int, int>("legDmg2", 0x118, 0xd),
                new Tuple<string, int, int>("legDmg2_Alias", 0x120, 0xe),
                new Tuple<string, int, int>("legDmg3", 0x128, 0xd),
                new Tuple<string, int, int>("legDmg3_Alias", 0x130, 0xe),
                new Tuple<string, int, int>("legDmg4", 0x138, 0xd),
                new Tuple<string, int, int>("legDmg4_Alias", 0x140, 0xe),
                new Tuple<string, int, int>("J_ChestGear_RI", 0x148, 0xd),
                new Tuple<string, int, int>("J_ChestGear_RI_required", 0x150, 0x7),
                new Tuple<string, int, int>("J_ChestGear_LE", 0x158, 0xd),
                new Tuple<string, int, int>("J_ChestGear_LE_required", 0x160, 0x7),
                new Tuple<string, int, int>("J_FrontPack", 0x168, 0xd),
                new Tuple<string, int, int>("J_FrontPack_required", 0x170, 0x7),
                new Tuple<string, int, int>("J_FrontPackLow", 0x178, 0xd),
                new Tuple<string, int, int>("J_FrontPackLow_required", 0x180, 0x7),
                new Tuple<string, int, int>("J_Backpack", 0x188, 0xd),
                new Tuple<string, int, int>("J_Backpack_required", 0x190, 0x7),
                new Tuple<string, int, int>("J_GearRear_RI", 0x198, 0xd),
                new Tuple<string, int, int>("J_GearRear_RI_required", 0x1a0, 0x7),
                new Tuple<string, int, int>("J_GearRear_LE", 0x1a8, 0xd),
                new Tuple<string, int, int>("J_GearRear_LE_required", 0x1b0, 0x7),
                new Tuple<string, int, int>("J_GearSideRear_RI", 0x1b8, 0xd),
                new Tuple<string, int, int>("J_GearSideRear_RI_required", 0x1c0, 0x7),
                new Tuple<string, int, int>("J_GearSideRear_LE", 0x1c8, 0xd),
                new Tuple<string, int, int>("J_GearSideRear_LE_required", 0x1d0, 0x7),
                new Tuple<string, int, int>("J_GearSide_RI", 0x1d8, 0xd),
                new Tuple<string, int, int>("J_GearSide_RI_required", 0x1e0, 0x7),
                new Tuple<string, int, int>("J_GearSide_LE", 0x1e8, 0xd),
                new Tuple<string, int, int>("J_GearSide_LE_required", 0x1f0, 0x7),
                new Tuple<string, int, int>("J_GearFront_RI", 0x1f8, 0xd),
                new Tuple<string, int, int>("J_GearFront_RI_required", 0x200, 0x7),
                new Tuple<string, int, int>("J_GearFront_LE", 0x208, 0xd),
                new Tuple<string, int, int>("J_GearFront_LE_required", 0x210, 0x7),
                new Tuple<string, int, int>("J_Helmet", 0x218, 0xd),
                new Tuple<string, int, int>("J_Helmet_required", 0x220, 0x7),
                new Tuple<string, int, int>("misc1", 0x228, 0xd),
                new Tuple<string, int, int>("misc1_required", 0x230, 0x7),
                new Tuple<string, int, int>("misc2", 0x238, 0xd),
                new Tuple<string, int, int>("misc2_required", 0x240, 0x7),
                new Tuple<string, int, int>("misc3", 0x248, 0xd),
                new Tuple<string, int, int>("misc3_required", 0x250, 0x7),
                new Tuple<string, int, int>("headImpactType", 0x268, 0x36),
                new Tuple<string, int, int>("bodyImpactType", 0x26c, 0x36),

            };

            private static readonly string[] CharacterImpactTypes =
            {
                "none",
                "flesh",
                "zombie",
                "armorlight",
                "armorheavy",
                "robotlight",
                "robotheavy",
                "robotboss",
                "skeleton"
            };

            private static readonly string[] CharacterPreviewTypes =
            {
                "generichuman",
                "genericrobot",
                "custom"
            };

            private static readonly string[] CharacterSkeletonTypes =
            {
                "base",
                "scaled80",
                "prop80"
            };

            private static readonly string[] CharacterVoiceTypes =
            {
                "american",
                "russian",
                "unita",
                "cuban",
                "mujahideen",
                "pdf",
                "pmc",
                "isi",
                "digbat",
                "yemeni",
                "terrorist",
                "lapd",
                "secretservice",
                "mp",
                "chinese"
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
            public string Name => "character";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.character;

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

                for (int i = 0; i < AssetCount; i++)
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

                var result = ConvertAssetBufferToGDTAsset(buffer, CharacterOffsets, instance, HandleCharacterSettings);

                result.Type = "character";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Character Specific Settings
            /// </summary>
            private static object HandleCharacterSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return CharacterSkeletonTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x34:
                        return CharacterVoiceTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x35:
                        return BitConverter.ToInt32(assetBuffer, offset);
                    case 0x36:
                        return CharacterImpactTypes[BitConverter.ToInt32(assetBuffer, offset)];
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
