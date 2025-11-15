using System;
using System.Collections.Generic;
using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.Base;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI.UI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Builders
{
    public class Structure_PitStopLevelStove : StructureBuilder, IPrefab
    {
        [SerializeField]
        private GameButton buttonPre;

        [SerializeField]
        private PosterObject standardNeonPosterPre;

        [SerializeField]
        private PosterObject infoPosterPre;

        [SerializeField]
        private PosterObject buttonInfoPosterPre;

        public void InitializePrefab(int variant)
        {
            buttonPre = AssetsStorage.gameButton;
            standardNeonPosterPre = AssetsHelper.LoadAsset<PosterObject>("StoreNeon_4");
            buttonInfoPosterPre = Instantiate(AssetsHelper.LoadAsset<PosterObject>("StoreNeon_4_wRestockBase"));
            buttonInfoPosterPre.name = "Adv_Poster_PIT_Stove_Button";
            buttonInfoPosterPre.textData[0].textKey = "Adv_PST_Cook_1";
            buttonInfoPosterPre.textData[1].textKey = "Adv_PST_Cook_2";
            buttonInfoPosterPre.textData[2].textKey = "50";

            infoPosterPre = ObjectsStorage.Posters.Find(x => x.name == "Adv_Poster_Kitchen_Stove");
        }

        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            StoreRoomFunction storeFunc = FindObjectOfType<StoreRoomFunction>();
            ec.CreateCell(12, new IntVector2(28, 10), storeFunc.Room);
            ec.CreateCell(9, new IntVector2(28, 11), storeFunc.Room);
            ec.CreateCell(6, new IntVector2(29, 10), storeFunc.Room);
            ec.CreateCell(3, new IntVector2(29, 11), storeFunc.Room);
            ec.ConnectCells(new IntVector2(29, 10), Direction.East);

            ec.GenerateLight(ec.cells[28, 11], Color.white, 8);
            ec.BuildPoster(buttonInfoPosterPre, ec.cells[29, 10], Direction.South);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[28, 10], Direction.South);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[28, 10], Direction.West);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[28, 11], Direction.West);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[28, 11], Direction.North);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[29, 11], Direction.North);
            ec.BuildPoster(standardNeonPosterPre, ec.cells[29, 11], Direction.East);
            ec.BuildPoster(infoPosterPre, ec.cells[30, 11], Direction.West);

            JohnnyKitchenStove stove = Instantiate(ObjectsStorage.Objects["johnny_kitchen_stove"].GetComponent<JohnnyKitchenStove>(), 
                ec.cells[28, 10].room.objectObject.transform);

            stove.transform.position = ec.cells[28, 10].FloorWorldPosition;
            stove.Assign(storeFunc);

            GameButton button = (GameButton)GameButton.Build(buttonPre, ec, default, Direction.South);
            button.transform.position = new Vector3(293f, 0f, 105f);
            button.transform.parent = storeFunc.Room.objectObject.transform;
            button.SetUp(stove);
        }

    }
}
