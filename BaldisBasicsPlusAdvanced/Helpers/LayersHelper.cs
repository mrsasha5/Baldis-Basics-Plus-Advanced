using System;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Helpers
{
    public class LayersHelper
    {

        public static LayerMask windows;

        public static LayerMask clickableCollidable; //entities???

        public static LayerMask ignoreRaycast;

        public static LayerMask ignoreRaycastB;

        //public readonly static LayerMask blockRaycast = LayerMask.NameToLayer("Block Raycast");

        public static LayerMask standardEntities;

        public static LayerMask clickableEntities;

        public static LayerMask billboard;

        public static LayerMask ui;

        public static LayerMask takenBalloonLayer;

        public static LayerMask gumCollisionMask;

        public static LayerMask entityCollisionMask;

        //public readonly static LayerMask principalLookerMask = 2326529;

        //18 - ClickableCollidable
        public static int ignorableCollidableObjects;

        public static void Initialize()
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
