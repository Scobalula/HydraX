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
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                public byte[] Padding2; // Always null?
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
                public long[] BufferPointers; // Note: needs more work, along with below, looks like pixel AND vertex shader settings for the pass
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
            /// Image Semantics
            /// </summary>
            public static string[] Semantics { get; } =
            {
                "UNKNOWN",
                "2d",
                "diffuseMap",
                "effectMap",
                "normalMap",
                "specularMask",
                "specularMap",
                "glossMap",
                "occlusionMap",
                "revealMap",
                "multipleMask",
                "thicknessMap",
                "camoMap",
                "One Channel",
                "Two Channel",
                "Emblem",
                "Custom",
                "LutTpage",
                "Light Cookie",
                "HDR",
                "Eye Caustic"
            };

            /// <summary>
            /// Image Compression
            /// </summary>
            public static string[] Compression { get; } =
            {
                "compressed",
                "compressed",
                "compressed high color",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed",
                "compressed"
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
            public long EndAddress { get; set; }

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
            public List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;
                EndAddress = StartAddress + (AssetCount * AssetSize);

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = instance.Reader.ReadStruct<MaterialAsset>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name        = instance.Reader.ReadNullTerminatedString(header.NamePointer).Split('|')[0],
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
                var header = instance.Reader.ReadStruct<MaterialAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                foreach (var result in ExportMTL(header, instance))
                {
                    instance.AddGDTAsset(result, result.Type, result.Name);
                }
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public static List<GameDataTable.Asset> ExportMTL(MaterialAsset header, HydraInstance instance)
            {
                var results = new List<GameDataTable.Asset>();

                var name = Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(header.NamePointer).Split('|')[0]);

                // We need the techset for buffer info
                var techset = instance.Reader.ReadStruct<MaterialTechniqueSet>(header.TechniquePointer);
                var mtlType = Path.GetFileNameWithoutExtension(instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(header.TechniquePointer)).Split('#')[0]);



                if(!instance.TechniqueSetCache.TryGetValue(mtlType, out var set))
                    throw new ArgumentException("Unknown material type.", mtlType);

                // Add base stuffs
                // Create asset
                var gdtAsset = new GameDataTable.Asset(name, "material");
                // Set Default Properties
                gdtAsset.Properties.Add("surfaceType", SurfaceTypes.TryGetValue(BitConverter.ToUInt32(header.FlagsAndSettings, 28), out var surfaceType) ? surfaceType : "<none>");
                gdtAsset.Properties.Add("template", "material.template");
                gdtAsset.Properties.Add("materialCategory", set.Category);
                gdtAsset.Properties.Add("materialType", mtlType);
                gdtAsset.Properties.Add("textureAtlasRowCount", header.FlagsAndSettings[6].ToString());
                gdtAsset.Properties.Add("textureAtlasColumnCount", header.FlagsAndSettings[7].ToString());
                gdtAsset.Properties.Add("usage", "<not in editor>");

                for (int j = 0; j < header.Counts[0]; j++)
                {
                    var materialImage = instance.Reader.ReadStruct<MaterialImage>(header.ImageTablePointer + (j * 0x20));

                    if (set.ImageSlots.TryGetValue(materialImage.SemanticHash, out string slot))
                    {
                        gdtAsset.Properties[slot] = instance.Reader.ReadNullTerminatedString(instance.Reader.ReadInt64(materialImage.ImagePointer + 0xF8));

                        // Ignore combo, overrides the actual material we're looking for!
                        if (gdtAsset.Properties[slot].ToString().EndsWith("_combo"))
                            throw new Exception("Invalid Material. Contains combo image, look for another material with same name!");

                        // TODO: Improve this (and material support overall)
                        var imgAsset = new GameDataTable.Asset(gdtAsset.Properties[slot].ToString(), "image");

                        imgAsset["baseImage"]         = string.Format("<EXPORT_DIRECTORY_FIND_REPLACE_ME_OwO>\\\\{0}.png", imgAsset.Name);
                        imgAsset["imageType"]         = "Texture";
                        imgAsset["type"]              = "image";
                        imgAsset["compressionMethod"] = Compression[instance.Reader.ReadBytes(materialImage.ImagePointer + 161, 1)[0]];
                        imgAsset["semantic"]          = Semantics[instance.Reader.ReadBytes(materialImage.ImagePointer + 161, 1)[0]];

                        results.Add(imgAsset);
                    }
                }

                for (int i = 0; i < header.SettingsPointers.Length; i++)
                {
                    // All pointers are 0, ignore
                    if (header.SettingsPointers[i].BufferPointers[0] == 0 &&
                        header.SettingsPointers[i].BufferPointers[1] == 0 &&
                        header.SettingsPointers[i].BufferPointers[2] == 0 &&
                        header.SettingsPointers[i].BufferPointers[3] == 0)
                        continue;

                    var technique = instance.Reader.ReadStruct<MaterialTechnique>(techset.TechniquePointers[i]);
                    var pass = instance.Reader.ReadStruct<MaterialTechniquePass>(technique.PassPointer);
                    var settings = ParseDXBC(instance.Reader.ReadBytes(pass.ShaderPointer, pass.ShaderSize));

                    if (settings == null)
                        continue;

                    foreach(var bufferPointer in header.SettingsPointers[i].BufferPointers)
                    {
                        var settingsInfo = instance.Reader.ReadStruct<MaterialSettingBuffer>(bufferPointer);
                        var settingsBuffer = instance.Reader.ReadBytes(settingsInfo.BufferPointer, (int)settingsInfo.Size);

                        foreach (var setting in settings)
                        {
                            // Attempt it, since some techsets mark settings, but then the settings are removed, etc. via .tweak = ""

                            try
                            {
                                if (set.Settings.TryGetValue(setting.Key, out var gdtInfo))
                                {
                                    // Everything is a float, even bools and ints
                                    switch (gdtInfo.DataType)
                                    {
                                        case SettingDataType.Boolean:
                                            {
                                                var value = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = (int)value;
                                                break;
                                            }
                                        case SettingDataType.UInt1:
                                            {
                                                var value = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)value;
                                                break;
                                            }
                                        case SettingDataType.UInt2:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)value2;
                                                break;
                                            }
                                        case SettingDataType.UInt3:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                                var value3 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)value2;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]) || value3 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[2]] = (uint)value3;
                                                break;
                                            }
                                        case SettingDataType.UInt4:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                                var value3 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                                var value4 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = (uint)value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = (uint)value2;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]) || value3 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[2]] = (uint)value3;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[3]) || value4 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[3]] = (uint)value4;
                                                break;
                                            }
                                        case SettingDataType.Float1:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = value1;
                                                break;
                                            }
                                        case SettingDataType.Float2:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = value2;
                                                break;
                                            }
                                        case SettingDataType.Float3:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                                var value3 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = value2;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]) || value3 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[2]] = value3;
                                                break;
                                            }
                                        case SettingDataType.Float4:
                                            {
                                                var value1 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor);
                                                var value2 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor);
                                                var value3 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor);
                                                var value4 = PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || value1 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = value1;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[1]) || value2 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[1]] = value2;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[2]) || value3 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[2]] = value3;
                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[3]) || value4 > 0)
                                                    gdtAsset[gdtInfo.GDTSlotNames[3]] = value4;
                                                break;
                                            }
                                        case SettingDataType.Color:
                                            {
                                                var r = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 00), gdtInfo.PostProcessor), 1.0, 0.0);
                                                var g = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 04), gdtInfo.PostProcessor), 1.0, 0.0);
                                                var b = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 08), gdtInfo.PostProcessor), 1.0, 0.0);
                                                var a = MathUtilities.Clamp(PerformPostProcess(BitConverter.ToSingle(settingsBuffer, setting.Value + 12), gdtInfo.PostProcessor), 1.0, 0.0);

                                                if (!gdtAsset.Properties.ContainsKey(gdtInfo.GDTSlotNames[0]) || (r > 0 && b > 0 && g > 0 && a > 0))
                                                {
                                                    gdtAsset[gdtInfo.GDTSlotNames[0]] = string.Format("{0:0.000000} {1:0.000000} {2:0.000000} {3:0.000000}", r, g, b, a);
                                                }
                                                break;
                                            }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }

                results.Add(gdtAsset);

                return results;
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
            public bool IsNullAsset(long nameAddress)
            {
                return nameAddress >= StartAddress && nameAddress <= EndAddress || nameAddress == 0;
            }
        }
    }
}
