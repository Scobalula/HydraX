using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Vehicle Logic
        /// </summary>
        private class Vehicle : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Vehicle Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] VehicleOffsets =
            {
                new Tuple<string, int, int>("type",                                                          0x8, 0x33),
                new Tuple<string, int, int>("scriptVehicleType",                                             0xc, 0x15),
                new Tuple<string, int, int>("archetype",                                                     0x10, 0x15),
                new Tuple<string, int, int>("scoretype",                                                     0x14, 0x15),
                new Tuple<string, int, int>("playerDrivenVersion",                                           0x18, 0x15),
                new Tuple<string, int, int>("category",                                                      0x1c, 0x4),
                new Tuple<string, int, int>("nonstick",                                                      0x20, 0x7),
                new Tuple<string, int, int>("remoteControl",                                                 0x24, 0x7),
                new Tuple<string, int, int>("bulletDamage",                                                  0x28, 0x7),
                new Tuple<string, int, int>("armorPiercingDamage",                                           0x2c, 0x7),
                new Tuple<string, int, int>("grenadeDamage",                                                 0x30, 0x7),
                new Tuple<string, int, int>("projectileDamage",                                              0x34, 0x7),
                new Tuple<string, int, int>("projectileSplashDamage",                                        0x38, 0x7),
                new Tuple<string, int, int>("heavyExplosiveDamage",                                          0x3c, 0x7),
                new Tuple<string, int, int>("burnDamage",                                                    0x40, 0x7),
                new Tuple<string, int, int>("grenadeLaunchScale",                                            0x44, 0x8),
                new Tuple<string, int, int>("projectileLaunchScale",                                         0x48, 0x8),
                new Tuple<string, int, int>("explosiveLaunchScale",                                          0x4c, 0x8),
                new Tuple<string, int, int>("deathLaunchScale",                                              0x50, 0x8),
                new Tuple<string, int, int>("grenadeDamageMultiplier",                                       0x54, 0x8),
                new Tuple<string, int, int>("projectileDamageMultiplier",                                    0x58, 0x8),
                new Tuple<string, int, int>("explosiveDamageMultiplier",                                     0x5c, 0x8),
                new Tuple<string, int, int>("bulletDamageMultiplier",                                        0x60, 0x8),
                new Tuple<string, int, int>("meleeDamageMultiplier",                                         0x64, 0x8),
                new Tuple<string, int, int>("cameraMode",                                                    0x68, 0x34),
                new Tuple<string, int, int>("autoRecenterOnAccel",                                           0x6c, 0x7),
                new Tuple<string, int, int>("thirdPersonDriver",                                             0x70, 0x7),
                new Tuple<string, int, int>("thirdPersonUseVehicleRoll",                                     0x74, 0x7),
                new Tuple<string, int, int>("thirdPersonCameraPitchVehicleRelative",                         0x78, 0x7),
                new Tuple<string, int, int>("thirdPersonCameraHeightWorldRelative",                          0x7c, 0x7),
                new Tuple<string, int, int>("thirdPersonCameraRange",                                        0x80, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraMinPitchClamp",                                0x84, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraMaxPitchClamp",                                0x88, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraHeightMin",                                    0x8c, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraHeight",                                       0x90, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraPitchMin",                                     0x94, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraPitch",                                        0x98, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraDriveOnWallHeight",                            0x9c, 0x8),
                new Tuple<string, int, int>("cameraAlwaysAutoCenter",                                        0xa0, 0x7),
                new Tuple<string, int, int>("cameraAutoCenterLerpRate",                                      0xa4, 0x8),
                new Tuple<string, int, int>("cameraAutoCenterMaxLerpRate",                                   0xa8, 0x8),
                new Tuple<string, int, int>("lodNoCull",                                                     0xac, 0x7),
                new Tuple<string, int, int>("thirdPersonCameraSpringDistance",                               0xb0, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraSpringTime",                                   0xb4, 0x8),
                new Tuple<string, int, int>("thirdPersonCameraHandbrakeTurnRateInc",                         0xb8, 0x8),
                new Tuple<string, int, int>("cameraRollFraction",                                            0xbc, 0x8),
                new Tuple<string, int, int>("cameraTag",                                                     0xc0, 0x15),
                new Tuple<string, int, int>("tagPlayerOffsetX",                                              0xc4, 0x8),
                new Tuple<string, int, int>("tagPlayerOffsetY",                                              0xc8, 0x8),
                new Tuple<string, int, int>("tagPlayerOffsetZ",                                              0xcc, 0x8),
                new Tuple<string, int, int>("cameraMinFOV",                                                  0xd0, 0x8),
                new Tuple<string, int, int>("cameraMaxFOV",                                                  0xd4, 0x8),
                new Tuple<string, int, int>("cameraMinFOVSpeed",                                             0xd8, 0x36),
                new Tuple<string, int, int>("cameraMaxFOVSpeed",                                             0xdc, 0x36),
                new Tuple<string, int, int>("blurMin",                                                       0xe0, 0x8),
                new Tuple<string, int, int>("blurMax",                                                       0xe4, 0x8),
                new Tuple<string, int, int>("blurMinSpeed",                                                  0xe8, 0x36),
                new Tuple<string, int, int>("blurMaxSpeed",                                                  0xec, 0x36),
                new Tuple<string, int, int>("blurInnerRadius",                                               0xf0, 0x8),
                new Tuple<string, int, int>("blurOuterRadius",                                               0xf4, 0x8),
                new Tuple<string, int, int>("killcamCollision",                                              0xf8, 0x7),
                new Tuple<string, int, int>("killcamDist",                                                   0xfc, 0x8),
                new Tuple<string, int, int>("killcamZDist",                                                  0x100, 0x8),
                new Tuple<string, int, int>("killcamMinDist",                                                0x104, 0x8),
                new Tuple<string, int, int>("killcamZTargetOffset",                                          0x108, 0x8),
                new Tuple<string, int, int>("killcamFOV",                                                    0x10c, 0x8),
                new Tuple<string, int, int>("killcamNearBlur",                                               0x110, 0x8),
                new Tuple<string, int, int>("killcamNearBlurStart",                                          0x114, 0x8),
                new Tuple<string, int, int>("killcamNearBlurEnd",                                            0x118, 0x8),
                new Tuple<string, int, int>("killcamFarBlur",                                                0x11c, 0x8),
                new Tuple<string, int, int>("killcamFarBlurStart",                                           0x120, 0x8),
                new Tuple<string, int, int>("killcamFarBlurEnd",                                             0x124, 0x8),
                new Tuple<string, int, int>("isDrivable",                                                    0x128, 0x7),
                new Tuple<string, int, int>("numberOfSeats",                                                 0x12c, 0x4),
                new Tuple<string, int, int>("numberOfGunners",                                               0x130, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder1",                                              0x134, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder2",                                              0x138, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder3",                                              0x13c, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder4",                                              0x140, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder5",                                              0x144, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder6",                                              0x148, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder7",                                              0x14c, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder8",                                              0x150, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder9",                                              0x154, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder10",                                             0x158, 0x4),
                new Tuple<string, int, int>("seatSwitchOrder11",                                             0x15c, 0x4),
                new Tuple<string, int, int>("driverControlledGunPos",                                        0x160, 0x4),
                new Tuple<string, int, int>("enterRadiusDriver",                                             0x164, 0x8),
                new Tuple<string, int, int>("enterRadiusGunner1",                                            0x168, 0x8),
                new Tuple<string, int, int>("enterRadiusGunner2",                                            0x16c, 0x8),
                new Tuple<string, int, int>("enterRadiusGunner3",                                            0x170, 0x8),
                new Tuple<string, int, int>("enterRadiusGunner4",                                            0x174, 0x8),
                new Tuple<string, int, int>("texureScrollScale",                                             0x178, 0x8),
                new Tuple<string, int, int>("wheelRotRate",                                                  0x17c, 0x8),
                new Tuple<string, int, int>("extraWheelRotScale",                                            0x180, 0x8),
                new Tuple<string, int, int>("wheelChildTakesSteerYaw",                                       0x184, 0x7),
                new Tuple<string, int, int>("useHeliBoneControllers",                                        0x188, 0x7),
                new Tuple<string, int, int>("minSpeed",                                                      0x18c, 0x36),
                new Tuple<string, int, int>("maxSpeed",                                                      0x190, 0x36),
                new Tuple<string, int, int>("defaultSpeed",                                                  0x194, 0x36),
                new Tuple<string, int, int>("maxSpeedVertical",                                              0x198, 0x36),
                new Tuple<string, int, int>("accel",                                                         0x19c, 0x36),
                new Tuple<string, int, int>("accelVertical",                                                 0x1a0, 0x36),
                new Tuple<string, int, int>("rotRate",                                                       0x1a4, 0x8),
                new Tuple<string, int, int>("rotAccel",                                                      0x1a8, 0x8),
                new Tuple<string, int, int>("angDampening",                                                  0x1ac, 0x8),
                new Tuple<string, int, int>("maxAngVelocityPitch",                                           0x1b0, 0x8),
                new Tuple<string, int, int>("maxTorquePitch",                                                0x1b4, 0x8),
                new Tuple<string, int, int>("rotMomentum",                                                   0x1b8, 0x8),
                new Tuple<string, int, int>("rotYawFromInput",                                               0x1bc, 0x7),
                new Tuple<string, int, int>("maxBodyPitch",                                                  0x1c0, 0x8),
                new Tuple<string, int, int>("maxBodyRoll",                                                   0x1c4, 0x8),
                new Tuple<string, int, int>("collisionDamage",                                               0x1c8, 0x8),
                new Tuple<string, int, int>("collisionSpeed",                                                0x1cc, 0x36),
                new Tuple<string, int, int>("suspensionTravel",                                              0x1d0, 0x8),
                new Tuple<string, int, int>("aiSlidingTurn",                                                 0x1d4, 0x7),
                new Tuple<string, int, int>("aiJumpingTraversal",                                            0x1d8, 0x7),
                new Tuple<string, int, int>("meleeAvoidance",                                                0x1dc, 0x7),
                new Tuple<string, int, int>("heliCollisionScalar",                                           0x1e0, 0x8),
                new Tuple<string, int, int>("viewPitchOffset",                                               0x1e4, 0x8),
                new Tuple<string, int, int>("viewInfluence",                                                 0x1e8, 0x8),
                new Tuple<string, int, int>("tiltFromAccelerationPitch",                                     0x1ec, 0x8),
                new Tuple<string, int, int>("tiltFromAccelerationRoll",                                      0x1f0, 0x8),
                new Tuple<string, int, int>("tiltFromDecelerationPitch",                                     0x1f4, 0x8),
                new Tuple<string, int, int>("tiltFromDecelerationRoll",                                      0x1f8, 0x8),
                new Tuple<string, int, int>("tiltFromVelocityPitch",                                         0x1fc, 0x8),
                new Tuple<string, int, int>("tiltFromVelocityRoll",                                          0x200, 0x8),
                new Tuple<string, int, int>("tiltFromPreviousPitch",                                         0x204, 0x8),
                new Tuple<string, int, int>("tiltFromPreviousRoll",                                          0x208, 0x8),
                new Tuple<string, int, int>("tiltSpeedPitch",                                                0x20c, 0x8),
                new Tuple<string, int, int>("tiltSpeedRoll",                                                 0x210, 0x8),
                new Tuple<string, int, int>("vehHelicopterHoverRadius",                                      0x214, 0x8),
                new Tuple<string, int, int>("vehHelicopterHoverSpeed",                                       0x218, 0x36),
                new Tuple<string, int, int>("vehHelicopterHoverAccel",                                       0x21c, 0x36),
                new Tuple<string, int, int>("turretWeapon",                                                  0x220, 0x0),
                new Tuple<string, int, int>("turretHorizSpanLeft",                                           0x228, 0x8),
                new Tuple<string, int, int>("turretHorizSpanRight",                                          0x22c, 0x8),
                new Tuple<string, int, int>("turretVertSpanUp",                                              0x230, 0x8),
                new Tuple<string, int, int>("turretVertSpanDown",                                            0x234, 0x8),
                new Tuple<string, int, int>("turretHorizResistLeft",                                         0x238, 0x8),
                new Tuple<string, int, int>("turretHorizResistRight",                                        0x23c, 0x8),
                new Tuple<string, int, int>("turretVertResistUp",                                            0x240, 0x8),
                new Tuple<string, int, int>("turretVertResistDown",                                          0x244, 0x8),
                new Tuple<string, int, int>("turretClampPlayerView",                                         0x248, 0x7),
                new Tuple<string, int, int>("turretLockTurretToPlayerView",                                  0x24c, 0x7),
                new Tuple<string, int, int>("turretFireFromCamera",                                          0x250, 0x7),
                new Tuple<string, int, int>("gunnerWeapon",                                                  0x258, 0x0),
                new Tuple<string, int, int>("gunnerWeapon1",                                                 0x260, 0x0),
                new Tuple<string, int, int>("gunnerWeapon2",                                                 0x268, 0x0),
                new Tuple<string, int, int>("gunnerWeapon3",                                                 0x270, 0x0),
                new Tuple<string, int, int>("passenger1HorizSpanLeft",                                       0x280, 0x8),
                new Tuple<string, int, int>("passenger1HorizSpanRight",                                      0x284, 0x8),
                new Tuple<string, int, int>("passenger1VertSpanUp",                                          0x288, 0x8),
                new Tuple<string, int, int>("passenger1VertSpanDown",                                        0x28c, 0x8),
                new Tuple<string, int, int>("passenger2HorizSpanLeft",                                       0x2a0, 0x8),
                new Tuple<string, int, int>("passenger2HorizSpanRight",                                      0x2a4, 0x8),
                new Tuple<string, int, int>("passenger2VertSpanUp",                                          0x2a8, 0x8),
                new Tuple<string, int, int>("passenger2VertSpanDown",                                        0x2ac, 0x8),
                new Tuple<string, int, int>("passenger3HorizSpanLeft",                                       0x2c0, 0x8),
                new Tuple<string, int, int>("passenger3HorizSpanRight",                                      0x2c4, 0x8),
                new Tuple<string, int, int>("passenger3VertSpanUp",                                          0x2c8, 0x8),
                new Tuple<string, int, int>("passenger3VertSpanDown",                                        0x2cc, 0x8),
                new Tuple<string, int, int>("passenger4HorizSpanLeft",                                       0x2e0, 0x8),
                new Tuple<string, int, int>("passenger4HorizSpanRight",                                      0x2e4, 0x8),
                new Tuple<string, int, int>("passenger4VertSpanUp",                                          0x2e8, 0x8),
                new Tuple<string, int, int>("passenger4VertSpanDown",                                        0x2ec, 0x8),
                new Tuple<string, int, int>("passenger5HorizSpanLeft",                                       0x300, 0x8),
                new Tuple<string, int, int>("passenger5HorizSpanRight",                                      0x304, 0x8),
                new Tuple<string, int, int>("passenger5VertSpanUp",                                          0x308, 0x8),
                new Tuple<string, int, int>("passenger5VertSpanDown",                                        0x30c, 0x8),
                new Tuple<string, int, int>("passenger6HorizSpanLeft",                                       0x320, 0x8),
                new Tuple<string, int, int>("passenger6HorizSpanRight",                                      0x324, 0x8),
                new Tuple<string, int, int>("passenger6VertSpanUp",                                          0x328, 0x8),
                new Tuple<string, int, int>("passenger6VertSpanDown",                                        0x32c, 0x8),
                new Tuple<string, int, int>("turretSpinSnd",                                                 0x340, 0x0),
                new Tuple<string, int, int>("turretStopSnd",                                                 0x348, 0x0),
                new Tuple<string, int, int>("jumpSnd",                                                       0x350, 0x0),
                new Tuple<string, int, int>("doubleJumpSnd",                                                 0x358, 0x0),
                new Tuple<string, int, int>("boostSnd",                                                      0x360, 0x0),
                new Tuple<string, int, int>("wallAttachSnd",                                                 0x368, 0x0),
                new Tuple<string, int, int>("wallDetachSnd",                                                 0x370, 0x0),
                new Tuple<string, int, int>("wheelRoadNoiseSnd",                                             0x398, 0x0),
                new Tuple<string, int, int>("wheelSlidingSnd",                                               0x3a0, 0x0),
                new Tuple<string, int, int>("wheelPeelingOutSnd",                                            0x3a8, 0x0),
                new Tuple<string, int, int>("skidSpeedMin",                                                  0x3b0, 0x8),
                new Tuple<string, int, int>("skidSpeedMax",                                                  0x3b4, 0x8),
                new Tuple<string, int, int>("peelSpeedMin",                                                  0x3b8, 0x8),
                new Tuple<string, int, int>("peelSpeedMax",                                                  0x3bc, 0x8),
                new Tuple<string, int, int>("futzSnd",                                                       0x3c0, 0x0),
                new Tuple<string, int, int>("futzBlend",                                                     0x3c8, 0x8),
                new Tuple<string, int, int>("soundDef",                                                      0x3d0, 0x1a),
                new Tuple<string, int, int>("animType",                                                      0x3d8, 0x3e),
                new Tuple<string, int, int>("animSet",                                                       0x3e0, 0x0),
                new Tuple<string, int, int>("scriptedAnimationEntry",                                        0x3e8, 0x7),
                new Tuple<string, int, int>("mantleAngleFront",                                              0x3ec, 0x8),
                new Tuple<string, int, int>("mantleAngleBack",                                               0x3f0, 0x8),
                new Tuple<string, int, int>("mantleAngleLeft",                                               0x3f4, 0x8),
                new Tuple<string, int, int>("mantleAngleRight",                                              0x3f8, 0x8),
                new Tuple<string, int, int>("extraWheelLeft1",                                               0x3fc, 0x15),
                new Tuple<string, int, int>("extraWheelRight1",                                              0x400, 0x15),
                new Tuple<string, int, int>("extraWheelLeft2",                                               0x404, 0x15),
                new Tuple<string, int, int>("extraWheelRight2",                                              0x408, 0x15),
                new Tuple<string, int, int>("rotorArmFrontLeft",                                             0x40c, 0x15),
                new Tuple<string, int, int>("rotorArmFrontRight",                                            0x410, 0x15),
                new Tuple<string, int, int>("rotorArmBackLeft",                                              0x414, 0x15),
                new Tuple<string, int, int>("rotorArmBackRight",                                             0x418, 0x15),
                new Tuple<string, int, int>("rotorArmFlapType1",                                             0x41c, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType2",                                             0x420, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType3",                                             0x424, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType4",                                             0x428, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType5",                                             0x42c, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType6",                                             0x430, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType7",                                             0x434, 0x4),
                new Tuple<string, int, int>("rotorArmFlapType8",                                             0x438, 0x4),
                new Tuple<string, int, int>("rotorArmFlapInf1",                                              0x43c, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf2",                                              0x440, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf3",                                              0x444, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf4",                                              0x448, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf5",                                              0x44c, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf6",                                              0x450, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf7",                                              0x454, 0x8),
                new Tuple<string, int, int>("rotorArmFlapInf8",                                              0x458, 0x8),
                new Tuple<string, int, int>("driverHideTag1",                                                0x45c, 0x15),
                new Tuple<string, int, int>("driverOtherHideTag1",                                           0x460, 0x15),
                new Tuple<string, int, int>("driverOtherHideTag2",                                           0x464, 0x15),
                new Tuple<string, int, int>("driverOtherHideTag3",                                           0x468, 0x15),
                new Tuple<string, int, int>("driverOtherHideTag4",                                           0x46c, 0x15),
                new Tuple<string, int, int>("mod0",                                                          0x470, 0xd),
                new Tuple<string, int, int>("mod1",                                                          0x478, 0xd),
                new Tuple<string, int, int>("mod2",                                                          0x480, 0xd),
                new Tuple<string, int, int>("mod3",                                                          0x488, 0xd),
                new Tuple<string, int, int>("tag0",                                                          0x490, 0x15),
                new Tuple<string, int, int>("tag1",                                                          0x494, 0x15),
                new Tuple<string, int, int>("tag2",                                                          0x498, 0x15),
                new Tuple<string, int, int>("tag3",                                                          0x49c, 0x15),
                new Tuple<string, int, int>("dmod0",                                                         0x4a0, 0xd),
                new Tuple<string, int, int>("dmod1",                                                         0x4a8, 0xd),
                new Tuple<string, int, int>("dmod2",                                                         0x4b0, 0xd),
                new Tuple<string, int, int>("dmod3",                                                         0x4b8, 0xd),
                new Tuple<string, int, int>("dtag0",                                                         0x4c0, 0x15),
                new Tuple<string, int, int>("dtag1",                                                         0x4c4, 0x15),
                new Tuple<string, int, int>("dtag2",                                                         0x4c8, 0x15),
                new Tuple<string, int, int>("dtag3",                                                         0x4cc, 0x15),
                new Tuple<string, int, int>("targetingImmuneSpecialty",                                      0x4d0, 0x3c),
                new Tuple<string, int, int>("targetTag1",                                                    0x4d4, 0x15),
                new Tuple<string, int, int>("targetTag2",                                                    0x4d8, 0x15),
                new Tuple<string, int, int>("targetTag3",                                                    0x4dc, 0x15),
                new Tuple<string, int, int>("targetTag4",                                                    0x4e0, 0x15),
                new Tuple<string, int, int>("aimAssistMinsX",                                                0x4e4, 0x8),
                new Tuple<string, int, int>("aimAssistMinsY",                                                0x4e8, 0x8),
                new Tuple<string, int, int>("aimAssistMinsZ",                                                0x4ec, 0x8),
                new Tuple<string, int, int>("aimAssistMaxsX",                                                0x4f0, 0x8),
                new Tuple<string, int, int>("aimAssistMaxsY",                                                0x4f4, 0x8),
                new Tuple<string, int, int>("aimAssistMaxsZ",                                                0x4f8, 0x8),
                new Tuple<string, int, int>("tracerOffsetForward",                                           0x4fc, 0x8),
                new Tuple<string, int, int>("tracerOffsetUp",                                                0x500, 0x8),
                new Tuple<string, int, int>("worldModel",                                                    0x508, 0xd),
                new Tuple<string, int, int>("viewModel",                                                     0x510, 0xd),
                new Tuple<string, int, int>("deathModel",                                                    0x518, 0xd),
                new Tuple<string, int, int>("enemyModel",                                                    0x520, 0xd),
                new Tuple<string, int, int>("modelSwapDelay",                                                0x528, 0x8),
                new Tuple<string, int, int>("exhaustFx",                                                     0x530, 0xa),
                new Tuple<string, int, int>("exhaustFxTag1",                                                 0x538, 0x15),
                new Tuple<string, int, int>("exhaustFxTag2",                                                 0x53c, 0x15),
                new Tuple<string, int, int>("surfaceFx",                                                     0x540, 0x19),
                new Tuple<string, int, int>("deathFxName",                                                   0x548, 0xa),
                new Tuple<string, int, int>("deathFxTag",                                                    0x550, 0x15),
                new Tuple<string, int, int>("deathFxSound",                                                  0x558, 0x0),
                new Tuple<string, int, int>("lightFxName1",                                                  0x560, 0xa),
                new Tuple<string, int, int>("lightFxName2",                                                  0x568, 0xa),
                new Tuple<string, int, int>("lightFxName3",                                                  0x570, 0xa),
                new Tuple<string, int, int>("lightFxName4",                                                  0x578, 0xa),
                new Tuple<string, int, int>("lightFxTag1",                                                   0x580, 0x15),
                new Tuple<string, int, int>("lightFxTag2",                                                   0x584, 0x15),
                new Tuple<string, int, int>("lightFxTag3",                                                   0x588, 0x15),
                new Tuple<string, int, int>("lightFxTag4",                                                   0x58c, 0x15),
                new Tuple<string, int, int>("radiusDamageMin",                                               0x590, 0x8),
                new Tuple<string, int, int>("radiusDamageMax",                                               0x594, 0x8),
                new Tuple<string, int, int>("radiusDamageRadius",                                            0x598, 0x8),
                new Tuple<string, int, int>("shootShock",                                                    0x5a0, 0x0),
                new Tuple<string, int, int>("deathQuakeScale",                                               0x5a8, 0x8),
                new Tuple<string, int, int>("deathQuakeDuration",                                            0x5ac, 0x8),
                new Tuple<string, int, int>("deathQuakeRadius",                                              0x5b0, 0x8),
                new Tuple<string, int, int>("rumbleType",                                                    0x5b8, 0x0),
                new Tuple<string, int, int>("rumbleScale",                                                   0x5c0, 0x8),
                new Tuple<string, int, int>("rumbleDuration",                                                0x5c4, 0x8),
                new Tuple<string, int, int>("rumbleRadius",                                                  0x5c8, 0x8),
                new Tuple<string, int, int>("rumbleBaseTime",                                                0x5cc, 0x8),
                new Tuple<string, int, int>("rumbleAdditionalTime",                                          0x5d0, 0x8),
                new Tuple<string, int, int>("healthDefault",                                                 0x5d4, 0x4),
                new Tuple<string, int, int>("team",                                                          0x5d8, 0x38),
                new Tuple<string, int, int>("boostAccelMultiplier",                                          0x5dc, 0x4),
                new Tuple<string, int, int>("boostDuration",                                                 0x5e0, 0x8),
                new Tuple<string, int, int>("boostDurationMin",                                              0x5e4, 0x8),
                new Tuple<string, int, int>("boostRecoveryScalar",                                           0x5e8, 0x8),
                new Tuple<string, int, int>("boostSpeedIncrease",                                            0x5ec, 0x8),
                new Tuple<string, int, int>("addToCompass",                                                  0x5f0, 0x7),
                new Tuple<string, int, int>("addToCompassEnemy",                                             0x5f4, 0x7),
                new Tuple<string, int, int>("addToCompassEnemyFiring",                                       0x5f8, 0x7),
                new Tuple<string, int, int>("compassIcon",                                                   0x600, 0x0),
                new Tuple<string, int, int>("compassIconTag",                                                0x610, 0x15),
                new Tuple<string, int, int>("compassScale",                                                  0x614, 0x8),
                new Tuple<string, int, int>("steerAxis",                                                     0x618, 0x0),
                new Tuple<string, int, int>("gasAxis",                                                       0x628, 0x0),
                new Tuple<string, int, int>("gasButton",                                                     0x638, 0x0),
                new Tuple<string, int, int>("reverseBrakeButton",                                            0x648, 0x0),
                new Tuple<string, int, int>("handBrakeButton",                                               0x658, 0x0),
                new Tuple<string, int, int>("attackButton",                                                  0x668, 0x0),
                new Tuple<string, int, int>("attackSecondaryButton",                                         0x678, 0x0),
                new Tuple<string, int, int>("boostButton",                                                   0x688, 0x0),
                new Tuple<string, int, int>("moveUpButton",                                                  0x698, 0x0),
                new Tuple<string, int, int>("moveDownButton",                                                0x6a8, 0x0),
                new Tuple<string, int, int>("switchSeatButton",                                              0x6b8, 0x0),
                new Tuple<string, int, int>("noButtonAutoRemap",                                             0x6c4, 0x7),
                new Tuple<string, int, int>("steerGraph",                                                    0x6c8, 0x0),
                new Tuple<string, int, int>("accelGraph",                                                    0x6d8, 0x0),
                new Tuple<string, int, int>("isNitrous",                                                     0x6e8, 0x7),
                new Tuple<string, int, int>("isFourWheelSteering",                                           0x6ec, 0x7),
                new Tuple<string, int, int>("enableMiddleWheelSteering",                                     0x6f0, 0x7),
                new Tuple<string, int, int>("useCollmap",                                                    0x6f4, 0x7),
                new Tuple<string, int, int>("radius",                                                        0x6f8, 0x8),
                new Tuple<string, int, int>("minHeight",                                                     0x6fc, 0x8),
                new Tuple<string, int, int>("maxHeight",                                                     0x700, 0x8),
                new Tuple<string, int, int>("lightCollisionSpeed",                                           0x704, 0x8),
                new Tuple<string, int, int>("lightCollisionRumble",                                          0x708, 0x17),
                new Tuple<string, int, int>("heavyCollisionSpeed",                                           0x710, 0x8),
                new Tuple<string, int, int>("heavyCollisionRumble",                                          0x718, 0x17),
                new Tuple<string, int, int>("jumpLandingRumble",                                             0x720, 0x17),
                new Tuple<string, int, int>("noDirectionalDamage",                                           0x728, 0x7),
                new Tuple<string, int, int>("fakeBodyStabilizer",                                            0x72c, 0x7),
                new Tuple<string, int, int>("turnInPlace",                                                   0x730, 0x7),
                new Tuple<string, int, int>("alwaysDriveForward",                                            0x734, 0x7),
                new Tuple<string, int, int>("smallVehicleCollision",                                         0x738, 0x7),
                new Tuple<string, int, int>("vehHelicopterBoundsRadius",                                     0x73c, 0x8),
                new Tuple<string, int, int>("vehHelicopterDecelerationFwd",                                  0x740, 0x8),
                new Tuple<string, int, int>("vehHelicopterDecelerationSide",                                 0x744, 0x8),
                new Tuple<string, int, int>("vehHelicopterDecelerationUp",                                   0x748, 0x8),
                new Tuple<string, int, int>("vehHelicopterTiltFromControllerAxes",                           0x74c, 0x8),
                new Tuple<string, int, int>("vehHelicopterPitchFromLookAxis",                                0x750, 0x7),
                new Tuple<string, int, int>("vehHelicopterTiltFromFwdAndYaw",                                0x754, 0x8),
                new Tuple<string, int, int>("vehHelicopterTiltFromFwdAndYaw_VelAtMaxTilt",                   0x758, 0x8),
                new Tuple<string, int, int>("vehHelicopterTiltMomentum",                                     0x75c, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMinTime",                                    0x760, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMaxTime",                                    0x764, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMinAccelForward",                            0x768, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMinAccelRight",                              0x76c, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMinAccelUp",                                 0x770, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMaxAccelForward",                            0x774, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMaxAccelRight",                              0x778, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterMaxAccelUp",                                 0x77c, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterVelocityThreshold",                          0x780, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterTiltPitchScale",                             0x784, 0x8),
                new Tuple<string, int, int>("vehHelicopterJitterTiltRollScale",                              0x788, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightRollRate",                                 0x78c, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightLocalOffsetForward",                       0x790, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightLocalOffsetRight",                         0x794, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightLocalOffsetUp",                            0x798, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightVelocityOffsetScalar",                     0x79c, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightOvershootScalar",                          0x7a0, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightDesiredLocationLerpRate",                  0x7a4, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightSpeedFalloffDistance",                     0x7a8, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightSpeedFalloffDistanceExponent",             0x7ac, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightControlLeewayAngle",                       0x7b0, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightFocusDampingAngle",                        0x7b4, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightFocusDampingAngleExponent",                0x7b8, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightFocusAngleDeltaMaxRoll",                   0x7bc, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightCameraShakeScalar",                        0x7c0, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightCameraOffsetForward",                      0x7c4, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightCameraOffsetRight",                        0x7c8, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightCameraOffsetUp",                           0x7cc, 0x8),
                new Tuple<string, int, int>("vehHelicopterDogfightCameraLerpTime",                           0x7d0, 0x8),
                new Tuple<string, int, int>("vehHelicopterFlapRotor",                                        0x7d4, 0x7),
                new Tuple<string, int, int>("vehHelicopterQuadRotor",                                        0x7d8, 0x7),
                new Tuple<string, int, int>("vehHelicopterAccelTwardsView",                                  0x7dc, 0x7),
                new Tuple<string, int, int>("vehHelicopterAccelTwardsViewWhenFiring",                        0x7e0, 0x7),
                new Tuple<string, int, int>("maxRotorRotationSpeed",                                         0x7e4, 0x8),
                new Tuple<string, int, int>("idleRotorRotationSpeed",                                        0x7e8, 0x8),
                new Tuple<string, int, int>("rotorArmRotateAroundY",                                         0x7ec, 0x7),
                new Tuple<string, int, int>("maxRotorArmMovementAngle",                                      0x7f0, 0x8),
                new Tuple<string, int, int>("maxRotorArmRotationAngle",                                      0x7f4, 0x8),
                new Tuple<string, int, int>("vehHelicopterMaintainHeight",                                   0x818, 0x7),
                new Tuple<string, int, int>("vehHelicopterMaintainMaxHeight",                                0x81c, 0x7),
                new Tuple<string, int, int>("vehHelicopterMaintainHeightLimit",                              0x820, 0x8),
                new Tuple<string, int, int>("vehHelicopterMaintainHeightAccel",                              0x824, 0x8),
                new Tuple<string, int, int>("vehHelicopterMaintainHeightMinimum",                            0x828, 0x8),
                new Tuple<string, int, int>("vehHelicopterMaintainHeightMaximum",                            0x82c, 0x8),
                new Tuple<string, int, int>("vehHelicopterMaintainCeilingMinimum",                           0x830, 0x8),
                new Tuple<string, int, int>("joltVehicle",                                                   0x834, 0x7),
                new Tuple<string, int, int>("joltVehicleDriver",                                             0x838, 0x7),
                new Tuple<string, int, int>("joltMaxTime",                                                   0x83c, 0x8),
                new Tuple<string, int, int>("joltTime",                                                      0x840, 0x8),
                new Tuple<string, int, int>("joltWaves",                                                     0x844, 0x8),
                new Tuple<string, int, int>("joltIntensity",                                                 0x848, 0x8),
                new Tuple<string, int, int>("maxSpeed",                                                      0x850, 0x36),
                new Tuple<string, int, int>("accel",                                                         0x854, 0x36),
                new Tuple<string, int, int>("nitrous_reverse_scale",                                         0x858, 0x8),
                new Tuple<string, int, int>("nitrous_steer_angle_max",                                       0x85c, 0x8),
                new Tuple<string, int, int>("nitrous_steer_angle_speed_scale",                               0x860, 0x8),
                new Tuple<string, int, int>("nitrous_steer_speed",                                           0x864, 0x8),
                new Tuple<string, int, int>("nitrous_wheel_radius",                                          0x868, 0x8),
                new Tuple<string, int, int>("nitrous_susp_spring_k",                                         0x86c, 0x8),
                new Tuple<string, int, int>("nitrous_susp_damp_k",                                           0x870, 0x8),
                new Tuple<string, int, int>("nitrous_susp_adj",                                              0x874, 0x8),
                new Tuple<string, int, int>("nitrous_susp_hard_limit",                                       0x878, 0x8),
                new Tuple<string, int, int>("nitrous_susp_min_height",                                       0x87c, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric",                                             0x880, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_assist",                                      0x884, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_assist_threshold",                            0x888, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_hand_brake_fwd",                              0x88c, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_hand_brake_side",                             0x890, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_load_factor_min",                             0x894, 0x8),
                new Tuple<string, int, int>("nitrous_tire_fric_load_factor_max",                             0x898, 0x8),
                new Tuple<string, int, int>("nitrous_hand_brake_fake_rotation_force",                        0x89c, 0x8),
                new Tuple<string, int, int>("nitrous_hand_brake_redirect_force",                             0x8a0, 0x8),
                new Tuple<string, int, int>("nitrous_body_mass",                                             0x8a4, 0x37),
                new Tuple<string, int, int>("nitrous_roll_stability",                                        0x8a8, 0x8),
                new Tuple<string, int, int>("nitrous_pitch_stability",                                       0x8ac, 0x8),
                new Tuple<string, int, int>("nitrous_roll_resistance",                                       0x8b0, 0x8),
                new Tuple<string, int, int>("nitrous_yaw_resistance",                                        0x8b4, 0x8),
                new Tuple<string, int, int>("nitrous_upright_strength",                                      0x8b8, 0x8),
                new Tuple<string, int, int>("nitrous_tire_damp_coast",                                       0x8bc, 0x8),
                new Tuple<string, int, int>("nitrous_tire_damp_brake",                                       0x8c0, 0x8),
                new Tuple<string, int, int>("nitrous_tire_damp_hand",                                        0x8c4, 0x8),
                new Tuple<string, int, int>("autoHandbrakeMinSpeed",                                         0x8c8, 0x36),
                new Tuple<string, int, int>("nitrous_max_climb_angle",                                       0x8cc, 0x8),
                new Tuple<string, int, int>("nitrous_hand_brake_slide_time",                                 0x8d0, 0x8),
                new Tuple<string, int, int>("nitrous_brake_to_reverse_time",                                 0x8d4, 0x8),
                new Tuple<string, int, int>("nitrous_hover_trace_start_offset",                              0x8d8, 0x8),
                new Tuple<string, int, int>("nitrous_hover_trace_length_from_ground",                        0x8dc, 0x8),
                new Tuple<string, int, int>("nitrous_hover_trace_length_from_wall",                          0x8e0, 0x8),
                new Tuple<string, int, int>("nitrous_hover_to_wall_gravity_scale",                           0x8e4, 0x8),
                new Tuple<string, int, int>("nitrous_hover_to_wall_upright_strength_multiplier",             0x8e8, 0x8),
                new Tuple<string, int, int>("nitrous_hover_to_wall_gravity_lerp_rate",                       0x8ec, 0x8),
                new Tuple<string, int, int>("nitrous_hover_to_ground_gravity_lerp_rate",                     0x8f0, 0x8),
                new Tuple<string, int, int>("nitrous_hover_to_ground_upright_strength_multiplier",           0x8f4, 0x8),
                new Tuple<string, int, int>("nitrous_hover_min_speed_to_apply_brakes",                       0x8f8, 0x8),
                new Tuple<string, int, int>("nitrous_hover_brake_scale_factor",                              0x8fc, 0x8),
                new Tuple<string, int, int>("nitrous_traction_type",                                         0x900, 0x35),
                new Tuple<string, int, int>("nitrous_wheel_offset_x",                                        0x910, 0x8),
                new Tuple<string, int, int>("nitrous_wheel_offset_y",                                        0x914, 0x8),
                new Tuple<string, int, int>("nitrous_wheel_offset_z",                                        0x918, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_min_x",                                            0x91c, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_min_y",                                            0x920, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_min_z",                                            0x924, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_max_x",                                            0x928, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_max_y",                                            0x92c, 0x8),
                new Tuple<string, int, int>("nitrous_bbox_max_z",                                            0x930, 0x8),
                new Tuple<string, int, int>("nitrous_mass_center_offset_x",                                  0x934, 0x8),
                new Tuple<string, int, int>("nitrous_mass_center_offset_y",                                  0x938, 0x8),
                new Tuple<string, int, int>("nitrous_mass_center_offset_z",                                  0x93c, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_min_x",                                     0x940, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_min_y",                                     0x944, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_min_z",                                     0x948, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_max_x",                                     0x94c, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_max_y",                                     0x950, 0x8),
                new Tuple<string, int, int>("nitrous_buoyancybox_max_z",                                     0x954, 0x8),
                new Tuple<string, int, int>("nitrous_water_speed_max",                                       0x958, 0x36),
                new Tuple<string, int, int>("nitrous_water_accel_max",                                       0x95c, 0x8),
                new Tuple<string, int, int>("nitrous_water_turn_accel",                                      0x960, 0x8),
                new Tuple<string, int, int>("nitrous_water_turn_speed_max",                                  0x964, 0x8),
                new Tuple<string, int, int>("nitrous_boat_ebrake_power",                                     0x968, 0x8),
                new Tuple<string, int, int>("nitrous_boat_motor_offset_x",                                   0x96c, 0x8),
                new Tuple<string, int, int>("nitrous_boat_motor_offset_y",                                   0x970, 0x8),
                new Tuple<string, int, int>("nitrous_boat_motor_offset_z",                                   0x974, 0x8),
                new Tuple<string, int, int>("nitrous_boat_speed_rise",                                       0x978, 0x8),
                new Tuple<string, int, int>("nitrous_boat_speed_tilt",                                       0x97c, 0x8),
                new Tuple<string, int, int>("nitrous_boat_side_fric",                                        0x980, 0x8),
                new Tuple<string, int, int>("nitrous_boat_forward_fric",                                     0x984, 0x8),
                new Tuple<string, int, int>("nitrous_boat_vertical_fric",                                    0x988, 0x8),
                new Tuple<string, int, int>("nitrous_motorcycle_max_lean",                                   0x98c, 0x8),
                new Tuple<string, int, int>("nitrous_jump_force",                                            0x990, 0x8),
                new Tuple<string, int, int>("nitrous_jump_force_in_air",                                     0x994, 0x8),
                new Tuple<string, int, int>("nitrous_jump_force_from_wall",                                  0x998, 0x8),
                new Tuple<string, int, int>("nitrous_jump_force_from_wall_max",                              0x99c, 0x8),
                new Tuple<string, int, int>("nitrous_jump_force_from_wall_extra_z",                          0x9a0, 0x8),
                new Tuple<string, int, int>("nitrous_jump_gravity_scale_upwards",                            0x9a4, 0x8),
                new Tuple<string, int, int>("nitrous_jump_gravity_scale_transition_z",                       0x9a8, 0x8),
                new Tuple<string, int, int>("nitrous_jump_gravity_scale_downwards",                          0x9ac, 0x8),
                new Tuple<string, int, int>("nitrous_jump_from_wall_stick_angle",                            0x9b0, 0x8),
                new Tuple<string, int, int>("nitrous_jumps_require_button_release",                          0x9b4, 0x6),
                new Tuple<string, int, int>("nitrous_drive_on_walls",                                        0x9b5, 0x6),
                new Tuple<string, int, int>("nitrous_charge_jump",                                           0x9b6, 0x6),
                new Tuple<string, int, int>("nitrous_linear_drag_scale",                                     0x9b8, 0x8),
                new Tuple<string, int, int>("nitrous_angular_drag_scale",                                    0x9bc, 0x8),
                new Tuple<string, int, int>("nitrous_gravity_scale",                                         0x9c0, 0x8),
                new Tuple<string, int, int>("doFootSteps",                                                   0x9c8, 0x6),
                new Tuple<string, int, int>("ignoreVortex",                                                  0x9cc, 0x6),
                new Tuple<string, int, int>("isSentient",                                                    0x9d0, 0x6),
                new Tuple<string, int, int>("isPathfinder",                                                  0x9d4, 0x6),
                new Tuple<string, int, int>("scriptbundlesettings",                                          0x9d8, 0x1b),
                new Tuple<string, int, int>("vehicleridersbundle",                                           0x9e0, 0x1b),
                new Tuple<string, int, int>("vehicleridersrobotbundle",                                      0x9e8, 0x1b),
                new Tuple<string, int, int>("assassinationBundle",                                           0x9f0, 0x1b),
                new Tuple<string, int, int>("animStateMachineName",                                          0x9f8, 0x26),
                new Tuple<string, int, int>("animSelectorTableName",                                         0xa00, 0x27),
                new Tuple<string, int, int>("animMappingTableName",                                          0xa08, 0x28),
                new Tuple<string, int, int>("antenna1SpringK",                                               0xa10, 0x8),
                new Tuple<string, int, int>("antenna1Damp",                                                  0xa14, 0x8),
                new Tuple<string, int, int>("antenna1Length",                                                0xa18, 0x8),
                new Tuple<string, int, int>("antenna1Gravity",                                               0xa1c, 0x8),
                new Tuple<string, int, int>("useAntenna1XAxis",                                              0xa20, 0x6),
                new Tuple<string, int, int>("antenna2SpringK",                                               0xa24, 0x8),
                new Tuple<string, int, int>("antenna2Damp",                                                  0xa28, 0x8),
                new Tuple<string, int, int>("antenna2Length",                                                0xa2c, 0x8),
                new Tuple<string, int, int>("antenna2Gravity",                                               0xa30, 0x8),
                new Tuple<string, int, int>("useAntenna2XAxis",                                              0xa34, 0x6),
                new Tuple<string, int, int>("antenna3SpringK",                                               0xa38, 0x8),
                new Tuple<string, int, int>("antenna3Damp",                                                  0xa3c, 0x8),
                new Tuple<string, int, int>("antenna3Length",                                                0xa40, 0x8),
                new Tuple<string, int, int>("antenna3Gravity",                                               0xa44, 0x8),
                new Tuple<string, int, int>("useAntenna3XAxis",                                              0xa48, 0x6),
                new Tuple<string, int, int>("antenna4SpringK",                                               0xa4c, 0x8),
                new Tuple<string, int, int>("antenna4Damp",                                                  0xa50, 0x8),
                new Tuple<string, int, int>("antenna4Length",                                                0xa54, 0x8),
                new Tuple<string, int, int>("antenna4Gravity",                                               0xa58, 0x8),
                new Tuple<string, int, int>("useAntenna4XAxis",                                              0xa5c, 0x6),
                new Tuple<string, int, int>("csvInclude",                                                    0xa60, 0x0),
                new Tuple<string, int, int>("customFloat0",                                                  0xa68, 0x8),
                new Tuple<string, int, int>("customFloat1",                                                  0xa6c, 0x8),
                new Tuple<string, int, int>("customFloat2",                                                  0xa70, 0x8),
                new Tuple<string, int, int>("customBool0",                                                   0xa74, 0x6),
                new Tuple<string, int, int>("customBool1",                                                   0xa78, 0x6),
                new Tuple<string, int, int>("customBool2",                                                   0xa7c, 0x6),
                new Tuple<string, int, int>("vehicleFootstepTable",                                          0xa80, 0x1f),
                new Tuple<string, int, int>("vehicleFootstepFXTable",                                        0xa88, 0x1d),
                new Tuple<string, int, int>("destructibleDef",                                               0xa90, 0x2a),
                new Tuple<string, int, int>("tacticalModeIcon",                                              0xa98, 0x10),
                new Tuple<string, int, int>("tacticalModeHeight",                                            0xaa0, 0x8),
            };

            /// <summary>
            /// Vehicle Types
            /// </summary>
            private static readonly string[] VehicleTypes =
            {
                "4 wheel",
                "motorcycle",
                "tank",
                "plane",
                "boat",
                "artillery",
                "helicopter"
            };

            /// <summary>
            /// Vehicle Camera Modes
            /// </summary>
            private static readonly string[] CameraModes =
            {
                "chase",
                "first",
                "view",
                "strafe",
                "horse",
                "oldtank",
                "hover",
                "vtol",
                "quadtank"
            };

            /// <summary>
            /// Traction Types
            /// </summary>
            private static readonly string[] NitrousTractionTypes =
            {
                "TRACTION_TYPE_FRONT",
                "TRACTION_TYPE_BACK",
                "TRACTION_TYPE_ALL_WD"
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
            public string Name => "vehicle";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.vehicle;

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

                var result = ConvertAssetBufferToGDTAsset(buffer, VehicleOffsets, instance, HandleVehicleSettings);

                result.Properties["mantleAngleFront"] = (Math.Acos((float)result.Properties["mantleAngleFront"])    / 0.017453292) / 0.5;
                result.Properties["mantleAngleBack"]  = (Math.Acos((float)result.Properties["mantleAngleBack"])     / 0.017453292) / 0.5;
                result.Properties["mantleAngleLeft"]  = (Math.Acos((float)result.Properties["mantleAngleLeft"])     / 0.017453292) / 0.5;
                result.Properties["mantleAngleRight"] = (Math.Acos((float)result.Properties["mantleAngleRight"])    / 0.017453292) / 0.5;


                result.Type = "vehicle";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Vehicle Specific Settings
            /// </summary>
            private static object HandleVehicleSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        return VehicleTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x34:
                        return CameraModes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x35:
                        return NitrousTractionTypes[BitConverter.ToInt32(assetBuffer, offset)];
                    case 0x36:
                        return BitConverter.ToSingle(assetBuffer, offset) / 17.6;
                    case 0x37:
                        return BitConverter.ToSingle(assetBuffer, offset) / 0.001;
                    case 0x38:
                        {
                            var value = BitConverter.ToInt32(assetBuffer, offset);

                            switch (value)
                            {
                                case 19:
                                    return "neutral";
                                case 3:
                                    return "team3";
                                case 2:
                                    return "axis";
                                case 1:
                                    return "allies";
                                default:
                                    return "";
                            }
                        }
                    case 0x3C:
                        var index = BitConverter.ToInt32(assetBuffer, offset);
                        return index >= SpecialtyNames.Length ? "none" : SpecialtyNames[index];
                    case 0x3E:
                        return BitConverter.ToInt32(assetBuffer, offset) == 0 ? "default" : "tank with turret";
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
