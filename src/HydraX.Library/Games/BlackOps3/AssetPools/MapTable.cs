using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HydraX.Library
{
    partial class BlackOps3
    {
        /// <summary>
        /// Black Ops 3 Map Table Logic
        /// </summary>
        private class MapTable : IAssetPool
        {
            #region Enums
            /// <summary>
            /// Session Mode
            /// </summary>
            private enum SessionMode : int
            {
                CORE = -1,
                ZM = 0,
                MP = 1,
                CP = 2
            }

            /// <summary>
            /// Campaign Mode
            /// </summary>
            private enum CampaignMode : int
            {
                Default = 0,
                Nightmares = 1
            }

            /// <summary>
            /// DLC Index
            /// </summary>
            private enum DLCIndex : int
            {
                Dev = -1,
                Base = 0,
                DLC0ZM = 1,
                DLC0MP = 2,
                DLC1 = 3,
                DLC2 = 4,
                DLC3 = 5,
                DLC4 = 6,
                DLC5 = 7,
                DLC6 = 12
            }

            /// <summary>
            /// Map Size
            /// </summary>
            private enum MapSize : int
            {
                Small = 0,
                Medium = 1,
                Large = 2
            }
            #endregion

            #region AssetStructures
            /// <summary>
            /// Map Table Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            private struct MapTableAsset
            {
                #region MapTableProperties
                public long NamePointer;
                public int MapEntryCount;
                public long MapEntriesPointer;
                public SessionMode SessionMode;
                public CampaignMode CampaignMode;
                public DLCIndex DLCIndex;
                #endregion
            }

            /// <summary>
            /// Map Table Entry Asset Structure
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 248)]
            private struct MapTableEntryAsset
            {
                #region MapTableEntryProperties
                public long NamePointer;
                public MapSize MapSize;
                public long MapNamePointer;
                public long MapNameCapsPointer;
                public long MapRootNamePointer;
                public long MapDescPointer;
                public long MapDescShortPointer;
                public long MissionNamePointer;
                public long MapLocationPointer;
                public long MapDatePointer;
                public long PresenceStringPointer;
                public long PreviewImageAssetPointer;
                public long LoadingImageNamePointer;
                public long CompassMapPointer;
                public long CompassMapCorruptPointer;
                public byte IsSafehouse;
                public byte IsSubLevel;
                public long SafehouseNamePointer;
                public long IntroMovieNamePointer;
                public long OutroMovieNamePointer;
                public long ObjectiveListNamePointer;
                public long CollectibleListNamePointer;
                public long AccoladeListAssetPointer;
                public long ObjectiveStringPointer;
                public float CloseRange;
                public float MediumRange;
                public float LongRange;
                public long ResistanceStringPointer;
                public long OperationsBackgroundImagePointer;
                public int IsFreerunMap;
                public int FreerunTrackIndex;
                public int MapVersion;
                public int PropertyCount;
                public long PropertiesPointer;
                public long MapID;
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
            public string Name => "maptable";

            /// <summary>
            /// Gets the Setting Group for this Pool
            /// </summary>
            public string SettingGroup => "Misc";

            /// <summary>
            /// Gets the Index of this Pool
            /// </summary>
            public int Index => (int)AssetPool.maptable;

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
                    var address = StartAddress + (i * AssetSize);
                    var header = instance.Reader.ReadStruct<MapTableAsset>(address);

                    if (IsNullAsset(header.NamePointer))
                        continue;

                    results.Add(new Asset()
                    {
                        Name = instance.Reader.ReadNullTerminatedString(header.NamePointer),
                        Type        = Name,
                        Status      = "Loaded",
                        Data        = address,
                        LoadMethod  = ExportAsset,
                        Zone = ((BlackOps3)instance.Game).ZoneNames[address],
                        Information = string.Format("Maps: {0}", header.MapEntryCount)
                    });
                }

                return results;
            }

            /// <summary>
            /// Exports the given asset from this pool
            /// </summary>
            public void ExportAsset(Asset asset, HydraInstance instance)
            {
                var header = instance.Reader.ReadStruct<MapTableAsset>((long)asset.Data);

                if (asset.Name != instance.Reader.ReadNullTerminatedString(header.NamePointer))
                    throw new Exception("The asset at the expected memory address has changed. Press the Load Game button to refresh the asset list.");

                var mapTable = new GameDataTable.Asset(asset.Name, "maptable");
                mapTable["mapCount"] = header.MapEntryCount;
                mapTable["sessionMode"] = (int)header.SessionMode;
                mapTable["campaignMode"] = (int)header.CampaignMode;
                mapTable["dlcIndex"] = (int)header.DLCIndex;

                var mapEntries = instance.Reader.ReadArray<MapTableEntryAsset>(header.MapEntriesPointer, header.MapEntryCount);

                for(int i = 0; i < mapEntries.Length; i++)
                {
                    string name = instance.Reader.ReadNullTerminatedString(mapEntries[i].NamePointer);

                    var mapEntry = new GameDataTable.Asset(name, "maptableentry");

                    mapEntry["size"] = (int)mapEntries[i].MapSize;

                    mapEntry["mapName"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapNamePointer);
                    mapEntry["mapNameCaps"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapNameCapsPointer);
                    mapEntry["rootMapName"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapRootNamePointer);
                    mapEntry["missionName"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MissionNamePointer);
                    mapEntry["mapDescription"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapDescPointer);
                    mapEntry["mapDescriptionShort"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapDescShortPointer);
                    mapEntry["mapLocation"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapLocationPointer);
                    mapEntry["mapDateTime"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].MapDatePointer);
                    mapEntry["presenceString"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].PresenceStringPointer);

                    mapEntry["previewImage"] = instance.Game.GetAssetName(mapEntries[i].PreviewImageAssetPointer, instance, 0xF8);
                    mapEntry["loadingImage"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].LoadingImageNamePointer);

                    mapEntry["compassMap"] = instance.Game.GetAssetName(mapEntries[i].CompassMapPointer, instance, 0xF8);
                    mapEntry["compassMapCorrupt"] = instance.Game.GetAssetName(mapEntries[i].CompassMapCorruptPointer, instance, 0xF8);

                    mapEntry["isSafeHouse"] = mapEntries[i].IsSafehouse;
                    mapEntry["isSubLevel"] = mapEntries[i].IsSubLevel;

                    mapEntry["Safehouse"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].SafehouseNamePointer);

                    mapEntry["introMovie"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].IntroMovieNamePointer);
                    mapEntry["outroMovie"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].OutroMovieNamePointer);

                    mapEntry["collectibles"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].CollectibleListNamePointer);
                    mapEntry["accoladelist"] = instance.Game.GetAssetName(mapEntries[i].AccoladeListAssetPointer, instance);
                    mapEntry["objectives"] = instance.Game.GetAssetName(mapEntries[i].ObjectiveListNamePointer, instance);
                    mapEntry["mapObjective"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].ObjectiveStringPointer);

                    mapEntry["engageClose"] = mapEntries[i].CloseRange;
                    mapEntry["engageMedium"] = mapEntries[i].MediumRange;
                    mapEntry["engageLong"] = mapEntries[i].LongRange;

                    mapEntry["resistanceText"] = instance.Reader.ReadNullTerminatedString(mapEntries[i].ResistanceStringPointer);

                    mapEntry["operationsBackground"] = instance.Game.GetAssetName(mapEntries[i].OperationsBackgroundImagePointer, instance, 0xF8);

                    mapEntry["isFreeRunMap"] = mapEntries[i].IsFreerunMap;
                    mapEntry["freerunTrackIndex"] = mapEntries[i].FreerunTrackIndex;
                    mapEntry["mapVersion"] = mapEntries[i].MapVersion;

                    mapTable["map" + (i + 1).ToString("00")] = mapEntry.Name;

                    var dataBlocks = instance.Reader.ReadArray<KVPBlock>(mapEntries[i].PropertiesPointer, mapEntries[i].PropertyCount);

                    for (int j = 0; j < dataBlocks.Length; j++)
                    {
                        // Determine type if we're int, since bool and int share same, but the pointer value will be different? (only 2 are bool anyway but just in case)
                        var dataType = dataBlocks[j].DataType;

                        if ((dataType == DataTypes.Int) && (dataBlocks[j].DataPointer & 0xFFFFFFFF) != dataBlocks[j].Data)
                            dataType = DataTypes.Bool;

                        string propertyName = string.Format("{0}_{1}", dataType.ToString().ToLower(), instance.Game.GetString(dataBlocks[j].DataNameStringIndex, instance));

                        // No idea why, in the tools it's like this but in-game the datatype makes it xstring
                        if (propertyName.Contains("bigPreviewImage"))
                            propertyName = "streamedimage_bigPreviewImage";

                        mapEntry[propertyName] = dataBlocks[j].DataType == DataTypes.Int ? dataBlocks[j].Data.ToString() : instance.Game.GetString(dataBlocks[j].DataStringIndex, instance);
                        continue;
                    }

                    instance.AddGDTAsset(mapEntry, mapEntry.Type, mapEntry.Name);
                }

                instance.AddGDTAsset(mapTable, mapTable.Type, mapTable.Name);

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
