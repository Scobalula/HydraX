using System;
using System.Collections.Generic;
using System.IO;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Tracer Logic
        /// </summary>
        private class Tracer : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Tracer Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] TracerOffsets =
            {
                new Tuple<string, int, int>("type",                                                          0x8, 0x33),
                new Tuple<string, int, int>("material",                                                      0x10, 0x34),
                new Tuple<string, int, int>("drawInterval",                                                  0x18, 0x4),
                new Tuple<string, int, int>("speed",                                                         0x1c, 0x8),
                new Tuple<string, int, int>("beamLength",                                                    0x20, 0x8),
                new Tuple<string, int, int>("beamWidth",                                                     0x24, 0x8),
                new Tuple<string, int, int>("screwRadius",                                                   0x28, 0x8),
                new Tuple<string, int, int>("screwDist",                                                     0x2c, 0x8),
                new Tuple<string, int, int>("fadeTime",                                                      0x30, 0x8),
                new Tuple<string, int, int>("fadeScale",                                                     0x34, 0x8),
                new Tuple<string, int, int>("texRepeatRate",                                                 0x38, 0x8),
                new Tuple<string, int, int>("curvePoints",                                                   0x3c, 0x4),
                new Tuple<string, int, int>("curveRadius",                                                   0x40, 0x8),
                new Tuple<string, int, int>("colorR0",                                                       0x44, 0x8),
                new Tuple<string, int, int>("colorG0",                                                       0x48, 0x8),
                new Tuple<string, int, int>("colorB0",                                                       0x4c, 0x8),
                new Tuple<string, int, int>("colorA0",                                                       0x50, 0x8),
                new Tuple<string, int, int>("colorR1",                                                       0x54, 0x8),
                new Tuple<string, int, int>("colorG1",                                                       0x58, 0x8),
                new Tuple<string, int, int>("colorB1",                                                       0x5c, 0x8),
                new Tuple<string, int, int>("colorA1",                                                       0x60, 0x8),
                new Tuple<string, int, int>("colorR2",                                                       0x64, 0x8),
                new Tuple<string, int, int>("colorG2",                                                       0x68, 0x8),
                new Tuple<string, int, int>("colorB2",                                                       0x6c, 0x8),
                new Tuple<string, int, int>("colorA2",                                                       0x70, 0x8),
                new Tuple<string, int, int>("colorR3",                                                       0x74, 0x8),
                new Tuple<string, int, int>("colorG3",                                                       0x78, 0x8),
                new Tuple<string, int, int>("colorB3",                                                       0x7c, 0x8),
                new Tuple<string, int, int>("colorA3",                                                       0x80, 0x8),
                new Tuple<string, int, int>("colorR4",                                                       0x84, 0x8),
                new Tuple<string, int, int>("colorG4",                                                       0x88, 0x8),
                new Tuple<string, int, int>("colorB4",                                                       0x8c, 0x8),
                new Tuple<string, int, int>("colorA4",                                                       0x90, 0x8),
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
            public string Name => "tracer";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Weapon";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.tracer;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, TracerOffsets, instance, HandleLaserSettings);

                result.Type = "tracer";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Laser Specific Settings
            /// </summary>
            private static object HandleLaserSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return BitConverter.ToInt32(assetBuffer, offset) == 0 ? "Laser" : "Smoke";
                    case 0x34:
                        return Path.GetFileNameWithoutExtension(instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, offset), instance));
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
