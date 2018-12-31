using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraLib
{
    using PoolLoadFunction = Action<AssetPool>;
    /// <summary>
    /// Holds Asset Pool Information
    /// </summary>
    public class AssetPool
    {
        /// <summary>
        /// Pool Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Asset Pool Index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// First Entry Address
        /// </summary>
        public long FirstEntry { get; set; }

        /// <summary>
        /// Size of each asset in this pool
        /// </summary>
        public int HeaderSize { get; set; }

        /// <summary>
        /// Number of Assets in Pool
        /// </summary>
        public int AssetCount { get; set; }

        /// <summary>
        /// Number of Assets in Pool
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Asset Pool Load Function
        /// </summary>
        public PoolLoadFunction LoadFunction { get; set; }

        /// <summary>
        /// End Address
        /// </summary>
        public long EndAddress { get { return FirstEntry + (HeaderSize * Size); } }

        /// <summary>
        /// Creates a new Asset Pool
        /// </summary>
        public AssetPool() { }

        /// <summary>
        /// Creates a new Asset Pool with the required information.
        /// </summary>
        /// <param name="name"></param>
        public AssetPool(string name, int index, PoolLoadFunction poolLoadFunction)
        {
            Name = name;
            Index = index;
            LoadFunction = poolLoadFunction;
        }

        /// <summary>
        /// Creates a new Asset Pool with the required information.
        /// </summary>
        /// <param name="name"></param>
        public AssetPool(string name, int entrySize, int index, PoolLoadFunction poolLoadFunction)
        {
            Name = name;
            HeaderSize = entrySize;
            Index = index;
            LoadFunction = poolLoadFunction;
        }

        /// <summary>
        /// Creates a new Asset Pool with the required information.
        /// </summary>
        /// <param name="name"></param>
        public AssetPool(string name, int entrySize, int index, int maxAssetCount, PoolLoadFunction poolLoadFunction)
        {
            Name = name;
            HeaderSize = entrySize;
            Index = index;
            Size = maxAssetCount;
            LoadFunction = poolLoadFunction;
        }

        /// <summary>
        /// Checks if an asset is null by checking if the Name pointer points to another asset within the pool (If it's null it may point to the next value asset)
        /// </summary>
        /// <param name="nameAddress">Asset</param>
        /// <returns>True if the asset is considered "null"</returns>
        public bool IsNullAsset(Asset asset)
        {
            return IsNullAsset(asset.NameLocation);
        }

        /// <summary>
        /// Checks if an asset is null by checking if the Name pointer points to another asset within the pool (If it's null it may point to the next value asset)
        /// </summary>
        /// <param name="nameAddress">Memory Address of the asset's Name</param>
        /// <returns>True if the asset is considered "null"</returns>
        public bool IsNullAsset(long nameAddress)
        {
            return nameAddress >= FirstEntry && nameAddress <= EndAddress || nameAddress == 0;
        }
    }
}
