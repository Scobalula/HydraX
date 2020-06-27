using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HydraX.Library.CommonStructures;
using HydraX.Library.AssetContainers;
using HydraX.Library.Utility;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 XCam Logic
        /// </summary>
        private class XCam : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// XCam Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XCamAsset
            {
                #region XCamProperties
                public long NamePointer;
                public long ParentScenePointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
                public byte[] Flags;
                public int CameraCount;
                public int FrameCount;
                public int FrameRate;
                public int NotetrackCount;
                public Vector4 AlignNodeRotation;
                public Vector3 AlignNodeOffset;
                public long TargetModelRootBonePointer;
                public long CamerasPointer;
                public long CameraSwitchPointer;
                public long NotetracksPointer;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2C)]
                public byte[] UnknownBytes; // Parent Scene Related
                public Vector3 RightStickRotationOffset;
                public Vector2 RightStickRotationDegrees;
                #endregion
            }

            /// <summary>
            /// XCam Camera Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XCamCamera
            {
                #region XCamCameraProperties
                public int NameStringIndex;
                public int Index;
                public int AnimationCount;
                public bool FocalLengthIsZero;
                public long AnimationsPointer;
                public float FarZ;
                #endregion
            }

            /// <summary>
            /// XCam Camera Anim Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct XCamCameraAnimation
            {
                #region XCamCameraAnimProperties
                public int Frame;
                public Vector3 Origin;
                public Vector4 Rotation;
                public float FieldofView;
                public float FocalLength;
                public float FDist;
                public float FStop;
                #endregion
            }

            /// <summary>
            /// XCam Target Model Root Bone Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XCamTargetModelRootBone
            {
                #region XCamTargetModelRootBoneProperties
                public int NameStringIndex;
                public int AnimationCount;
                public long AnimationsPointer;
                #endregion
            }

            /// <summary>
            /// XCam Target Model Root Bone Anim Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct XCamTargetModelRootBoneAnimation
            {
                #region XCamTargetModelRootBoneAnimProperties
                public Vector3 Origin;
                public Vector4 Rotation;
                #endregion
            }

            /// <summary>
            /// XCam Notetrack Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XCamNotetrack
            {
                #region XCamNotetrackProperties
                public int NameStringIndex;
                public int Frame;
                #endregion
            }

            /// <summary>
            /// XCam Camera Switch Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct XCamSwitch
            {
                #region XCamSwitchProperties
                public int Frame;
                public int Camera1Index;
                public int Camera2Index;
                public int Dissolve;
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
            public string Name => "xcam";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.xcam;

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
                    var header = instance.Reader.ReadStruct<XCamAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Frames: {0}", header.FrameCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<XCamAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var path = Path.Combine(instance.ExportFolder, "xanim_export", "hydra_xcams", asset.Name + ".xcam_export");

                var gdtAsset = new GameDataTable.Asset(asset.Name, "xcam");

                gdtAsset["hide_hud"]                    = header.Flags[0];
                gdtAsset["is_looping"]                  = header.Flags[1];
                gdtAsset["hide_local_player"]           = header.Flags[2];
                gdtAsset["use_firstperson_player"]      = header.Flags[3];
                gdtAsset["disableNearDof"]              = header.Flags[4];
                gdtAsset["autoMotionBlur"]              = header.Flags[5];
                gdtAsset["easeAnimationsOut"]           = header.Flags[6];
                gdtAsset["rightStickRotateOffsetX"]     = header.RightStickRotationOffset.X;
                gdtAsset["rightStickRotateOffsetY"]     = header.RightStickRotationOffset.Y;
                gdtAsset["rightStickRotateOffsetZ"]     = header.RightStickRotationOffset.Z;
                gdtAsset["rightStickRotateMaxDegreesX"] = header.RightStickRotationDegrees.X;
                gdtAsset["rightStickRotateMaxDegreesY"] = header.RightStickRotationDegrees.Y;
                gdtAsset["parent_scene"]                = instance.Reader.ReadNullTerminatedString(header.ParentScenePointer);
                gdtAsset["filename"]                    = Path.Combine("hydra_xcams", asset.Name + ".xcam_export");

                instance.AddGDTAsset(gdtAsset, gdtAsset.Type, gdtAsset.Name);

                var xcam = new XCamObj
                {
                    Cameras        = new XCamObj.Camera[header.CameraCount],
                    Notetracks     = new XCamObj.Notetrack[header.NotetrackCount],
                    CameraSwitches = new XCamObj.CameraSwitch[header.CameraSwitchPointer > 0 ? header.FrameCount : 0]
                };

                int frameCount = 0;

                var cameras = instance.Reader.ReadArray<XCamCamera>(header.CamerasPointer, header.CameraCount);

                for(int i = 0; i < cameras.Length; i++)
                {

                    xcam.Cameras[i] = new XCamObj.Camera
                    {
                        Farz = cameras[i].FarZ,
                        Name = instance.Game.GetString(cameras[i].NameStringIndex, instance),
                        Animations = new XCamObj.CameraAnimation[cameras[i].AnimationCount]
                    };

                    var animations = instance.Reader.ReadArray<XCamCameraAnimation>(cameras[i].AnimationsPointer, cameras[i].AnimationCount);

                    for (int j = 0; j < animations.Length; j++)
                    {
                        xcam.Cameras[i].Animations[j] = new XCamObj.CameraAnimation();

                        Matrix matrix = new Quaternion()
                        {
                            X = animations[i].Rotation.X,
                            Y = animations[i].Rotation.Y,
                            Z = animations[i].Rotation.Z,
                            W = animations[i].Rotation.W,
                        }.ToMatrix();

                        xcam.Cameras[i].Animations[j].Dir[0]      = matrix.X[0];
                        xcam.Cameras[i].Animations[j].Dir[1]      = matrix.Y[0];
                        xcam.Cameras[i].Animations[j].Dir[2]      = matrix.Z[0];
                        xcam.Cameras[i].Animations[j].Up[0]       = matrix.X[2];
                        xcam.Cameras[i].Animations[j].Up[1]       = matrix.Y[2];
                        xcam.Cameras[i].Animations[j].Up[2]       = matrix.Z[2];
                        xcam.Cameras[i].Animations[j].Right[0]    = matrix.X[1] * -1;
                        xcam.Cameras[i].Animations[j].Right[1]    = matrix.Y[1] * -1;
                        xcam.Cameras[i].Animations[j].Right[2]    = matrix.Z[1] * -1;
                        xcam.Cameras[i].Animations[j].Origin[0]   = animations[i].Origin.X;
                        xcam.Cameras[i].Animations[j].Origin[1]   = animations[i].Origin.Y;
                        xcam.Cameras[i].Animations[j].Origin[2]   = animations[i].Origin.Z;
                        xcam.Cameras[i].Animations[j].FieldOfView = animations[i].FieldofView;
                        xcam.Cameras[i].Animations[j].FocalLength = animations[i].FocalLength;
                        xcam.Cameras[i].Animations[j].FDist       = animations[i].FDist;
                        xcam.Cameras[i].Animations[j].FStop       = animations[i].FStop;
                        xcam.Cameras[i].Animations[j].Frame       = j;

                        if ((j + 1) > frameCount)
                            frameCount = j + 1;

                        if (j == 0)
                        {
                            xcam.Cameras[i].Origin      = xcam.Cameras[i].Animations[j].Origin;
                            xcam.Cameras[i].Dir         = xcam.Cameras[i].Animations[j].Dir;
                            xcam.Cameras[i].Up          = xcam.Cameras[i].Animations[j].Up;
                            xcam.Cameras[i].Right       = xcam.Cameras[i].Animations[j].Right;
                            xcam.Cameras[i].FieldOfView = xcam.Cameras[i].Animations[j].FieldOfView;
                            xcam.Cameras[i].FocalLength = xcam.Cameras[i].Animations[j].FocalLength;
                            xcam.Cameras[i].FDist       = xcam.Cameras[i].Animations[j].FDist;
                            xcam.Cameras[i].FStop       = xcam.Cameras[i].Animations[j].FStop;
                        }
                    }
                }

                if (header.TargetModelRootBonePointer > 0)
                {
                    var rootBone = instance.Reader.ReadStruct<XCamTargetModelRootBone>(header.TargetModelRootBonePointer);

                    xcam.TargetModelBoneRoots = new XCamObj.TargetModelBoneRoot[1]
                    {
                        new XCamObj.TargetModelBoneRoot()
                        {
                            Name = instance.Game.GetString(rootBone.NameStringIndex, instance),
                            Animation = new XCamObj.TargetModelBoneRootFrame[rootBone.AnimationCount]
                        }
                    };
    
                    var animations = instance.Reader.ReadArray<XCamTargetModelRootBoneAnimation>(rootBone.AnimationsPointer, rootBone.AnimationCount);

                    for (int i = 0; i < rootBone.AnimationCount; i++)
                    {
                        Matrix matrix = new Quaternion()
                        {
                            X = animations[i].Rotation.X,
                            Y = animations[i].Rotation.Y * -1,
                            Z = animations[i].Rotation.Z * -1,
                            W = animations[i].Rotation.W,
                        }.ToMatrix();

                        xcam.TargetModelBoneRoots[0].Animation[i] = new XCamObj.TargetModelBoneRootFrame();
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[0] = animations[i].Origin.X;
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[1] = animations[i].Origin.Y * -1;
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[2] = animations[i].Origin.Z;
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["x"] = matrix.X;
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["y"] = matrix.Y;
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["z"] = matrix.Z;
                        xcam.TargetModelBoneRoots[0].Animation[i].Frame = i;
                    }
                }

                var notetracks = instance.Reader.ReadArray<XCamNotetrack>(header.NotetracksPointer, header.NotetrackCount);

                for (int i = 0; i < notetracks.Length; i++)
                {
                    xcam.Notetracks[i] = new XCamObj.Notetrack(instance.Game.GetString(notetracks[i].NameStringIndex, instance), notetracks[i].Frame);
                }


                if(header.CameraSwitchPointer > 0)
                {
                    var switches = instance.Reader.ReadArray<XCamSwitch>(header.CameraSwitchPointer, header.FrameCount);

                    for (int i = 0; i < switches.Length; i++)
                    {
                        xcam.CameraSwitches[i] = new XCamObj.CameraSwitch()
                        {
                            Cameras = new int[2] { switches[i].Camera1Index, switches[i].Camera2Index },
                            Dissolve = switches[i].Dissolve,
                            Frame = switches[i].Frame
                        };
                    }
                }


                xcam.FrameCount = frameCount;

                xcam.Save(path);

                // Done
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
