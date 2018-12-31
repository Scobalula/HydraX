using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;

namespace HydraLib.Games
{
    /// <summary>
    /// Black Ops 3 Logic
    /// </summary>
    public partial class BlackOps3
    {
        /// <summary>
        /// Asset Pool Data
        /// </summary>
        public struct AssetPoolInfo
        {
            /// <summary>
            /// A pointer to the asset pool
            /// </summary>
            public long PoolPointer { get; set; }

            /// <summary>
            /// Entry Size
            /// </summary>
            public int AssetSize { get; set; }

            /// <summary>
            /// Max Asset Count/Pool Size
            /// </summary>
            public int PoolSize { get; set; }

            /// <summary>
            /// Null Padding
            /// </summary>
            public int Padding { get; set; }

            /// <summary>
            /// Numbers of Assets in this Pool
            /// </summary>
            public int AssetCount { get; set; }

            /// <summary>
            /// Next Free Header/Slot
            /// </summary>
            public long NextSlot { get; set; }
        }

        /// <summary>
        /// Game Addresses
        /// </summary>
        private static Dictionary<string, GameDBInfo[]> DBInfos = new Dictionary<string, GameDBInfo[]>()
        {
            // Core (Black Ops 3 only has 1 exe)
            { "Core", new GameDBInfo[]
            // Known Addresses
            {
                new GameDBInfo(0x93FA290, 0x4D4F104)
            }
            }
        };

        /// <summary>
        /// Number of assets pools in Black Ops 3
        /// </summary>
        private const int AssetPoolCount = 107;

        /// <summary>
        /// Supported Assets
        /// </summary>
        private static AssetPool[] AssetPools =
        {
            new AssetPool("zbarrier",               65,     ZBarrier.Load),
            new AssetPool("sound",                  10,     Sound.Load),
            new AssetPool("xcam",                   84,     XCam.Load ),
            new AssetPool("rumble",                 71,     Rumble.Load ),
            new AssetPool("weaponcamo",             33,     WeaponCamo.Load ),
            new AssetPool("physpreset",             0,      PhysicsPreset.Load ),
            new AssetPool("customizationtable",     34,     CustomizationTable.Load ),
            new AssetPool("animmappingtable",       76,     AnimationMappingTable.Load ),
            new AssetPool("animselectortable",      75,     AnimationSelectorTable.Load ),
            new AssetPool("animstatemachine",       77,     AnimationStateMachine.Load ),
            new AssetPool("behaviortree",           78,     BehaviorTree.Load ),
            new AssetPool("ttf",                    80,     RawFile.Load ),
            new AssetPool("rawfile",                47,     RawFile.Load ),
            new AssetPool("scriptparsetree",        54,     RawFile.Load ),
            new AssetPool("stringtable",            48,     StringTable.Load ),
            new AssetPool("structuredtable",        49,     StructuredTable.Load ),
            new AssetPool("localize",               22,     Localize.Load ),
        };

        public static void LoadAssets()
        {
            // Resulting Assets
            List<Asset> assets = new List<Asset>();
            // Create buffer
            byte[] assetDBBuffer = Hydra.ActiveGameReader.ReadBytes(Hydra.ActiveGameReader.GetBaseAddress() + Hydra.ActiveDBInfo.AssetDBAddress, 0x20 * AssetPoolCount);
            // Loop each pool
            foreach (var assetPool in AssetPools)
            {
                // Read Asset Pool
                var assetPoolInfo = ByteUtil.BytesToStruct<AssetPoolInfo>(assetDBBuffer, assetPool.Index * 0x20);
                // Set Data
                assetPool.FirstEntry = assetPoolInfo.PoolPointer;
                assetPool.Size = assetPoolInfo.PoolSize;
                assetPool.HeaderSize = assetPoolInfo.AssetSize;
                // Pass to load function
                assetPool.LoadFunction(assetPool);
            }
        }


        /// <summary>
        /// Gets required addresses and sets them in Hydra.ActiveDBInfo, if not valid, returns false
        /// </summary>
        public static bool GetAddresses(string addressKey)
        {
            // Loop through known addresses
            foreach(var dbInfo in DBInfos[addressKey])
            {
                // Validate XModel Name
                if (Hydra.ActiveGameReader.ReadNullTerminatedString(Hydra.ActiveGameReader.ReadInt64(Hydra.ActiveGameReader.ReadInt64(Hydra.ActiveGameReader.GetBaseAddress() + dbInfo.AssetDBAddress + 0x80))) == "void")
                {
                    // Set it
                    Hydra.ActiveDBInfo = dbInfo;
                    // Done
                    return true;
                }
            }
            // Scan for it
            var assetDBScan = Hydra.ActiveGameReader.FindBytes(
                new byte?[] { 0x63, 0xC1, 0x48, 0x8D, 0x05, null, null, null, null, 0x49, 0xC1, 0xE0, null, 0x4C, 0x03, 0xC0 },
                Hydra.ActiveGameReader.GetBaseAddress(),
                Hydra.ActiveGameReader.GetBaseAddress() + Hydra.ActiveGameReader.GetModuleMemorySize(),
                true);
            var stringDBScan = Hydra.ActiveGameReader.FindBytes(
                new byte?[] { 0x4C, 0x03, 0xF6, 0x33, 0xDB, 0x49, null, null, 0x8B, 0xD3, 0x8D, 0x7B },
                Hydra.ActiveGameReader.GetBaseAddress(),
                Hydra.ActiveGameReader.GetBaseAddress() + Hydra.ActiveGameReader.GetModuleMemorySize(),
                true);
            // Validate results
            if(assetDBScan.Length > 0 && stringDBScan.Length > 0)
            {
                // Read and mark addresses
                var dbInfo = new GameDBInfo(
                    (Hydra.ActiveGameReader.ReadInt32(assetDBScan[0] + 0x5) + (assetDBScan[0] + 0x9)) - Hydra.ActiveGameReader.GetBaseAddress(),
                    (Hydra.ActiveGameReader.ReadInt32(stringDBScan[0] + 0x1D) + (stringDBScan[0] + 0x21)) - Hydra.ActiveGameReader.GetBaseAddress());
                // Validate XModel Name
                if (Hydra.ActiveGameReader.ReadNullTerminatedString(Hydra.ActiveGameReader.ReadInt64(Hydra.ActiveGameReader.ReadInt64(Hydra.ActiveGameReader.GetBaseAddress() + dbInfo.AssetDBAddress + 0x80))) == "void")
                {
                    // Set it
                    Hydra.ActiveDBInfo = dbInfo;
                    // Done
                    return true;
                }
            }
            // Failed
            return false;
        }

        /// <summary>
        /// Loads any asset pool fom Bo3
        /// </summary>
        /// <param name="assetPool"></param>
        public static void LoadPool(AssetPool assetPool)
        {
            for (int i = 0; i < assetPool.Size; i++)
            {
                // Read Name
                var namePointer = Hydra.ActiveGameReader.ReadInt64(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                // Check is it a null/empty slot
                if (assetPool.IsNullAsset(namePointer))
                    continue;
                // Create new asset
                Hydra.LoadedAssets.Add(new Asset()
                {
                    Name = Hydra.ActiveGameReader.ReadNullTerminatedString(namePointer),
                    HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                    ExportFunction = null,
                    Type = assetPool.Name,
                    Information = "N/A"
                });
            }
        }

        /// <summary>
        /// Returns a string from Black Ops III's String Pool
        /// </summary>
        /// <param name="index">Index of the string</param>
        /// <returns>String if in Pool Range, otherwise null.</returns>
        public static string GetString(int index)
        {
            return Hydra.ActiveGameReader.ReadNullTerminatedString(Hydra.ActiveGameReader.GetBaseAddress() + Hydra.ActiveDBInfo.StringDBAddress + (0x1C * index));
        }

        /// <summary>
        /// Gets Asset Name for the given asset pointer and offset in the struct
        /// </summary>
        public static string GetAssetName(long assetAddress, long namePointerOffset = 0)
        {
            // Return asset Name
            return assetAddress == 0 ? "" : Hydra.ActiveGameReader.ReadNullTerminatedString(Hydra.ActiveGameReader.ReadInt64(assetAddress + namePointerOffset));
        }
    }
}
