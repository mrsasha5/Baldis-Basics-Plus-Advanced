using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.Game.Spawning;
using BaldisBasicsPlusAdvanced.Game.WeightedSelections;
using BaldisBasicsPlusAdvanced.SerializableData;
using BaldisBasicsPlusAdvanced.SerializableData.Rooms;
using BepInEx;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Cache
{
    //I should do something with this thing in the future
    //And generation too
    internal class ObjectsStorage
    {

        private static Dictionary<string, Color> roomColors = new Dictionary<string, Color>()
        {
            { "English", Color.magenta },
            { "SchoolCouncil", new Color(0.4335f, 0.1679f, 0.1601f) }, //111 43 41
            { "AdvancedClass", Color.green },
            { "CornField", Color.cyan }
        };

        public static Dictionary<string, Color> RoomColors => roomColors;

        private static List<CellTextureSerializableData> cellTextureData = new List<CellTextureSerializableData>();

        private static Dictionary<string, BaseSpawningData> spawningData = new Dictionary<string, BaseSpawningData>();

        public static List<CellTextureSerializableData> CellTextureData => cellTextureData;

        public static Dictionary<string, BaseSpawningData> SpawningData => spawningData;

        private static List<CustomRoomData> customRoomData = new List<CustomRoomData>();

        //API data starts here

        private static Dictionary<PluginInfo, List<WeightedCouncilTopic>> topics = new Dictionary<PluginInfo, List<WeightedCouncilTopic>>();

        public static Dictionary<PluginInfo, List<WeightedCouncilTopic>> Topics => topics;

        private static Dictionary<PluginInfo, List<string>> tips = new Dictionary<PluginInfo, List<string>>();

        public static Dictionary<PluginInfo, List<string>> TipKeys => tips;

        private static Dictionary<PluginInfo, List<string>> words = new Dictionary<PluginInfo, List<string>>();

        public static Dictionary<PluginInfo, List<string>> SymbolMachineWords => words;

        //Ends here

        private static Dictionary<string, SceneObject> sceneObjects = new Dictionary<string, SceneObject>();

        private static Dictionary<string, BaseTrigger> triggers = new Dictionary<string, BaseTrigger>();

        private static Dictionary<string, Spelloon> spelloons = new Dictionary<string, Spelloon>();

        private static List<WeightedPosterObject> weightedPosterObjects = new List<WeightedPosterObject>();

        private static Dictionary<string, ItemObject> itemObjects = new Dictionary<string, ItemObject>();

        private static Dictionary<string, NPC> npcs = new Dictionary<string, NPC>();

        private static Dictionary<string, RandomEvent> events = new Dictionary<string, RandomEvent>();

        private static Dictionary<string, StructureBuilder> structureBuilders = new Dictionary<string, StructureBuilder>();

        private static Dictionary<string, SodaMachine> sodaMachines = new Dictionary<string, SodaMachine>();

        private static Dictionary<string, Canvas> overlays = new Dictionary<string, Canvas>();

        private static Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

        private static Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

        private static Dictionary<string, RoomGroup> roomGroups = new Dictionary<string, RoomGroup>();

        private static Dictionary<string, RoomFunctionContainer> roomFunctionsContainers = new Dictionary<string, RoomFunctionContainer>();

        private static List<PosterObject> posters = new List<PosterObject>();

        public static Dictionary<string, SceneObject> SceneObjects => sceneObjects;

        public static Dictionary<string, NPC> Npcs => npcs;

        public static List<CustomRoomData> CustomRoomData => customRoomData;

        public static List<WeightedPosterObject> WeightedPosterObjects => weightedPosterObjects;

        public static Dictionary<string, ItemObject> ItemObjects => itemObjects;

        public static Dictionary<string, RandomEvent> Events => events;

        public static Dictionary<string, StructureBuilder> StructureBuilders => structureBuilders;

        public static Dictionary<string, SodaMachine> SodaMachines => sodaMachines;

        public static Dictionary<string, Canvas> Overlays => overlays;

        public static Dictionary<string, GameObject> Objects => objects;

        public static Dictionary<string, Entity> Entities => entities;

        public static Dictionary<string, Spelloon> Spelloons => spelloons;

        public static Dictionary<string, RoomGroup> RoomGroups => roomGroups;

        public static Dictionary<string, RoomFunctionContainer> RoomFunctionsContainers => roomFunctionsContainers;

        public static List<PosterObject> Posters => posters;

        public static Dictionary<string, BaseTrigger> Triggers => triggers;

    }

}
