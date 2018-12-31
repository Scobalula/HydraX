using HydraLib.Assets;
using PhilLibX;
using System;
using System.Runtime.InteropServices;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class CustomizationTable
        {
            /// <summary>
            /// Impact Types
            /// </summary>
            public static string[] ImpactTypes =
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

            /// <summary>
            /// Data Block Data Types
            /// </summary>
            public enum DataTypes : int
            {
                Image            = 0xF,
                Anim             = 0x3,
                PlayerAnim       = 0x4,
                XCam             = 0xD,
                XModel           = 0x6,
                XString          = 0x0,
                FX               = 0x9,
                Int              = 0x1,
                Bool,
            }

            /// <summary>
            /// Player Gender
            /// </summary>
            public enum PlayerGender : long
            {
                /// <summary>
                /// Male Characters
                /// </summary>
                Male = 0,

                /// <summary>
                /// Female Characters
                /// </summary>
                Female = 1,
            }

            /// <summary>
            /// Bo3 AI ASM Header
            /// </summary>
            public struct CustomizationTableHeader
            {
                /// <summary>
                /// Pointer to the name of this asset
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Number of Player Bodies
                /// </summary>
                public int BodyCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Player Bodies
                /// </summary>
                public long BodiesPointer { get; set; }

                /// <summary>
                /// Number of Player Heads
                /// </summary>
                public int HeadCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding1 { get; set; }

                /// <summary>
                /// Pointer to the Player Heads
                /// </summary>
                public long HeadsPointer { get; set; }
            }

            public struct CharacterDataBlock
            {
                /// <summary>
                /// Index of the Name of this Data in the String Database
                /// </summary>
                public int DataNameStringIndex { get; set; }

                /// <summary>
                /// Index of the Data in the String Database
                /// </summary>
                public int DataStringIndex { get; set; }

                /// <summary>
                /// Data Type
                /// </summary>
                public DataTypes DataType { get; set; }

                /// <summary>
                /// Data
                /// </summary>
                public int Data { get; set; }

                /// <summary>
                /// Pointer to the Data (i.e. if XModel -> XModel Asset, etc.)
                /// </summary>
                public long DataPointer { get; set; }
            }

            /// <summary>
            /// Bo3 Player Body
            /// </summary>
            public struct PlayerBody
            {
                /// <summary>
                /// Player Body FX Block
                /// </summary>
                public struct FX
                {
                    /// <summary>
                    /// Pointer to the Char Movement Sounds Asset
                    /// </summary>
                    public long CharacterMovementSoundsPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Char Movement FX Asset
                    /// </summary>
                    public long CharacterMovementFXPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Char Footsteps Asset
                    /// </summary>
                    public long CharacterFootstepsPointer { get; set; }

                    /// <summary>
                    /// keep werry quiet im hunting wabbits Asset
                    /// </summary>
                    public long CharacterFootstepsQuietPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Char Footsteps NPC Asset
                    /// </summary>
                    public long CharacterFootstepsNPCPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Char Footsteps Loud NPC Asset
                    /// </summary>
                    public long CharacterFootstepsNPCLoudPointer { get; set; }

                    /// <summary>
                    /// Pointer to the Char Footsteps Quiet NPC Asset
                    /// </summary>
                    public long CharacterFootstepsNPCQuietPointer { get; set; }
                }

                /// <summary>
                /// Card Back Block
                /// </summary>
                public struct CardBack
                {
                    /// <summary>
                    /// Pointer to the Card Back Icon
                    /// </summary>
                    public long CardBackIconNamePointer { get; set; }

                    /// <summary>
                    /// Pointer to the Real Name
                    /// </summary>
                    public long RealNamePointer { get; set; }

                    /// <summary>
                    /// Character Age
                    /// </summary>
                    public long Age { get; set; }

                    /// <summary>
                    /// Pointer to the Gender as a String
                    /// </summary>
                    public long GenderPointer { get; set; }

                    /// <summary>
                    /// Pointer to the bio
                    /// </summary>
                    public long BioPointer { get; set; }

                    /// <summary>
                    /// Card Back Icon
                    /// </summary>
                    public struct Item
                    {
                        /// <summary>
                        /// Pointer to the Card Back Icon
                        /// </summary>
                        public long CardBackIconNamePointer { get; set; }

                        /// <summary>
                        /// Pointer to the Card Back Sub Icon
                        /// </summary>
                        public long CardBackSubIconNamePointer { get; set; }

                        /// <summary>
                        /// Pointer to the Description
                        /// </summary>
                        public long DescriptionPointer { get; set; }

                        /// <summary>
                        /// Pointer to the Description
                        /// </summary>
                        public long SubDescriptionPointer { get; set; }

                        /// <summary>
                        /// Pointer to the Schema String
                        /// </summary>
                        public long SchemaPointer { get; set; }
                    }

                    /// <summary>
                    /// Card Back Weapon
                    /// </summary>
                    public Item Weapon { get; set; }

                    /// <summary>
                    /// Card Back Ability
                    /// </summary>
                    public Item Ability { get; set; }
                }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the Hero Name
                /// </summary>
                public long HeroNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Description
                /// </summary>
                public long DescriptionPointer { get; set; }

                /// <summary>
                /// Pointer to the Hero Weapon Name
                /// </summary>
                public long HeroWeaponNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Hero Ability
                /// </summary>
                public long HeroAbilityPointer { get; set; }

                /// <summary>
                /// Pointer to the Body Sound Context
                /// </summary>
                public long BodySoundContextPointer { get; set; }

                /// <summary>
                /// Player Gender
                /// </summary>
                public PlayerGender Gender { get; set; }

                /// <summary>
                /// Pointer to the Hero Reward Icon
                /// </summary>
                public long HeroRewardIconPointer { get; set; }

                /// <summary>
                /// Pointer to the Locked Background with Character Icon
                /// </summary>
                public long LockedCharacterBackgroundIconNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Background with Character Icon
                /// </summary>
                public long CharacterBackgroundIconNamePointer { get; set; }

                /// <summary>
                /// Null Bytes
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
                public byte[] Padding;

                /// <summary>
                /// Pointer to the Unlocked Silver Render Icon
                /// </summary>
                public long UnlockedSilverRenderIconNamePointer { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private long Padding1 { get; set; }

                /// <summary>
                /// Pointer to the Defulat Hero Render Weapon Icon
                /// </summary>
                public long DefaultHeroRenderWeaponIconPointer { get; set; }

                /// <summary>
                /// Pointer to the Defulat Hero Render Ability Icon
                /// </summary>
                public long DefaultHeroRenderAbilityIconPointer { get; set; }

                /// <summary>
                /// Pointer to the Equipped Weapon Icon
                /// </summary>
                public long EquippedWeaponIconNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Equipped Ability Icon
                /// </summary>
                public long EquippedAbilityIconNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Unquipped Weapon Icon
                /// </summary>
                public long UnequippedWeaponIconNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Unequipped Ability Icon
                /// </summary>
                public long UnequippedAbilityIconNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Player List Icon
                /// </summary>
                public long PlayerListIconPointer { get; set; }

                /// <summary>
                /// Number of Body Styles
                /// </summary>
                public int BodyStyleCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding2 { get; set; }

                /// <summary>
                /// Pointer to the Body Styles
                /// </summary>
                public long BodyStylesPointer { get; set; }

                /// <summary>
                /// Number of Body Styles
                /// </summary>
                public int HelmetStyleCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding3 { get; set; }

                /// <summary>
                /// Pointer to the Head Styles
                /// </summary>
                public long HelmetStylesPointer { get; set; }

                /// <summary>
                /// Flags
                /// </summary>
                public long Flags { get; set; }

                /// <summary>
                /// Unknown Bytes
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
                public byte[] Padding1xx;

                /// <summary>
                /// Number of Data Blocks
                /// </summary>
                public int DataCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding4 { get; set; }

                /// <summary>
                /// Pointer to the Data Blocks
                /// </summary>
                public long DataPointer { get; set; }

                /// <summary>
                /// Body FX Block
                /// </summary>
                public FX FXAssets { get; set; }

                /// <summary>
                /// Pointer to the Friendly Dog Tag Model Asset
                /// </summary>
                public long DogTagFriendlyModelPointer { get; set; }

                /// <summary>
                /// Pointer to the Enemy Dog Tag Model Asset
                /// </summary>
                public long DogTagEnemyModelPointer { get; set; }

                /// <summary>
                /// Player Card Back
                /// </summary>
                public CardBack CardBackBlock { get; set; }
            }

            public struct PlayerBodyStyle
            {
                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long DisplayNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long IconPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long ThirdPersonPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long ViewarmsPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long FirstPersonLegsPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long FirstPersonCinematicPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long CleanTorsoPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long RightArmGonePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long LeftArmGonePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long CleanLegsPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long RightLegGonePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long LeftLegGonePointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long BothLegsGonePointer { get; set; }

                /// <summary>
                /// Number of Accents/Skins
                /// </summary>
                public int AccentCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Accents
                /// </summary>
                public long AccentsPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public int ImpactTypeIndex { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public int ImpactTypeCorpseIndex { get; set; }

                /// <summary>
                /// Pointer to the Gib Definition
                /// </summary>
                public long GibDefinitionPointer { get; set; }

                /// <summary>
                /// Pointer to the Name
                /// </summary>
                public long MovementFXOverridePointer { get; set; }

                /// <summary>
                /// Unknown (Possibly related to unimplemented hide tags)
                /// </summary>
                public long UnknownNull1 { get; set; }

                /// <summary>
                /// Unknown (Possibly related to unimplemented hide tags)
                /// </summary>
                public long UnknownNull2 { get; set; }
            }

            /// <summary>
            /// Bo3 Accent/Skin Pointer
            /// </summary>
            public struct PlayerAccent
            {
                /// <summary>
                /// Number of Accents/Skins
                /// </summary>
                public int Count { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Pointer to the Accents
                /// </summary>
                public long DataPointer { get; set; }
            }

            /// <summary>
            /// Bo3 Player Head
            /// </summary>
            public struct PlayerHead
            {
                /// <summary>
                /// Pointer to the name of this Head
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the display name of this Head
                /// </summary>
                public long DisplayNamePointer { get; set; }

                /// <summary>
                /// Pointer to the Head's Icon Image Asset
                /// </summary>
                public long IconPointer { get; set; }

                /// <summary>
                /// Pointer to the Head's XModel Asset
                /// </summary>
                public long ModelPointer { get; set; }

                /// <summary>
                /// Player Gender
                /// </summary>
                public PlayerGender Gender { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private long Padding { get; set; }
            }

            public static void Load(AssetPool assetPool)
            {
                // Loop through entire pool, and add what valid assets
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read Customization Table
                    var customizationTable = Hydra.ActiveGameReader.ReadStruct<CustomizationTableHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(customizationTable.NamePointer))
                        continue;
                    // Add Asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(customizationTable.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = String.Format("Heads - {0} : Bodies - {1}", customizationTable.HeadCount, customizationTable.BodyCount),
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var customizationTableHeader = Hydra.ActiveGameReader.ReadStruct<CustomizationTableHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(customizationTableHeader.NamePointer))
                    return false;
                // Add to GDT
                Hydra.GDTs["Character"].AddAsset(asset.Name, new GameDataTable.Asset(asset.Name, "charactercustomizationtable"));
                Hydra.GDTs["Character"].Assets[asset.Name].Properties["bodyTypeCount"] = customizationTableHeader.BodyCount.ToString();
                Hydra.GDTs["Character"].Assets[asset.Name].Properties["headCount"]     = customizationTableHeader.HeadCount.ToString();
                // Load Heads
                var playerHeadBuffer = Hydra.ActiveGameReader.ReadBytes(customizationTableHeader.HeadsPointer, customizationTableHeader.HeadCount * 48);
                // Loop through them
                for (int i = 0; i < customizationTableHeader.HeadCount; i++)
                {
                    // Load Head
                    var playerHead = ByteUtil.BytesToStruct<PlayerHead>(playerHeadBuffer, i * 48);
                    // Get Name
                    string name = Hydra.ActiveGameReader.ReadNullTerminatedString(playerHead.NamePointer);
                    // Add to table
                    Hydra.GDTs["Character"].Assets[asset.Name].Properties[String.Format("head{0}", (i + 1).ToString().PadLeft(2, '0'))] = name;
                    // Convert to generic Head and add to GDT
                    Hydra.GDTs["Character"].AddAsset(name, "playerhead", new PlayerHeadObj()
                    {
                        DisplayName = Hydra.ActiveGameReader.ReadNullTerminatedString(playerHead.DisplayNamePointer),
                        Model = GetAssetName(playerHead.ModelPointer),
                        Icon = GetAssetName(playerHead.IconPointer, 0xF8),
                        Gender = (int)playerHead.Gender,
                    });
                }
                // Load Heads
                var playerBodiesBuffer = Hydra.ActiveGameReader.ReadBytes(customizationTableHeader.BodiesPointer, customizationTableHeader.BodyCount * 448);
                // Loop through them
                for (int i = 0; i < customizationTableHeader.BodyCount; i++)
                {
                    // Load Body
                    var playerBody = ByteUtil.BytesToStruct<PlayerBody>(playerBodiesBuffer, i * 448);
                    // Get Name
                    string bodyName = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.NamePointer);
                    // Add to table
                    Hydra.GDTs["Character"].Assets[asset.Name].Properties[String.Format("bodyType{0}", (i + 1).ToString().PadLeft(2, '0'))] = bodyName;
                    // Create it and add it#
                    Hydra.GDTs["Character"].AddAsset(bodyName, "playerbodytype", new PlayerBodyObj()
                    {
                        // Grab Base Strings
                        DisplayName      = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.HeroNamePointer),
                        Description      = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.HeroNamePointer),
                        HeroWeapon       = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.HeroWeaponNamePointer),
                        HeroAbility      = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.HeroAbilityPointer),
                        BodySoundContext = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.BodySoundContextPointer),
                        Gender           = ((int)playerBody.Gender).ToString(),

                        // Set Icons
                        Background               = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.LockedCharacterBackgroundIconNamePointer),
                        BackgroundWithCharacter  = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CharacterBackgroundIconNamePointer),
                        AbilityIconEquipped      = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.EquippedAbilityIconNamePointer),
                        AbilityIconUnequipped    = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.UnequippedAbilityIconNamePointer),
                        WeaponIconEquipped       = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.EquippedWeaponIconNamePointer),
                        WeaponIconUnequipped     = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.UnequippedWeaponIconNamePointer),
                        DefaultHeroRender        = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.DefaultHeroRenderWeaponIconPointer),
                        DefaultHeroRenderAbility = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.DefaultHeroRenderAbilityIconPointer),
                        FrozenMomentRender       = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.UnlockedSilverRenderIconNamePointer),
                        RewardIcon               = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.HeroRewardIconPointer),
                        ZombiePlayerIcon         = GetAssetName(playerBody.PlayerListIconPointer, 0xF8),

                        // Set Card Back Data
                        CardBackIcon = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.CardBackIconNamePointer),
                        RealName     = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.CardBackIconNamePointer),
                        Age          = playerBody.CardBackBlock.Age.ToString(),
                        GenderString = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.GenderPointer),
                        Bio          = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.BioPointer),

                        // Set Weapon
                        WeaponCardBackIcon    = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Weapon.CardBackIconNamePointer),
                        WeaponCardBackSubIcon = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Weapon.CardBackSubIconNamePointer),
                        WeaponCardBackDesc    = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Weapon.DescriptionPointer),
                        WeaponSubItemDesc     = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Weapon.SubDescriptionPointer),
                        WeaponSchema          = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Weapon.SchemaPointer),

                        // Set Ability
                        AbilityCardBackIcon    = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Ability.CardBackIconNamePointer),
                        AbilityCardBackSubIcon = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Ability.CardBackSubIconNamePointer),
                        AbilityCardBackDesc    = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Ability.DescriptionPointer),
                        AbilitySubItemDesc     = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Ability.SubDescriptionPointer),
                        AbilitySchema          = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBody.CardBackBlock.Ability.SchemaPointer),

                        // FX
                        CharacterFootsteps         = GetAssetName(playerBody.FXAssets.CharacterFootstepsPointer),
                        CharacterFootstepsNPC      = GetAssetName(playerBody.FXAssets.CharacterFootstepsNPCPointer),
                        CharacterFootstepsNPCLoud  = GetAssetName(playerBody.FXAssets.CharacterFootstepsNPCLoudPointer),
                        CharacterFootstepsNPCQuiet = GetAssetName(playerBody.FXAssets.CharacterFootstepsNPCQuietPointer),
                        CharacterFootstepsQuiet    = GetAssetName(playerBody.FXAssets.CharacterFootstepsQuietPointer),
                        CharacterMovementFx        = GetAssetName(playerBody.FXAssets.CharacterMovementFXPointer),
                        CharacterMovementSounds    = GetAssetName(playerBody.FXAssets.CharacterMovementSoundsPointer),

                        // Set Counts
                        BodyStyleCount   = playerBody.BodyStyleCount.ToString(),
                        HelmetStyleCount = playerBody.HelmetStyleCount.ToString()
                    });
                    // Load Player Body Styles
                    var playerBodyStyleBuffer = Hydra.ActiveGameReader.ReadBytes(playerBody.BodyStylesPointer, playerBody.BodyStyleCount * 168);
                    // Loop through player styles
                    for (int j = 0; j < playerBody.BodyStyleCount; j++)
                    {
                        // Load Body Style
                        var playerBodyStyle = ByteUtil.BytesToStruct<PlayerBodyStyle>(playerBodyStyleBuffer, j * 168);
                        // Get Name
                        var name = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBodyStyle.NamePointer);
                        // Add to player
                        Hydra.GDTs["Character"].Assets[bodyName].Properties[String.Format("bodyStyle{0}", (j + 1).ToString().PadLeft(2, '0'))] = name;
                        // Convert to generic body style and add to GDT
                        Hydra.GDTs["Character"].AddAsset(name, "playerbodystyle", new PlayerBodyStyleObj()
                        {
                            // Set Display Name
                            DisplayName = Hydra.ActiveGameReader.ReadNullTerminatedString(playerBodyStyle.DisplayNamePointer),

                            // Set Icon
                            Icon = GetAssetName(playerBodyStyle.IconPointer, 0xF8),

                            // Set XModels
                            ThirdPersonModel            = GetAssetName(playerBodyStyle.ThirdPersonPointer),
                            GibTorsoClean               = GetAssetName(playerBodyStyle.CleanTorsoPointer),
                            GibTorsoLeft                = GetAssetName(playerBodyStyle.LeftArmGonePointer),
                            GibTorsoRight               = GetAssetName(playerBodyStyle.RightArmGonePointer),
                            GibLegsClean                = GetAssetName(playerBodyStyle.CleanLegsPointer),
                            GibLegsLeft                 = GetAssetName(playerBodyStyle.LeftLegGonePointer),
                            GibLegsRight                = GetAssetName(playerBodyStyle.RightLegGonePointer),
                            GibLegsBoth                 = GetAssetName(playerBodyStyle.BothLegsGonePointer),
                            FirstPersonCinematicModel   = GetAssetName(playerBodyStyle.FirstPersonCinematicPointer),
                            FirstPersonLegsModel        = GetAssetName(playerBodyStyle.FirstPersonLegsPointer),
                            ViewArmsModel               = GetAssetName(playerBodyStyle.ViewarmsPointer),

                            // Set FX
                            CharacterMovementFXOverride = GetAssetName(playerBodyStyle.MovementFXOverridePointer),

                            // Set Counts
                            AccentColor1Count = 0,
                            AccentColorCount = playerBodyStyle.AccentCount,

                            // Set Impact Types
                            ImpactType       = ImpactTypes[playerBodyStyle.ImpactTypeIndex],
                            ImpactTypeCorpse = ImpactTypes[playerBodyStyle.ImpactTypeCorpseIndex],

                            // Set Gib
                            GibDef = GetAssetName(playerBodyStyle.GibDefinitionPointer)
                        });
                        // Check for accents
                        if(playerBodyStyle.AccentCount > 0 && playerBodyStyle.AccentsPointer > 0)
                        {
                            // Load Accents
                            var accentsBuffer = Hydra.ActiveGameReader.ReadBytes(playerBodyStyle.AccentsPointer, playerBodyStyle.AccentCount * 16);
                            // Loop through them
                            for(int x = 0; x < playerBodyStyle.AccentCount && x < 6; x++)
                            {
                                // Load Accent
                                var accent = ByteUtil.BytesToStruct<PlayerAccent>(accentsBuffer, x * 16);
                                // Load Buffer
                                var accentOptionsBuffer = Hydra.ActiveGameReader.ReadBytes(accent.DataPointer, accent.Count * 8);
                                // Set it
                                Hydra.GDTs["Character"].Assets[name].Properties["accentColor1OptionsCount"] = accent.Count.ToString();
                                // Loop through them
                                for (int y = 0; y < accent.Count && y < 6; y++)
                                    Hydra.GDTs["Character"].Assets[name].Properties[String.Format("color_1_{0}", y + 1)] = GetAssetName(BitConverter.ToInt64(accentOptionsBuffer, y * 8));
                            }
                        }
                    }
                    // Load Player Body Styles
                    var playerHelmetStyleBuffer = Hydra.ActiveGameReader.ReadBytes(playerBody.HelmetStylesPointer, playerBody.HelmetStyleCount * 168);
                    // Loop through player styles
                    for (int j = 0; j < playerBody.HelmetStyleCount; j++)
                    {
                        // Load Helemt Style
                        var playerHelmetStyle = ByteUtil.BytesToStruct<PlayerBodyStyle>(playerHelmetStyleBuffer, j * 168);
                        // Get Name
                        var name = Hydra.ActiveGameReader.ReadNullTerminatedString(playerHelmetStyle.NamePointer);
                        // Add to player
                        Hydra.GDTs["Character"].Assets[bodyName].Properties[String.Format("helmetStyle{0}", (j + 1).ToString().PadLeft(2, '0'))] = name;
                        // Convert to generic helemt style and add to GDT
                        Hydra.GDTs["Character"].AddAsset(name, "playerhelmetstyle", new PlayerHelmetStyleObj()
                        {
                            // Set Display Name
                            DisplayName = Hydra.ActiveGameReader.ReadNullTerminatedString(playerHelmetStyle.DisplayNamePointer),

                            // Set Icon
                            Icon = GetAssetName(playerHelmetStyle.IconPointer, 0xF8),

                            // Set XModels
                            Model = GetAssetName(playerHelmetStyle.ThirdPersonPointer),

                            // Set Counts
                            AccentColor1Count = 0,
                            AccentColorCount  = playerHelmetStyle.AccentCount,

                            // Set Impact Types
                            ImpactType = ImpactTypes[playerHelmetStyle.ImpactTypeIndex],
                            ImpactTypeCorpse = ImpactTypes[playerHelmetStyle.ImpactTypeCorpseIndex],
                        });
                        // Check for accents
                        if (playerHelmetStyle.AccentCount > 0 && playerHelmetStyle.AccentsPointer > 0)
                        {
                            // Load Accents
                            var accentsBuffer = Hydra.ActiveGameReader.ReadBytes(playerHelmetStyle.AccentsPointer, playerHelmetStyle.AccentCount * 16);
                            // Loop through them
                            for (int x = 0; x < playerHelmetStyle.AccentCount && x < 6; x++)
                            {
                                // Load Accent
                                var accent = ByteUtil.BytesToStruct<PlayerAccent>(accentsBuffer, x * 16);
                                // Load Buffer
                                var accentOptionsBuffer = Hydra.ActiveGameReader.ReadBytes(accent.DataPointer, accent.Count * 8);
                                // Set it
                                Hydra.GDTs["Character"].Assets[name].Properties["accentColor1OptionsCount"] = accent.Count.ToString();
                                // Loop through them
                                for (int y = 0; y < accent.Count && y < 6; y++)
                                    Hydra.GDTs["Character"].Assets[name].Properties[String.Format("color_1_{0}", y + 1)] = GetAssetName(BitConverter.ToInt64(accentOptionsBuffer, y * 8));
                            }
                        }
                    }
                    // Check for data blocks
                    if (playerBody.DataCount > 0 && playerBody.DataPointer > 0)
                    {
                        // Load Accents
                        var dataBuffer = Hydra.ActiveGameReader.ReadBytes(playerBody.DataPointer, playerBody.DataCount * 24);
                        // Loop through them
                        for(int j = 0; j < playerBody.DataCount; j++)
                        {
                            // Load Data
                            var data = ByteUtil.BytesToStruct<CharacterDataBlock>(dataBuffer, j * 24);
                            // Determine type if we're int, since bool and int share same, but the pointer value will be different? (only 2 are bool anyway but just in case)
                            var dataType = data.DataType;
                            // Check for int
                            if (dataType == DataTypes.Int && (data.DataPointer & 0xFFFFFFFF) != data.Data)
                                dataType = DataTypes.Bool;
                            // Build GDT Propert Name from Type and Name
                            string propertyName = String.Format("{0}_{1}", dataType.ToString().ToLower(), GetString(data.DataNameStringIndex));
                            // Convert
                            Hydra.GDTs["Character"].Assets[bodyName].Properties[propertyName] = data.DataType == DataTypes.Int ? data.Data.ToString() :GetString(data.DataStringIndex);
                        }
                    }
                }
                // Done
                return true;
            }
        }
    }
}
