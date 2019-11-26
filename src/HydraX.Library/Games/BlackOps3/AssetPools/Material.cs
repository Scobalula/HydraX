using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;
using PhilLibX;
using PhilLibX.IO;
using PhilLibX.Mathematics;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Techset Logic
        /// </summary>
        private class Material : IAssetPool
        {
            /// <summary>
            /// Black Ops 3 Material Asset
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct MaterialAsset
            {
                public long NamePointer { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
                public byte[] FlagsAndSettings;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
                public MaterialSettings[] SettingsPointers;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] Counts;
                public long TechniquePointer { get; set; }
                public long ImageTablePointer { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
                public byte[] Padding2;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct MaterialSettings
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
                public byte[] Padding2; // Always null?
                public long UnkPointer;
                public long SettingsPointer;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct MaterialSettingBuffer
            {
                public long UnkPointer;
                public long Padding;
                public long Padding2;
                public long BufferPointer;
                public long Size;
                public long BufferPointer2;
                public long Size2;
                public long NamePointer;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct MaterialImage
            {
                public long ImagePointer { get; set; }
                public uint SemanticHash { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
                public byte[] Padding;
            }

            /// <summary>
            /// DirectX Shader Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct DXBCContainerHeader
            {
                public uint HeaderFourCC;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                public byte[] Checksum;
                public int Version;
                public int ContainerSizeInBytes;
                public int PartCount;
            }

            /// <summary>
            /// DirectX Part Header
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct DXBCPartHeader
            {
                public int PartFourCC;
                public int PartSize;
            }

            /// <summary>
            /// DirectX Variable
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct DXBCConstantVariable
            {
                public int NameOffset;
                public int DataOffset;
                public int DataSize;
                public int Flags;
                public int TypeOffset;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
                public byte[] Padding;
            }

            /// <summary>
            /// DirectX Variable
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            struct DXBCDataType
            {
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
                public byte[] Padding;
                public int TypeNameOffset;
            }

            #region AssetStructures
            /// <summary>
            /// Techset Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MaterialTechniqueSet
            {
                #region TechsetProperties
                public long NamePointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] Flags;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
                public long[] TechniquePointers;
                #endregion
            }

            /// <summary>
            /// Technique Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MaterialTechnique
            {
                #region WeaponCamoEntryProperties
                public long NamePointer;
                public long Unk;
                public long UnkPointer;
                public int Unk02;
                public int Unk03;
                public long UnkPointer02;
                public long PassPointer;
                #endregion
            }

            /// <summary>
            /// Technique Shader Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct MaterialTechniquePass
            {
                #region WeaponCamoEntryProperties
                public long NamePointer;
                public long DefsPointer;
                public long UnkFunctionPointer;
                public long ShaderPointer;
                public int ShaderSize;
                #endregion
            }
            #endregion

            /// <summary>
            /// Black Ops 3 Surface Types
            /// </summary>
            public static Dictionary<uint, string> SurfaceTypes = new Dictionary<uint, string>()
            {
                { 23068672, "asphalt" },
                { 1048576 , "bark" },
                { 2097152, "brick" },
                { 3145728, "carpet" },
                { 4194304, "cloth" },
                { 5242880, "concrete" },
                { 6291456, "dirt" },
                { 7340032, "flesh" },
                { 8388608, "foliage" },
                { 9437184, "glass" },
                { 10485760, "grass" },
                { 11534336,"gravel"  },
                { 12582912, "ice" },
                { 13631488, "metal" },
                { 14680064, "mud" },
                { 15728640, "paper" },
                { 16777216, "plaster" },
                { 17825792, "rock" },
                { 18874368, "sand" },
                { 19922944, "snow" },
                { 20971520, "water"  },
                { 22020096, "wood" },
                { 24117248, "ceramic" },
                { 25165824, "plastic" },
                { 26214400, "rubber" },
                { 27262976, "cushion"  },
                { 28311552, "fruit" },
                { 29360128, "paintedmetal" },
                { 31457280, "tallgrass" },
                { 32505856, "riotshield" },
                { 30408704, "player" },
                { 33554432, "metalthin" },
                { 34603008, "metalhollow" },
                { 35651584, "metalcatwalk" },
                { 36700160, "metalcar" },
                { 37748736, "glasscar" },
                { 38797312, "glassbulletproof" },
                { 39845888, "watershallow" },
                { 40894464, "bodyarmor" }
            };

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
            public string Name => "material";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Materials";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.material;

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
                    var header = instance.Reader.ReadStruct<MaterialAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new GameAsset()
                    {
                        Name = Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(header.NamePointer).Split('|')[0]),
                        HeaderAddress = StartAddress + (i * AssetSize),
                        AssetPool = this,
                        Type = Name,
                        Information = string.Format("Type: {0}", Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(header.TechniquePointer)).Split('#')[0]))
                });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public HydraStatus Export(GameAsset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<MaterialAsset>(asset.HeaderAddress);

                if (asset.Name != Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(header.NamePointer).Split('|')[0]))
                    return HydraStatus.MemoryChanged;

                // We need the techset for buffer info
                var techset = instance.Reader.ReadStruct<MaterialTechniqueSet>(header.TechniquePointer);
                var mtlType = Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(header.TechniquePointer)).Split('#')[0]);



                if(!instance.TechniqueSetCache.TryGetValue(mtlType, out var set))
                    throw new ArgumentException("Unknown material type.", mtlType);

                // Add base stuffs
                // Create asset
                var gdtAsset = new GameDataTable.Asset(asset.Name, "material");
                // Set Default Properties
                gdtAsset.Properties.Add("surfaceType", SurfaceTypes.TryGetValue(BitConverter.ToUInt32(header.FlagsAndSettings, 28), out var surfaceType) ? surfaceType : "<none>");
                gdtAsset.Properties.Add("template", "material.template");
                gdtAsset.Properties.Add("materialCategory", set.Category);
                gdtAsset.Properties.Add("materialType", mtlType);
                gdtAsset.Properties.Add("textureAtlasRowCount", header.FlagsAndSettings[6].ToString());
                gdtAsset.Properties.Add("textureAtlasColumnCount", header.FlagsAndSettings[7].ToString());
                gdtAsset.Properties.Add("usage", "<not in editor>");

                for(int i = 0; i < header.SettingsPointers.Length; i++)
                {
                    if (header.SettingsPointers[i].SettingsPointer == 0)
                        continue;

                    var technique = instance.Reader.ReadStruct<MaterialTechnique>(techset.TechniquePointers[i]);
                    var pass = instance.Reader.ReadStruct<MaterialTechniquePass>(technique.PassPointer);
                    var settings = ParseDXBC(instance.Reader.ReadBytes(pass.ShaderPointer, pass.ShaderSize));

                    if (settings == null)
                        throw new ArgumentException("Failed to find $Globals in DirectX Byte Code RDEF part", "settings");

                    var settingsInfo = instance.Reader.ReadStruct<MaterialSettingBuffer>(header.SettingsPointers[i].SettingsPointer);
                    var settingsBuffer = instance.Reader.ReadBytes(settingsInfo.BufferPointer, (int)settingsInfo.Size);

                    foreach(var setting in settings)
                    {
                        if(set.Settings.TryGetValue(setting.Key, out var gdtInfo))
                        {
                            switch (gdtInfo.DataType)
                            {
                                case SettingDataType.Boolean:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = (int)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.UInt1:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.UInt2:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.UInt3:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.UInt4:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[3]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.Float1:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.Float2:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.Float3:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.Float4:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[3]))
                                            gdtAsset[gdtInfo.GDTSlotNames[1]] = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor);
                                        break;
                                    }
                                case SettingDataType.Color:
                                    {
                                        if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]))
                                        {
                                            var r = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor), 1.0, 0.0);
                                            var g = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor), 1.0, 0.0);
                                            var b = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor), 1.0, 0.0);
                                            var a = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor), 1.0, 0.0);

                                            gdtAsset[gdtInfo.GDTSlotNames[0]] = string.Format("{0:0.000000} {1:0.000000} {2:0.000000} {3:0.000000}", r, g, b, a);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }


                for (int j = 0; j < header.Counts[0]; j++)
                {
                    var materialImage = instance.Reader.ReadStruct<MaterialImage>(header.ImageTablePointer + (j * 0x20));

                    if (set.ImageSlots.TryGetValue(materialImage.SemanticHash, out string slot))
                    {
                        gdtAsset.Properties[slot] = instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(materialImage.ImagePointer + 0xF8));
                    }
                }

                instance.GDTs["Misc"][asset.Name] = gdtAsset;

                return HydraStatus.Success;
            }

            public static double PerformPostProcess(double value, SettingPostProcess processor)
            {
                switch(processor)
                {
                    case SettingPostProcess.LinearToRGB: return value <= 0.0031308 ? value * 12.92 : 1.055 * Math.Pow(value, 1.0 / 2.4) - 0.055;
                    case SettingPostProcess.Radians: return value * 57.2958;
                    case SettingPostProcess.SqRt: return Math.Sqrt(value);
                    default: return value;
                }
            }

            public static Dictionary<string, int> ParseDXBC(byte[] buffer)
            {
                using (var reader = new BinaryReader(new MemoryStream(buffer)))
                {
                    var header = reader.ReadStruct<DXBCContainerHeader>();
                    var partOffsets = reader.ReadArray<int>(header.PartCount);

                    foreach (var partOffset in partOffsets)
                    {
                        reader.BaseStream.Position = partOffset;
                        if (reader.ReadStruct<DXBCPartHeader>().PartFourCC == 0x46454452) // RDEF
                            return ParseResourceDefinitions(reader);
                    }
                }

                return null;
            }

            public static Dictionary<string, int> ParseResourceDefinitions(BinaryReader reader)
            {
                var begin = reader.BaseStream.Position;

                var constantBufferCount = reader.ReadInt32();
                var constantBufferOffset = reader.ReadInt32();
                var resourceBindingCount = reader.ReadInt32();
                var resourceBindingOffset = reader.ReadInt32();

                reader.BaseStream.Position = begin + constantBufferOffset;

                for (int i = 0; i < constantBufferCount; i++)
                {
                    var nameOffset = reader.ReadInt32();
                    var varCount = reader.ReadInt32();
                    var varOffset = reader.ReadInt32();
                    reader.BaseStream.Position += 12;

                    if (reader.ReadNullTerminatedString(begin + nameOffset) == "$Globals")
                    {
                        var globals = new Dictionary<string, int>();

                        foreach (var type in reader.ReadArray<DXBCConstantVariable>(begin + varOffset, varCount))
                            globals[reader.ReadNullTerminatedString(begin + type.NameOffset)] = type.DataOffset;

                        return globals;
                    }
                }

                return null;
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
