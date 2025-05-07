/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class PlaceableFanItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            Fan fan = Instantiate(ObjectsStorage.Entities["fan"]).GetComponent<Fan>();

            fan.initialize(pm.ec, pm.cameraBase.position, pm.cameraBase.rotation, livingTime: 20f, turnOff: true);

            Destroy(gameObject);
            return true;
        }

        public ItemSpawningData createData()
        {
            return new ItemSpawningData()
            {
                spawnsOnEndlessMode = true,
                spawnsOnMysteryRooms = true,
                spawnsOnRooms = true
            };
        }
    }
}
*/