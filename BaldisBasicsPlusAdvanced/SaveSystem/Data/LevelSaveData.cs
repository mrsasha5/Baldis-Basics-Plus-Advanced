using BaldisBasicsPlusAdvanced.Game.Objects.Voting.Topics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.SaveSystem.Data
{
    //[Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class LevelSaveData
    {
        //versions list
        //1d
        //2d
        //3f

        public float version = 3f;

        public List<Character> bannedCharacters = new List<Character>();

        public List<int?> remainingFloorsToUnban = new List<int?>();

        public bool hammerPurchased;

        public int hammerLives;

        public int boughtHammerPrice;

        public void OnLoadNextLevel(bool afterPitStop)
        {
        }

        public void OnLoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            if (sceneObject.manager is PitstopGameManager && !restarting)
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

            }
        }

        public void OnLoad()
        {
        }

        public void OnSave()
        {
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
            version = levelSaveData.version;
            hammerPurchased = levelSaveData.hammerPurchased;
            hammerLives = levelSaveData.hammerLives;
            boughtHammerPrice = levelSaveData.boughtHammerPrice;
            remainingFloorsToUnban = levelSaveData.remainingFloorsToUnban;
            bannedCharacters = new List<Character>(levelSaveData.bannedCharacters);
            remainingFloorsToUnban = new List<int?>(levelSaveData.remainingFloorsToUnban);
        }

    }
}
