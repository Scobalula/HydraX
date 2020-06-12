using System;
using System.Collections.Generic;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Attachment Unique Logic
        /// </summary>
        private class AttachmentUnique : IAssetPool
        {
            #region Tables
            /// <summary>
            /// Attachment Properties and Struct Offsets
            /// </summary>
            private static readonly Tuple<string, int, int>[] AttachmentUniqueOffsets =
            {
                new Tuple<string, int, int>("displayName",                                                   0x8, 0x0),
                new Tuple<string, int, int>("displayDesc",                                                   0x10, 0x0),
                new Tuple<string, int, int>("modHudIcon",                                                    0x18, 0x10),
                new Tuple<string, int, int>("attachmentImage",                                               0x20, 0x10),
                new Tuple<string, int, int>("attachmentType",                                                0x28, 0x33),
                new Tuple<string, int, int>("locNone",                                                       0xf88, 0x8),
                new Tuple<string, int, int>("locHelmet",                                                     0xf8c, 0x8),
                new Tuple<string, int, int>("locHead",                                                       0xf90, 0x8),
                new Tuple<string, int, int>("locNeck",                                                       0xf94, 0x8),
                new Tuple<string, int, int>("locTorsoUpper",                                                 0xf98, 0x8),
                new Tuple<string, int, int>("locTorsoMid",                                                   0xf9c, 0x8),
                new Tuple<string, int, int>("locTorsoLower",                                                 0xfa0, 0x8),
                new Tuple<string, int, int>("locRightArmUpper",                                              0xfa4, 0x8),
                new Tuple<string, int, int>("locRightArmLower",                                              0xfac, 0x8),
                new Tuple<string, int, int>("locRightHand",                                                  0xfb4, 0x8),
                new Tuple<string, int, int>("locLeftArmUpper",                                               0xfa8, 0x8),
                new Tuple<string, int, int>("locLeftArmLower",                                               0xfb0, 0x8),
                new Tuple<string, int, int>("locLeftHand",                                                   0xfb8, 0x8),
                new Tuple<string, int, int>("locRightLegUpper",                                              0xfbc, 0x8),
                new Tuple<string, int, int>("locRightLegLower",                                              0xfc4, 0x8),
                new Tuple<string, int, int>("locRightFoot",                                                  0xfcc, 0x8),
                new Tuple<string, int, int>("locLeftLegUpper",                                               0xfc0, 0x8),
                new Tuple<string, int, int>("locLeftLegLower",                                               0xfc8, 0x8),
                new Tuple<string, int, int>("locLeftFoot",                                                   0xfd0, 0x8),
                new Tuple<string, int, int>("locGun",                                                        0xfd4, 0x8),
                new Tuple<string, int, int>("damage",                                                        0x3e8, 0x4),
                new Tuple<string, int, int>("minDamage",                                                     0x3fc, 0x4),
                new Tuple<string, int, int>("maxDamageRange",                                                0x400, 0x8),
                new Tuple<string, int, int>("minDamageRange",                                                0x414, 0x8),
                new Tuple<string, int, int>("damage2",                                                       0x3ec, 0x4),
                new Tuple<string, int, int>("damage3",                                                       0x3f0, 0x4),
                new Tuple<string, int, int>("damage4",                                                       0x3f4, 0x4),
                new Tuple<string, int, int>("damage5",                                                       0x3f8, 0x4),
                new Tuple<string, int, int>("damageRange2",                                                  0x404, 0x8),
                new Tuple<string, int, int>("damageRange3",                                                  0x408, 0x8),
                new Tuple<string, int, int>("damageRange4",                                                  0x40c, 0x8),
                new Tuple<string, int, int>("damageRange5",                                                  0x410, 0x8),
                new Tuple<string, int, int>("hideTags",                                                      0xe68, 0x34),
                new Tuple<string, int, int>("camo",                                                          0x358, 0x36),
                new Tuple<string, int, int>("attachment0_ModelAssociation",                                  0x68, 0x33),
                new Tuple<string, int, int>("attachment1_ModelAssociation",                                  0x158, 0x33),
                new Tuple<string, int, int>("attachment0_CosmeticVariantOverride",                           0x70, 0x2c),
                new Tuple<string, int, int>("attachment1_CosmeticVariantOverride",                           0x160, 0x2c),
                new Tuple<string, int, int>("attachment0_ViewModel_model0",                                  0x78, 0xd),
                new Tuple<string, int, int>("attachment0_ViewModel_model1",                                  0xa8, 0xd),
                new Tuple<string, int, int>("attachment1_ViewModel_model0",                                  0x168, 0xd),
                new Tuple<string, int, int>("attachment1_ViewModel_model1",                                  0x198, 0xd),
                new Tuple<string, int, int>("attachment0_ViewModelADS_model0",                               0x80, 0xd),
                new Tuple<string, int, int>("attachment0_ViewModelADS_model1",                               0xb0, 0xd),
                new Tuple<string, int, int>("attachment1_ViewModelADS_model0",                               0x170, 0xd),
                new Tuple<string, int, int>("attachment1_ViewModelADS_model1",                               0x1a0, 0xd),
                new Tuple<string, int, int>("attachment0_WorldModel_model0",                                 0xd8, 0xd),
                new Tuple<string, int, int>("attachment0_WorldModel_model1",                                 0x108, 0xd),
                new Tuple<string, int, int>("attachment1_WorldModel_model0",                                 0x1c8, 0xd),
                new Tuple<string, int, int>("attachment1_WorldModel_model1",                                 0x1f8, 0xd),
                new Tuple<string, int, int>("attachment0_ViewModelTag_model0",                               0x88, 0x0),
                new Tuple<string, int, int>("attachment0_ViewModelTag_model1",                               0xb8, 0x0),
                new Tuple<string, int, int>("attachment1_ViewModelTag_model0",                               0x178, 0x0),
                new Tuple<string, int, int>("attachment1_ViewModelTag_model1",                               0x1a8, 0x0),
                new Tuple<string, int, int>("attachment0_WorldModelTag_model0",                              0xe8, 0x0),
                new Tuple<string, int, int>("attachment0_WorldModelTag_model1",                              0x118, 0x0),
                new Tuple<string, int, int>("attachment1_WorldModelTag_model0",                              0x1d8, 0x0),
                new Tuple<string, int, int>("attachment1_WorldModelTag_model1",                              0x208, 0x0),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetX_model0",                           0x90, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetY_model0",                           0x94, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetZ_model0",                           0x98, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetX_model1",                           0xc0, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetY_model1",                           0xc4, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetZ_model1",                           0xc8, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetX_model0",                           0x180, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetY_model0",                           0x184, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetZ_model0",                           0x188, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetX_model1",                           0x1b0, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetY_model1",                           0x1b4, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetZ_model1",                           0x1b8, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetX_model0",                          0xf0, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetY_model0",                          0xf4, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetZ_model0",                          0xf8, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetX_model1",                          0x120, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetY_model1",                          0x124, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetZ_model1",                          0x128, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetX_model0",                          0x1e0, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetY_model0",                          0x1e4, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetZ_model0",                          0x1e8, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetX_model1",                          0x210, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetY_model1",                          0x214, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetZ_model1",                          0x218, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetPitch_model0",                       0x9c, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetYaw_model0",                         0xa0, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetRoll_model0",                        0xa4, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetPitch_model1",                       0xcc, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetYaw_model1",                         0xd0, 0x8),
                new Tuple<string, int, int>("attachment0_ViewModelOffsetRoll_model1",                        0xd4, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetPitch_model0",                       0x18c, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetYaw_model0",                         0x190, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetRoll_model0",                        0x194, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetPitch_model1",                       0x1bc, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetYaw_model1",                         0x1c0, 0x8),
                new Tuple<string, int, int>("attachment1_ViewModelOffsetRoll_model1",                        0x1c4, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetPitch_model0",                      0xfc, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetYaw_model0",                        0x100, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetRoll_model0",                       0x104, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetPitch_model1",                      0x12c, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetYaw_model1",                        0x130, 0x8),
                new Tuple<string, int, int>("attachment0_WorldModelOffsetRoll_model1",                       0x134, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetPitch_model0",                      0x1ec, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetYaw_model0",                        0x1f0, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetRoll_model0",                       0x1f4, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetPitch_model1",                      0x21c, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetYaw_model1",                        0x220, 0x8),
                new Tuple<string, int, int>("attachment1_WorldModelOffsetRoll_model1",                       0x224, 0x8),
                new Tuple<string, int, int>("attachment0_displayNameShort",                                  0x138, 0x0),
                new Tuple<string, int, int>("attachment0_displayNameLong",                                   0x140, 0x0),
                new Tuple<string, int, int>("attachment0_description",                                       0x148, 0x0),
                new Tuple<string, int, int>("attachment0_uiMaterial",                                        0x150, 0x10),
                new Tuple<string, int, int>("attachment1_displayNameShort",                                  0x228, 0x0),
                new Tuple<string, int, int>("attachment1_displayNameLong",                                   0x230, 0x0),
                new Tuple<string, int, int>("attachment1_description",                                       0x238, 0x0),
                new Tuple<string, int, int>("attachment1_uiMaterial",                                        0x240, 0x10),
                new Tuple<string, int, int>("attachViewModel1",                                              0xee8, 0xd),
                new Tuple<string, int, int>("attachViewModel2",                                              0xef0, 0xd),
                new Tuple<string, int, int>("attachViewModel3",                                              0xef8, 0xd),
                new Tuple<string, int, int>("attachViewModel4",                                              0xf00, 0xd),
                new Tuple<string, int, int>("attachViewModel5",                                              0xf08, 0xd),
                new Tuple<string, int, int>("attachWorldModel1",                                             0xf10, 0xd),
                new Tuple<string, int, int>("attachWorldModel2",                                             0xf18, 0xd),
                new Tuple<string, int, int>("attachWorldModel3",                                             0xf20, 0xd),
                new Tuple<string, int, int>("attachWorldModel4",                                             0xf28, 0xd),
                new Tuple<string, int, int>("attachWorldModel5",                                             0xf30, 0xd),
                new Tuple<string, int, int>("attachViewModelTag1",                                           0xf38, 0x0),
                new Tuple<string, int, int>("attachViewModelTag2",                                           0xf40, 0x0),
                new Tuple<string, int, int>("attachViewModelTag3",                                           0xf48, 0x0),
                new Tuple<string, int, int>("attachViewModelTag4",                                           0xf50, 0x0),
                new Tuple<string, int, int>("attachViewModelTag5",                                           0xf58, 0x0),
                new Tuple<string, int, int>("attachWorldModelTag1",                                          0xf60, 0x0),
                new Tuple<string, int, int>("attachWorldModelTag2",                                          0xf68, 0x0),
                new Tuple<string, int, int>("attachWorldModelTag3",                                          0xf70, 0x0),
                new Tuple<string, int, int>("attachWorldModelTag4",                                          0xf78, 0x0),
                new Tuple<string, int, int>("attachWorldModelTag5",                                          0xf80, 0x0),
                new Tuple<string, int, int>("attachViewModelOffsetX1",                                       0x268, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetY1",                                       0x26c, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetZ1",                                       0x270, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetX2",                                       0x274, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetY2",                                       0x278, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetZ2",                                       0x27c, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetX3",                                       0x280, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetY3",                                       0x284, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetZ3",                                       0x288, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetX4",                                       0x28c, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetY4",                                       0x290, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetZ4",                                       0x294, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetX5",                                       0x298, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetY5",                                       0x29c, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetZ5",                                       0x2a0, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetX1",                                      0x2a4, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetY1",                                      0x2a8, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetZ1",                                      0x2ac, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetX2",                                      0x2b0, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetY2",                                      0x2b4, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetZ2",                                      0x2b8, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetX3",                                      0x2bc, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetY3",                                      0x2c0, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetZ3",                                      0x2c4, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetX4",                                      0x2c8, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetY4",                                      0x2cc, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetZ4",                                      0x2d0, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetX5",                                      0x2d4, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetY5",                                      0x2d8, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetZ5",                                      0x2dc, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetPitch1",                                   0x2e0, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetYaw1",                                     0x2e4, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetRoll1",                                    0x2e8, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetPitch2",                                   0x2ec, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetYaw2",                                     0x2f0, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetRoll2",                                    0x2f4, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetPitch3",                                   0x2f8, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetYaw3",                                     0x2fc, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetRoll3",                                    0x300, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetPitch4",                                   0x304, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetYaw4",                                     0x308, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetRoll4",                                    0x30c, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetPitch5",                                   0x310, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetYaw5",                                     0x314, 0x8),
                new Tuple<string, int, int>("attachViewModelOffsetRoll5",                                    0x318, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetPitch1",                                  0x31c, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetYaw1",                                    0x320, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetRoll1",                                   0x324, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetPitch2",                                  0x328, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetYaw2",                                    0x32c, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetRoll2",                                   0x330, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetPitch3",                                  0x334, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetYaw3",                                    0x338, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetRoll3",                                   0x33c, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetPitch4",                                  0x340, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetYaw4",                                    0x344, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetRoll4",                                   0x348, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetPitch5",                                  0x34c, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetYaw5",                                    0x350, 0x8),
                new Tuple<string, int, int>("attachWorldModelOffsetRoll5",                                   0x354, 0x8),
                new Tuple<string, int, int>("disableBaseWeaponAttachment",                                   0x360, 0x6),
                new Tuple<string, int, int>("disableBaseWeaponClip",                                         0x361, 0x6),
                new Tuple<string, int, int>("overrideBaseWeaponAttachmentOffsets",                           0x362, 0x6),
                new Tuple<string, int, int>("viewModelOffsetBaseAttachmentX",                                0x364, 0x8),
                new Tuple<string, int, int>("viewModelOffsetBaseAttachmentY",                                0x368, 0x8),
                new Tuple<string, int, int>("viewModelOffsetBaseAttachmentZ",                                0x36c, 0x8),
                new Tuple<string, int, int>("worldModelOffsetBaseAttachmentX",                               0x370, 0x8),
                new Tuple<string, int, int>("worldModelOffsetBaseAttachmentY",                               0x374, 0x8),
                new Tuple<string, int, int>("worldModelOffsetBaseAttachmentZ",                               0x378, 0x8),
                new Tuple<string, int, int>("altWeapon",                                                     0x40, 0x0),
                new Tuple<string, int, int>("DualWieldWeapon",                                               0x50, 0x0),
                new Tuple<string, int, int>("adsOverlayShader",                                              0x380, 0x11),
                new Tuple<string, int, int>("adsOverlayShaderLowRes",                                        0x388, 0x11),
                new Tuple<string, int, int>("adsOverlayReticle",                                             0x390, 0x35),
                new Tuple<string, int, int>("firstRaiseTime",                                                0x394, 0x9),
                new Tuple<string, int, int>("altRaiseTime",                                                  0x398, 0x9),
                new Tuple<string, int, int>("altDropTime",                                                   0x39c, 0x9),
                new Tuple<string, int, int>("adsAltRaiseTime",                                               0x3a0, 0x9),
                new Tuple<string, int, int>("adsAltDropTime",                                                0x3a4, 0x9),
                new Tuple<string, int, int>("reloadAmmoAdd",                                                 0x3a8, 0x4),
                new Tuple<string, int, int>("reloadStartAdd",                                                0x3ac, 0x4),
                new Tuple<string, int, int>("segmentedReload",                                               0x3b0, 0x6),
                new Tuple<string, int, int>("idleAnim",                                                      0x848, 0x0),
                new Tuple<string, int, int>("idleAnimLeft",                                                  0xd80, 0x0),
                new Tuple<string, int, int>("emptyIdleAnim",                                                 0x850, 0x0),
                new Tuple<string, int, int>("emptyIdleAnimLeft",                                             0xd90, 0x0),
                new Tuple<string, int, int>("fireIntroAnim",                                                 0x858, 0x0),
                new Tuple<string, int, int>("fireAnim",                                                      0x860, 0x0),
                new Tuple<string, int, int>("fireAnimLeft",                                                  0xd48, 0x0),
                new Tuple<string, int, int>("fireDelayAnim",                                                 0x868, 0x0),
                new Tuple<string, int, int>("fireDelayAnimLeft",                                             0xd50, 0x0),
                new Tuple<string, int, int>("quickTossAnim",                                                 0x870, 0x0),
                new Tuple<string, int, int>("holdFireAnim",                                                  0x878, 0x0),
                new Tuple<string, int, int>("holdFireLoopAnim",                                              0x880, 0x0),
                new Tuple<string, int, int>("holdFireCancelAnim",                                            0x888, 0x0),
                new Tuple<string, int, int>("holdFireAnimLeft",                                              0xd58, 0x0),
                new Tuple<string, int, int>("holdFireLoopAnimLeft",                                          0xd60, 0x0),
                new Tuple<string, int, int>("holdFireCancelAnimLeft",                                        0xd68, 0x0),
                new Tuple<string, int, int>("lastShotAnim",                                                  0x890, 0x0),
                new Tuple<string, int, int>("lastShotAnimLeft",                                              0xd78, 0x0),
                new Tuple<string, int, int>("detonateAnim",                                                  0xb78, 0x0),
                new Tuple<string, int, int>("rechamberAnim",                                                 0x898, 0x0),
                new Tuple<string, int, int>("meleeAnim",                                                     0x8a0, 0x0),
                new Tuple<string, int, int>("meleeMissAnim",                                                 0x8a8, 0x0),
                new Tuple<string, int, int>("meleeAnimEmpty",                                                0x8b0, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimHit",                                            0x8f8, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimFatal",                                          0x8e8, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimFatalClose",                                     0x8f0, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimMiss",                                           0x900, 0x0),
                new Tuple<string, int, int>("meleePowerAnim",                                                0x8b8, 0x0),
                new Tuple<string, int, int>("meleePowerInAnim",                                              0x8c0, 0x0),
                new Tuple<string, int, int>("meleePowerLoopAnim",                                            0x8c8, 0x0),
                new Tuple<string, int, int>("meleePowerOutAnim",                                             0x8d0, 0x0),
                new Tuple<string, int, int>("meleePowerAnimLeft",                                            0x8d8, 0x0),
                new Tuple<string, int, int>("meleeChargeAnim",                                               0x8e0, 0x0),
                new Tuple<string, int, int>("meleeChargeAnimAbove",                                          0x908, 0x0),
                new Tuple<string, int, int>("meleeCharge2AnimIn",                                            0x910, 0x0),
                new Tuple<string, int, int>("meleeCharge2AnimFatal",                                         0x918, 0x0),
                new Tuple<string, int, int>("meleeCharge2AnimFatalClose",                                    0x920, 0x0),
                new Tuple<string, int, int>("meleeCharge2AnimLeft",                                          0x928, 0x0),
                new Tuple<string, int, int>("meleeCharge2AnimMiss",                                          0x930, 0x0),
                new Tuple<string, int, int>("meleeCharge3AnimIn",                                            0x938, 0x0),
                new Tuple<string, int, int>("meleeCharge3AnimFatal",                                         0x940, 0x0),
                new Tuple<string, int, int>("meleeCharge3AnimFatalClose",                                    0x948, 0x0),
                new Tuple<string, int, int>("meleeCharge3AnimMiss",                                          0x950, 0x0),
                new Tuple<string, int, int>("meleeLeftAnim",                                                 0x958, 0x0),
                new Tuple<string, int, int>("meleeLeftChargeAnim",                                           0x960, 0x0),
                new Tuple<string, int, int>("meleeLeftChargeAnimFatal",                                      0x968, 0x0),
                new Tuple<string, int, int>("meleeLeftChargeAnimHit",                                        0x970, 0x0),
                new Tuple<string, int, int>("meleeLeftChargeAnimMiss",                                       0x978, 0x0),
                new Tuple<string, int, int>("reloadAnim",                                                    0x980, 0x0),
                new Tuple<string, int, int>("reloadAnimLeft",                                                0xda0, 0x0),
                new Tuple<string, int, int>("reloadEmptyAnim",                                               0x988, 0x0),
                new Tuple<string, int, int>("reloadEmptyAnimLeft",                                           0xd98, 0x0),
                new Tuple<string, int, int>("reloadStartAnim",                                               0x990, 0x0),
                new Tuple<string, int, int>("reloadEndAnim",                                                 0x998, 0x0),
                new Tuple<string, int, int>("reloadQuickAnim",                                               0x9a0, 0x0),
                new Tuple<string, int, int>("reloadQuickEmptyAnim",                                          0x9a8, 0x0),
                new Tuple<string, int, int>("reloadSpecialComboAnim",                                        0x9b0, 0x0),
                new Tuple<string, int, int>("reloadSpecialComboEmptyAnim",                                   0x9b8, 0x0),
                new Tuple<string, int, int>("reloadSpecialComboQuickAnim",                                   0x9c0, 0x0),
                new Tuple<string, int, int>("reloadSpecialComboQuickEmptyAnim",                              0x9c8, 0x0),
                new Tuple<string, int, int>("raiseAnim",                                                     0x9d0, 0x0),
                new Tuple<string, int, int>("dropAnim",                                                      0x9e0, 0x0),
                new Tuple<string, int, int>("firstRaiseAnim",                                                0x9d8, 0x0),
                new Tuple<string, int, int>("altRaiseAnim",                                                  0x9e8, 0x0),
                new Tuple<string, int, int>("altDropAnim",                                                   0x9f0, 0x0),
                new Tuple<string, int, int>("adsAltRaiseAnim",                                               0x9f8, 0x0),
                new Tuple<string, int, int>("adsAltDropAnim",                                                0xa00, 0x0),
                new Tuple<string, int, int>("quickRaiseAnim",                                                0xa08, 0x0),
                new Tuple<string, int, int>("quickDropAnim",                                                 0xa10, 0x0),
                new Tuple<string, int, int>("emptyRaiseAnim",                                                0xa18, 0x0),
                new Tuple<string, int, int>("emptyDropAnim",                                                 0xa20, 0x0),
                new Tuple<string, int, int>("jumpAnim",                                                      0xdc8, 0x0),
                new Tuple<string, int, int>("jumpLandAnim",                                                  0xdd8, 0x0),
                new Tuple<string, int, int>("fallAnim",                                                      0xde8, 0x0),
                new Tuple<string, int, int>("walkAnim",                                                      0xdf8, 0x0),
                new Tuple<string, int, int>("jukeLeftAnim",                                                  0xe08, 0x0),
                new Tuple<string, int, int>("jukeRightAnim",                                                 0xe10, 0x0),
                new Tuple<string, int, int>("jukeForwardAnim",                                               0xe18, 0x0),
                new Tuple<string, int, int>("jukeBackwardAnim",                                              0xe20, 0x0),
                new Tuple<string, int, int>("jukeLeftADSAnim",                                               0xe28, 0x0),
                new Tuple<string, int, int>("jukeRightADSAnim",                                              0xe30, 0x0),
                new Tuple<string, int, int>("jukeForwardADSAnim",                                            0xe38, 0x0),
                new Tuple<string, int, int>("jukeBackwardADSAnim",                                           0xe40, 0x0),
                new Tuple<string, int, int>("chargeSprintInAnim",                                            0xa68, 0x0),
                new Tuple<string, int, int>("chargeSprintLoopAnim",                                          0xa70, 0x0),
                new Tuple<string, int, int>("chargeSprintOutAnim",                                           0xa78, 0x0),
                new Tuple<string, int, int>("sprintInAnim",                                                  0xa80, 0x0),
                new Tuple<string, int, int>("sprintLoopAnim",                                                0xa88, 0x0),
                new Tuple<string, int, int>("sprintOutAnim",                                                 0xa90, 0x0),
                new Tuple<string, int, int>("sprintInEmptyAnim",                                             0xa98, 0x0),
                new Tuple<string, int, int>("sprintLoopEmptyAnim",                                           0xaa0, 0x0),
                new Tuple<string, int, int>("sprintOutEmptyAnim",                                            0xaa8, 0x0),
                new Tuple<string, int, int>("lowReadyInAnim",                                                0xab0, 0x0),
                new Tuple<string, int, int>("lowReadyLoopAnim",                                              0xab8, 0x0),
                new Tuple<string, int, int>("lowReadyOutAnim",                                               0xac0, 0x0),
                new Tuple<string, int, int>("contFireInAnim",                                                0xac8, 0x0),
                new Tuple<string, int, int>("contFireLoopAnim",                                              0xad0, 0x0),
                new Tuple<string, int, int>("contFireOutAnim",                                               0xad8, 0x0),
                new Tuple<string, int, int>("crawlInAnim",                                                   0xae0, 0x0),
                new Tuple<string, int, int>("crawlForwardAnim",                                              0xae8, 0x0),
                new Tuple<string, int, int>("crawlBackAnim",                                                 0xaf0, 0x0),
                new Tuple<string, int, int>("crawlRightAnim",                                                0xaf8, 0x0),
                new Tuple<string, int, int>("crawlLeftAnim",                                                 0xb00, 0x0),
                new Tuple<string, int, int>("crawlOutAnim",                                                  0xb08, 0x0),
                new Tuple<string, int, int>("crawlEmptyInAnim",                                              0xb10, 0x0),
                new Tuple<string, int, int>("crawlEmptyForwardAnim",                                         0xb18, 0x0),
                new Tuple<string, int, int>("crawlEmptyBackAnim",                                            0xb20, 0x0),
                new Tuple<string, int, int>("crawlEmptyRightAnim",                                           0xb28, 0x0),
                new Tuple<string, int, int>("crawlEmptyLeftAnim",                                            0xb30, 0x0),
                new Tuple<string, int, int>("crawlEmptyOutAnim",                                             0xb38, 0x0),
                new Tuple<string, int, int>("adsFireAnim",                                                   0xb80, 0x0),
                new Tuple<string, int, int>("adsFireDelayAnim",                                              0xb88, 0x0),
                new Tuple<string, int, int>("adsLastShotAnim",                                               0xb90, 0x0),
                new Tuple<string, int, int>("adsRechamberAnim",                                              0xba0, 0x0),
                new Tuple<string, int, int>("adsUpAnim",                                                     0xda8, 0x0),
                new Tuple<string, int, int>("adsDownAnim",                                                   0xdb0, 0x0),
                new Tuple<string, int, int>("adsUpOtherScopeAnim",                                           0xdb8, 0x0),
                new Tuple<string, int, int>("adsFireIntroAnim",                                              0xb98, 0x0),
                new Tuple<string, int, int>("slide_in",                                                      0xba8, 0x0),
                new Tuple<string, int, int>("slide_in_air",                                                  0xbb0, 0x0),
                new Tuple<string, int, int>("slide_loop",                                                    0xbb8, 0x0),
                new Tuple<string, int, int>("slide_out",                                                     0xbc0, 0x0),
                new Tuple<string, int, int>("leap_in",                                                       0xbc8, 0x0),
                new Tuple<string, int, int>("leap_loop",                                                     0xbd0, 0x0),
                new Tuple<string, int, int>("leap_cancel",                                                   0xbd8, 0x0),
                new Tuple<string, int, int>("leap_out",                                                      0xbe0, 0x0),
                new Tuple<string, int, int>("trmInAnim",                                                     0xb40, 0x0),
                new Tuple<string, int, int>("trmOutAnim",                                                    0xb48, 0x0),
                new Tuple<string, int, int>("trmInLowAnim",                                                  0xb50, 0x0),
                new Tuple<string, int, int>("trmOutLowAnim",                                                 0xb58, 0x0),
                new Tuple<string, int, int>("trmOverAnim",                                                   0xb60, 0x0),
                new Tuple<string, int, int>("trmOverLeftAnim",                                               0xb68, 0x0),
                new Tuple<string, int, int>("trmOverRightAnim",                                              0xb70, 0x0),
                new Tuple<string, int, int>("wallrunInAnim",                                                 0xc28, 0x0),
                new Tuple<string, int, int>("wallrunLoopAnim",                                               0xc30, 0x0),
                new Tuple<string, int, int>("wallrunOutAnim",                                                0xc38, 0x0),
                new Tuple<string, int, int>("swimTransitionFromLand",                                        0xc40, 0x0),
                new Tuple<string, int, int>("swimIdleLoop",                                                  0xc48, 0x0),
                new Tuple<string, int, int>("swimIdleLoopLeft",                                              0xd88, 0x0),
                new Tuple<string, int, int>("swimCombatIdleIn",                                              0xc50, 0x0),
                new Tuple<string, int, int>("swimCombatIdle",                                                0xc58, 0x0),
                new Tuple<string, int, int>("swimCombatOut",                                                 0xc60, 0x0),
                new Tuple<string, int, int>("swimCombatFire",                                                0xc68, 0x0),
                new Tuple<string, int, int>("swimCombatFireLeft",                                            0xd70, 0x0),
                new Tuple<string, int, int>("swimCombatAdsFire",                                             0xc70, 0x0),
                new Tuple<string, int, int>("swimMovingIn",                                                  0xc78, 0x0),
                new Tuple<string, int, int>("swimMovingForward",                                             0xc80, 0x0),
                new Tuple<string, int, int>("swimMovingBackward",                                            0xc88, 0x0),
                new Tuple<string, int, int>("swimMovingLeft",                                                0xc90, 0x0),
                new Tuple<string, int, int>("swimMovingRight",                                               0xc98, 0x0),
                new Tuple<string, int, int>("swimMovingOut",                                                 0xca0, 0x0),
                new Tuple<string, int, int>("swimSprintIn",                                                  0xca8, 0x0),
                new Tuple<string, int, int>("swimSprintLoop",                                                0xcb0, 0x0),
                new Tuple<string, int, int>("swimSprintSurfaceLoop",                                         0xcb8, 0x0),
                new Tuple<string, int, int>("swimSprintOut",                                                 0xcc0, 0x0),
                new Tuple<string, int, int>("swimTransitionToLand",                                          0xcc8, 0x0),
                new Tuple<string, int, int>("swimRaise",                                                     0xcd0, 0x0),
                new Tuple<string, int, int>("swimQuickRaise",                                                0xcd8, 0x0),
                new Tuple<string, int, int>("swimDrop",                                                      0xce0, 0x0),
                new Tuple<string, int, int>("swimDiveIn",                                                    0xce8, 0x0),
                new Tuple<string, int, int>("swimDiveLoop",                                                  0xcf0, 0x0),
                new Tuple<string, int, int>("swimDiveOut",                                                   0xcf8, 0x0),
                new Tuple<string, int, int>("doubleJumpIn",                                                  0xc00, 0x0),
                new Tuple<string, int, int>("doubleJumpLoop",                                                0xc08, 0x0),
                new Tuple<string, int, int>("doubleJumpCancel",                                              0xc10, 0x0),
                new Tuple<string, int, int>("doubleJumpOut",                                                 0xc18, 0x0),
                new Tuple<string, int, int>("castAbility",                                                   0xa60, 0x0),
                new Tuple<string, int, int>("castIn",                                                        0xa28, 0x0),
                new Tuple<string, int, int>("castLoop",                                                      0xa30, 0x0),
                new Tuple<string, int, int>("castOut",                                                       0xa38, 0x0),
                new Tuple<string, int, int>("castOutHit",                                                    0xa40, 0x0),
                new Tuple<string, int, int>("castPassiveIn",                                                 0xa48, 0x0),
                new Tuple<string, int, int>("castPassiveLoop",                                               0xa50, 0x0),
                new Tuple<string, int, int>("castPassiveOut",                                                0xa58, 0x0),
                new Tuple<string, int, int>("grappleNoTarget",                                               0xd00, 0x0),
                new Tuple<string, int, int>("grappleStart",                                                  0xd08, 0x0),
                new Tuple<string, int, int>("grappleExtendLoop",                                             0xd10, 0x0),
                new Tuple<string, int, int>("grappleAnchorAscend",                                           0xd18, 0x0),
                new Tuple<string, int, int>("grappleAscendLoop",                                             0xd20, 0x0),
                new Tuple<string, int, int>("grappleAscendEnd",                                              0xd28, 0x0),
                new Tuple<string, int, int>("grappleAnchorYank",                                             0xd30, 0x0),
                new Tuple<string, int, int>("grappleYankLoop",                                               0xd38, 0x0),
                new Tuple<string, int, int>("grappleYankEnd",                                                0xd40, 0x0),
                new Tuple<string, int, int>("fireSound",                                                     0x420, 0x0),
                new Tuple<string, int, int>("fireSoundPlayer",                                               0x428, 0x0),
                new Tuple<string, int, int>("fireBurstSound",                                                0x430, 0x0),
                new Tuple<string, int, int>("fireBurstSoundPlayer",                                          0x438, 0x0),
                new Tuple<string, int, int>("loopFireSound",                                                 0x440, 0x0),
                new Tuple<string, int, int>("loopFireSoundPlayer",                                           0x448, 0x0),
                new Tuple<string, int, int>("loopFireSoundLeft",                                             0x450, 0x0),
                new Tuple<string, int, int>("loopFireSoundPlayerLeft",                                       0x458, 0x0),
                new Tuple<string, int, int>("loopFireEndSound",                                              0x460, 0x0),
                new Tuple<string, int, int>("loopFireEndSoundPlayer",                                        0x468, 0x0),
                new Tuple<string, int, int>("startFireSound",                                                0x470, 0x0),
                new Tuple<string, int, int>("stopFireSound",                                                 0x478, 0x0),
                new Tuple<string, int, int>("startFireSoundPlayer",                                          0x480, 0x0),
                new Tuple<string, int, int>("stopFireSoundPlayer",                                           0x488, 0x0),
                new Tuple<string, int, int>("lastShotSound",                                                 0x490, 0x0),
                new Tuple<string, int, int>("lastShotSoundPlayer",                                           0x498, 0x0),
                new Tuple<string, int, int>("killcamStartFireSound",                                         0x4a0, 0x0),
                new Tuple<string, int, int>("killcamStartFireSoundPlayer",                                   0x4a8, 0x0),
                new Tuple<string, int, int>("crackSound",                                                    0x4b0, 0x0),
                new Tuple<string, int, int>("whizbySound",                                                   0x4b8, 0x0),
                new Tuple<string, int, int>("rotateLoopSound",                                               0x500, 0x0),
                new Tuple<string, int, int>("rotateLoopSoundPlayer",                                         0x508, 0x0),
                new Tuple<string, int, int>("viewFlashEffect",                                               0x528, 0xa),
                new Tuple<string, int, int>("worldFlashEffect",                                              0x530, 0xa),
                new Tuple<string, int, int>("viewFlashOffsetF",                                              0x538, 0x8),
                new Tuple<string, int, int>("viewFlashOffsetR",                                              0x53c, 0x8),
                new Tuple<string, int, int>("viewFlashOffsetU",                                              0x540, 0x8),
                new Tuple<string, int, int>("worldFlashOffsetF",                                             0x544, 0x8),
                new Tuple<string, int, int>("worldFlashOffsetR",                                             0x548, 0x8),
                new Tuple<string, int, int>("worldFlashOffsetU",                                             0x54c, 0x8),
                new Tuple<string, int, int>("tracerType",                                                    0x550, 0x16),
                new Tuple<string, int, int>("enemyTracerType",                                               0x558, 0x16),
                new Tuple<string, int, int>("laserType",                                                     0x560, 0x2d),
                new Tuple<string, int, int>("laserTypeWorld",                                                0x568, 0x2d),
                new Tuple<string, int, int>("adsDofStart",                                                   0x570, 0x8),
                new Tuple<string, int, int>("adsDofEnd",                                                     0x574, 0x8),
                new Tuple<string, int, int>("overrideLeftHandIK",                                            0x580, 0x6),
                new Tuple<string, int, int>("overrideLeftHandProneIK",                                       0x581, 0x6),
                new Tuple<string, int, int>("ikLeftHandOffsetF",                                             0x584, 0x8),
                new Tuple<string, int, int>("ikLeftHandOffsetR",                                             0x588, 0x8),
                new Tuple<string, int, int>("ikLeftHandOffsetU",                                             0x58c, 0x8),
                new Tuple<string, int, int>("ikLeftHandRotationP",                                           0x590, 0x8),
                new Tuple<string, int, int>("ikLeftHandRotationY",                                           0x594, 0x8),
                new Tuple<string, int, int>("ikLeftHandRotationR",                                           0x598, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneOffsetF",                                        0x59c, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneOffsetR",                                        0x5a0, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneOffsetU",                                        0x5a4, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneRotationP",                                      0x5a8, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneRotationY",                                      0x5ac, 0x8),
                new Tuple<string, int, int>("ikLeftHandProneRotationR",                                      0x5b0, 0x8),
                new Tuple<string, int, int>("stickiness",                                                    0x5b8, 0x37),
                new Tuple<string, int, int>("customFloat0",                                                  0x810, 0x8),
                new Tuple<string, int, int>("customFloat1",                                                  0x814, 0x8),
                new Tuple<string, int, int>("customFloat2",                                                  0x818, 0x8),
                new Tuple<string, int, int>("customFloat3",                                                  0x81c, 0x8),
                new Tuple<string, int, int>("customFloat4",                                                  0x820, 0x8),
                new Tuple<string, int, int>("customFloat5",                                                  0x824, 0x8),
                new Tuple<string, int, int>("customBool0",                                                   0x828, 0x6),
                new Tuple<string, int, int>("customBool1",                                                   0x82c, 0x6),
                new Tuple<string, int, int>("customBool2",                                                   0x830, 0x6),
                new Tuple<string, int, int>("customBool3",                                                   0x834, 0x6),
                new Tuple<string, int, int>("customBool4",                                                   0x838, 0x6),
                new Tuple<string, int, int>("customBool5",                                                   0x83c, 0x6),
                new Tuple<string, int, int>("iCanDoQuickToss",                                               0x5b4, 0x4),
                new Tuple<string, int, int>("ammoCountEquipment",                                            0x5bc, 0x4),
                new Tuple<string, int, int>("startAmmo",                                                     0x5c0, 0x4),
                new Tuple<string, int, int>("maxAmmo",                                                       0x5c4, 0x4),
                new Tuple<string, int, int>("explosionRadius",                                               0x5c8, 0x4),
                new Tuple<string, int, int>("explosionRadiusMultiplier",                                     0x5cc, 0x8),
                new Tuple<string, int, int>("fuseTime",                                                      0x5d0, 0x9),
                new Tuple<string, int, int>("proximityDetonation",                                           0x5d4, 0x4),
                new Tuple<string, int, int>("proximityAlarmInnerRadius",                                     0x5d8, 0x4),
                new Tuple<string, int, int>("proximityAlarmOuterRadius",                                     0x5dc, 0x4),
                new Tuple<string, int, int>("proximityAlarmActivationDelay",                                 0x5e0, 0x9),
                new Tuple<string, int, int>("chainEventRadius",                                              0x5e4, 0x4),
                new Tuple<string, int, int>("chainEventTime",                                                0x5e8, 0x9),
                new Tuple<string, int, int>("chainEventMax",                                                 0x5ec, 0x4),
                new Tuple<string, int, int>("multiDetonation",                                               0x5f0, 0x4),
                new Tuple<string, int, int>("multiDetonationFragmentSpeed",                                  0x5f4, 0x4),
                new Tuple<string, int, int>("curveballForce",                                                0x5f8, 0x8),
                new Tuple<string, int, int>("curveballMaxRadius",                                            0x5fc, 0x4),
                new Tuple<string, int, int>("lockOnMaxRange",                                                0x600, 0x4),
                new Tuple<string, int, int>("lockOnMaxRangeNoLineOfSight",                                   0x604, 0x4),
                new Tuple<string, int, int>("lockOnSpeed",                                                   0x608, 0x4),
                new Tuple<string, int, int>("explodeWhenStationary",                                         0x60c, 0x4),
                new Tuple<string, int, int>("movementMultiplierStrafe",                                      0x624, 0x8),
                new Tuple<string, int, int>("movementMultiplierWalk",                                        0x628, 0x8),
                new Tuple<string, int, int>("movementMultiplierRun",                                         0x62c, 0x8),
                new Tuple<string, int, int>("movementMultiplierSprint",                                      0x630, 0x8),
                new Tuple<string, int, int>("movementMultiplierProne",                                       0x634, 0x8),
                new Tuple<string, int, int>("movementMultiplierCrouch",                                      0x638, 0x8),
                new Tuple<string, int, int>("movementMultiplierSlide",                                       0x63c, 0x8),
                new Tuple<string, int, int>("movementMultiplierWallrun",                                     0x640, 0x8),
                new Tuple<string, int, int>("movementMultiplierDoubleJump",                                  0x644, 0x8),
                new Tuple<string, int, int>("movementMultiplierJump",                                        0x648, 0x8),
                new Tuple<string, int, int>("movementMultiplierLeap",                                        0x64c, 0x8),
                new Tuple<string, int, int>("movementMultiplierSprintBob",                                   0x650, 0x8),
                new Tuple<string, int, int>("movementMultiplierNonSprintBob",                                0x654, 0x8),
                new Tuple<string, int, int>("movementMultiplierSwim",                                        0x658, 0x8),
                new Tuple<string, int, int>("blurAmount",                                                    0x6e0, 0x8),
                new Tuple<string, int, int>("blurRadiusInner",                                               0x6e4, 0x8),
                new Tuple<string, int, int>("blurRadiusOuter",                                               0x6e8, 0x8),
                new Tuple<string, int, int>("blurInTime",                                                    0x6f0, 0x4),
                new Tuple<string, int, int>("blurOutTime",                                                   0x6f4, 0x4),
                new Tuple<string, int, int>("blurOutScale",                                                  0x6ec, 0x8),
                new Tuple<string, int, int>("camo_bread_crumb_duration",                                     0x664, 0x4),
                new Tuple<string, int, int>("camo_invisibility_alert_time",                                  0x668, 0x4),
                new Tuple<string, int, int>("camo_invisibility_flicker_extension_time",                      0x66c, 0x4),
                new Tuple<string, int, int>("camo_invisibility_flicker_radius_extension",                    0x670, 0x8),
                new Tuple<string, int, int>("camo_invisibility_radius",                                      0x674, 0x4),
                new Tuple<string, int, int>("camo_invisibility_takedown_response_radius",                    0x678, 0x4),
                new Tuple<string, int, int>("camo_invisibility_takedown_reveal_time",                        0x67c, 0x4),
                new Tuple<string, int, int>("camo_takedown_power_gain",                                      0x680, 0x4),
                new Tuple<string, int, int>("escort_drone_bullet_dmg_power_loss",                            0x684, 0x8),
                new Tuple<string, int, int>("escort_drone_burst_count_max",                                  0x688, 0x8),
                new Tuple<string, int, int>("escort_drone_burst_count_min",                                  0x68c, 0x8),
                new Tuple<string, int, int>("escort_drone_burst_power_loss",                                 0x690, 0x4),
                new Tuple<string, int, int>("escort_drone_burst_wait_time",                                  0x694, 0x4),
                new Tuple<string, int, int>("escort_drone_exp_dmg_power_loss",                               0x698, 0x8),
                new Tuple<string, int, int>("escort_drone_hover_dist",                                       0x69c, 0x4),
                new Tuple<string, int, int>("escort_drone_launch_dist",                                      0x6a0, 0x4),
                new Tuple<string, int, int>("escort_drone_misc_dmg_power_loss",                              0x6a4, 0x8),
                new Tuple<string, int, int>("escort_drone_target_acquire_time",                              0x6a8, 0x4),
                new Tuple<string, int, int>("escort_drone_tether_max_dist",                                  0x6ac, 0x4),
                new Tuple<string, int, int>("escort_drone_tether_min_dist",                                  0x6b0, 0x4),
                new Tuple<string, int, int>("flicker_on_damage",                                             0x6b4, 0x4),
                new Tuple<string, int, int>("flicker_on_power_loss",                                         0x6b8, 0x4),
                new Tuple<string, int, int>("flicker_on_power_low",                                          0x6bc, 0x4),
                new Tuple<string, int, int>("flicker_on_whizby",                                             0x6c0, 0x4),
                new Tuple<string, int, int>("multiRocket_acquisition_time",                                  0x6c8, 0x4),
                new Tuple<string, int, int>("multiRocket_fire_interval",                                     0x6cc, 0x4),
                new Tuple<string, int, int>("multiRocket_fire_power_loss",                                   0x6d0, 0x4),
                new Tuple<string, int, int>("multirocket_target_number",                                     0x6d4, 0x4),
                new Tuple<string, int, int>("multiRocket_target_radius",                                     0x6d8, 0x4),
                new Tuple<string, int, int>("power_attack_loss",                                             0x718, 0x4),
                new Tuple<string, int, int>("power_damage_factor",                                           0x720, 0x8),
                new Tuple<string, int, int>("power_flicker_chance",                                          0x724, 0x8),
                new Tuple<string, int, int>("power_flicker_frequency",                                       0x728, 0x4),
                new Tuple<string, int, int>("power_flicker_threshold",                                       0x72c, 0x4),
                new Tuple<string, int, int>("power_juke_loss",                                               0x738, 0x4),
                new Tuple<string, int, int>("power_jump_loss",                                               0x73c, 0x4),
                new Tuple<string, int, int>("power_melee_loss",                                              0x740, 0x4),
                new Tuple<string, int, int>("power_move_loss",                                               0x744, 0x4),
                new Tuple<string, int, int>("power_move_loss_speed",                                         0x748, 0x4),
                new Tuple<string, int, int>("power_on_damage_factor",                                        0x74c, 0x8),
                new Tuple<string, int, int>("power_power_bonus",                                             0x750, 0x4),
                new Tuple<string, int, int>("power_recharge_delay",                                          0x754, 0x4),
                new Tuple<string, int, int>("power_recharge_delay_max",                                      0x758, 0x4),
                new Tuple<string, int, int>("power_recharge_rate",                                           0x75c, 0x8),
                new Tuple<string, int, int>("power_replenish_factor",                                        0x764, 0x8),
                new Tuple<string, int, int>("power_sprint_loss",                                             0x774, 0x4),
                new Tuple<string, int, int>("power_shut_off_penalty",                                        0x77c, 0x4),
                new Tuple<string, int, int>("power_turn_off_penalty",                                        0x780, 0x4),
                new Tuple<string, int, int>("power_usable_threshold",                                        0x784, 0x4),
                new Tuple<string, int, int>("power_usage_rate",                                              0x788, 0x8),
                new Tuple<string, int, int>("pulse_duration",                                                0x78c, 0x4),
                new Tuple<string, int, int>("pulse_margin",                                                  0x790, 0x4),
                new Tuple<string, int, int>("pulse_reveal_time",                                             0x794, 0x4),
                new Tuple<string, int, int>("pulse_reveal_camo_time",                                        0x798, 0x4),
                new Tuple<string, int, int>("pulse_reveal_time_viewModel",                                   0x79c, 0x4),
                new Tuple<string, int, int>("pulse_share_radius",                                            0x7a0, 0x4),
                new Tuple<string, int, int>("pulse_enemy_share_type",                                        0x7a4, 0x39),
                new Tuple<string, int, int>("pulse_share_type",                                              0x7a8, 0x3a),
                new Tuple<string, int, int>("pulse_type",                                                    0x7ac, 0x3b),
                new Tuple<string, int, int>("pulse_max_range",                                               0x7b0, 0x4),
                new Tuple<string, int, int>("shield_blast_protection_120",                                   0x7b4, 0x8),
                new Tuple<string, int, int>("shield_blast_protection_180",                                   0x7b8, 0x8),
                new Tuple<string, int, int>("shield_blast_protection_30",                                    0x7bc, 0x8),
                new Tuple<string, int, int>("shield_blast_protection_60",                                    0x7c0, 0x8),
                new Tuple<string, int, int>("shield_reflect_actor_accuracy_multiplier",                      0x7c4, 0x8),
                new Tuple<string, int, int>("shield_reflect_aim_assist_lerp",                                0x7c8, 0x8),
                new Tuple<string, int, int>("shield_reflect_damage_multiplier",                              0x7cc, 0x8),
                new Tuple<string, int, int>("shield_reflect_power_gain",                                     0x7d0, 0x4),
                new Tuple<string, int, int>("shield_reflect_power_loss",                                     0x7d4, 0x4),
                new Tuple<string, int, int>("fastFire",                                                      0x804, 0x6),
                new Tuple<string, int, int>("fastReload",                                                    0x805, 0x6),
                new Tuple<string, int, int>("fastSwitch",                                                    0x806, 0x6),
                new Tuple<string, int, int>("fastMelee",                                                     0x807, 0x6),
                new Tuple<string, int, int>("fastToss",                                                      0x808, 0x6),
                new Tuple<string, int, int>("fastEquipmentUse",                                              0x809, 0x6),
                new Tuple<string, int, int>("fastADS",                                                       0x80a, 0x6),
            };

            /// <summary>
            /// Attachment Types
            /// </summary>
            private static readonly string[] AttachmentUniqueTypes =
            {
                "none",
                "acog",
                "damage",
                "dualclip",
                "dualoptic",
                "dw",
                "dynzoom",
                "extbarrel",
                "extclip",
                "fastads",
                "fastreload",
                "fmj",
                "gmod0",
                "gmod1",
                "gmod2",
                "gmod3",
                "gmod4",
                "gmod5",
                "gmod6",
                "gmod7",
                "gl",
                "grip",
                "holo",
                "ir",
                "is",
                "mk",
                "mms",
                "notracer",
                "precision",
                "quickdraw",
                "rangefinder",
                "recon",
                "reddot",
                "reflex",
                "rf",
                "sf",
                "stackfire",
                "stalker",
                "steadyaim",
                "supply",
                "suppressed",
                "swayreduc",
                "tacknife",
                "vzoom"
            };

            /// <summary>
            /// Attachment Reticles
            /// </summary>
            private static readonly string[] AttachmentUniqueReticles =
            {
                "none",
                "crosshair",
                "FG42",
                "Springfield",
                "binoculars"
            };

            /// <summary>
            /// Attachment Stickiness
            /// </summary>
            private static readonly string[] AttachmentStickiness =
            {
                "Ignore",
                "Don't stick",
                "Stick to all",
                "Stick to all",
                "except ai and clients",
                "Stick to ground",
                "Stick to ground",
                "maintain yaw",
                "Stick to flesh"
            };

            /// <summary>
            /// Attachment Pulse Types
            /// </summary>
            private static readonly string[] AttachmentUniquePulseTypes =
            {
                "default",
                "none",
                "minimap",
                "viewport",
                "both"
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
            public string Name => "attachmentunique";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Weapon";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.attachmentunique;

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

                var assetBuffer = new byte[0xFE0];

                // Start by stitching the asset together so we can deal with a large buffer,
                // linker generates it in this fashion, but once loaded into memory, it's split into
                // sections with pointers
                // Base XAsset
                Array.Copy(buffer, 0, assetBuffer, 0, buffer.Length);
                // Animation Table
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 952), 1576), 0, assetBuffer, 0x840, 1576);
                // Hide Tags
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 96), 128), 0, assetBuffer, 0xE68, 128);
                // Attachment Data (View/World Model Model Pointers/Tags)
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 584), 40), 0, assetBuffer, 0xEE8, 40);
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 592), 40), 0, assetBuffer, 0xF10, 40);
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 600), 40), 0, assetBuffer, 0xF38, 40);
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 608), 40), 0, assetBuffer, 0xF60, 40);
                // Location Multipliers
                Array.Copy(instance.Reader.ReadBytes(BitConverter.ToInt64(buffer, 0x3E0), 80), 0, assetBuffer, 0xF88, 80);

                var result = ConvertAssetBufferToGDTAsset(assetBuffer, AttachmentUniqueOffsets, instance, HandleAttachmentUniqueSettings);

                result.Type = "attachmentunique";
                result.Name = asset.Name;
                instance.AddGDTAsset(result, result.Type, result.Name);

                return;
            }

            /// <summary>
            /// Handles Attachment Unique Specific settings
            /// </summary>
            private static object HandleAttachmentUniqueSettings(GameDataTable.Asset asset, byte[] assetBuffer, int offset, int type, HydraInstance instance)
            {
                switch (type)
                {
                    case 0x33:
                        {
                            return AttachmentUniqueTypes[BitConverter.ToInt32(assetBuffer, offset)];
                        }
                    case 0x34:
                        {
                            string hideTagsString = "";

                            for (int i = 0; i < 0x20; i++)
                                hideTagsString += instance.Game.GetString(BitConverter.ToInt32(assetBuffer, 0xe68 + (i * 4)), instance) + "\\r\\n";

                            return hideTagsString;
                        }
                    case 0x35:
                        {
                            return AttachmentUniqueReticles[BitConverter.ToInt32(assetBuffer, offset)];
                        }
                    case 0x36:
                        {
                            return instance.Game.GetAssetName(BitConverter.ToInt64(assetBuffer, offset), instance);
                        }
                    case 0x37:
                        {
                            return AttachmentStickiness[BitConverter.ToInt32(assetBuffer, offset)];
                        }
                    case 0x38:
                    case 0x39:
                    case 0x3A:
                    case 0x3B:
                        {
                            return AttachmentUniquePulseTypes[BitConverter.ToInt32(assetBuffer, offset)];
                        }
                    default:
                        {
                            return null;
                        }
                }
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
