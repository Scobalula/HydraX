using System.Collections.Generic;

namespace HydraX.Library
{
    public interface IGame
    {
        /// <summary>
        /// Gets or Sets the Game's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Game's Process Names
        /// </summary>
        string[] ProcessNames { get; }

        /// <summary>
        /// Gets the Memory Addresses of the Asset Pools
        /// </summary>
        long AssetPoolsAddress { get; }

        /// <summary>
        /// Gets the Memory Addresses of the Asset Sizes
        /// </summary>
        long AssetSizesAddress { get; }

        /// <summary>
        /// Gets the Memory Addresses of the String Pool
        /// </summary>
        long StringPoolAddress { get; }

        /// <summary>
        /// Gets or Sets the Base Address
        /// </summary>
        long BaseAddress { get; set; }

        /// <summary>
        /// Gets or Sets the Process Index (Matches the Address + Process Name Array)
        /// </summary>
        int ProcessIndex { get; set; }

        /// <summary>
        /// Gets or Sets the List of Asset Pools
        /// </summary>
        List<IAssetPool> AssetPools { get; set; }

        /// <summary>
        /// Gets or Sets the Zone Names for each asset
        /// </summary>
        Dictionary<long, string> ZoneNames { get; set; }

        /// <summary>
        /// Validates the games addresses
        /// </summary>
        /// <returns>True if the addresses are valid, otherwise false</returns>
        bool Initialize(HydraInstance instance);

        /// <summary>
        /// Gets a string from the string table in the game's memory
        /// </summary>
        string GetString(long index, HydraInstance instance);

        /// <summary>
        /// Gets the asset name at the given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="offset"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        string GetAssetName(long address, HydraInstance instance, long offset = 0);

        /// <summary>
        /// Cleans the Asset Name to match what the tools expect
        /// </summary>
        /// <param name="type">Asset Type</param>
        /// <param name="assetName">Asset Name</param>
        /// <returns>Cleaned Asset Name</returns>
        string CleanAssetName(HydraAssetType type, string assetName);

        /// <summary>
        /// Creates a shallow copy of the Game Object
        /// </summary>
        /// <returns>Copied Game Object</returns>
        object Clone();
    }
}
