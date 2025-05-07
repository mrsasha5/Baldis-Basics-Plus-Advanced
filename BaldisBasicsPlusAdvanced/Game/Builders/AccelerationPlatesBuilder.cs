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
            ObjectPlacer objectPlacer = ReflectionHelper.getValue<ObjectPlacer>(this, "objectPlacer");
            List<GameObject> objectsPlaced = ReflectionHelper.getValue<List<GameObject>>(objectPlacer, "objectsPlaced");
            foreach (GameObject obj in objectsPlaced)
            {
                obj.GetComponent<AccelerationPlate>().chooseBestRotation();
            }
        }

    }
}
