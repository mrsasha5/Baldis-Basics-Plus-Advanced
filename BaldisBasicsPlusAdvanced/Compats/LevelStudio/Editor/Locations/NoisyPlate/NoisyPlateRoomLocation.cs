using System.Collections.Generic;
using PlusLevelStudio.Editor;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.NoisyPlate
{
    public class NoisyPlateRoomLocation : IEditorDeletable
    {

        //Serializable
        public string builderPrefab;

        public int cooldown;

        public int generosity;

        //Not serializable
        public EditorRoom room;

        public List<GameObject> allocatedPlates = new List<GameObject>();

        public NoisyPlateStructureLocation owner;

        public bool OnDelete(EditorLevelData data)
        {
            owner.OnRoomDelete(this, true);
            return true;
        }
    }
}
