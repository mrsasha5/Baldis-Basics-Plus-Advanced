using BaldisBasicsPlusAdvanced.Game.Objects.Plates;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using BaldisBasicsPlusAdvanced.Patches.GameEventsProvider;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class PressurePlatesBuilder : GenericHallBuilder, IGameManagerEventsReceiver
    {
        private List<IButtonReceiver> potentialButtonReceivers = new List<IButtonReceiver>();

        private List<PressurePlate> plates = new List<PressurePlate>();

        private System.Random cRng;

        public override void Build(EnvironmentController ec, LevelBuilder builder, RoomController room, System.Random cRng)
        {
            if (cRng.Next(0, 101) <= 70) return;
            this.cRng = cRng;
            base.Build(ec, builder, room, cRng);
            ObjectPlacer objectPlacer = ReflectionHelper.getValue<ObjectPlacer>(this, "objectPlacer");
            List<GameObject> objectsPlaced = ReflectionHelper.getValue<List<GameObject>>(objectPlacer, "objectsPlaced");
            foreach (GameObject obj in objectsPlaced)
            {
                plates.Add(obj.GetComponent<PressurePlate>());
            }
            BaseGameManagerEvents.register(this);
        }

        private void collectPotentialReceiversData()
        {
            potentialButtonReceivers.Clear();
            potentialButtonReceivers.AddRange(FindObjectsOfType<BeltManager>());
            potentialButtonReceivers.AddRange(FindObjectsOfType<RotoHall>());
            potentialButtonReceivers.AddRange(FindObjectsOfType<LockdownDoor>());

            List<IButtonReceiver> newReceiversList = new List<IButtonReceiver>(potentialButtonReceivers);
            newReceiversList.controlledMix(cRng);
            potentialButtonReceivers.Clear();

            int maxCount = cRng.Next(1, 4);

            if (maxCount > newReceiversList.Count)
            {
                maxCount = newReceiversList.Count;
            }

            for (int i = 0; i < maxCount; i++)
            {
                potentialButtonReceivers.Add(newReceiversList[i]);
            }
        }

        public void onManagerInitPost()
        {
            collectPotentialReceiversData();
            if (potentialButtonReceivers.Count == 0)
            {
                foreach (PressurePlate plate in plates)
                {
                    Destroy(plate.gameObject);
                }
                plates.Clear();
            }
            foreach (PressurePlate plate in plates)
            {
                collectPotentialReceiversData();
                plate.connectTo(new List<IButtonReceiver>(potentialButtonReceivers));
            }
            BaseGameManagerEvents.unregister(this);
            potentialButtonReceivers.Clear();
        }
    }
}
