using System.IO;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.UI;
using BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Visuals;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Helpers;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline
{
    internal class ZiplinePointLocation : PointLocation
    {

        public string prefabForBuilder;

        public ushort uses;

        public ushort percentageDistanceToBreak = 50;

        //Not serializable fields
        private bool destroyed;

        private GameObject hangerVisual;

        private ZiplinePointLocation connectedPoint;

        public ZiplineStructureLocation owner;

        public LineRenderer renderer;

        public void Compile(StructureInfo info)
        {
            info.data.Add(new StructureDataInfo()
            {
                prefab = prefabForBuilder,
                position = PlusStudioLevelLoader.Extensions.ToData(position), //I am writing this way because
                                                                              //extension methods from Loader
                                                                              //and Studio are conflicting and when
                                                                              //I am using PlusStudioLevelLoader namespace
                                                                              //I cannot use .ToInt() for ByteVector2
                                                                              //anymore.
                direction = PlusDirection.North,
                data = EncodeData()
            });
        }

        private int EncodeData()
        {
            int num = 0;

            num |= uses;
            num = num << 16;
            num |= percentageDistanceToBreak;

            return num;
        }

        public void ReadData(BinaryReader reader, StringCompressor compressor)
        {
            prefabForBuilder = compressor.ReadStoredString(reader);
            position = reader.ReadByteVector2().ToInt();

            uses = reader.ReadUInt16();
            percentageDistanceToBreak = reader.ReadUInt16();
        }

        public void WriteData(BinaryWriter writer, StringCompressor compressor)
        {
            compressor.WriteStoredString(writer, prefabForBuilder);
            writer.Write(position.ToByte());

            writer.Write(uses);
            writer.Write(percentageDistanceToBreak);
        }

        public void LoadDefaults()
        {
            ZiplineHanger hangerPrefab =
                LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder].GetComponent<ZiplineHanger>();

            uses = (ushort)hangerPrefab.MinMaxUses.z;
        }

        public void ConnectTo(ZiplinePointLocation loc)
        {
            connectedPoint = loc;
            loc.connectedPoint = this;

            ZiplineHanger hangerPrefab = 
                LevelLoaderPlugin.Instance.structureAliases[owner.type].prefabAliases[prefabForBuilder].GetComponent<ZiplineHanger>();

            Vector3 pos = position.GetVector3FromCellPosition();
            Vector3 endPos = loc.position.GetVector3FromCellPosition();

            hangerVisual = GameObject.Instantiate(LevelStudioIntegration.GetVisualPrefab(owner.type, prefabForBuilder));
            hangerVisual.transform.position = pos + hangerPrefab.Offset * (endPos - pos).normalized + Vector3.up * 5f;
            hangerVisual.GetComponent<EditorSettingsableComponent>().OnSettingsClicked = OnSettingsClicked;
            hangerVisual.GetComponent<EditorDeletableObject>().toDelete = this;
        }

        private void OnSettingsClicked()
        {
            Singleton<EditorController>.Instance.HoldUndo();

            ZiplineExchangeHandler handler = EditorController.Instance.CreateUI<ZiplineExchangeHandler>(
                "ZiplineConfig", AssetsHelper.modPath + "Compats/LevelStudio/UI/ZiplineHangerConfig.json");
            handler.OnInitialized(this);
            handler.Refresh();
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            base.CleanupVisual(visualObject);
            destroyed = true;
            owner.locations.Remove(this);

            if (hangerVisual != null) GameObject.Destroy(hangerVisual.gameObject);
            if (renderer != null) GameObject.Destroy(renderer.gameObject);
            if (connectedPoint != null && !connectedPoint.destroyed) EditorController.Instance.RemoveVisual(connectedPoint);
        }

    }
}
