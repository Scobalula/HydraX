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
        private class XAnim : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// XAnim Vector Types
            /// </summary>
            private enum TranslationVectorType : byte
            {
                UShortVec = 0,
                ByteVec = 1,
            }

            /// <summary>
            /// XAnim Part Type
            /// </summary>
            private enum PartType : int
            {
                NoQuat                = 0x0,
                HalfQuat              = 0x1,
                FullQuat              = 0x2,
                HalfQuatNoSize        = 0x3,
                FullQuatNoSize        = 0x4,
                SmallTranslation      = 0x5,
                FullTranslation       = 0x6,
                FullTranslationNoSize = 0x7,
                NoTranslation         = 0x8,
                AllParts              = 0x9,
            }

            /// <summary>
            /// XAnim Notify Info Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XAnimNotifyInfo
            {
                public int Type;
                public float Time;
                public int Param1;
                public int Param2;
            }

            /// <summary>
            /// XAnim Notifications Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XAnimNotifies
            {
                public long NotifyInfoPointer;
                public byte Count;
            }

            /// <summary>
            /// Raw File Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private unsafe struct XAnimAsset
            {
                public long NamePointer;
                public int RandomDataByteCount;
                public int DataShortCount;
                public int ExtraChannelDataCount;
                public int DataByteCount;
                public int DataIntCount;
                public int RandomDataIntCount;
                public ushort FrameCount;
                public ushort BoneCount;
                public fixed byte Flags[12];
                public fixed ushort BoneCounts[10]; // Above is total, this is across different part types, index using the Parts enum
                public fixed byte Flags2[4];
                public int RandomDataShortCount;
                public int IndexCount;
                public float FrameRate;
                public float Frequency;
                public float PrimedLength;
                public float LoopEntryTime;
                public int IKPitchLayerCount;
                public int IKPitchBoneCount;
                public long NamesPointer;
                public long DataBytePointer;
                public long DataShortPointer;
                public long DataIntPointer;
                public long RandomDataShortPointer;
                public long RandomDataBytePointer;
                public long RandomDataIntPointer;
                public long ExtraChannelDataPointer;
                public long IndicesPointer;
                public long IKPitchLayersPointer;
                public long IKPitchBonesPointer;
                public XAnimNotifies Notes;
                public XAnimNotifies StartupNotes;
                public XAnimNotifies ShutdownNotes;
                public long DeltaPartsPointer;

                /// <summary>
                /// Converts pointers to -1 to indicate use
                /// </summary>
                public void ConvertPointers()
                {
                    if (NamePointer > 0) NamePointer                         = 0x4C554C38304335;
                    if (NamesPointer > 0) NamesPointer                       = 0x4C554C38304335;
                    if (DataBytePointer > 0) DataBytePointer                 = 0x4C554C38304335;
                    if (DataShortPointer > 0) DataShortPointer               = 0x4C554C38304335;
                    if (DataIntPointer > 0) DataIntPointer                   = 0x4C554C38304335;
                    if (RandomDataShortPointer > 0) RandomDataShortPointer   = 0x4C554C38304335;
                    if (RandomDataBytePointer > 0) RandomDataBytePointer     = 0x4C554C38304335;
                    if (RandomDataIntPointer > 0) RandomDataIntPointer       = 0x4C554C38304335;
                    if (ExtraChannelDataPointer > 0) ExtraChannelDataPointer = 0x4C554C38304335;
                    if (IndicesPointer > 0) IndicesPointer                   = 0x4C554C38304335;
                    if (IKPitchLayersPointer > 0) IKPitchLayersPointer       = 0x4C554C38304335;
                    if (IKPitchLayersPointer > 0) IKPitchLayersPointer       = 0x4C554C38304335;
                    if (DeltaPartsPointer > 0) DeltaPartsPointer             = 0x4C554C38304335;

                    if (Notes.NotifyInfoPointer > 0) Notes.NotifyInfoPointer = 0x4C554C38304335;
                    if (StartupNotes.NotifyInfoPointer > 0) Notes.NotifyInfoPointer = 0x4C554C38304335;
                    if (ShutdownNotes.NotifyInfoPointer > 0) Notes.NotifyInfoPointer = 0x4C554C38304335;
                }
            }

            /// <summary>
            /// XAnim Delta Part Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XAnimDeltaPart
            {
                public long TranslationsPointer;
                public long Quaternions2DPointer;
                public long QuaternionsPointer;

                /// <summary>
                /// Converts pointers to -1 to indicate use
                /// </summary>
                public void ConvertPointers()
                {
                    if (TranslationsPointer > 0) TranslationsPointer = 0x4C554C38304335;
                    if (Quaternions2DPointer > 0) Quaternions2DPointer = 0x4C554C38304335;
                    if (QuaternionsPointer > 0) QuaternionsPointer = 0x4C554C38304335;
                }
            }

            /// <summary>
            /// XAnim Delta Part Translations Structure
            /// </summary>
            [StructLayout(LayoutKind.Explicit)]
            private struct XAnimDeltaPartTrans
            {
                [FieldOffset(0)]
                public ushort Size;
                [FieldOffset(2)]
                public byte SmallTrans;
                [FieldOffset(8)]
                public Vector3 Frame0;
                [FieldOffset(8)]
                public Vector3 Min;
                [FieldOffset(20)]
                public Vector3 Max;
                [FieldOffset(32)]
                public long FramesPointer;
            }

            /// <summary>
            /// XAnim Types
            /// </summary>
            public string[] XAnimTypes =
            {
                "absolute",
                "relative",
                "delta",
                "mp_torso",
                "mp_legs",
                "mp_fullbody",
                "additive",
                "delta3d",
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
            public string Name => "xanim";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.xanim;

            /// <summary>
            /// Loads Assets from this Asset Pool
            /// </summary>
            public unsafe List<Asset> Load(HydraInstance instance)
            {
                var results = new List<Asset>();

                var poolInfo = instance.Reader.ReadStruct<AssetPoolInfo>(instance.Game.AssetPoolsAddress + (Index * 0x20));

                StartAddress = poolInfo.PoolPointer;
                AssetSize = poolInfo.AssetSize;
                AssetCount = poolInfo.PoolSize;

                var voidXAnim = new XAnimAsset();

                var headers = instance.Reader.ReadArrayUnsafe<XAnimAsset>(StartAddress, AssetCount);

                for (int i = 0; i < AssetCount; i++)
                {
                    var header = headers[i];

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    var name = instance.Reader.ReadNullTerminatedString(header.NamePointer);

                    if (header.DataBytePointer        != voidXAnim.DataBytePointer &&
                        header.DataShortPointer       != voidXAnim.DataShortPointer &&
                        header.DataIntPointer         != voidXAnim.DataIntPointer &&
                        header.RandomDataBytePointer  != voidXAnim.RandomDataBytePointer &&
                        header.RandomDataShortPointer != voidXAnim.RandomDataShortPointer &&
                        header.RandomDataIntPointer   != voidXAnim.RandomDataBytePointer)
                    {
                        var address = StartAddress + (i * AssetSize);

                        results.Add(new Asset()
                        {
                            Name        = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                            Type        = Name,
                            Zone        = ((BlackOps3)instance.Game).ZoneNames[address],
                            Information = string.Format("Bones: {0} Frames: {1} Type: {2}", header.BoneCount, header.FrameCount, XAnimTypes[header.Flags2[0]]),
                            Status      = "Loaded",
                            Data        = address,
                            LoadMethod  = ExportAsset,
                        });
                    }
                    else if(name == "void")
                    {
                        voidXAnim = header;
                        continue;
                    }
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var xanimAsset = instance.Reader.ReadStruct<XAnimAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(xanimAsset.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                string path = Path.Combine("exported_files", instance.Game.Name, "share", "raw", "xanim_raw", asset.Name + ".xanim_raw");
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var writer = new BinaryWriter(File.Create(path)))
                using (var infoWriter = new StreamWriter(path + ".info.txt"))
                {
                    var fileHeader = instance.Reader.ReadStruct<XAnimAsset>((long)asset.Data);
                    fileHeader.ConvertPointers();

                    writer.WriteStruct(fileHeader);

                    writer.WriteNullTerminatedString(instance.Reader.ReadNullTerminatedString(xanimAsset.NamePointer));

                    for (int i = 0; i < xanimAsset.BoneCount; i++)
                        writer.WriteNullTerminatedString(instance.Game.GetString(instance.Reader.ReadInt32(xanimAsset.NamesPointer + i * 4), instance));

                    writer.Write(instance.Reader.ReadBytes(xanimAsset.DataBytePointer, xanimAsset.DataByteCount));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.DataShortPointer, xanimAsset.DataShortCount * 2));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.DataIntPointer, xanimAsset.DataIntCount * 4));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.RandomDataBytePointer, xanimAsset.RandomDataByteCount));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.RandomDataShortPointer, xanimAsset.RandomDataShortCount * 2));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.RandomDataIntPointer, xanimAsset.RandomDataIntCount * 4));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.ExtraChannelDataPointer, xanimAsset.ExtraChannelDataCount));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.IKPitchLayersPointer, xanimAsset.IKPitchLayerCount * 8));
                    writer.Write(instance.Reader.ReadBytes(xanimAsset.IKPitchBonesPointer, xanimAsset.IKPitchBoneCount * 28));

                    WriteNotetracks(xanimAsset.Notes,           infoWriter, writer, instance);
                    WriteNotetracks(xanimAsset.StartupNotes,    infoWriter, writer, instance);
                    WriteNotetracks(xanimAsset.ShutdownNotes,   infoWriter, writer, instance);

                    // Delta data requires a bit more work
                    if (xanimAsset.DeltaPartsPointer > 0)
                    {
                        var xanimDeltaParts = instance.Reader.ReadStruct<XAnimDeltaPart>(xanimAsset.DeltaPartsPointer);
                        var xanimDeltaPartsFile = instance.Reader.ReadStruct<XAnimDeltaPart>(xanimAsset.DeltaPartsPointer);
                        xanimDeltaPartsFile.ConvertPointers();

                        writer.WriteStruct(xanimDeltaPartsFile);

                        if (xanimDeltaParts.TranslationsPointer > 0)
                        {
                            var translationCount = instance.Reader.ReadUInt16(xanimDeltaParts.TranslationsPointer) + 1;
                            var byteTranslations = instance.Reader.ReadUInt16(xanimDeltaParts.TranslationsPointer + 2);

                            writer.Write(translationCount);
                            writer.Write(byteTranslations);

                            if (translationCount == 1)
                            {
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.TranslationsPointer + 8, 12));
                            }
                            else
                            {
                                var indexBufferSize = xanimAsset.FrameCount >= 0x100 ? translationCount * 2 : translationCount;
                                var frameBufferSize = byteTranslations == 0 ? translationCount * 6 : translationCount * 3;

                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.TranslationsPointer + 8, 12));
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.TranslationsPointer + 20, 12));
                                writer.Write(instance.Reader.ReadBytes(instance.Reader.ReadInt64(xanimDeltaParts.TranslationsPointer + 32), frameBufferSize));
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.TranslationsPointer + 40, indexBufferSize));
                            }
                        }

                        if (xanimDeltaParts.Quaternions2DPointer > 0)
                        {
                            var rotationCount = instance.Reader.ReadUInt16(xanimDeltaParts.Quaternions2DPointer) + 1;

                            writer.Write(rotationCount);

                            if (rotationCount == 1)
                            {
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.Quaternions2DPointer + 8, 4));
                            }
                            else
                            {
                                var indexBufferSize = xanimAsset.FrameCount >= 0x100 ? rotationCount * 2 : rotationCount;

                                writer.Write(instance.Reader.ReadBytes(instance.Reader.ReadInt64(xanimDeltaParts.Quaternions2DPointer + 8), rotationCount * 4));
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.Quaternions2DPointer + 16, indexBufferSize));
                            }
                        }

                        if (xanimDeltaParts.QuaternionsPointer > 0)
                        {
                            var rotationCount = instance.Reader.ReadUInt16(xanimDeltaParts.QuaternionsPointer) + 1;

                            writer.Write(rotationCount);

                            if (rotationCount == 1)
                            {
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.QuaternionsPointer + 8, 8));
                            }
                            else
                            {
                                var indexBufferSize = xanimAsset.FrameCount >= 0x100 ? rotationCount * 2 : rotationCount;

                                writer.Write(instance.Reader.ReadBytes(instance.Reader.ReadInt64(xanimDeltaParts.QuaternionsPointer + 8), rotationCount * 8));
                                writer.Write(instance.Reader.ReadBytes(xanimDeltaParts.QuaternionsPointer + 16, indexBufferSize));
                            }
                        }
                    }
                }
            }

            private void WriteNotetracks(XAnimNotifies notifies, StreamWriter infoWriter, BinaryWriter writer, HydraInstance instance)
            {
                var notifyInfo = instance.Reader.ReadArrayUnsafe<XAnimNotifyInfo>(notifies.NotifyInfoPointer, notifies.Count);

                foreach (var notify in notifyInfo)
                {
                    var time = notify.Time;
                    var type = instance.Game.GetString(notify.Type, instance);
                    var p1 = instance.Game.GetString(notify.Param1, instance);
                    var p2 = instance.Game.GetString(notify.Param2, instance);

                    writer.Write(time);
                    writer.WriteNullTerminatedString(type);
                    writer.WriteNullTerminatedString(p1);
                    writer.WriteNullTerminatedString(p2);

                    infoWriter.WriteLine("Time:     {0}", time);
                    infoWriter.WriteLine("Type:     {0}", type);
                    infoWriter.WriteLine("Param1:   {0}", p1);
                    infoWriter.WriteLine("Param2:   {0}", p2);
                    infoWriter.WriteLine();
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
