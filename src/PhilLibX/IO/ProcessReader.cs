// ------------------------------------------------------------------------
// PhilLibX - My Utility Library
// Copyright(c) 2018 Philip/Scobalula
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ------------------------------------------------------------------------
// File: IO/ProcessReader.cs
// Author: Philip/Scobalula
// Description: A class to help with reading the memory of other processes.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PhilLibX.IO
{
    /// <summary>
    /// A class to help with reading the memory of other processes.
    /// </summary>
    public class ProcessReader
    {
        /// <summary>
        /// A structure to hold information about a module from a Process IO
        /// </summary>
        public struct Module
        {
            /// <summary>
            /// Gets the File Path of the Module
            /// </summary>
            public string FilePath { get; private set; }

            /// <summary>
            /// Gets the File Name of the Module
            /// </summary>
            public string FileName { get => Path.GetFileName(FilePath); }

            /// <summary>
            /// Gets the Directory of the Module
            /// </summary>
            public string FileDirectory { get => Path.GetDirectoryName(FilePath); }

            /// <summary>
            /// Gets the Base Address of the Module
            /// </summary>
            public IntPtr BaseAddress { get; private set; }

            /// <summary>
            /// Gets the address of the entry point for the module
            /// </summary>
            public IntPtr EntryPointAddress { get; private set; }

            /// <summary>
            /// Gets the size of the module. This size only includes the static module and data.
            /// </summary>
            public int Size { get; private set; }

            /// <summary>
            /// Creates a new Module structure holds information about a module from a Process IO
            /// </summary>
            /// <param name="path">Path of the Module</param>
            /// <param name="baseAddress">File Name of the Module</param>
            /// <param name="size">Size of the module. This size should only include the static module and data.</param>
            /// <param name="entryPoint">Entry point for the module</param>
            public Module(string path, IntPtr baseAddress, IntPtr entryPoint, int size)
            {
                FilePath = path;
                BaseAddress = baseAddress;
                EntryPointAddress = entryPoint;
                Size = size;
            }
        }

        /// <summary>
        /// Internal Process Property
        /// </summary>
        private Process _Process { get; set; }

        /// <summary>
        /// Internal Handle Property
        /// </summary>
        private IntPtr _Handle { get; set; }

        /// <summary>
        /// Active Process
        /// </summary>
        public Process ActiveProcess
        {
            get { return _Process; }
            set
            {
                _Process = value;
                _Handle = _Process.Handle;
            }
        }

        /// <summary>
        /// Active Process Handle
        /// </summary>
        public IntPtr Handle { get { return _Handle; } }

        /// <summary>
        /// Initalizes a Process Reader with a Process
        /// </summary>
        public ProcessReader(Process process)
        {
            ActiveProcess = process;
        }


        public Module GetModule(string moduleName)
        {
            return GetModule(moduleName.ToLower(), Modules);
        }

        public static Module GetModule(string moduleName, IEnumerable<Module> modules)
        {
            foreach(var module in modules)
            {
                if(module.FileName.ToLower() == moduleName)
                {
                    return module;
                }
            }

            return new Module(null, IntPtr.Zero, IntPtr.Zero, -1);
        }

        /// <summary>
        /// Reads bytes from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <param name="numBytes">The number of bytes to be read.</param>
        /// <returns>Bytes read</returns>
        public byte[] ReadBytes(long address, int numBytes)
        {
            return MemoryUtil.ReadBytes(Handle, address, numBytes);
        }

        /// <summary>
        /// Reads 64Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public long ReadInt64(long address)
        {
            return MemoryUtil.ReadInt64(Handle, address);
        }

        /// <summary>
        /// Reads an unsigned 64Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public ulong ReadUInt64(long address)
        {
            return MemoryUtil.ReadUInt64(Handle, address);
        }

        /// <summary>
        /// Reads 32Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public int ReadInt32(long address)
        {
            return MemoryUtil.ReadInt32(Handle, address);
        }

        /// <summary>
        /// Reads 32Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public uint ReadUInt32(long address)
        {
            return MemoryUtil.ReadUInt32(Handle, address);
        }

        /// <summary>
        /// Reads a 16Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public short ReadInt16(long address)
        {
            return MemoryUtil.ReadInt16(Handle, address);
        }

        /// <summary>
        /// Reads an unsigned 16Bit Integer from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public ushort ReadUInt16(long address)
        {
            return MemoryUtil.ReadUInt16(Handle, address);
        }

        /// <summary>
        /// Reads a 4 byte single precision floating point number from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public float ReadSingle(long address)
        {
            return MemoryUtil.ReadSingle(Handle, address);
        }

        /// <summary>
        /// Reads an 8 byte double precision floating point number from the Processes Memory
        /// </summary>
        /// <param name="address">The address of the data to be read.</param>
        /// <returns>Resulting Data</returns>
        public double ReadDouble(long address)
        {
            return MemoryUtil.ReadDouble(Handle, address);
        }

        /// <summary>
        /// Reads a string from the processes' memory terminated by a null byte.
        /// </summary>
        /// <param name="address">Memory Address</param>
        /// <param name="bufferSize">Buffer Read Size</param>
        /// <returns>Resulting String</returns>
        public string ReadNullTerminatedString(long address, int bufferSize = 0xFF)
        {
            return MemoryUtil.ReadNullTerminatedString(Handle, address, bufferSize);
        }

        /// <summary>
        /// Reads an array of the given type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="address">Memory Address</param>
        /// <param name="count">Number of items</param>
        /// <returns>Resulting array</returns>
        public T[] ReadArray<T>(long address, int count)
        {
            // Get Byte Count
            var structSize = Marshal.SizeOf<T>();
            var size = count * structSize;
            // Allocate Array
            var result = new T[count];
            // Check for primitives, we can use BlockCopy for them
            if (typeof(T).IsPrimitive)
            {
                // Copy
                Buffer.BlockCopy(ReadBytes(address, size), 0, result, 0, size);
            }
            // Slightly more complex structures, we can use the struct functs
            else
            {
                // Loop through
                for (int i = 0; i < count; i++)
                {
                    // Read it into result
                    result[i] = ReadStruct<T>(address + (i * structSize));
                }
            }
            // Done
            return result;
        }

        /// <summary>
        /// Reads an array of the given type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="address">Memory Address</param>
        /// <param name="count">Number of items</param>
        /// <returns>Resulting array</returns>
        public unsafe T[] ReadArrayUnsafe<T>(long address, int count) where T : unmanaged
        {
            var buffer = ReadBytes(address, count * sizeof(T));
            var result = new T[count];

            fixed(byte* a = buffer)
            fixed(T*    b = result)
                Buffer.MemoryCopy(a, b, buffer.Length, buffer.Length);

            return result;
        }

        /// <summary>
        /// Reads a struct from the Processes Memory
        /// </summary>
        /// <typeparam name="T">Struct Type</typeparam>
        /// <param name="address">Memory Address</param>
        /// <returns>Resulting Struct</returns>
        public T ReadStruct<T>(long address)
        {
            return MemoryUtil.ReadStruct<T>(Handle, address);
        }

        /// <summary>
        /// Reads a struct from the Processes Memory
        /// </summary>
        /// <typeparam name="T">Struct Type</typeparam>
        /// <param name="address">Memory Address</param>
        /// <returns>Resulting Struct</returns>
        public unsafe T ReadStructUnsafe<T>(long address) where T : unmanaged
        {
            var buffer = ReadBytes(address, sizeof(T));


            fixed (byte* a = buffer)
                return *(T*)a;
        }

        /// <summary>
        /// Searches for bytes in the Processes Memory
        /// </summary>
        /// <param name="needle">Byte Sequence to scan for.</param>
        /// <param name="startAddress">Address to start the search at.</param>
        /// <param name="endAddress">Address to end the search at.</param>
        /// <param name="firstMatch">If we should stop the search at the first result.</param>
        /// <param name="bufferSize">Byte Buffer Size</param>
        /// <returns>Results</returns>
        public long[] FindBytes(byte?[] needle, long startAddress, long endAddress, bool firstMatch = false, int bufferSize = 0xFFFF)
        {
            return MemoryUtil.FindBytes(Handle, needle, startAddress, endAddress, firstMatch, bufferSize);
        }

        public Module[] Modules
        {
            get
            {
                var ntSizeof = Marshal.SizeOf<NativeMethods.NtModuleInfo>();
                var modules = new IntPtr[1024];
                var results = new List<Module>(1024);

                if (NativeMethods.EnumProcessModulesEx(ActiveProcess.Handle, modules, IntPtr.Size * modules.Length, out var size, ListModules.All) == true)
                {
                    var count = size / IntPtr.Size;

                    for (int i = 0; i < count; i++)
                    {
                        StringBuilder strbld = new StringBuilder(1024);

                        NativeMethods.GetModuleFileNameEx(ActiveProcess.Handle, modules[i], strbld, strbld.Capacity);
                        NativeMethods.GetModuleInformation(ActiveProcess.Handle, modules[i], out var info, ntSizeof);

                        results.Add(new Module(strbld.ToString(), info.BaseOfDll, info.EntryPoint, info.SizeOfImage));
                    }
                }

                return results.ToArray();
            }
        }

        /// <summary>
        /// Gets the Active Processes' Base Address
        /// </summary>
        /// <returns>Base Address of the Active Process</returns>
        public long GetBaseAddress()
        {
            return (long)ActiveProcess?.MainModule.BaseAddress;
        }

        /// <summary>
        /// Gets the size of the Main Module Size
        /// </summary>
        /// <returns>Main Module Size</returns>
        public long GetModuleMemorySize()
        {
            return (long)ActiveProcess?.MainModule.ModuleMemorySize;
        }
    }
}
