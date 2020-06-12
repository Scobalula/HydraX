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
        /// Black Ops 3 Behavior State Machine Logic
        /// </summary>
        private class BehaviorStateMachine : IAssetPool
        {
            #region Enums
            /// <summary>
            /// State Flags
            /// </summary>
            private enum StateFlags : int
            {
                WildCard            = 0x1,
                PlannerState        = 0x1,
                EvaluateStateForASM = 0x4,
                TerminalState       = 0x8,
                EntryState          = 0x10,
            }

            /// <summary>
            /// Connection Flags
            /// </summary>
            private enum ConnectionFlags : int
            {
                WaitTillStateFinish      = 0x2,
                EvaluateOnStateFailure   = 0x4,
                EvaluateDuringTransition = 0x8,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Behavior State Machine Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x30)]
            private struct BehaviorStateMachineAsset
            {
                #region BehaviorStateMachineProperties
                public long NamePointer;
                public long StatesPointer;
                public int StateCount;
                public int EntryStatePointer;
                #endregion
            }

            /// <summary>
            /// Behavior State Machine State Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x18)]
            private struct State
            {
                #region BehaviorStateMachineStateProperties
                public int NameIndex;
                public int ASMStateNameIndex;
                public int EntryPointConditionIndex;
                public int StartFunctionPlannerStateIndex; // plannerstate if Flags & PlannerState
                public int UpdateFunctionIndex;
                public int TerminateFunctionIndex;
                public int ActionNotifyIndex;
                public long ConnectionsPointer;
                public int ConnectionCount;
                public StateFlags Flags;
                #endregion
            }

            /// <summary>
            /// Behavior State Machine Connection Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 16)]
            private struct Connection
            {
                #region BehaviorStateMachineStateProperties
                public int ToStateIndex;
                public int ConditionFunctionIndex;
                public ConnectionFlags Flags;
                public int MinRunningStateTime;
                #endregion
            }
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
            public string Name => "behaviorstatemachine";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.behaviorstatemachine;

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
                    var header = instance.Reader.ReadStruct<BehaviorStateMachineAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("States: {0}", header.StateCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<BehaviorStateMachineAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var machineObj = new BehaviorStateMachineObj()
                {
                    States = new BehaviorStateMachineObj.StateObj[header.StateCount]
                };

                var states = instance.Reader.ReadArray<State>(header.StatesPointer, header.StateCount);

                for(int i = 0; i < header.StateCount; i++)
                {
                    machineObj.States[i] = new BehaviorStateMachineObj.StateObj()
                    {
                        Name = instance.Game.GetString(states[i].NameIndex, instance),
                            
                        AnimationStateName  = instance.Game.GetString(states[i].ASMStateNameIndex, instance),
                        EntryPointCondition = instance.Game.GetString(states[i].EntryPointConditionIndex, instance),
                        StartFunction       = !states[i].Flags.HasFlag(StateFlags.PlannerState) ? instance.Game.GetString(states[i].StartFunctionPlannerStateIndex, instance) : "",
                        PlannerFunction     = states[i].Flags.HasFlag(StateFlags.PlannerState) ? instance.Game.GetString(states[i].StartFunctionPlannerStateIndex, instance) : "",
                        UpdateFunction      = instance.Game.GetString(states[i].UpdateFunctionIndex, instance),
                        TerminateFunction   = instance.Game.GetString(states[i].TerminateFunctionIndex, instance),
                        ActionNotify        = instance.Game.GetString(states[i].ActionNotifyIndex, instance),

                        Wildcard            = states[i].Flags.HasFlag(StateFlags.WildCard),
                        EvaluateStateForASM = states[i].Flags.HasFlag(StateFlags.EvaluateStateForASM),
                        PlannerState        = states[i].Flags.HasFlag(StateFlags.PlannerState),
                        TerminalState       = states[i].Flags.HasFlag(StateFlags.TerminalState),
                        EntryState          = states[i].Flags.HasFlag(StateFlags.EntryState),

                        Connections = states[i].ConnectionCount > 0 ? new BehaviorStateMachineObj.ConnectionObj[states[i].ConnectionCount] : null
                    };

                    var connections = instance.Reader.ReadArray<Connection>(states[i].ConnectionsPointer, states[i].ConnectionCount);

                    for(int j = 0; j < connections.Length; j++)
                    {
                        machineObj.States[i].Connections[j] = new BehaviorStateMachineObj.ConnectionObj()
                        {
                            ToState = instance.Game.GetString(states[connections[j].ToStateIndex].NameIndex, instance),

                            WaitTillStateFinish = connections[j].Flags.HasFlag(ConnectionFlags.WaitTillStateFinish),
                            EvaluateDuringTransition = connections[j].Flags.HasFlag(ConnectionFlags.EvaluateDuringTransition),
                            EvaluateOnStateFailure = connections[j].Flags.HasFlag(ConnectionFlags.EvaluateOnStateFailure),

                            MinRunningStateTime = connections[j].MinRunningStateTime,
                        };
                    }
                }

                machineObj.Save(Path.Combine(instance.BehaviorFolder, asset.Name));

                // Done
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
