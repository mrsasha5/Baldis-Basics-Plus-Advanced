using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Rooms
{
    public class PartyLightColors : MonoBehaviour
    {

        private List<Color> colors = new List<Color>()
        {
            Color.red,
            Color.yellow,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta
        };

        private int colorIndex;

        private RoomController room;

        private float time;

        private float timeCooldown = 0.5f;

        public void Initialize(RoomController room)
        {
            this.room = room;
        }

        private void Update()
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                time = timeCooldown;
                UpdateLights();
                colorIndex++;
                if (colorIndex > colors.Count - 1) colorIndex = 0;
            }
        }

        private void UpdateLights()
        {
            for (int i = 0; i < room.cells.Count; i++)
            {
                if (room.cells[i].hasLight)
                {
                    room.cells[i].lightColor = colors[colorIndex];
                    room.cells[i].SetLight(on: true);
                }
            }
        }

        public void NormalizeLights()
        {
            for (int i = 0; i < room.cells.Count; i++)
            {
                if (room.cells[i].hasLight)
                {
                    room.cells[i].lightColor = Color.white;
                    room.cells[i].SetLight(on: true);
                }
            }
        }

    }
}
