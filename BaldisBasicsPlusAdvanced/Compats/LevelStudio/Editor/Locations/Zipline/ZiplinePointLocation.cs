using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Patches;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline
{
    public class ZiplinePointLocation : PointLocation
    {

        public string hangerPrefab;

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

        public void ConnectTo(ZiplinePointLocation loc)
        {
            connectedPoint = loc;
            loc.connectedPoint = this;

            ZiplineHanger hangerPrefabObject = 
                LevelLoaderPlugin.Instance.structureAliases[structLoc.type].prefabAliases[hangerPrefab].GetComponent<ZiplineHanger>();

            Vector3 pos = position.GetVector3FromCellPosition();
            Vector3 endPos = loc.position.GetVector3FromCellPosition();

            hangerVisual = GameObject.Instantiate(LevelStudioIntegration.hangerVisuals[hangerPrefab]);
            hangerVisual.transform.position = pos + hangerPrefabObject.Offset * (endPos - pos).normalized + Vector3.up * 5f;
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
