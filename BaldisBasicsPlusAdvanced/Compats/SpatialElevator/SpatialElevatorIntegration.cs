using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.UI;
using The3DElevator.MonoBehaviours.ElevatorCoreComponents;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.SpatialElevator
{
    public class SpatialElevatorIntegration : CompatibilityModule
    {

        public override bool IsIntegrable()
        {
            return base.IsIntegrable() && AdvancedCore.spatialElevatorIntegrationEnabled;
        }

        public SpatialElevatorIntegration() : base()
        {
            guid = "pixelguy.pixelmodding.baldiplus.3delevator";
            versionInfo = new VersionInfo(this);
        }

        protected override void InitializeOnAssetsLoadPost()
        {
            base.InitializeOnAssetsLoadPost();

            TextMeshPro tmpText = ObjectsCreator.CreateTextMesh(
                BaldiFonts.ComicSans12, new Vector2(35f, 100f), null, Vector3.zero);
            tmpText.transform.localScale = Vector3.one * 0.5f;
            tmpText.color = Color.green;

            LobbyElevator elv = AssetsHelper.LoadAsset<LobbyElevator>();

            Transform mainParent = new GameObject("Screen").transform;
            mainParent.SetParent(elv.transform, false);
            mainParent.localPosition = new Vector3(0f, 10f, 18f);
            mainParent.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject screenObj = GameObject.Instantiate(
                AssetLoader.ModelFromFile(AssetsHelper.modPath + "Models/TipsScreen/TipThing.obj"));
            screenObj.name = "Model";
            screenObj.transform.SetParent(mainParent, false);

            Collider collider = screenObj.GetComponent<Collider>();
            GameObject.Destroy(collider);

            tmpText.transform.parent = mainParent;
            tmpText.transform.localPosition = new Vector3(0f, 12f, 4.05f);

            screenObj.transform.localScale = Vector3.one * 0.5f;
        }

    }
}
