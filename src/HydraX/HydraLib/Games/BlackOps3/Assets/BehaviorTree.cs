using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PhilLibX;
using HydraLib.Assets;

namespace HydraLib.Games
{
    public partial class BlackOps3
    {
        public class BehaviorTree
        {
            /// <summary>
            /// Behavior Types
            /// </summary>
            public static string[] BehaviorTypes =
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

            /// <summary>
            /// Bo3 AI ASM Header
            /// </summary>
            public struct BehaviorTreeHeader
            {
                /// <summary>
                /// Pointer to the name of this raw file
                /// </summary>
                public long NamePointer { get; set; }

                /// <summary>
                /// Pointer to the Behaviors
                /// </summary>
                public long BehaviorsPointer { get; set; }

                /// <summary>
                /// Number of Behaviors
                /// </summary>
                public int BehaviorCount { get; set; }

                /// <summary>
                /// Null Padding
                /// </summary>
                private int Padding { get; set; }

                /// <summary>
                /// Possibly to identify ones required by other assets, etc. not needed by us
                /// </summary>
                public long UnknownIndicesPointer { get; set; }

                /// <summary>
                /// Possibly to identify ones required by other assets, etc. not needed by us
                /// </summary>
                public int UnknownIndicesCount { get; set; }
            }

            /// <summary>
            /// Bo3 Behavior
            /// </summary>
            public struct Behavior
            {
                /// <summary>
                /// Index of the Name of this Behavior in the String Database
                /// </summary>
                public int NameStringIndex { get; set; }

                /// <summary>
                /// Behavior Type Index
                /// </summary>
                public int TypeIndex { get; set; }

                /// <summary>
                /// Behavior Index
                /// </summary>
                public int Index { get; set; }

                /// <summary>
                /// Behavior Parent Index (-1 for root)
                /// </summary>
                public int ParentIndex { get; set; }

                /// <summary>
                /// Pointer to the Indices of this Behavior's Children
                /// </summary>
                public long ChildIndicesPointer { get; set; }

                /// <summary>
                /// Number of Child Behaviors
                /// </summary>
                public int ChildCount { get; set; }

                /// <summary>
                /// String Indices
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
                public int[] StringIndices;

                /// <summary>
                /// Integer/Float Data (Depending on Type
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public byte[] Data;
            }

            public static void Load(AssetPool assetPool)
            {
                // Loop through entire pool, and add what valid assets
                for (int i = 0; i < assetPool.Size; i++)
                {
                    // Read BehaviorTree
                    var behaviorTree = Hydra.ActiveGameReader.ReadStruct<BehaviorTreeHeader>(assetPool.FirstEntry + (i * assetPool.HeaderSize));
                    // Check is it a null/empty slot
                    if (assetPool.IsNullAsset(behaviorTree.NamePointer))
                        continue;
                    // Add Asset
                    Hydra.LoadedAssets.Add(new Asset()
                    {
                        Name = Hydra.ActiveGameReader.ReadNullTerminatedString(behaviorTree.NamePointer),
                        HeaderAddress = assetPool.FirstEntry + (i * assetPool.HeaderSize),
                        ExportFunction = Export,
                        Type = assetPool.Name,
                        Information = String.Format("Behaviors - {0}", behaviorTree.BehaviorCount),
                    });
                }
            }

            public static bool Export(Asset asset)
            {
                // Read Header
                var behaviorTree = Hydra.ActiveGameReader.ReadStruct<BehaviorTreeHeader>(asset.HeaderAddress);
                // Check name pointer, if it's changed, our asset has changed
                if (asset.Name != Hydra.ActiveGameReader.ReadNullTerminatedString(behaviorTree.NamePointer))
                    return false;

                // Create output path  
                string path = Path.Combine("exported_files", Hydra.ActiveGameName, "behavior", asset.Name);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                // Root Behavior
                BehaviorObj rootBehaviorObj = null;
                // Create Behavior Array
                var behaviorObjs = new BehaviorObj[behaviorTree.BehaviorCount];

                // Load Behavior Buffer
                var behaviorBuffer = Hydra.ActiveGameReader.ReadBytes(behaviorTree.BehaviorsPointer, behaviorTree.BehaviorCount * 72);
                // Loop through them
                for(int i = 0; i < behaviorTree.BehaviorCount; i++)
                {
                    // Load Behavior
                    var behavior = ByteUtil.BytesToStruct<Behavior>(behaviorBuffer, i * 72);

                    // Create new Behavior object
                    var behaviorObj = new BehaviorObj
                    {
                        // Set Base Data
                        Name = GetString(behavior.NameStringIndex),
                        Type = BehaviorTypes[behavior.TypeIndex]
                    };

                    // Check for root
                    if (behavior.ParentIndex == -1)
                        rootBehaviorObj = behaviorObj;

                    // Add data based off type
                    switch(behaviorObj.Type)
                    {
                        // Action Behaviors
                        case "action":
                        case "behavior_state_machine":
                            behaviorObj.ASMStateName      = GetString(behavior.StringIndices[3]);
                            behaviorObj.ActionName        = GetString(behavior.StringIndices[4]);
                            behaviorObj.ActionNotify      = GetString(behavior.StringIndices[5]);
                            behaviorObj.StartFunction     = GetString(behavior.StringIndices[6]);
                            behaviorObj.UpdateFunction    = GetString(behavior.StringIndices[7]);
                            behaviorObj.TerminateFunction = GetString(behavior.StringIndices[8]);
                            behaviorObj.LoopingAction     = BitConverter.ToInt32(behavior.Data, 0);
                            behaviorObj.ActionTimeMax     = BitConverter.ToInt32(behavior.Data, 4);
                            break;
                        // Condition Script Behaviors
                        case "condition_script":
                        case "condition_blackboard":
                        case "condition_script_negate":
                        case "condition_service_script":
                            behaviorObj.ScriptFunction = GetString(behavior.StringIndices[6]);
                            behaviorObj.InterruptName  = GetString(behavior.StringIndices[7]);
                            behaviorObj.CoolDownMin    = BitConverter.ToInt32(behavior.Data, 0);
                            behaviorObj.CooldDownMax   = BitConverter.ToInt32(behavior.Data, 4);
                            break;
                        // Random Behaviors
                        case "probability_selector":
                        case "decorator_random":
                            behaviorObj.PercentChance = BitConverter.ToSingle(behavior.Data, 0);
                            break;
                        default:
                            break;
                    }

                    // Add child indices if we have any
                    if(behavior.ChildCount > 0 && behavior.ChildIndicesPointer > 0)
                    {
                        // Set Arrays
                        behaviorObj.ChildIndices = new int[behavior.ChildCount];
                        behaviorObj.Children = new BehaviorObj[behavior.ChildCount];
                        // Read Indices
                        var indicesBuffer = Hydra.ActiveGameReader.ReadBytes(behavior.ChildIndicesPointer, behavior.ChildCount * 4);
                        // Copy
                        Buffer.BlockCopy(indicesBuffer, 0, behaviorObj.ChildIndices, 0, indicesBuffer.Length);
                    }

                    // Add it
                    behaviorObjs[i] = behaviorObj;
                }

                // Loop through results
                foreach(var behaviorObj in behaviorObjs)
                    // Add children
                    for (int i = 0; i < (behaviorObj.Children?.Length ?? 0); i++)
                        // Add it from index
                        behaviorObj.Children[i] = behaviorObjs[behaviorObj.ChildIndices[i]];

                // Save to JSON file
                rootBehaviorObj.Save(path);

                // Done
                return true;
            }
        }
    }
}
