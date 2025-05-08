using BaldisBasicsPlusAdvanced.Cache;
using MTM101BaldAPI;
using PlusLevelFormat;
using PlusLevelLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class RoomHelper
    {
        public static T SetupRoomFunction<T>(RoomFunctionContainer container) where T : RoomFunction, new()
        {
            T func = container.gameObject.AddComponent<T>();
            if (ReflectionHelper.GetValue<List<RoomFunction>>(container, "functions") == null)
                ReflectionHelper.SetValue<List<RoomFunction>>(container, "functions", new List<RoomFunction>());
            container.AddFunction(func);
            return func;
        }

        //squaredShape - If True, the room will (internally) turn into a square shape. This is highly recommended for loading Special Rooms.
        public static RoomAsset CreateAssetFromPath(string path, bool isOffLimits, bool autoAssignFunctionContainer, RoomFunctionContainer existingContainer = null, bool isAHallway = false, bool isASecretRoom = false, Texture2D mapBg = null, bool keepTextures = true, bool squaredShape = false)
        {

            int idx = isAHallway ? 0 : 1;

            RoomAsset rAsset;
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                LevelAsset lvlAsset = CustomLevelLoader.LoadLevelAsset(LevelExtensions.ReadLevel(reader));
                rAsset = ScriptableObject.CreateInstance<RoomAsset>();


                // **************** Load Room Asset Data **************
                // Note, everything must be stored as value, not reference; LevelAssets will be destroyed afterwards
                // There should be only a single room per level asset (only uses rooms[idx])

                rAsset.activity = lvlAsset.rooms[idx].activity.GetNew();
                rAsset.basicObjects = new List<BasicObjectData>(lvlAsset.rooms[idx].basicObjects);
                rAsset.blockedWallCells = new List<IntVector2>(lvlAsset.rooms[idx].blockedWallCells);
                rAsset.category = lvlAsset.rooms[idx].category;

                IntVector2 biggestSize = default;

                foreach (CellData cell in lvlAsset.tile)
                {
                    if (cell.roomId == idx && cell.type != 16)
                    {
                        if (isAHallway)
                            cell.type = 0;

                        rAsset.cells.Add(cell);
                        if (biggestSize.x < cell.pos.x) // Separated each axis, to actually give a square shape
                            biggestSize.x = cell.pos.x;

                        if (biggestSize.z < cell.pos.z)
                            biggestSize.z = cell.pos.z;
                    }
                }
                List<IntVector2> posList = rAsset.cells.ConvertAll(x => x.pos);

                rAsset.color = lvlAsset.rooms[idx].color;
                rAsset.doorMats = lvlAsset.rooms[idx].doorMats;

                rAsset.entitySafeCells = new List<IntVector2>(posList);
                rAsset.eventSafeCells = new List<IntVector2>(posList); // Ignore editor's implementation of this, it's horrible and the green marker should work better
                for (int i = 0; i < rAsset.basicObjects.Count; i++)
                {
                    var obj = rAsset.basicObjects[i];
                    if (obj.prefab.name == "nonSafeCellMarker")
                    {
                        var pos = IntVector2.GetGridPosition(obj.position);
                        if (!isAHallway)
                        {
                            rAsset.entitySafeCells.Remove(pos);
                            rAsset.eventSafeCells.Remove(pos);
                        }
                        rAsset.basicObjects.RemoveAt(i--);
                    }
                }


                rAsset.forcedDoorPositions = new List<IntVector2>(lvlAsset.rooms[idx].forcedDoorPositions);
                rAsset.hasActivity = lvlAsset.rooms[idx].hasActivity;
                rAsset.itemList = new List<WeightedItemObject>(lvlAsset.rooms[idx].itemList);
                rAsset.items = new List<ItemData>(lvlAsset.rooms[idx].items);
                for (int i = 0; i < rAsset.basicObjects.Count; i++)
                {
                    BasicObjectData obj = rAsset.basicObjects[i];
                    if (obj.prefab.name == "itemSpawnMarker")
                    {
                        rAsset.basicObjects.RemoveAt(i--);
                        rAsset.itemSpawnPoints.Add(new ItemSpawnPoint() { weight = 50, position = new Vector2(obj.position.x, obj.position.z) });
                    }
                }

                rAsset.keepTextures = keepTextures;
                rAsset.ceilTex = lvlAsset.rooms[idx].ceilTex;
                rAsset.florTex = lvlAsset.rooms[idx].florTex;
                rAsset.wallTex = lvlAsset.rooms[idx].wallTex;
                rAsset.mapMaterial = lvlAsset.rooms[idx].mapMaterial;
                rAsset.offLimits = isOffLimits;

                for (int i = 0; i < rAsset.basicObjects.Count; i++)
                {
                    var obj = rAsset.basicObjects[i];
                    var pos = IntVector2.GetGridPosition(obj.position);
                    if (obj.prefab.name == "potentialDoorMarker")
                    {
                        rAsset.basicObjects.RemoveAt(i--);
                        if (!isAHallway)
                        {
                            rAsset.potentialDoorPositions.Add(pos);
                            rAsset.blockedWallCells.Remove(pos);
                        }
                    }
                    else if (obj.prefab.name == "forcedDoorMarker")
                    {
                        rAsset.basicObjects.RemoveAt(i--);
                        if (!isAHallway)
                        {
                            rAsset.forcedDoorPositions.Add(pos);
                            rAsset.blockedWallCells.Remove(pos);
                        }
                    }
                }

                rAsset.requiredDoorPositions = new List<IntVector2>(lvlAsset.rooms[idx].requiredDoorPositions); // It seems required has a higher priority than forced, but has no apparent difference
                if (isASecretRoom) // secret room :O
                    rAsset.secretCells.AddRange(rAsset.cells.Select(x => x.pos));
                else
                    rAsset.secretCells = new List<IntVector2>(lvlAsset.rooms[idx].secretCells);

                for (int i = 0; i < rAsset.basicObjects.Count; i++)
                {
                    BasicObjectData obj = rAsset.basicObjects[i];
                    if (obj.prefab.name == "lightSpotMarker")
                    {
                        rAsset.basicObjects.RemoveAt(i--);
                        rAsset.standardLightCells.Add(IntVector2.GetGridPosition(obj.position));
                    }
                }

                rAsset.type = lvlAsset.rooms[idx].type;

                rAsset.name = $"Room_{rAsset.category}_{Path.GetFileNameWithoutExtension(path)}";
                ((UnityEngine.Object)rAsset).name = rAsset.name;

                if (existingContainer != null)
                {
                    rAsset.roomFunctionContainer = existingContainer;
                }
                else if (!isAHallway && autoAssignFunctionContainer) // No container for hallway
                {
                    RoomFunctionContainer roomFunctionContainer = 
                        PlusLevelLoaderPlugin.Instance.roomSettings.Values.ToList().Find(x => x.container != null
                        && x.category == rAsset.category)?.container;

                    if (roomFunctionContainer == null)
                    {
                        roomFunctionContainer = new GameObject(rAsset.name + "FunctionContainer").AddComponent<RoomFunctionContainer>();
                        ReflectionHelper.SetValue<List<RoomFunction>>(roomFunctionContainer, "functions", new List<RoomFunction>());
                        roomFunctionContainer.gameObject.ConvertToPrefab(true);
                    }

                    rAsset.roomFunctionContainer = roomFunctionContainer;
                }

                if (mapBg != null)
                {
                    rAsset.mapMaterial = new Material(rAsset.mapMaterial);
                    rAsset.mapMaterial.SetTexture("_MapBackground", mapBg);
                    rAsset.mapMaterial.shaderKeywords = new string[] { "_KEYMAPSHOWBACKGROUND_ON" };
                    rAsset.mapMaterial.name = rAsset.name;
                }
                else if (isAHallway)
                    rAsset.mapMaterial = null; // hallways have no material

                if (squaredShape && biggestSize.z > 0 && biggestSize.x > 0 && !isAHallway) // Fillup empty spots
                {
                    for (int x = 0; x <= biggestSize.x; x++)
                    {
                        for (int z = 0; z <= biggestSize.z; z++)
                        {
                            IntVector2 pos = new IntVector2(x, z);
                            if (!rAsset.cells.Any(_x => _x.pos == pos))
                            {
                                rAsset.cells.Add(new CellData() { pos = pos });
                                rAsset.secretCells.Add(pos);
                            }
                        }
                    }
                }

                UnityEngine.Object.Destroy(lvlAsset); // Remove the created level asset from memory
            }
            return rAsset;
        }


        public static Material CreateMapMaterial(string name, Texture2D mapBg)
        {
            Material mapMaterial = new Material(AssetsHelper.LoadAsset<Material>("MapTile_Standard"));
            mapMaterial.SetTexture("_MapBackground", mapBg);
            mapMaterial.shaderKeywords = new string[] { "_KEYMAPSHOWBACKGROUND_ON" };
            mapMaterial.name = name;
            return mapMaterial;
        }

        public static List<IntVector2> CreateVector2Range(IntVector2 vector1, IntVector2 vector2)
        {
            int x1 = vector1.x;
            int z1 = vector1.z;
            List<IntVector2> vector2s = new List<IntVector2>();
            while (x1 < vector2.x)
            {
                while (z1 < vector2.z)
                {
                    vector2s.Add(new IntVector2(x1, z1));
                    z1++;
                }
                z1 = vector1.z;
                x1++;
            }
            return vector2s;
        }
    }
}
