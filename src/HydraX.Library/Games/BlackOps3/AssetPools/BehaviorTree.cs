using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HydraX.Library.AssetContainers;

namespace HydraX.Library
{
    public partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Behavior Tree Logic
        /// </summary>
        private class BehaviorTree : IAssetPool
        {
            #region AssetStructures
            /// <summary>
            /// Behavior Tree Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x28)]
            private struct BehaviorTreeAsset
            {
                #region BehaviorTreeProperties
                public long NamePointer;
                public long NodesPointer;
                public int NodeCount;
                public long UnknownIndicesPointer;
                public long UnknownIndicesCount;
                #endregion
            }

            /// <summary>
            /// Behavior Tree Node Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x48)]
            private struct Node
            {
                #region BehaviorTreeNodeProperties
                public int NameStringIndex { get; set; }
                public int Type { get; set; }
                public int Index { get; set; }
                public int ParentIndex { get; set; }
                public long ChildIndicesPointer { get; set; }
                public int ChildCount { get; set; }
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
                public int[] StringIndices;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] Data;
                #endregion
            }
            #endregion

            #region Tables
            /// <summary>
            /// Behavior Types
            /// </summary>
            private static readonly string[] BehaviorTypes =
            {
                "action",
                "condition_blackboard",
                "condition_script",
                "condition_script_negate",
                "condition_service_script",
                "decorator_random",
                "decorator_script",
                "decorator_timer",
                "parallel",
                "sequence",
                "selector",
                "probability_selector",
                "behavior_state_machine",
                "link_node",
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
            public string Name => "behaviortree";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.behaviortree;

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
                    var header = instance.Reader.ReadStruct<BehaviorTreeAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("Nodes: {0}", header.NodeCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<BehaviorTreeAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                BehaviorObj rootBehaviorObj = null;

                var behaviorObjs = new BehaviorObj[header.NodeCount];

                var nodes = instance.Reader.ReadArray<Node>(header.NodesPointer, header.NodeCount);

                for(int i = 0; i < nodes.Length; i++)
                {
                    behaviorObjs[i] = new BehaviorObj
                    {
                        Name = instance.Game.GetString(nodes[i].NameStringIndex, instance),
                        Type = BehaviorTypes[nodes[i].Type],
                        ChildIndices = instance.Reader.ReadArray<int>(nodes[i].ChildIndicesPointer, nodes[i].ChildCount),
                        Children = nodes[i].ChildCount > 0 ? new BehaviorObj[nodes[i].ChildCount] : null
                    };

                    switch(behaviorObjs[i].Type)
                    {
                        case "action":
                        case "behavior_state_machine":
                            behaviorObjs[i].ASMStateName      = instance.Game.GetString(nodes[i].StringIndices[3], instance);
                            behaviorObjs[i].ActionName        = instance.Game.GetString(nodes[i].StringIndices[4], instance);
                            behaviorObjs[i].ActionNotify      = instance.Game.GetString(nodes[i].StringIndices[5], instance);
                            behaviorObjs[i].StartFunction     = instance.Game.GetString(nodes[i].StringIndices[6], instance);
                            behaviorObjs[i].UpdateFunction    = instance.Game.GetString(nodes[i].StringIndices[7], instance);
                            behaviorObjs[i].TerminateFunction = instance.Game.GetString(nodes[i].StringIndices[8], instance);
                            behaviorObjs[i].LoopingAction     = BitConverter.ToInt32(nodes[i].Data, 0);
                            behaviorObjs[i].ActionTimeMax     = BitConverter.ToInt32(nodes[i].Data, 4);
                            break;
                        case "condition_script":
                        case "condition_blackboard":
                        case "condition_script_negate":
                        case "condition_service_script":
                            behaviorObjs[i].ScriptFunction = instance.Game.GetString(nodes[i].StringIndices[6], instance);
                            behaviorObjs[i].InterruptName  = instance.Game.GetString(nodes[i].StringIndices[7], instance);
                            behaviorObjs[i].CoolDownMin    = BitConverter.ToInt32(nodes[i].Data, 0);
                            behaviorObjs[i].CooldDownMax   = BitConverter.ToInt32(nodes[i].Data, 4);
                            break;
                        case "probability_selector":
                        case "decorator_random":
                            behaviorObjs[i].PercentChance = BitConverter.ToSingle(nodes[i].Data, 0);
                            break;
                        default:
                            break;
                    }

                    if (nodes[i].ParentIndex == -1)
                        rootBehaviorObj = behaviorObjs[i];
                }

                foreach (var behaviorObj in behaviorObjs)
                    for (int i = 0; i < behaviorObj.ChildIndices.Length; i++)
                        behaviorObj.Children[i] = behaviorObjs[behaviorObj.ChildIndices[i]];

                rootBehaviorObj.Save(Path.Combine(instance.BehaviorFolder, asset.Name));

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
