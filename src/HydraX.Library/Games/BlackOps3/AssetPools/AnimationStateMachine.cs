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
        /// Black Ops 3 Animation State Machine Logic
        /// </summary>
        private class AnimationStateMachine : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Sub State Flags
            /// </summary>
            private enum Flags : int
            {
                Terminal            = 0x1,
                LoopSync            = 0x2,
                MultipleDelta       = 0x4,
                Parametric2D        = 0x8,
                Coderate            = 0x10,
                AllowTransDecAim    = 0x20,
                ForceFire           = 0x40,
                CleanLoop           = 0x80,
                AnimDrivenLocmotion = 0x100,
                SpeedBlend          = 0x200,
            }

            /// <summary>
            /// Sub State Requirements
            /// </summary>
            private enum Requirements : int
            {
                DeltaRequiresTranslation = 0x1,
                RequiresRagdollNotetrack = 0x2,
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Animation State Machine Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x38)]
            private struct AnimationStateMachineAsset
            {
                #region AnimationStateMachineProperties
                public long NamePointer;
                public long RootStatesPointer;
                public int RootStateCount;
                public long SubStatesPointer;
                public int SubStateCount;
                public long TransitionsPointer;
                public int TransitionCount;
                #endregion
            }

            /// <summary>
            /// Animation State Machine Root State Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0x18)]
            private struct RootState
            {
                #region AnimationStateMachineRootStateProperties
                public int NameStringIndex;
                public long SubStateIndicesPointer;
                public int SubStateCount;
                #endregion
            }

            /// <summary>
            /// Animation State Machine Transition Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct Transition
            {
                #region AnimationStateMachineTransitionProperties
                public int NameStringIndex;
                public Requirements Requirements;
                public Flags Flags;
                public int PathStringIndex;
                public int Unk1;
                public int Unk2;
                public int Unk3;
                public int AnimationSelectorStringIndex;
                public int AimSelectorStringIndex;
                public int ShootSelectorStringIndex;
                public int DeltaLayerFunctionStringIndex;
                public int ASMClientNofityStringIndex;
                #endregion
            }

            /// <summary>
            /// Animation State Machine Sub State Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x40)]
            private struct SubState
            {
                #region AnimationStateMachineSubStateProperties
                public int NameStringIndex;
                public int PathStringIndex;
                public Requirements Requirements;
                public Flags Flags;
                public int ParentMainStateIndex;
                public int AnimationSelectorStringIndex;
                public int AimSelectorStringIndex;
                public int ShootSelectorStringIndex;
                public int TransitionDecoratorStringIndex;
                public int DeltaLayerFunctionStringIndex;
                public int TransDecLayerFunctionStringIndex;
                public int ASMClientNofityStringIndex;
                public long TransitionIndicesPointer;
                public int TransitionCount;
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
            public string Name => "animstatemachine";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "AI";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.animstatemachine;

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
                    var header = instance.Reader.ReadStruct<AnimationStateMachineAsset>(StartAddress + (i * AssetSize));

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
                        Information = string.Format("States: {0}", header.SubStateCount + header.RootStateCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<AnimationStateMachineAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expect memory address has changed. Press the Load Game button to refresh the asset list.");

                var transitionObjects = new AnimationStateMachineObj.StateObj[header.TransitionCount];
                var transitions = instance.Reader.ReadArray<Transition>(header.TransitionsPointer, header.TransitionCount);

                for(int i = 0; i < header.TransitionCount; i++)
                {
                    transitionObjects[i] = new AnimationStateMachineObj.StateObj()
                    {
                        Name = instance.Game.GetString(transitions[i].NameStringIndex, instance),

                        AnimationSelector     = instance.Game.GetString(transitions[i].AnimationSelectorStringIndex, instance),
                        AimSelector           = instance.Game.GetString(transitions[i].AimSelectorStringIndex, instance),
                        ShootSelector         = instance.Game.GetString(transitions[i].ShootSelectorStringIndex, instance),
                        DeltaLayerFunction    = instance.Game.GetString(transitions[i].DeltaLayerFunctionStringIndex, instance),
                        ASMClientNotify       = instance.Game.GetString(transitions[i].ASMClientNofityStringIndex, instance),

                        RequiresRagdollNote      = transitions[i].Requirements.HasFlag(Requirements.RequiresRagdollNotetrack),
                        DeltaRequiresTranslation = transitions[i].Requirements.HasFlag(Requirements.DeltaRequiresTranslation),
                        Terminal                 = transitions[i].Flags.HasFlag(Flags.Terminal),
                        LoopSync                 = transitions[i].Flags.HasFlag(Flags.LoopSync),
                        MultipleDelta            = transitions[i].Flags.HasFlag(Flags.MultipleDelta),
                        Parametric2D             = transitions[i].Flags.HasFlag(Flags.Parametric2D),
                        Coderate                 = transitions[i].Flags.HasFlag(Flags.Coderate),
                        AllowTransDecAim         = transitions[i].Flags.HasFlag(Flags.AllowTransDecAim),
                        ForceFire                = transitions[i].Flags.HasFlag(Flags.ForceFire),
                        CleanLoop                = transitions[i].Flags.HasFlag(Flags.CleanLoop),
                        AnimDrivenLocmotion      = transitions[i].Flags.HasFlag(Flags.AnimDrivenLocmotion),
                        SpeedBlend               = transitions[i].Flags.HasFlag(Flags.SpeedBlend),
                    };
                }

                var subStateObjects = new AnimationStateMachineObj.StateObj[header.SubStateCount];
                var subStates = instance.Reader.ReadArray<SubState>(header.SubStatesPointer, header.SubStateCount);

                for (int i = 0; i < header.SubStateCount; i++)
                {
                    subStateObjects[i] = new AnimationStateMachineObj.StateObj()
                    {
                        Name = instance.Game.GetString(subStates[i].NameStringIndex, instance),


                        AnimationSelector     = instance.Game.GetString(subStates[i].AnimationSelectorStringIndex, instance),
                        AimSelector           = instance.Game.GetString(subStates[i].AimSelectorStringIndex, instance),
                        ShootSelector         = instance.Game.GetString(subStates[i].ShootSelectorStringIndex, instance),
                        TransitionDecorator   = instance.Game.GetString(subStates[i].TransitionDecoratorStringIndex, instance),
                        DeltaLayerFunction    = instance.Game.GetString(subStates[i].DeltaLayerFunctionStringIndex, instance),
                        TransDecLayerFunction = instance.Game.GetString(subStates[i].TransDecLayerFunctionStringIndex, instance),
                        ASMClientNotify       = instance.Game.GetString(subStates[i].ASMClientNofityStringIndex, instance),


                        RequiresRagdollNote      = subStates[i].Requirements.HasFlag(Requirements.RequiresRagdollNotetrack),
                        DeltaRequiresTranslation = subStates[i].Requirements.HasFlag(Requirements.DeltaRequiresTranslation),
                        Terminal                 = subStates[i].Flags.HasFlag(Flags.Terminal),
                        LoopSync                 = subStates[i].Flags.HasFlag(Flags.LoopSync),
                        MultipleDelta            = subStates[i].Flags.HasFlag(Flags.MultipleDelta),
                        Parametric2D             = subStates[i].Flags.HasFlag(Flags.Parametric2D),
                        Coderate                 = subStates[i].Flags.HasFlag(Flags.Coderate),
                        AllowTransDecAim         = subStates[i].Flags.HasFlag(Flags.AllowTransDecAim),
                        ForceFire                = subStates[i].Flags.HasFlag(Flags.ForceFire),
                        CleanLoop                = subStates[i].Flags.HasFlag(Flags.CleanLoop),
                        AnimDrivenLocmotion      = subStates[i].Flags.HasFlag(Flags.AnimDrivenLocmotion),
                        SpeedBlend               = subStates[i].Flags.HasFlag(Flags.SpeedBlend),

                        Transitions              = subStates[i].TransitionCount > 0 ? new Dictionary<string, AnimationStateMachineObj.StateObj>() : null,
                    };

                    var indices = instance.Reader.ReadArray<int>(subStates[i].TransitionIndicesPointer, subStates[i].TransitionCount);

                    foreach(var index in indices)
                        subStateObjects[i].Transitions[transitionObjects[index].Name] = transitionObjects[index];
                }

                var animStateMachine = new AnimationStateMachineObj
                {
                    RootStates = new Dictionary<string, AnimationStateMachineObj.StateObj>()
                };

                var rootStates = instance.Reader.ReadArray<RootState>(header.RootStatesPointer, header.RootStateCount);


                for(int i = 0; i < rootStates.Length; i++)
                {
                    var name = instance.Game.GetString(rootStates[i].NameStringIndex, instance);


                    animStateMachine.RootStates[name] = new AnimationStateMachineObj.StateObj
                    {
                        SubStates = rootStates[i].SubStateIndicesPointer > 0 ? new Dictionary<string, AnimationStateMachineObj.StateObj>() : null
                    };

                    var indices = instance.Reader.ReadArray<int>(rootStates[i].SubStateIndicesPointer, rootStates[i].SubStateCount);

                    foreach (var index in indices)
                        animStateMachine.RootStates[name].SubStates[subStateObjects[index].Name] = subStateObjects[index];
                }

                animStateMachine.Save(Path.Combine(instance.AnimationStateMachinesFolder, asset.Name));

                // Done
                return;
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
