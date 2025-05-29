using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Rooms.Functions
{
    public class LockedDoorsFunction : RoomFunction
    {
        [SerializeField]
        private bool doorsLocked;

        [SerializeField]
        private bool updateEnabled;

        public void SetLockedDoors(bool state)
        {
            doorsLocked = state;
        }

        public void SetAutoUpdate(bool state)
        {
            updateEnabled = state;
        }

        public void UpdateDoors()
        {
            for (int i = 0; i < room.doors.Count; i++)
            {
                if (doorsLocked && !room.doors[i].locked)
                {
                    room.doors[i].Lock(true);
                } else if (room.doors[i].locked)
                {
                    room.doors[i].Unlock();
                }
            }
        }

        private void Update()
        {
            if (updateEnabled) UpdateDoors();
        }

    }
}
