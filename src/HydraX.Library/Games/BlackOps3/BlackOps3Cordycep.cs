using PhilLibX.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HydraX.Library
{
    /// <summary>
    /// Black Ops 3 support for Cordycep, allowing assets to be loaded without the game running.
    ///
    /// Cordycep stores assets in per-type linked lists, while HydraX's asset pools expect the
    /// game's contiguous pool arrays. To avoid changing every asset pool, a "snapshot" of the
    /// linked lists is written into Cordycep's memory as contiguous arrays along with a pool
    /// info table matching the in-game layout, and the pools are pointed at that instead.
    /// Pointers within the copied headers still target Cordycep's memory, so all reads work.
    /// </summary>
    public partial class BlackOps3
    {
        /// <summary>
        /// Cordycep's Process Name
        /// </summary>
        public const string CordycepProcessName = "cordycep.cli";

        /// <summary>
        /// Black Ops 3's Game ID within Cordycep's state file
        /// </summary>
        public const string CordycepGameID = "BLKOPS03";

        /// <summary>
        /// Gets or Sets whether we are reading from Cordycep instead of the game
        /// </summary>
        public bool IsCordycep { get; set; }

        /// <summary>
        /// Gets or Sets the address of Cordycep's String Pool (strings are stored by offset)
        /// </summary>
        public long CordycepStringsAddress { get; set; }

        #region Structures
        /// <summary>
        /// Cordycep's XAsset Pool (ps::XAssetPool)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct CordycepXAssetPool
        {
            public long Root;
            public long End;
            public long LookupTable;
            public long HeaderMemory;
            public long AssetMemory;
        }

        /// <summary>
        /// Cordycep's XAsset Entry (ps::XAsset)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct CordycepXAsset
        {
            public long Header;
            public long Temp;
            public long Next;
            public long Previous;
            public long ID;
            public long Type;
            public long HeaderSize;
            public long ExtendedDataPtrOffset;
            public long ExtendedDataSize;
            public long FirstChild;
            public long LastChild;
            public long Owner;
        }
        #endregion

        #region NativeMethods
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, int dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        private const int MEM_COMMIT     = 0x1000;
        private const int MEM_RESERVE    = 0x2000;
        private const int MEM_RELEASE    = 0x8000;
        private const int PAGE_READWRITE = 0x04;
        #endregion

        /// <summary>
        /// The snapshot allocated within Cordycep by the previous load, so it can be freed on reload
        /// </summary>
        private static int LastSnapshotProcessID;
        private static long LastSnapshotAddress;

        /// <summary>
        /// Fast File names by ps::FastFile pointer
        /// </summary>
        private readonly Dictionary<long, string> FastFileNames = new Dictionary<long, string>();

        /// <summary>
        /// Builds a snapshot of Cordycep's asset lists as in-game style pools within Cordycep's memory
        /// </summary>
        public bool InitializeCordycep(HydraInstance instance)
        {
            ZoneNames = new Dictionary<long, string>();
            FastFileNames.Clear();

            var reader = instance.Reader;
            var processHandle = reader.Handle;
            var processID = reader.ActiveProcess.Id;

            // Free the snapshot from a previous load of the same Cordycep instance
            if (LastSnapshotAddress != 0 && LastSnapshotProcessID == processID)
                VirtualFreeEx(processHandle, (IntPtr)LastSnapshotAddress, IntPtr.Zero, MEM_RELEASE);
            LastSnapshotAddress = 0;

            var poolCount = Enum.GetValues(typeof(AssetPool)).Length;
            var pools = reader.ReadArrayUnsafe<CordycepXAssetPool>(AssetPoolsAddress, poolCount);

            // We only need the pools HydraX can actually load
            var supportedPools = new HashSet<int>(HydraInstance.GetAssetPools(this).Select(x => x.Index));

            // Walk the linked list of each supported pool
            var poolAssets = new List<CordycepXAsset>[poolCount];
            long totalSize = poolCount * 0x20;

            for (int i = 0; i < poolCount; i++)
            {
                var assets = new List<CordycepXAsset>();
                poolAssets[i] = assets;

                if (!supportedPools.Contains(i))
                    continue;

                for (var next = pools[i].Root; next != 0;)
                {
                    var entry = reader.ReadStructUnsafe<CordycepXAsset>(next);
                    next = entry.Next;

                    // Skip the list's root slot and temp assets (name-only placeholders)
                    if (entry.Header == 0 || entry.Temp != 0)
                        continue;

                    assets.Add(entry);
                }

                if (assets.Count > 0)
                    totalSize += assets.Count * assets[0].HeaderSize;
            }

            if (totalSize > int.MaxValue)
                throw new Exception("The Cordycep asset snapshot exceeds the maximum supported size.");

            var snapshot = (long)VirtualAllocEx(processHandle, IntPtr.Zero, (IntPtr)totalSize, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);

            if (snapshot == 0)
                throw new Exception("Failed to allocate memory within Cordycep for the asset snapshot.");

            LastSnapshotProcessID = processID;
            LastSnapshotAddress = snapshot;

            var buffer = new byte[totalSize];
            long headerOffset = poolCount * 0x20;

            using (var infoWriter = new BinaryWriter(new MemoryStream(buffer)))
            {
                for (int i = 0; i < poolCount; i++)
                {
                    var assets = poolAssets[i];
                    var assetSize = assets.Count > 0 ? (int)assets[0].HeaderSize : 0;

                    // Matches the in-game AssetPoolInfo layout
                    infoWriter.Write(snapshot + headerOffset); // PoolPointer
                    infoWriter.Write(assetSize);               // AssetSize
                    infoWriter.Write(assets.Count);            // PoolSize
                    infoWriter.Write(0);                       // Padding
                    infoWriter.Write(assets.Count);            // AssetCount
                    infoWriter.Write(0L);                      // FreeSlot

                    foreach (var asset in assets)
                    {
                        Array.Copy(reader.ReadBytes(asset.Header, assetSize), 0, buffer, headerOffset, assetSize);
                        ZoneNames[snapshot + headerOffset] = GetFastFileName(reader, asset.Owner);
                        headerOffset += assetSize;
                    }
                }
            }

            if (!WriteProcessMemory(processHandle, (IntPtr)snapshot, buffer, (IntPtr)buffer.Length, out _))
                throw new Exception("Failed to write the asset snapshot to Cordycep.");

            AssetPoolsAddress = snapshot;

            return true;
        }

        /// <summary>
        /// Gets the name of the given Fast File (ps::FastFile)
        /// </summary>
        private string GetFastFileName(ProcessReader reader, long fastFileAddress)
        {
            if (fastFileAddress == 0)
                return "unknown";

            if (FastFileNames.TryGetValue(fastFileAddress, out var name))
                return name;

            // The name is an MSVC std::string at offset 0x8
            name = ReadMSVCString(reader, fastFileAddress + 0x8);

            if (string.IsNullOrWhiteSpace(name))
                name = "unknown";

            FastFileNames[fastFileAddress] = name;

            return name;
        }

        /// <summary>
        /// Reads an MSVC std::string (16 byte small string buffer/pointer, size, capacity)
        /// </summary>
        private static string ReadMSVCString(ProcessReader reader, long address)
        {
            var size     = reader.ReadInt64(address + 0x10);
            var capacity = reader.ReadInt64(address + 0x18);

            if (size <= 0 || size > 256 || capacity < size)
                return "";

            var dataAddress = capacity >= 16 ? reader.ReadInt64(address) : address;

            return Encoding.ASCII.GetString(reader.ReadBytes(dataAddress, (int)size));
        }
    }
}
