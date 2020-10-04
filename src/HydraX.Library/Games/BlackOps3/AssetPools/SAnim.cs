using PhilLibX.IO;
using SELib;
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
        private class SAnim : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Half Precision Float Struct
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct HalfFloat
            {
                public ushort Value;

                public HalfFloat(ushort value)
                {
                    Value = value;
                }

                public static unsafe implicit operator float(HalfFloat half)
                {
                    // From: https://github.com/microsoft/DirectXMath/
                    var mantissa = (uint)(half.Value & 0x03FF);
                    var exponent = (uint)(half.Value & 0x7C00);

                    if (exponent == 0x7C00) // INF/NAN
                    {
                        exponent = 0x8f;
                    }
                    else if (exponent != 0) // The value is normalized
                    {
                        exponent = (uint)(((int)half.Value >> 10) & 0x1F);
                    }
                    else if (mantissa != 0) // The value is denormalized
                    {
                        // Normalize the value in the resulting float
                        exponent = 1;

                        do
                        {
                            exponent--;
                            mantissa <<= 1;
                        } while ((mantissa & 0x0400) == 0);

                        mantissa &= 0x03FF;
                    }
                    else // The value is zero
                    {
                        exponent = unchecked((uint)-112);
                    }

                    var result =
                        (((uint)(half.Value) & 0x8000) << 16) // Sign
                        | ((exponent + 112) << 23)       // Exponent
                        | (mantissa << 13);

                    return *((float*)&result);
                }
            }

            /// <summary>
            /// Quaternion Struct
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct Quaternion
            {
                public HalfFloat X;
                public HalfFloat Y;
                public HalfFloat Z;
                public HalfFloat W;
            }

            /// <summary>
            /// Siege Anim Shot Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Size = 112)]
            internal unsafe struct GfxSiegeAnim
            {
                public long NamePointer;
                public long UncompressedPointer;
                public long CompressedPointer;
                public long ShotsPointer;
                public long EventsPointer;
                public fixed byte Unknown[44];
                public int NodeCount;
                public int FrameCount;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 136)]
            internal unsafe struct GfxSiegeAnimUncompressed
            {
                public fixed byte Unknown[40];
                public long DataHandleName;
                public int Unk;
                public int DataHandleSize;
                public long DataHandlePointer;
                public fixed byte UnknownFunctionPointers[24];
                public int SizeofBasePoseData;
                public int SizeofQuatPerFrame;
                public int SizeofTransPerFrame;
                public int UnknownValue;
                public int QuatsOffset;
                public int TransOffset;
                public int NodeCount;
                public int FrameCount;
                public int DataSize;
                public int QuatPrecision;
                public int TransPrecision;
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
            public string Name => "sanim";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "RawFile";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.sanim;

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
                    var header = instance.Reader.ReadStruct<GfxSiegeAnim>(StartAddress + (i * AssetSize));

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var address = StartAddress + (i * AssetSize);

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format($"Nodes: {header.NodeCount} Frames: {header.FrameCount}")
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<GfxSiegeAnim>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");
                // Haven't seen a compressed SAnim yet
                if (header.UncompressedPointer == 0)
                    throw new Exception("Compressed SAnims are not supported yet");

                var uncompressedInfo = instance.Reader.ReadStruct<GfxSiegeAnimUncompressed>(header.UncompressedPointer);

                string path = Path.Combine("exported_files", instance.Game.Name, "sanim", asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using(var dataReader = new BinaryReader(new MemoryStream(instance.Reader.ReadBytes(
                    uncompressedInfo.DataHandlePointer,
                    uncompressedInfo.DataHandleSize))))
                {
                    dataReader.BaseStream.Position = 0;

                    using(var basePoseWriter = new StreamWriter(path + "_base_pose.mel"))
                    {
                        basePoseWriter.Write($"select ");
                        for (int b = 0; b < uncompressedInfo.NodeCount; b++)
                            basePoseWriter.Write($" smod_bone{b}");
                        basePoseWriter.WriteLine(";");
                        basePoseWriter.WriteLine("moveJointsMode 1;");

                        dataReader.BaseStream.Position = 0;

                        for (int b = 0; b < uncompressedInfo.NodeCount; b++)
                        {
                            var x = dataReader.ReadSingle();
                            var y = dataReader.ReadSingle();
                            var z = dataReader.ReadSingle();
                            var w = dataReader.ReadSingle();

                            basePoseWriter.WriteLine($"setAttr smod_bone{b}.t {x * 2.54f} {y * 2.54f} {z * 2.54f};");
                        }

                        basePoseWriter.WriteLine("moveJointsMode 0;");
                    }

                    var anim = new SEAnim()
                    {
                        AnimType = AnimationType.Relative
                    };

                    dataReader.BaseStream.Position = uncompressedInfo.QuatsOffset;

                    for (int frame = 0; frame < uncompressedInfo.FrameCount; frame++)
                    {
                        for (int boneIndex = 0; boneIndex < uncompressedInfo.NodeCount; boneIndex++)
                        {
                            anim.AddRotationKey
                            (
                                $"smod_bone{boneIndex}",
                                frame,
                                new HalfFloat(dataReader.ReadUInt16()),
                                new HalfFloat(dataReader.ReadUInt16()),
                                new HalfFloat(dataReader.ReadUInt16()),
                                new HalfFloat(dataReader.ReadUInt16())
                            );
                        }
                    }

                    dataReader.BaseStream.Position = uncompressedInfo.TransOffset;

                    for (int frame = 0; frame < uncompressedInfo.FrameCount; frame++)
                    {
                        for (int boneIndex = 0; boneIndex < uncompressedInfo.NodeCount; boneIndex++)
                        {
                            anim.AddTranslationKey
                            (
                                $"smod_bone{boneIndex}",
                                frame,
                                new HalfFloat(dataReader.ReadUInt16()),
                                new HalfFloat(dataReader.ReadUInt16()),
                                new HalfFloat(dataReader.ReadUInt16())
                            );

                            // Skip padding
                            dataReader.BaseStream.Position += 2;
                        }
                    }

                    anim.Write(path + ".seanim");
                }



                return;
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
