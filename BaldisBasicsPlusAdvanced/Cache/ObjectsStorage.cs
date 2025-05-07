using System;
using System.Collections.Generic;
using System.Text;
using BaldisBasicsPlusAdvanced.Game;
using BaldisBasicsPlusAdvanced.Game.Events;
using BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using BaldisBasicsPlusAdvanced.Game.Objects.Triggers;
using BaldisBasicsPlusAdvanced.SerializableData;
using BepInEx;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Cache
{
    public class ObjectsStorage
    {
        //extra datas

        private static Dictionary<string, BaseSpawningData> spawningData = new Dictionary<string, BaseSpawningData>();

        public static Dictionary<string, BaseSpawningData> SpawningData => spawningData;

        private static List<CustomRoomData> roomDatas = new List<CustomRoomData>();

        //datas that was used by API

        private static Dictionary<PluginInfo, List<string>> tips = new Dictionary<PluginInfo, List<string>>();

        public static Dictionary<PluginInfo, List<string>> TipKeys => tips;

        private static Dictionary<PluginInfo, List<string>> words = new Dictionary<PluginInfo, List<string>>();

        public static Dictionary<PluginInfo, List<string>> SymbolMachineWords => words;

        //end of extra datas

        private static Dictionary<string, TriggerBase> triggers = new Dictionary<string, TriggerBase>();

        private static Dictionary<string, Spelloon> spelloons = new Dictionary<string, Spelloon>();

        private static List<WeightedPosterObject> weightedPosterObjects = new List<WeightedPosterObject>();

        private static Dictionary<string, WeightedItemObject> weightedItemObjects = new Dictionary<string, WeightedItemObject>();

        private static Dictionary<string, WeightedItem> weigthedItems = new Dictionary<string, WeightedItem>();

        private static Dictionary<string, ItemObject> itemObjects = new Dictionary<string, ItemObject>();

        private static Dictionary<string, Sprite> editorSprites = new Dictionary<string, Sprite>();

        private static Dictionary<string, WeightedRandomEvent> weightedEvents = new Dictionary<string, WeightedRandomEvent>();

        private static Dictionary<string, RandomEvent> events = new Dictionary<string, RandomEvent>();

        private static Dictionary<string, WeightedObjectBuilder> weightedObjectBuilders = new Dictionary<string, WeightedObjectBuilder>();

        private static Dictionary<string, ObjectBuilder> forcedSpecialObjectBuilders = new Dictionary<string, ObjectBuilder>();

        private static Dictionary<string, GenericHallBuilder> hallBuilders = new Dictionary<string, GenericHallBuilder>();

        private static Dictionary<string, SodaMachineBase> sodaMachines = new Dictionary<string, SodaMachineBase>();

        private static Dictionary<string, Canvas> overlays = new Dictionary<string, Canvas>();

        private static Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

        private static Dictionary<string, Entity> entities = new Dictionary<string, Entity>();

        private static Dictionary<string, GameButton> gameButtons = new Dictionary<string, GameButton>();

        private static Dictionary<string, RoomGroup> roomGroups = new Dictionary<string, RoomGroup>();

        private static Dictionary<string, RoomFunctionContainer> roomFunctionsContainers = new Dictionary<string, RoomFunctionContainer>();

        //18 - ClickableCollidable
        private static int layerMaskIgnorableObjects = 
            //btw, doors uses entity buffers
            ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities" //"Ignore Raycast B" needed?
                , "EntityBuffer") | 1 << 18);

        private static LayerMask projectileLayerMask = (LayerMask)2113537;

        public static LayerMask ProjectileLayerMask => projectileLayerMask;


        // Used for Raycasts!
        public static int RaycastMaskIgnorableObjects => layerMaskIgnorableObjects;

        public static List<CustomRoomData> RoomDatas => roomDatas;

        public static List<WeightedPosterObject> WeightedPosterObjects => weightedPosterObjects;

        public static Dictionary<string, WeightedItemObject> WeightedItemObjects => weightedItemObjects;

        public static Dictionary<string, WeightedItem> WeightedItems => weigthedItems;

        public static Dictionary<string, ItemObject> ItemsObjects => itemObjects;

        public static Dictionary<string, Sprite> EditorSprites => editorSprites;

        public static Dictionary<string, WeightedRandomEvent> WeightedEvents => weightedEvents;

        public static Dictionary<string, RandomEvent> Events => events;

        public static Dictionary<string, WeightedObjectBuilder> WeightedObjectBuilders => weightedObjectBuilders;

        public static Dictionary<string, GenericHallBuilder> HallBuilders => hallBuilders;

        public static Dictionary<string, ObjectBuilder> ForcedSpecialObjectBuilders => forcedSpecialObjectBuilders;

        public static Dictionary<string, SodaMachineBase> SodaMachines => sodaMachines;

        public static Dictionary<string, Canvas> Overlays => overlays;

        public static Dictionary<string, GameObject> Objects => objects;

        public static Dictionary<string, Entity> Entities => entities;

        public static Dictionary<string, GameButton> GameButtons => gameButtons;

        public static Dictionary<string, Spelloon> Spelloons => spelloons;

        public static Dictionary<string, RoomGroup> RoomGroups => roomGroups;

        public static Dictionary<string, RoomFunctionContainer> RoomFunctionsContainers => roomFunctionsContainers;

        public static Dictionary<string, TriggerBase> Triggers => triggers;

    }

}
