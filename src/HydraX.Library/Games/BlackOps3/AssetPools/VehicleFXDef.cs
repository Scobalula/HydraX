using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 VehicleFXDef Logic
        /// </summary>
        private class VehicleFXDef : IAssetPool
        {
            #region Tables
            /// <summary>
            /// VehicleFXDef Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] VehicleFXDefOffsets =
            {
                new Tuple<string, int, int>("csvInclude",                                                    0x8, 0x0),
                new Tuple<string, int, int>("type",                                                          0x10, 0x33),
                new Tuple<string, int, int>("treadFxDefault",                                                0x18, 0xa),
                new Tuple<string, int, int>("treadFxBark",                                                   0x20, 0xa),
                new Tuple<string, int, int>("treadFxBrick",                                                  0x28, 0xa),
                new Tuple<string, int, int>("treadFxCarpet",                                                 0x30, 0xa),
                new Tuple<string, int, int>("treadFxCloth",                                                  0x38, 0xa),
                new Tuple<string, int, int>("treadFxConcrete",                                               0x40, 0xa),
                new Tuple<string, int, int>("treadFxDirt",                                                   0x48, 0xa),
                new Tuple<string, int, int>("treadFxFlesh",                                                  0x50, 0xa),
                new Tuple<string, int, int>("treadFxFoliage",                                                0x58, 0xa),
                new Tuple<string, int, int>("treadFxGlass",                                                  0x60, 0xa),
                new Tuple<string, int, int>("treadFxGrass",                                                  0x68, 0xa),
                new Tuple<string, int, int>("treadFxGravel",                                                 0x70, 0xa),
                new Tuple<string, int, int>("treadFxIce",                                                    0x78, 0xa),
                new Tuple<string, int, int>("treadFxMetal",                                                  0x80, 0xa),
                new Tuple<string, int, int>("treadFxMud",                                                    0x88, 0xa),
                new Tuple<string, int, int>("treadFxPaper",                                                  0x90, 0xa),
                new Tuple<string, int, int>("treadFxPlaster",                                                0x98, 0xa),
                new Tuple<string, int, int>("treadFxRock",                                                   0xa0, 0xa),
                new Tuple<string, int, int>("treadFxSand",                                                   0xa8, 0xa),
                new Tuple<string, int, int>("treadFxSnow",                                                   0xb0, 0xa),
                new Tuple<string, int, int>("treadFxWater",                                                  0xb8, 0xa),
                new Tuple<string, int, int>("treadFxWood",                                                   0xc0, 0xa),
                new Tuple<string, int, int>("treadFxAsphalt",                                                0xc8, 0xa),
                new Tuple<string, int, int>("treadFxCeramic",                                                0xd0, 0xa),
                new Tuple<string, int, int>("treadFxPlastic",                                                0xd8, 0xa),
                new Tuple<string, int, int>("treadFxRubber",                                                 0xe0, 0xa),
                new Tuple<string, int, int>("treadFxCushion",                                                0xe8, 0xa),
                new Tuple<string, int, int>("treadFxFruit",                                                  0xf0, 0xa),
                new Tuple<string, int, int>("treadFxPaintedMetal",                                           0xf8, 0xa),
                new Tuple<string, int, int>("treadFxMetalThin",                                              0x118, 0xa),
                new Tuple<string, int, int>("treadFxMetalHollow",                                            0x120, 0xa),
                new Tuple<string, int, int>("treadFxMetalCatwalk",                                           0x128, 0xa),
                new Tuple<string, int, int>("treadFxMetalCar",                                               0x130, 0xa),
                new Tuple<string, int, int>("treadFxGlassCar",                                               0x138, 0xa),
                new Tuple<string, int, int>("treadFxGlassBulletproof",                                       0x140, 0xa),
                new Tuple<string, int, int>("treadFxWaterShallow",                                           0x148, 0xa),
                new Tuple<string, int, int>("treadFxBodyArmor",                                              0x150, 0xa),
                new Tuple<string, int, int>("peelFxDefault",                                                 0x158, 0xa),
                new Tuple<string, int, int>("peelFxBark",                                                    0x160, 0xa),
                new Tuple<string, int, int>("peelFxBrick",                                                   0x168, 0xa),
                new Tuple<string, int, int>("peelFxCarpet",                                                  0x170, 0xa),
                new Tuple<string, int, int>("peelFxCloth",                                                   0x178, 0xa),
                new Tuple<string, int, int>("peelFxConcrete",                                                0x180, 0xa),
                new Tuple<string, int, int>("peelFxDirt",                                                    0x188, 0xa),
                new Tuple<string, int, int>("peelFxFlesh",                                                   0x190, 0xa),
                new Tuple<string, int, int>("peelFxFoliage",                                                 0x198, 0xa),
                new Tuple<string, int, int>("peelFxGlass",                                                   0x1a0, 0xa),
                new Tuple<string, int, int>("peelFxGrass",                                                   0x1a8, 0xa),
                new Tuple<string, int, int>("peelFxGravel",                                                  0x1b0, 0xa),
                new Tuple<string, int, int>("peelFxIce",                                                     0x1b8, 0xa),
                new Tuple<string, int, int>("peelFxMetal",                                                   0x1c0, 0xa),
                new Tuple<string, int, int>("peelFxMud",                                                     0x1c8, 0xa),
                new Tuple<string, int, int>("peelFxPaper",                                                   0x1d0, 0xa),
                new Tuple<string, int, int>("peelFxPlaster",                                                 0x1d8, 0xa),
                new Tuple<string, int, int>("peelFxRock",                                                    0x1e0, 0xa),
                new Tuple<string, int, int>("peelFxSand",                                                    0x1e8, 0xa),
                new Tuple<string, int, int>("peelFxSnow",                                                    0x1f0, 0xa),
                new Tuple<string, int, int>("peelFxWater",                                                   0x1f8, 0xa),
                new Tuple<string, int, int>("peelFxWood",                                                    0x200, 0xa),
                new Tuple<string, int, int>("peelFxAsphalt",                                                 0x208, 0xa),
                new Tuple<string, int, int>("peelFxCeramic",                                                 0x210, 0xa),
                new Tuple<string, int, int>("peelFxPlastic",                                                 0x218, 0xa),
                new Tuple<string, int, int>("peelFxRubber",                                                  0x220, 0xa),
                new Tuple<string, int, int>("peelFxCushion",                                                 0x228, 0xa),
                new Tuple<string, int, int>("peelFxFruit",                                                   0x230, 0xa),
                new Tuple<string, int, int>("peelFxPaintedMetal",                                            0x238, 0xa),
                new Tuple<string, int, int>("peelFxMetalThin",                                               0x258, 0xa),
                new Tuple<string, int, int>("peelFxMetalHollow",                                             0x260, 0xa),
                new Tuple<string, int, int>("peelFxMetalCatwalk",                                            0x268, 0xa),
                new Tuple<string, int, int>("peelFxMetalCar",                                                0x270, 0xa),
                new Tuple<string, int, int>("peelFxGlassCar",                                                0x278, 0xa),
                new Tuple<string, int, int>("peelFxGlassBulletproof",                                        0x280, 0xa),
                new Tuple<string, int, int>("peelFxWaterShallow",                                            0x288, 0xa),
                new Tuple<string, int, int>("peelFxBodyArmor",                                               0x290, 0xa),
                new Tuple<string, int, int>("skidFxDefault",                                                 0x298, 0xa),
                new Tuple<string, int, int>("skidFxBark",                                                    0x2a0, 0xa),
                new Tuple<string, int, int>("skidFxBrick",                                                   0x2a8, 0xa),
                new Tuple<string, int, int>("skidFxCarpet",                                                  0x2b0, 0xa),
                new Tuple<string, int, int>("skidFxCloth",                                                   0x2b8, 0xa),
                new Tuple<string, int, int>("skidFxConcrete",                                                0x2c0, 0xa),
                new Tuple<string, int, int>("skidFxDirt",                                                    0x2c8, 0xa),
                new Tuple<string, int, int>("skidFxFlesh",                                                   0x2d0, 0xa),
                new Tuple<string, int, int>("skidFxFoliage",                                                 0x2d8, 0xa),
                new Tuple<string, int, int>("skidFxGlass",                                                   0x2e0, 0xa),
                new Tuple<string, int, int>("skidFxGrass",                                                   0x2e8, 0xa),
                new Tuple<string, int, int>("skidFxGravel",                                                  0x2f0, 0xa),
                new Tuple<string, int, int>("skidFxIce",                                                     0x2f8, 0xa),
                new Tuple<string, int, int>("skidFxMetal",                                                   0x300, 0xa),
                new Tuple<string, int, int>("skidFxMud",                                                     0x308, 0xa),
                new Tuple<string, int, int>("skidFxPaper",                                                   0x310, 0xa),
                new Tuple<string, int, int>("skidFxPlaster",                                                 0x318, 0xa),
                new Tuple<string, int, int>("skidFxRock",                                                    0x320, 0xa),
                new Tuple<string, int, int>("skidFxSand",                                                    0x328, 0xa),
                new Tuple<string, int, int>("skidFxSnow",                                                    0x330, 0xa),
                new Tuple<string, int, int>("skidFxWater",                                                   0x338, 0xa),
                new Tuple<string, int, int>("skidFxWood",                                                    0x340, 0xa),
                new Tuple<string, int, int>("skidFxAsphalt",                                                 0x348, 0xa),
                new Tuple<string, int, int>("skidFxCeramic",                                                 0x350, 0xa),
                new Tuple<string, int, int>("skidFxPlastic",                                                 0x358, 0xa),
                new Tuple<string, int, int>("skidFxRubber",                                                  0x360, 0xa),
                new Tuple<string, int, int>("skidFxCushion",                                                 0x368, 0xa),
                new Tuple<string, int, int>("skidFxFruit",                                                   0x370, 0xa),
                new Tuple<string, int, int>("skidFxPaintedMetal",                                            0x378, 0xa),
                new Tuple<string, int, int>("skidFxMetalThin",                                               0x398, 0xa),
                new Tuple<string, int, int>("skidFxMetalHollow",                                             0x3a0, 0xa),
                new Tuple<string, int, int>("skidFxMetalCatwalk",                                            0x3a8, 0xa),
                new Tuple<string, int, int>("skidFxMetalCar",                                                0x3b0, 0xa),
                new Tuple<string, int, int>("skidFxGlassCar",                                                0x3b8, 0xa),
                new Tuple<string, int, int>("skidFxGlassBulletproof",                                        0x3c0, 0xa),
                new Tuple<string, int, int>("skidFxWaterShallow",                                            0x3c8, 0xa),
                new Tuple<string, int, int>("skidFxBodyArmor",                                               0x3d0, 0xa),
            };

            /// <summary>
            /// VehicleFXDef Types
            /// </summary>
            private static readonly string[] VehicleFXDefTypes =
            {
                "nitrous",
                "ground",
                "aircraft",
                "boat",
                "tank",
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
            public string Name => "vehiclefxdef";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.vehiclefxdef;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, VehicleFXDefOffsets, instance, HandleVehicleFXDefSettings);

                result.Type = "vehiclefxdef";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Vehicle Specific Settings
            /// </summary>
            private static object HandleVehicleFXDefSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return VehicleFXDefTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    default:
                        return null;
                }
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
