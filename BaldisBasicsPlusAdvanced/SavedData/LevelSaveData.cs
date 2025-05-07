using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using System;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.SavedData
{
    [Serializable]
    public class LevelSaveData
    {
        //versions list
        //1d
        //2d
        //3f

        public float version = 3f;

        public List<Character> bannedCharacters = new List<Character>();

        public List<int?> remainingFloorsToUnban = new List<int?>();

        public List<TopicBase> topics = new List<TopicBase>();

        [NonSerialized]
        public List<TopicBase> addTopicsOnTheNextFloor = new List<TopicBase>();

        public bool hammerPurchased;

        public int hammerLives;

        public int boughtHammerPrice;

        public bool TopicIsActive<T>() where T : TopicBase
        {
            for (int i = 0; i < topics.Count; i++)
            {
                if (topics[i] is T) return true;
            }
            return false;
        }

        public void OnLoadNextLevel(bool afterPitStop)
        {
            if (!afterPitStop)
            {
                if (hammerLives > 0)
                {
                    hammerLives--;
                    if (hammerLives <= 0)
                    {
                        BuyExpelHammer(0, cancelPurchase: true);
                    }
                }

                List<int> indexes = new List<int>();
                for (int i = 0; i < remainingFloorsToUnban.Count; i++)
                {
                    if (remainingFloorsToUnban[i] == null) continue;
                    remainingFloorsToUnban[i]--;
                    if (remainingFloorsToUnban[i] <= 0)
                    {
                        indexes.Add(i);
                    }
                }

                int addendIndex = 0;
                foreach (int index in indexes)
                {
                    bannedCharacters.RemoveAt(index + addendIndex);
                    remainingFloorsToUnban.RemoveAt(index + addendIndex);
                    addendIndex--;
                }

                for (int i = 0; i < addTopicsOnTheNextFloor.Count; i++)
                {
                    topics.Add(addTopicsOnTheNextFloor[i]);
                    addTopicsOnTheNextFloor.RemoveAt(i);
                    i--;
                }

            } else
            {
                addTopicsOnTheNextFloor.Clear();
            }

            for (int i = 0; i < topics.Count; i++)
            {
                if (topics[i].SaveToNextFloors)
                {
                    topics[i].OnLoadNextLevel(afterPitStop);
                    Singleton<BaseGameManager>.Instance.Ec.OnEnvironmentBeginPlay += () =>
                        topics[i].OnEnvBeginPlay(Singleton<BaseGameManager>.Instance.Ec);
                }
                else
                {
                    topics[i].OnDestroying();
                    topics.RemoveAt(i);
                    i--;
                }
            }
        }

        public void OnLoad()
        {
            for (int i = 0; i < topics.Count; i++)
            {
                topics[i].OnLoad();
            }
        }

        public void OnSave()
        {
            for (int i = 0; i < topics.Count; i++)
            {
                topics[i].OnSave();
            }
        }

        public void BanCharacter(Character @char, int floorToUnban)
        {
            bannedCharacters.Add(@char);
            if (floorToUnban >= 0) remainingFloorsToUnban.Add(floorToUnban);
            else remainingFloorsToUnban.Add(null);
        }

        public void BuyExpelHammer(int price, int lives = 2, bool cancelPurchase = false)
        {
            hammerPurchased = !cancelPurchase;
            hammerLives = cancelPurchase ? 0 : lives;
            boughtHammerPrice = price;
        }

        public void LoadFrom(LevelSaveData levelSaveData)
        {
            this.version = levelSaveData.version;
            this.hammerPurchased = levelSaveData.hammerPurchased;
            this.hammerLives = levelSaveData.hammerLives;
            this.boughtHammerPrice = levelSaveData.boughtHammerPrice;
            this.remainingFloorsToUnban = levelSaveData.remainingFloorsToUnban;
            this.bannedCharacters = new List<Character>(levelSaveData.bannedCharacters);
            this.remainingFloorsToUnban = new List<int?>(levelSaveData.remainingFloorsToUnban);
            this.topics = new List<TopicBase>(topics);
        }

        /*public void Save(BinaryWriter writer)
        {
            writer.Write(version);
            writer.Write(hammerPurchased);
            writer.Write(boughtHammerPrice);
            writer.Write(hammerLives);

            writer.Write(bannedCharacters.Count);

            for (int i = 0; i < remainingFloorsToUnban.Count; i++)
            {
                writer.Write(remainingFloorsToUnban[i]);
            }
            
            for (int i = 0; i < bannedCharacters.Count; i++)
            {
                writer.Write((int)bannedCharacters[i]);
            }
        }

        public void Load(BinaryReader reader)
        {
            double _version = reader.ReadDouble();
            if (version != _version)
            {
                //AdvancedCore.Logging.LogWarning("Saved version mismatch detected! Converting old to new...!");
                ConvertOldVersionToNew(_version, reader);
                return;
            }
            version = _version;
            hammerPurchased = reader.ReadBoolean();
            boughtHammerPrice = reader.ReadInt32();
            hammerLives = reader.ReadInt32();

            int charactersCount = reader.ReadInt32();

            remainingFloorsToUnban = new List<int>();
            for (int i = 0; i < charactersCount; i++)
            {
                remainingFloorsToUnban.Add(reader.ReadInt32());
            }
            
            bannedCharacters = new List<Character>();
            for (int i = 0; i < charactersCount; i++)
            {
                bannedCharacters.Add((Character)reader.ReadInt32());
            }
            
        }

        public void ConvertOldVersionToNew(double version, BinaryReader reader)
        {
            switch (version)
            {
                case 1d:
                    //structure
                    //needed to avoid data corrupting for other mods
                    reader.ReadBoolean(); //hammer bought
                    int charactersCount = reader.ReadInt32(); //chars count
                    for (int i = 0; i < charactersCount; i++)
                    {
                        reader.ReadInt32(); //banned character
                    }

                    AdvancedCore.Logging.LogWarning("Saved version mismatch detected! Setting all to defaults...");

                    //conversion
                    //...
                    break;
            }
        }
        */

    }
}
