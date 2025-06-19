using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class LayersHelper
    {

        public readonly static LayerMask windows;

        public readonly static LayerMask clickableCollidable; //entities???

        public readonly static LayerMask ignoreRaycast;

        public readonly static LayerMask ignoreRaycastB;

        //public readonly static LayerMask blockRaycast = LayerMask.NameToLayer("Block Raycast");

        public readonly static LayerMask standardEntities;

        public readonly static LayerMask clickableEntities;

        public readonly static LayerMask billboard;

        public readonly static LayerMask ui;

        public readonly static LayerMask takenBalloonLayer;

        //public readonly static LayerMask map = LayerMask.NameToLayer("Map");

        public readonly static LayerMask gumCollisionMask;

        public readonly static LayerMask entityCollisionMask;

        // A specific layer used by the Principal's Locker component to see the npcs.
        //public readonly static LayerMask principalLookerMask = 2326529;

        //18 - ClickableCollidable
        public readonly static int ignorableCollidableObjects;

        static LayersHelper()
        {
            windows = LayerMask.NameToLayer("Windows");
            clickableCollidable = LayerMask.NameToLayer("ClickableCollidable");
            standardEntities = LayerMask.NameToLayer("StandardEntities");
            clickableEntities = LayerMask.NameToLayer("ClickableEntities");
            ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
            ignoreRaycastB = LayerMask.NameToLayer("Ignore Raycast B");
            billboard = LayerMask.NameToLayer("Billboard");
            ui = LayerMask.NameToLayer("UI");
            gumCollisionMask = 2113537;
            entityCollisionMask = 2113541;
            ignorableCollidableObjects =
            //btw, doors uses entity buffers
            ~(LayerMask.GetMask("NPCs", "Player", "Ignore Raycast", "StandardEntities", "ClickableEntities" //"Ignore Raycast B" needed?
                , "EntityBuffer", "Block Raycast") | 1 << 18);
            takenBalloonLayer = 29;
        }

#warning I should find another way in the future
        public static void SetIgnoreCollisionForPlayer(bool state)
        {
            //Debug.Log("Ignores standard: " + Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), standardEntities));
            //Debug.Log("Ignores clickable: " + Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("Player"), clickableEntities));
            int layer = LayerMask.NameToLayer("Player");
            Physics.IgnoreLayerCollision(layer, standardEntities, state);
            Physics.IgnoreLayerCollision(layer, clickableEntities, state);
            Physics.IgnoreLayerCollision(layer, LayerMask.NameToLayer("NPCs"), state);
        }

    }
}
