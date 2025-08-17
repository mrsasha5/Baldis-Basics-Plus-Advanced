using BaldisBasicsPlusAdvanced.Compats.SpatialElevator.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using The3DElevator.MonoBehaviours.ElevatorCoreComponents;
using The3DElevator.MonoBehaviours.ElevatorObjects;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator
{
    public class SpatialElevatorIntegration : CompatibilityModule
    {

        public SpatialElevatorIntegration() : base()
        {
            guid = "pixelguy.pixelmodding.baldiplus.3delevator";
            versionInfo = new VersionInfo(this);

            CreateConfigValue("3D Elevator",
                "Adds a basic mod features for 3D Elevator!\n" +
                "Not recommended to disable since you'll lose access to the content like: tips & Hammer of Force.");
        }

        protected override void InitializeOnAssetsLoadPost()
        {
            base.InitializeOnAssetsLoadPost();

            LobbyElevator elv = AssetsHelper.LoadAsset<LobbyElevator>();

            //Chalkboard
            SpatialChalkboard board =
                new GameObject("Spatial Chalkboard").AddComponent<SpatialChalkboard>();
            board.transform.SetParent(elv.transform, false);
            board.transform.localPosition = new Vector3(-22.3f, 17f, 11f);
            board.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            board.InitializePrefab(1);
            //Chalkboard ends

            //Tips Screen
            TextMeshPro tipText = ObjectsCreator.CreateTextMesh(
                BaldiFonts.ComicSans12, new Vector2(35f, 100f), null, Vector3.zero);
            tipText.transform.localScale = Vector3.one * 0.5f;
            tipText.color = Color.green;

            Transform mainParent = new GameObject("Screen").transform;
            mainParent.SetParent(elv.transform, false);
            mainParent.localPosition = new Vector3(0f, 10f, 18f);
            mainParent.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject screenObj = GameObject.Instantiate(
                AssetLoader.ModelFromFile(AssetsHelper.modPath + "Models/TipsScreen/TipPanelModified.obj"));
            screenObj.name = "Model";
            screenObj.transform.SetParent(mainParent, false);

            Collider collider = screenObj.GetComponent<Collider>();
            GameObject.Destroy(collider);

            tipText.transform.parent = mainParent;
            tipText.transform.localPosition = new Vector3(0f, 11.8f, 4.05f);

            screenObj.transform.localScale = Vector3.one * 0.5f;

            ElevatorAppearanceHole hole = GameObject.Instantiate(AssetsHelper.LoadAsset<ElevatorAppearanceHole>());
            hole.name = "AppearanceHole_SpatialTipsScreen";
            hole.transform.SetParent(elv.transform, false);

            SpatialTipsMonitor tipsScreen = mainParent.gameObject.AddComponent<SpatialTipsMonitor>();
            tipsScreen.InitializePrefab(1);
            tipsScreen.hole = hole;
            //Tips Screen ends
        }

    }
}
