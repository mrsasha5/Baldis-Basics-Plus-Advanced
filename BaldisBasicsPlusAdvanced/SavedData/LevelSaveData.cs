using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BaldisBasicsPlusAdvanced.SavedData
{
    public class LevelSaveData
    {
        public double version = 1d;

        public Character[] bannedCharacters = new Character[0];

        public bool hammerBoughtOnCurrentFloor = false;

        public void onLoadNextLevel()
        {
            bannedCharacters = new Character[0];
            hammerBoughtOnCurrentFloor = false;
        }

        public void setDefaults()
        {
            bannedCharacters = new Character[0];
            hammerBoughtOnCurrentFloor = false;
        }

        public void loadFrom(LevelSaveData levelSaveData)
        {
            this.version = levelSaveData.version;
            this.bannedCharacters = (Character[])levelSaveData.bannedCharacters.Clone();
            this.hammerBoughtOnCurrentFloor = levelSaveData.hammerBoughtOnCurrentFloor;
        }

        public void save(BinaryWriter writer)
        {
            writer.Write(version);
            writer.Write(hammerBoughtOnCurrentFloor);
            writer.Write(bannedCharacters.Length);
            for (int i = 0; i < bannedCharacters.Length; i++)
            {
                writer.Write((int)bannedCharacters[0]);
            }
        }

        public void load(BinaryReader reader)
        {
            version = reader.ReadDouble();
            hammerBoughtOnCurrentFloor = reader.ReadBoolean();
            int charactersCount = reader.ReadInt32();
            bannedCharacters = new Character[charactersCount];
            for (int i = 0; i < charactersCount; i++)
            {
                bannedCharacters[i] = (Character)reader.ReadInt32();
            }
        }


    }
}
