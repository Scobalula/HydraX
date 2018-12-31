using System;
using System.IO;
using System.Runtime.InteropServices;
using HydraLib.Assets;
using HydraLib.Utility;
using PhilLibX;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class XCam
        {
            /// <summary>
            /// Bo3 StructuredTable Header
            /// </summary>
            public struct XCamHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long UnknownPointer { get; set; }

                /// <summary>
                /// Number of Properties of all Entries
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] Flags;

                /// <summary>
                /// Number of Cameras this XCam has
                /// </summary>
                public int CameraCount { get; set; }

                /// <summary>
                /// Number of Frames this XCam has
                /// </summary>
                public int FrameCount { get; set; }

                /// <summary>
                /// Frame Rate of this XCam
                /// </summary>
                public int FrameRate { get; set; }

                /// <summary>
                /// Number of Notetracks this XCam has
                /// </summary>
                public int NotetrackCount { get; set; }

                /// <summary>
                /// Align Node Rotation Quaternion
                /// </summary>
                public Vector4 AlignNodeRotation { get; set; }

                /// <summary>
                /// Align Node XYZ Offset
                /// </summary>
                public Vector3 AlignNodeOffset { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the target model root bone
                /// </summary>
                public long TargetModelRootBonePointer { get; set; }

                /// <summary>
                /// Pointer to the Cameras
                /// </summary>
                public long CamerasPointer { get; set; }

                /// <summary>
                /// Pointer to the Camera Switches
                /// </summary>
                public long CameraSwitchPointer { get; set; }

                /// <summary>
                /// Pointer to the Notetracks
                /// </summary>
                public long NotetracksPointer { get; set; }

                /// <summary>
                /// Seems to be for scene assignment stuff, but since we don't have them I'll need to look into it later
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x2C)]
                public byte[] UnknownBytes;

                /// <summary>
                /// Right Stick Rotation Offset
                /// </summary>
                public Vector3 RightStickRotationOffset { get; set; }

                /// <summary>
                /// Right Stick Rotation Degrees
                /// </summary>
                public Vector2 RightStickRotationDegrees { get; set; }
            }

            public struct XCamCamera
            {
                /// <summary>
                /// Index of the Name of this Camera in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Index of this Camera
                /// </summary>
                public int Index { get; set; }

                /// <summary>
                /// Number of Frames/Animations this Camera has
                /// </summary>
                public int AnimationCount { get; set; }

                /// <summary>
                /// Unknown Int
                /// </summary>
                public int Unk { get; set; }

                /// <summary>
                /// Pointer to the Animations
                /// </summary>
                public long AnimationsPointer { get; set; }

                /// <summary>
                /// Far Z Value
                /// </summary>
                public float FarZ { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }
            }

            public struct XCamTargetModelRootBone
            {
                /// <summary>
                /// Index of the Name of this Bone in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Number of Frames/Animations this Bone has
                /// </summary>
                public int AnimationCount { get; set; }

                /// <summary>
                /// Pointer to the Animations
                /// </summary>
                public long AnimationsPointer { get; set; }
            }

            public struct XCamTargetModelRootBoneAnimation
            {
                /// <summary>
                /// Animation Origin
                /// </summary>
                public Vector3 Origin { get; set; }

                /// <summary>
                /// Animation Quaternion Rotation
                /// </summary>
                public Vector4 Rotation { get; set; }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct XCamCameraAnimation
            {
                /// <summary>
                /// Animation Frame
                /// </summary>
                public int Frame { get; set; }

                /// <summary>
                /// Animation Origin
                /// </summary>
                public Vector3 Origin { get; set; }

                /// <summary>
                /// Animation Quaternion Rotation
                /// </summary>
                public Vector4 Rotation { get; set; }

                /// <summary>
                /// Animation Field of View
                /// </summary>
                public float FieldofView { get; set; }

                /// <summary>
                /// Animation Focal Length
                /// </summary>
                public float FocalLength { get; set; }

                /// <summary>
                /// Animation F-Dist
                /// </summary>
                public float FDist { get; set; }

                /// <summary>
                /// Animation F-Stop
                /// </summary>
                public float FStop { get; set; }
            }

            public struct XCamNotetrack
            {
                /// <summary>
                /// Index of the Name of this Notetrack in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Frame at which this notification occurs
                /// </summary>
                public int Frame { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read XCam
                    var xcamHeader = Hydra.ActiveGameReader.ReadStruct<XCamHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(xcamHeader.NamePointer))
                        continue;
                    // Create new asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name           = Hydra.ActiveGameReader.ReadNullTerminatedString(xcamHeader.NamePointer),
                        HeaderAddress  = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type           = assetPool.Name,
                        Information    = String.Format("Frames: {0} - Framerate: {1}", xcamHeader.FrameCount, xcamHeader.FrameRate)
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var xcamHeader = Hydra.ActiveGameReader.ReadStruct<XCamHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(xcamHeader.NamePointer))
                    return false;
                // Path Result
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "xanim_export\\hydra_xcams", asset.Name + ".xcam_export");
                // Create XCam
                var xcam = new XCamObj()
                {
                    // Set Flags
                    HideHud          = xcamHeader.Flags[0].ToString(),
                    IsLooping        = xcamHeader.Flags[1].ToString(),
                    HideLocalPlayer  = xcamHeader.Flags[2].ToString(),
                    UseFPSPlayer     = xcamHeader.Flags[3].ToString(),
                    DisableNearFov   = xcamHeader.Flags[4].ToString(),
                    AutoMotionBlur   = xcamHeader.Flags[5].ToString(),
                    EaseAnimationOut = xcamHeader.Flags[6].ToString(),
                    // Copy Right Stick Offset
                    RightStickRotationOffsetX = xcamHeader.RightStickRotationOffset.X.ToString(),
                    RightStickRotationOffsetY = xcamHeader.RightStickRotationOffset.Y.ToString(),
                    RightStickRotationOffsetZ = xcamHeader.RightStickRotationOffset.Z.ToString(),
                    // Copy Right Stick Degrees
                    RightStickRotationDegreesX = xcamHeader.RightStickRotationDegrees.X.ToString(),
                    RightStickRotationDegreesY = xcamHeader.RightStickRotationDegrees.Y.ToString(),
                    // Set Path
                    Filename = Path.Combine("hydra_xcams", asset.Name + ".xcam_export")
                };
                // Set GDT Properties
                Hydra.GDTs["XCams"].AddAsset(asset.Name, "xcam", xcam);
                // Load Notetracks if we have any
                if(xcamHeader.CameraCount > 0 && xcamHeader.CamerasPointer > 0)
                {
                    // Set Cameras
                    xcam.Cameras = new XCamObj.Camera[xcamHeader.CameraCount];
                    // Load Camera Buffer
                    var cameraBuffer = Hydra.ActiveGameReader.ReadBytes(xcamHeader.CamerasPointer, xcamHeader.CameraCount * 32);
                    // Loop through them
                    for (int i = 0; i < xcamHeader.CameraCount; i++)
                    {
                        // Load Camera
                        var camera = ByteUtil.BytesToStruct<XCamCamera>(cameraBuffer, i * 32);
                        // Set Camera
                        xcam.Cameras[i] = new XCamObj.Camera
                        {
                            // Add Values
                            Farz       = camera.FarZ,
                            Name       = GetString(camera.NameStringIndex),
                            Animations = new XCamObj.CameraAnimation[camera.AnimationCount]
                        };
                        // Load Animation Buffer
                        var animationBuffer = Hydra.ActiveGameReader.ReadBytes(camera.AnimationsPointer, camera.AnimationCount * 48);
                        // Loop through them
                        for(int j = 0; j < camera.AnimationCount; j++)
                        {
                            // Load Animation
                            var animation = ByteUtil.BytesToStruct<XCamCameraAnimation>(animationBuffer, j * 48);
                            // Set Animation
                            xcam.Cameras[i].Animations[j] = new XCamObj.CameraAnimation();
                            // Rotations are stored as Quaternions, but in XCAM_EXPORT are Matrices
                            Matrix matrix = new Quaternion()
                            {
                                X = animation.Rotation.X,
                                Y = animation.Rotation.Y,
                                Z = animation.Rotation.Z,
                                W = animation.Rotation.W,
                            }.ToMatrix();
                            // Set Matrix (Right is inverted)
                            xcam.Cameras[i].Animations[j].Dir[0]   = matrix.X.X;
                            xcam.Cameras[i].Animations[j].Dir[1]   = matrix.Y.X;
                            xcam.Cameras[i].Animations[j].Dir[2]   = matrix.Z.X;
                            xcam.Cameras[i].Animations[j].Up[0]    = matrix.X.Z;
                            xcam.Cameras[i].Animations[j].Up[1]    = matrix.Y.Z;
                            xcam.Cameras[i].Animations[j].Up[2]    = matrix.Z.Z;
                            xcam.Cameras[i].Animations[j].Right[0] = matrix.X.Y * -1;
                            xcam.Cameras[i].Animations[j].Right[1] = matrix.Y.Y * -1;
                            xcam.Cameras[i].Animations[j].Right[2] = matrix.Z.Y * -1;
                            // Set Origin
                            xcam.Cameras[i].Animations[j].Origin[0] = animation.Origin.X;
                            xcam.Cameras[i].Animations[j].Origin[1] = animation.Origin.Y;
                            xcam.Cameras[i].Animations[j].Origin[2] = animation.Origin.Z;
                            // Set Properties
                            xcam.Cameras[i].Animations[j].FieldOfView = animation.FieldofView;
                            xcam.Cameras[i].Animations[j].FocalLength = animation.FocalLength;
                            xcam.Cameras[i].Animations[j].FDist       = animation.FDist;
                            xcam.Cameras[i].Animations[j].FStop       = animation.FStop;
                            // Copy the values from the first camera
                            if (j == 0)
                            {
                                // Copy Direction
                                xcam.Cameras[i].Origin = xcam.Cameras[i].Animations[j].Origin;
                                xcam.Cameras[i].Dir    = xcam.Cameras[i].Animations[j].Dir;
                                xcam.Cameras[i].Up     = xcam.Cameras[i].Animations[j].Up;
                                xcam.Cameras[i].Right  = xcam.Cameras[i].Animations[j].Right;
                                // Copy Info
                                xcam.Cameras[i].FieldOfView = xcam.Cameras[i].Animations[j].FieldOfView;
                                xcam.Cameras[i].FocalLength = xcam.Cameras[i].Animations[j].FocalLength;
                                xcam.Cameras[i].FDist       = xcam.Cameras[i].Animations[j].FDist;
                                xcam.Cameras[i].FStop       = xcam.Cameras[i].Animations[j].FStop;
                            }
                        }
                    }
                }
                // Load the Root Target Bone (there can only be 1)
                if(xcamHeader.TargetModelRootBonePointer > 0)
                {
                    // Read the root bone, there is only 1, even if one enters multiple into the xcam
                    var rootBone = Hydra.ActiveGameReader.ReadStruct<XCamTargetModelRootBone>(xcamHeader.TargetModelRootBonePointer);
                    // Make new XCam Root Bone, and set values
                    xcam.TargetModelBoneRoots = new XCamObj.TargetModelBoneRoot[1]
                    {
                        new XCamObj.TargetModelBoneRoot()
                        {
                            Name = GetString(rootBone.NameStringIndex),
                            Animation = new XCamObj.TargetModelBoneRootFrame[rootBone.AnimationCount]
                        }
                    };
                    // Load Animation Buffer
                    var animationBuffer = Hydra.ActiveGameReader.ReadBytes(rootBone.AnimationsPointer, rootBone.AnimationCount * 28);
                    // Loop through them
                    for (int i = 0; i < rootBone.AnimationCount; i++)
                    {
                        // Load Notetrack
                        var animation = ByteUtil.BytesToStruct<XCamTargetModelRootBoneAnimation>(animationBuffer, i * 28);
                        // Rotations are stored as Quaternions, but in XCAM_EXPORT are Matrices
                        Matrix matrix = new Quaternion()
                        {
                            X = animation.Rotation.X,
                            Y = animation.Rotation.Y * -1,
                            Z = animation.Rotation.Z * -1,
                            W = animation.Rotation.W,
                        }.ToMatrix();
                        // Add to XCam
                        xcam.TargetModelBoneRoots[0].Animation[i] = new XCamObj.TargetModelBoneRootFrame();
                        // Set Origin (Y is inverted)
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[0] = animation.Origin.X;
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[1] = animation.Origin.Y * -1;
                        xcam.TargetModelBoneRoots[0].Animation[i].Offset[2] = animation.Origin.Z;
                        // Set Matrix
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["x"] = matrix.X.ToArray();
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["y"] = matrix.Y.ToArray();
                        xcam.TargetModelBoneRoots[0].Animation[i].Axis["z"] = matrix.Z.ToArray();
                    }
                }
                // Load Notetracks if we have any
                if (xcamHeader.NotetrackCount > 0 && xcamHeader.NotetracksPointer > 0)
                {
                    // Set Notetracks
                    xcam.Notetracks = new XCamObj.Notetrack[xcamHeader.NotetrackCount];
                    // Load Notetrack Buffer
                    var notetracksBuffer = Hydra.ActiveGameReader.ReadBytes(xcamHeader.NotetracksPointer, xcamHeader.NotetrackCount * 8);
                    // Loop through them
                    for(int i = 0; i < xcamHeader.NotetrackCount; i++)
                    {
                        // Load Notetrack
                        var notetrack = ByteUtil.BytesToStruct<XCamNotetrack>(notetracksBuffer, i * 8);
                        // Add to XCam
                        xcam.Notetracks[i] = new XCamObj.Notetrack(GetString(notetrack.NameStringIndex), notetrack.Frame);
                    }
                }
                // Save XCam
                xcam.Save(path);
                // Done
                return true;
            }
        }
    }
}
