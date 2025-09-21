using System.IO.Compression;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline
{
    public class ZiplinePointLocation : PointLocation
    {

        public string hangerPrefab;

        public ushort uses;

        public ushort percentageDistanceToBreak = 50;

        //Not serializable fields
        private bool destroyed;

        private GameObject hangerVisual;

        private ZiplineStructureLocation structLoc;

        private ZiplinePointLocation connectedPoint;

        public LineRenderer renderer;

        public ZiplinePointLocation(ZiplineStructureLocation structLoc)
        {
            this.structLoc = structLoc;
        }

        public int EncodeData()
        {
            int num = 0;

            num |= uses;
            num = num << 16;
            num |= percentageDistanceToBreak;

            return num;
        }

        public void LoadEncodedData(int num)
        {
            uses = (ushort)(num >> 16);
            percentageDistanceToBreak = (ushort)num;
        }

        public void ConnectTo(ZiplinePointLocation loc)
        {
            connectedPoint = loc;
            loc.connectedPoint = this;

            ZiplineHanger hangerPrefabObject = 
                LevelLoaderPlugin.Instance.structureAliases[structLoc.type].prefabAliases[hangerPrefab].GetComponent<ZiplineHanger>();

            uses = (ushort)hangerPrefabObject.MinMaxUses.z;

            Vector3 pos = position.GetVector3FromCellPosition();
            Vector3 endPos = loc.position.GetVector3FromCellPosition();

            hangerVisual = GameObject.Instantiate(LevelStudioIntegration.hangerVisuals[hangerPrefab]);
            hangerVisual.transform.position = pos + hangerPrefabObject.Offset * (endPos - pos).normalized + Vector3.up * 5f;
            hangerVisual.GetComponent<EditorSettingsableComponent>().OnSettingsClicked = OnSettingsClicked;
        }

        private void OnSettingsClicked()
        {
            Singleton<EditorController>.Instance.HoldUndo();

            ZiplineExchangeHandler handler = EditorController.Instance.CreateUI<ZiplineExchangeHandler>(
                "GumDispenserConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/ZiplineHangerConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            base.CleanupVisual(visualObject);
            destroyed = true;
            structLoc.locations.Remove(this);

            if (hangerVisual != null) GameObject.Destroy(hangerVisual.gameObject);
            if (renderer != null) GameObject.Destroy(renderer.gameObject);
            if (connectedPoint != null && !connectedPoint.destroyed) EditorController.Instance.RemoveVisual(connectedPoint);
        }

    }
}
