using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    internal class LayersHelper
    {

        private static bool warningNotifSpawned;

        public static LayerMask windows;

        public static LayerMask ignoreRaycast;

        public static LayerMask ignoreRaycastB;

        public static LayerMask npcs;

        public static LayerMask player; 

        public static LayerMask standardEntities;

        public static LayerMask clickableEntities;

        public static LayerMask collidableEntities;

        public static LayerMask clickableCollidableEntities;

        public static LayerMask noInterCollisionEntities;

        public static LayerMask billboard;

        public static LayerMask ui;

        public static LayerMask takenBalloonLayer;

        public static LayerMask gumCollisionMask;

        public static LayerMask entityCollisionMask;

        public static int ignorableCollidableObjects;

        public static void Initialize()
        {
#if DEBUG
            AdvancedCore.Logging.LogInfo("[LayersHelper] All game layers:");
            for (int i = 0; i < 32; i++)
            {
                AdvancedCore.Logging.LogInfo($"{i} | {LayerMask.LayerToName(i)}");
            }
#endif

            windows = LayerFromName("Windows");
            standardEntities = LayerFromName("StandardEntities");
            clickableEntities = LayerFromName("ClickableEntities");
            collidableEntities = LayerFromName("CollidableEntities");
            clickableCollidableEntities = LayerFromName("ClickableCollidableEntities");
            noInterCollisionEntities = LayerFromName("NoInterCollisionEntities"); //Used by students from event
            ignoreRaycast = LayerFromName("Ignore Raycast");
            ignoreRaycastB = LayerFromName("Ignore Raycast B");
            billboard = LayerFromName("Billboard");
            ui = LayerFromName("UI");
            npcs = LayerFromName("NPCs");
            player = LayerFromName("Player");
            gumCollisionMask = 2113537;
            entityCollisionMask = 2113541;
            ignorableCollidableObjects =
            //Btw, doors use entity buffers
            ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities"
                , "EntityBuffer", "Block Raycast") | 1 << 18);
            takenBalloonLayer = 29;
        }

        public static LayerMask LayerFromName(string name)
        {
            LayerMask layer = LayerMask.NameToLayer(name);
            if (layer < 0 && !warningNotifSpawned)
            {
                warningNotifSpawned = true;
                NotificationManager.Instance.Queue(
                    "Adv_Notif_LayersError", AssetsStorage.sounds["buzz_elv"], isForced: true);
            }
            return layer;
        }

#warning I should find another way in the future (and I don't like idea of invoking Entity.IgnoreEntity each time)
        public static void SetIgnoreCollisionForPlayer(bool state)
        {
            Physics.IgnoreLayerCollision(player, standardEntities, state);
            Physics.IgnoreLayerCollision(player, clickableEntities, state);
            Physics.IgnoreLayerCollision(player, clickableCollidableEntities, state);
            Physics.IgnoreLayerCollision(player, npcs, state);
            Physics.IgnoreLayerCollision(player, noInterCollisionEntities, state);
        }

    }
}
