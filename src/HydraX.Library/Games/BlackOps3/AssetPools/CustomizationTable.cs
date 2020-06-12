using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Customization Table Logic
        /// </summary>
        private class CustomizationTable : IAssetPool
        {
            #region Enus
            /// <summary>
            /// Player Gender
            /// </summary>
            private enum PlayerGender : int
            {
                Male = 0,
                Female = 1,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Customization Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct CustomizationTableAsset
            {
                #region CustomizationTableProperties
                public long NamePointer;
                public int BodyCount;
                public long BodiesPointer;
                public int HeadCount;
                public long HeadsPointer;
                #endregion
            }

            /// <summary>
            /// Player Head Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 48)]
            private struct PlayerHeadAsset
            {
                #region PlayerBodyTypeProperties
                public long NamePointer;
                public long DisplayNamePointer;
                public long IconPointer;
                public long ModelPointer;
                public PlayerGender Gender;
                #endregion
            }

            /// <summary>
            /// Player Body Type Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct PlayerBodyTypeAsset
            {
                #region PlayerBodyTypeProperties
                public long NamePointer;
                public long DisplayNamePointer;
                public long DescriptionPointer;
                public long HeroWeaponPointer;
                public long HeroAbilityPointer;
                public long BodySoundContextPointer;
                public long MpDialogPointer;
                public long RewardIconPointer;
                public long BackgroundPointer;
                public long BackgroundWithCharacterPointer;
                public long LockedImagePointer;
                public long PersonalizeRenderPointer;
                public long FrozenMomentRenderPointer;
                public long FrozenMomentOverlayPointer;
                public long DefaultHeroRenderPointer;
                public long DefaultHeroRenderAbilityPointer;
                public long WeaponIconEquippedPointer;
                public long AbilityIconEquippedPointer;
                public long WeaponIconUnequippedPointer;
                public long AbilityIconUnequippedPointer;
                public long ZombiePlayerIconPointer;
                public int BodyStyleCount;
                public long BodyStylesPointer;
                public int HelmetStyleCount;
                public long HelmetStylesPointer;
                public PlayerGender Gender;
                public bool Disabled;
                public long FrontendVignetteStructPointer;
                public long FrontendVignetteXCamPointer;
                public long FrontendVignetteXAnimPointer;
                public long FrontendVignetteWeaponModelPointer;
                public int DataBlockCount;
                public long DataBlocksPointer;
                public long CharacterMovementSoundsPointer;
                public long CharacterMovementFxPointer;
                public long CharacterFootstepsPointer;
                public long CharacterFootstepsQuietPointer;
                public long CharacterFootstepsNPCPointer;
                public long CharacterFootstepsNPCLoudPointer;
                public long CharacterFootstepsNPCQuietPointer;
                public long DogtagFriendlyPointer;
                public long DogTagEnemyPointer;
                public long CardBackIconPointer;
                public long RealNamePointer;
                public int Age;
                public long GenderStringPointer;
                public long BioPointer;
                public long WeaponCardBackIconPointer;
                public long WeaponCardBackSubIconPointer;
                public long WeaponCardBackDescPointer;
                public long WeaponSubItemDescPointer;
                public long WeaponSchemaPointer;
                public long AbilityCardBackIconPointer;
                public long AbilityCardBackSubIconPointer;
                public long AbilityCardBackDescPointer;
                public long AbilitySubItemDescPointer;
                public long AbilitySchemaPointer;
                #endregion
            }

            /// <summary>
            /// Player Body Style Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0xA8)]
            private struct PlayerBodyStyleAsset
            {
                #region PlayerBodyStyleProperties
                public long NamePointer;
                public long DisplayNamePointer;
                public long IconImagePointer;
                public long XmodelPointer;
                public long ViewArmsModelPointer;
                public long FirstPersonLegsModelPointer;
                public long FirstPersonCinematicModelPointer;
                public long GibTorsoCleanPointer;
                public long GibTorsoRightPointer;
                public long GibTorsoLeftPointer;
                public long GibLegsCleanPointer;
                public long GibLegsRightPointer;
                public long GibLegsLeftPointer;
                public long GibLegsBothPointer;
                public int AccentColorCount;
                public long AccentsPointer;
                public int ImpactType;
                public int ImpactTypeCorpse;
                public long GibDefPointer;
                public long CharacterMovementFxOverridePointer;
                #endregion
            }

            /// <summary>
            /// Player Accent Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x10)]
            private struct PlayerAccent
            {
                #region PlayerBodyStyleProperties
                public int DataCount;
                public long DataPointer;
                #endregion
            }
            #endregion

            #region Tables
            /// <summary>
            /// Impact Types
            /// </summary>
            private static readonly string[] ImpactTypes =
            {
                "none", // Not included in our AWI
                "flesh",
                "zombie",
                "armor light",
                "armor heavy",
                "robot light",
                "robot heavy",
                "robot boss", // Not included in our AWI
                "power armor",
                "skeleton",
                "flesh corpse",
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
            public string Name => "customizationtable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.customizationtable;

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
                    var header = instance.Reader.ReadStruct<CustomizationTableAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Bodies: {0}", header.BodyCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<CustomizationTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var customizationTable = new GameDataTable.Asset(asset.Name, "charactercustomizationtable");
                customizationTable["bodyTypeCount"] = header.BodyCount;
                customizationTable["headCount"] = header.HeadCount;

                var playerHeads = instance.Reader.ReadArray<PlayerHeadAsset>(header.HeadsPointer, header.HeadCount);

                for(int i = 0; i < playerHeads.Length; i++)
                {
                    string name = instance.Reader.ReadNullTerminatedString(playerHeads[i].NamePointer);

                    var playerHead = new GameDataTable.Asset(name, "playerhead");

                    playerHead["displayName"] = instance.Reader.ReadNullTerminatedString(playerHeads[i].DisplayNamePointer);
                    playerHead["iconImage"] = instance.Game.GetAssetName(playerHeads[i].IconPointer, instance, 0xF8);
                    playerHead["model"] = instance.Game.GetAssetName(playerHeads[i].ModelPointer, instance);
                    playerHead["gender"] = (int)playerHeads[i].Gender;

                    customizationTable["head" + (i + 1).ToString("00")] = playerHead.Name;

                    instance.AddGDTAsset(playerHead, playerHead.Type, playerHead.Name);
                }

                var playerBodyTypes = instance.Reader.ReadArray<PlayerBodyTypeAsset>(header.BodiesPointer, header.BodyCount);

                for (int i = 0; i < playerBodyTypes.Length; i++)
                {
                    var playerBodyType = new GameDataTable.Asset(instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].NamePointer), "playerbodytype");

                    playerBodyType["abilityCardBackDesc"]                = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilityCardBackDescPointer);
                    playerBodyType["abilityCardBackIcon"]                = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilityCardBackIconPointer);
                    playerBodyType["abilityCardBackSubIcon"]             = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilityCardBackSubIconPointer);
                    playerBodyType["abilityIconEquipped"]                = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilityIconEquippedPointer);
                    playerBodyType["abilityIconUnequipped"]              = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilityIconUnequippedPointer);
                    playerBodyType["abilitySchema"]                      = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilitySchemaPointer);
                    playerBodyType["abilitySubItemDesc"]                 = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].AbilitySubItemDescPointer);
                    playerBodyType["age"]                                = playerBodyTypes[i].Age;
                    playerBodyType["background"]                         = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].BackgroundPointer);
                    playerBodyType["backgroundWithCharacter"]            = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].BackgroundWithCharacterPointer);
                    playerBodyType["bio"]                                = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].BioPointer);
                    playerBodyType["bodySoundContext"]                   = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].BodySoundContextPointer);
                    playerBodyType["bodyStyleCount"]                     = playerBodyTypes[i].BodyStyleCount;
                    playerBodyType["cardBackIcon"]                       = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].CardBackIconPointer);
                    playerBodyType["characterFootstepsNPCLoud"]          = instance.Game.GetAssetName(playerBodyTypes[i].CharacterFootstepsNPCLoudPointer, instance);
                    playerBodyType["characterFootstepsNPC"]              = instance.Game.GetAssetName(playerBodyTypes[i].CharacterFootstepsNPCPointer, instance);
                    playerBodyType["characterFootstepsNPCQuiet"]         = instance.Game.GetAssetName(playerBodyTypes[i].CharacterFootstepsNPCQuietPointer, instance);
                    playerBodyType["characterFootsteps"]                 = instance.Game.GetAssetName(playerBodyTypes[i].CharacterFootstepsPointer, instance);
                    playerBodyType["characterFootstepsQuiet"]            = instance.Game.GetAssetName(playerBodyTypes[i].CharacterFootstepsQuietPointer, instance);
                    playerBodyType["characterMovementFx"]                = instance.Game.GetAssetName(playerBodyTypes[i].CharacterMovementFxPointer, instance);
                    playerBodyType["characterMovementSounds"]            = instance.Game.GetAssetName(playerBodyTypes[i].CharacterMovementSoundsPointer, instance);
                    playerBodyType["defaultHeroRenderAbility"]           = instance.Game.GetAssetName(playerBodyTypes[i].DefaultHeroRenderAbilityPointer, instance, 0xF8);
                    playerBodyType["defaultHeroRender"]                  = instance.Game.GetAssetName(playerBodyTypes[i].DefaultHeroRenderPointer, instance, 0xF8);
                    playerBodyType["description"]                        = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].DescriptionPointer);
                    playerBodyType["disabled"]                           = playerBodyTypes[i].Disabled;
                    playerBodyType["displayName"]                        = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].DisplayNamePointer);
                    playerBodyType["dogTagEnemy"]                        = instance.Game.GetAssetName(playerBodyTypes[i].DogTagEnemyPointer, instance);
                    playerBodyType["dogtagFriendly"]                     = instance.Game.GetAssetName(playerBodyTypes[i].DogtagFriendlyPointer, instance);
                    playerBodyType["frontendVignetteStruct"]             = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].FrontendVignetteStructPointer);
                    playerBodyType["frontendVignetteWeaponModel"]        = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].FrontendVignetteWeaponModelPointer);
                    playerBodyType["frontendVignetteXAnim"]              = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].FrontendVignetteXAnimPointer);
                    playerBodyType["frontendVignetteXCam"]               = instance.Game.GetAssetName(playerBodyTypes[i].FrontendVignetteXCamPointer, instance);
                    playerBodyType["frozenMomentOverlay"]                = instance.Game.GetAssetName(playerBodyTypes[i].FrozenMomentOverlayPointer, instance);
                    playerBodyType["frozenMomentRender"]                 = instance.Game.GetAssetName(playerBodyTypes[i].FrozenMomentRenderPointer, instance);
                    playerBodyType["gender"]                             = (int)playerBodyTypes[i].Gender;
                    playerBodyType["genderString"]                       = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].GenderStringPointer);
                    playerBodyType["helmetStyleCount"]                   = playerBodyTypes[i].HelmetStyleCount;
                    playerBodyType["heroAbility"]                        = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].HeroAbilityPointer);
                    playerBodyType["heroWeapon"]                         = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].HeroWeaponPointer);
                    playerBodyType["lockedImage"]                        = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].LockedImagePointer);
                    playerBodyType["mpDialog"]                           = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].MpDialogPointer);
                    playerBodyType["personalizeRender"]                  = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].PersonalizeRenderPointer);
                    playerBodyType["realName"]                           = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].RealNamePointer);
                    playerBodyType["rewardIcon"]                         = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].RewardIconPointer);
                    playerBodyType["weaponCardBackDesc"]                 = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponCardBackDescPointer);
                    playerBodyType["weaponCardBackIcon"]                 = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponCardBackIconPointer);
                    playerBodyType["weaponCardBackSubIcon"]              = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponCardBackSubIconPointer);
                    playerBodyType["weaponIconEquipped"]                 = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponIconEquippedPointer);
                    playerBodyType["weaponIconUnequipped"]               = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponIconUnequippedPointer);
                    playerBodyType["weaponSchema"]                       = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponSchemaPointer);
                    playerBodyType["weaponSubItemDesc"]                  = instance.Reader.ReadNullTerminatedString(playerBodyTypes[i].WeaponSubItemDescPointer);
                    playerBodyType["zombiePlayerIcon"]                   = instance.Game.GetAssetName(playerBodyTypes[i].ZombiePlayerIconPointer, instance, 0xF8);

                    var playerBodyStyles = instance.Reader.ReadArray<PlayerBodyStyleAsset>(playerBodyTypes[i].BodyStylesPointer, playerBodyTypes[i].BodyStyleCount);

                    for(int j = 0; j < playerBodyStyles.Length; j++)
                    {
                        var playerBodyStyle = new GameDataTable.Asset(instance.Reader.ReadNullTerminatedString(playerBodyStyles[j].NamePointer), "playerbodystyle");

                        playerBodyStyle["accentColorCount"]                   = playerBodyStyles[j].AccentColorCount;
                        playerBodyStyle["characterMovementFxOverride"]        = instance.Game.GetAssetName(playerBodyStyles[j].CharacterMovementFxOverridePointer, instance);
                        playerBodyStyle["displayName"]                        = instance.Reader.ReadNullTerminatedString(playerBodyStyles[j].DisplayNamePointer);
                        playerBodyStyle["firstPersonCinematicModel"]          = instance.Game.GetAssetName(playerBodyStyles[j].FirstPersonCinematicModelPointer, instance);
                        playerBodyStyle["firstPersonLegsModel"]               = instance.Game.GetAssetName(playerBodyStyles[j].FirstPersonLegsModelPointer, instance);
                        playerBodyStyle["gibDef"]                             = instance.Game.GetAssetName(playerBodyStyles[j].GibDefPointer, instance);
                        playerBodyStyle["gibLegsBoth"]                        = instance.Game.GetAssetName(playerBodyStyles[j].GibLegsBothPointer, instance);
                        playerBodyStyle["gibLegsClean"]                       = instance.Game.GetAssetName(playerBodyStyles[j].GibLegsCleanPointer, instance);
                        playerBodyStyle["gibLegsLeft"]                        = instance.Game.GetAssetName(playerBodyStyles[j].GibLegsLeftPointer, instance);
                        playerBodyStyle["gibLegsRight"]                       = instance.Game.GetAssetName(playerBodyStyles[j].GibLegsRightPointer, instance);
                        playerBodyStyle["gibTorsoClean"]                      = instance.Game.GetAssetName(playerBodyStyles[j].GibTorsoCleanPointer, instance);
                        playerBodyStyle["gibTorsoLeft"]                       = instance.Game.GetAssetName(playerBodyStyles[j].GibTorsoLeftPointer, instance);
                        playerBodyStyle["gibTorsoRight"]                      = instance.Game.GetAssetName(playerBodyStyles[j].GibTorsoRightPointer, instance);
                        playerBodyStyle["iconImage"]                          = instance.Game.GetAssetName(playerBodyStyles[j].IconImagePointer, instance, 0xF8);
                        playerBodyStyle["impactTypeCorpse"]                   = ImpactTypes[playerBodyStyles[j].ImpactTypeCorpse];
                        playerBodyStyle["impactType"]                         = ImpactTypes[playerBodyStyles[j].ImpactType];
                        playerBodyStyle["viewArmsModel"]                      = instance.Game.GetAssetName(playerBodyStyles[j].ViewArmsModelPointer, instance);
                        playerBodyStyle["xmodel"]                             = instance.Game.GetAssetName(playerBodyStyles[j].XmodelPointer, instance);

                        var accents = instance.Reader.ReadArray<PlayerAccent>(playerBodyStyles[j].AccentsPointer, playerBodyStyles[j].AccentColorCount);

                        for(int k = 0; k < accents.Length; k++)
                        {
                            var accentOptions = instance.Reader.ReadArray<long>(accents[k].DataPointer, accents[k].DataCount);

                            playerBodyStyle[string.Format("accentColor{0}OptionsCount", k + 1)] = accents[k].DataCount;

                            for(int l = 0; l < accentOptions.Length; l++)
                            {
                                playerBodyStyle[string.Format("color_{0}_{1}", k + 1, l + 1)] = instance.Game.GetAssetName(accentOptions[l], instance);
                            }
                        }

                        playerBodyType["bodyStyle" + (j + 1).ToString("00")] = playerBodyStyle.Name;

                        instance.AddGDTAsset(playerBodyStyle, playerBodyStyle.Type, playerBodyStyle.Name);
                    }

                    var playerHelmetStyles = instance.Reader.ReadArray<PlayerBodyStyleAsset>(playerBodyTypes[i].HelmetStylesPointer, playerBodyTypes[i].HelmetStyleCount);

                    for (int j = 0; j < playerHelmetStyles.Length; j++)
                    {
                        var playerHelmetStyle = new GameDataTable.Asset(instance.Reader.ReadNullTerminatedString(playerHelmetStyles[j].NamePointer), "playerhelmetstyle");

                        playerHelmetStyle["accentColorCount"]            = playerHelmetStyles[j].AccentColorCount;
                        playerHelmetStyle["displayName"]                 = instance.Reader.ReadNullTerminatedString(playerHelmetStyles[j].DisplayNamePointer);
                        playerHelmetStyle["iconImage"]                   = instance.Game.GetAssetName(playerHelmetStyles[j].IconImagePointer, instance, 0xF8);
                        playerHelmetStyle["impactTypeCorpse"]            = ImpactTypes[playerHelmetStyles[j].ImpactTypeCorpse];
                        playerHelmetStyle["impactType"]                  = ImpactTypes[playerHelmetStyles[j].ImpactType];
                        playerHelmetStyle["xmodel"]                      = instance.Game.GetAssetName(playerHelmetStyles[j].XmodelPointer, instance);

                        var accents = instance.Reader.ReadArray<PlayerAccent>(playerHelmetStyles[j].AccentsPointer, playerHelmetStyles[j].AccentColorCount);

                        for (int k = 0; k < accents.Length; k++)
                        {
                            var accentOptions = instance.Reader.ReadArray<long>(accents[k].DataPointer, accents[k].DataCount);

                            playerHelmetStyle[string.Format("accentColor{0}OptionsCount", k + 1)] = accents[k].DataCount;

                            for (int l = 0; l < accentOptions.Length; l++)
                            {
                                playerHelmetStyle[string.Format("color_{0}_{1}", k + 1, l + 1)] = instance.Game.GetAssetName(accentOptions[l], instance);
                            }
                        }

                        playerBodyType["helmetStyle" + (j + 1).ToString("00")] = playerHelmetStyle.Name;

                        instance.AddGDTAsset(playerHelmetStyle, playerHelmetStyle.Type, playerHelmetStyle.Name);
                    }

                    var dataBlocks = instance.Reader.ReadArray<KVPBlock>(playerBodyTypes[i].DataBlocksPointer, playerBodyTypes[i].DataBlockCount);

                    for(int j = 0; j < dataBlocks.Length; j++)
                    {
                        // Determine type if we're int, since bool and int share same, but the pointer value will be different? (only 2 are bool anyway but just in case)
                        var dataType = dataBlocks[i].DataType;

                        if (dataType == DataTypes.Int && (dataBlocks[i].DataPointer & 0xFFFFFFFF) != dataBlocks[i].Data)
                            dataType = DataTypes.Bool;

                        string propertyName = string.Format("{0}_{1}", dataType.ToString().ToLower(), instance.Game.GetString(dataBlocks[i].DataNameStringIndex, instance));

                        playerBodyType[propertyName] = dataBlocks[i].DataType == DataTypes.Int ? dataBlocks[i].Data.ToString() : instance.Game.GetString(dataBlocks[i].DataStringIndex, instance);
                    }

                    customizationTable["bodyType" + (i + 1).ToString("00")] = playerBodyType.Name;

                    instance.AddGDTAsset(playerBodyType, playerBodyType.Type, playerBodyType.Name);
                }

                instance.AddGDTAsset(customizationTable, customizationTable.Type, customizationTable.Name);

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
