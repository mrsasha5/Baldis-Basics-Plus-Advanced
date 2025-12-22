using BaldisBasicsPlusAdvanced.Cache;
using HarmonyLib;
using MTM101BaldAPI;
using Newtonsoft.Json;

namespace BaldisBasicsPlusAdvanced.Generation.Data
{
    internal class RoomGroupSpawnData : BaseSpawnData<RoomGroup>
    {
        [JsonProperty("reference")]
        private string Serialization_Reference
        {
            set
            {
                instance = ObjectStorage.RoomGroups[value];
            }
        }

        public override void Register(string name, int floor, SceneObject sceneObject, CustomLevelObject levelObject)
        {
            if (standardData.IsFloorIncluded(floor, levelObject.type))
            {
                RoomGroup roomGroup = new RoomGroup();
                roomGroup.name = instance.name;
                roomGroup.minRooms = instance.minRooms;
                roomGroup.maxRooms = instance.maxRooms;
                roomGroup.ceilingTexture = instance.ceilingTexture;
                roomGroup.wallTexture = instance.wallTexture;
                roomGroup.floorTexture = instance.floorTexture;
                roomGroup.light = instance.light;

                levelObject.roomGroup = levelObject.roomGroup.AddToArray(roomGroup);
            }
            
        }

    }
}
