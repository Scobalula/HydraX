using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraX.Library
{
    public interface IAssetPool
    {
        /// <summary>
        /// Gets the Asset Pool Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Setting Group for this Asset Pool
        /// </summary>
        string SettingGroup { get; }

        /// <summary>
        /// Gets the Asset Pool Index
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the Asset Header Size
        /// </summary>
        int AssetSize { get; set; }

        /// <summary>
        /// Gets or Sets the number of Asset slots in this pool
        /// </summary>
        int AssetCount { get; set; }

        /// <summary>
        /// Gets or Sets the start Address of this pool
        /// </summary>
        long StartAddress { get; set; }

        /// <summary>
        /// Gets or Sets the end Address of this pool
        /// </summary>
        long EndAddress { get; set; }

        /// <summary>
        /// Loads Assets from the given Asset Pool
        /// </summary>
        List<GameAsset> Load(HydraInstance instance);

        /// <summary>
        /// Exports the given asset from the game
        /// </summary>
        HydraStatus Export(GameAsset asset, HydraInstance instance);

        /// <summary>
        /// Checks if the given asset is null
        /// </summary>
        bool IsNullAsset(GameAsset asset);

        /// <summary>
        /// Checks if the given pointer points to a null slot
        /// </summary>
        bool IsNullAsset(long nameAddress);
    }
}
