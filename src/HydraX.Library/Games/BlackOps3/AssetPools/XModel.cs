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
            public List<GameAsset> Load(HydraInstance instance)
            {
                var results = new List<GameAsset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.BaseAddress + instance.Game.AssetPoolsAddresses[instance.Game.ProcessIndex] + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                for(int i = 0; i < AssetCount; i++)
                {
                    var namePointer = instance.Reader.ReadInt64(StartAddress + (i * AssetSize));

                    if (IsNullAsset(namePointer))
                        continue;

                    var name = instance.Reader.ReadNullTerminatedString(namePointer);

                    results.Add(new GameAsset()
                    {
                        Name          = name,
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool     = this,
                        Type          = Name,
                        Information   = "N/A"
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var buffer = instance.Reader.ReadBytes(asset.HeaderAddress, 392);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(BitConverter.ToInt64(buffer, 0)))
                    return HydraStatus.MemoryChanged;

                var mtlInfoPointer = BitConverter.ToInt64(buffer, 200);
                var mtlCount = instance.Reader.ReadInt32(mtlInfoPointer);
                var mtlListPointer = instance.Reader.ReadInt64(mtlInfoPointer + 8);
                var mtlPointers = instance.Reader.ReadArray<long>(mtlListPointer, (int)mtlCount);

                var gdt = new GameDataTable();

                foreach (var mtlPointer in mtlPointers)
                {
                    foreach (var result in Material.ExportMTL(instance.Reader.ReadStruct<Material.MaterialAsset>(mtlPointer), instance))
                        gdt[result.Name] = result;
                }

                var path = Path.Combine(instance.ExportFolder, "model_export", "hydra_model_gdts", asset.Name + ".gdt");
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                gdt.Save(path);

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
