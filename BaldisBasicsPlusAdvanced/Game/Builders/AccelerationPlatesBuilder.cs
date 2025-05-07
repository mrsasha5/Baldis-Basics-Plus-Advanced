using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class AccelerationPlatesBuilder : GenericHallBuilder
    {
        public override void Build(EnvironmentController ec, LevelBuilder builder, RoomController room, System.Random cRng)
        {
            base.Build(ec, builder, room, cRng);
            List<GameObject> objectsPlaced = ReflectionHelper.GetValue<List<GameObject>>
                (ReflectionHelper.GetValue<ObjectPlacer>(this, "objectPlacer"), "objectsPlaced");
            foreach (GameObject obj in objectsPlaced)
            {
                obj.GetComponent<AccelerationPlate>().ChooseBestRotation();
            }
        }

    }
}
