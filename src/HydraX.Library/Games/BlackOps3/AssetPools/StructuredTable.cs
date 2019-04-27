using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;
using HydraX.Library.AssetContainers;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Structured Table Logic
        /// </summary>
        private class StructuredTable : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Data Types
            /// </summary>
            public enum EntryDataType : int
            {
                Null   = 0,
                String = 1,
                Int32  = 2,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Structured Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x30)]
            private struct StructuredTableAsset
            {
                #region StructuredTableProperties
                public long NamePointer;
                public int DataCount;
                public int PropertyCount;
                public int EntryCount;
                public int Padding;
                public long DataPointer;
                public long DataIndicesPointer;
                public long PropertiesPointer;
                public long PropertyIndicesPointer;
                #endregion
            }

            /// <summary>
            /// Structured Table Property Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct StructuredTableProperty
            {
                #region StructuredTablePropertyProperties
                public long StringPointer;
                public int Hash;
                public int Index;
                #endregion
            }

            /// <summary>
            /// Structured Table Entry Data Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x18)]
            private struct StructuredTableEntryData
            {
                #region StructuredTablePropertyProperties
                public EntryDataType DataType;
                public long StringPointer;
                public int IntegerValue;
                public int Checksum;
                #endregion
            }
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
            public string Name => "structuredtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Meta Data";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.structuredtable;

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
                    var header = instance.Reader.ReadStruct<StructuredTableAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Type = Name,
                        Information = string.Format("Entries: {0} - Properties: {1}", header.EntryCount, header.PropertyCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<StructuredTableAsset>(asset.HeaderAddress);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    return HydraStatus.MemoryChanged;

                var data            = instance.Reader.ReadArray<StructuredTableEntryData>(header.DataPointer, header.DataCount);
                var properties      = instance.Reader.ReadArray<StructuredTableProperty>(header.PropertiesPointer, header.PropertyCount);
                var propertyNames   = new string[header.PropertyCount];

                for (int i = 0; i < propertyNames.Length; i++)
                    propertyNames[i] = instance.Reader.ReadNullTerminatedString(properties[i].StringPointer);

                var structuredTableObj = new StructuredTableObj(header.EntryCount);

                for(int i = 0; i < header.EntryCount; i++)
                {
                    structuredTableObj.Data[i] = new Dictionary<string, object>();

                    for(int j = 0; j < header.PropertyCount; j++)
                    {
                        var dataIndex = (i * header.PropertyCount) + j;

                        // Switch by type, add if it's valid
                        switch (data[dataIndex].DataType)
                        {
                            // Strings
                            case EntryDataType.String:
                                structuredTableObj.Data[i][propertyNames[j]] = instance.Reader.ReadNullTerminatedString(data[dataIndex].StringPointer);
                                break;
                            // Integer
                            case EntryDataType.Int32:
                                structuredTableObj.Data[i][propertyNames[j]] = data[dataIndex].IntegerValue;
                                break;
                            // Null
                            default:
                                break;
                        }
                    }
                }

                structuredTableObj.Save(Path.Combine(instance.ExportFolder, asset.Name));

                // Done
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
