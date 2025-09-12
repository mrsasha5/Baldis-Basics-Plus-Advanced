using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaldisBasicsPlusAdvanced.Game.Builders;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline
{
    public class ZiplineStructureLocation : StructureLocation
    {

        public List<ZiplinePointLocation> locations = new List<ZiplinePointLocation>();

        private List<LineRenderer> lineRenderers = new List<LineRenderer>();

        public virtual ZiplinePointLocation CreateNewChild(string hangerPrefab, IntVector2 pos)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].position == pos)
                {
                    return null;
                }
            }

            ZiplinePointLocation loc = new ZiplinePointLocation
            {
                hangerPrefab = hangerPrefab, //Serializable prefab
                prefab = "adv_zipline_pillar", //Prefab for editor only
                position = pos,
                deleteAction = OnDeleteLocation
            };

            locations.Add(loc);

            if (locations.Count % 2 == 0)
            {
                Structure_Zipline builderPre = 
                    (Structure_Zipline)PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type].structure;

                LineRenderer renderer = builderPre.GetLineRenderer();
                builderPre.UpdateRenderer(renderer, 
                    locations[locations.Count - 2].position.ToWorld(),
                    locations[locations.Count - 1].position.ToWorld());

                lineRenderers.Add(renderer);

                loc.renderer = renderer;
            }

            return loc;
        }

        private bool OnDeleteLocation(EditorLevelData level, PointLocation loc)
        {
            int index = locations.IndexOf((ZiplinePointLocation)loc);

            AttemptToDestroyRenderer(loc);

            if ((index + 1) % 2 != 0)
            {
                AttemptToDestroyRenderer(locations[index + 1]);

                EditorController.Instance.RemoveVisual(locations[index]);
                EditorController.Instance.RemoveVisual(locations[index + 1]);

                locations.RemoveAt(index);
                locations.RemoveAt(index);
            }
            else
            {
                AttemptToDestroyRenderer(locations[index - 1]);

                EditorController.Instance.RemoveVisual(locations[index]);
                EditorController.Instance.RemoveVisual(locations[index - 1]);

                locations.RemoveAt(index);
                locations.RemoveAt(index - 1);
            }

            return true;
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            foreach (LineRenderer renderer in lineRenderers)
            {
                if (renderer == null) continue;
                GameObject.Destroy(renderer.gameObject);
            }
        }

        public override StructureInfo Compile(EditorLevelData data, BaldiLevel level)
        {
            StructureInfo structInfo = new StructureInfo(type);

            for (int i = 0; i < locations.Count; i++)
            {
                structInfo.data.Add(new StructureDataInfo()
                {
                    prefab = locations[i].hangerPrefab,
                    position = new MystIntVector2(locations[i].position.x, locations[i].position.z),
                    direction = PlusDirection.North
                });
            }

            return structInfo;
        }

        public override GameObject GetVisualPrefab()
        {
            return null;
        }

        public override void InitializeVisual(GameObject visualObject)
        {
            Debug.Log("InitializeVisual");
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.AddVisual(locations[i]);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            Debug.Log("UpdateVisual");
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.UpdateVisual(locations[i]);
            }
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            return true;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            reader.ReadByte(); //Version
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string hangerPrefab = compressor.ReadStoredString(reader);
                IntVector2 position = reader.ReadByteVector2().ToInt();

                ZiplinePointLocation loc = CreateNewChild(hangerPrefab, position);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write((byte)1); //Version
            writer.Write(locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                compressor.WriteStoredString(writer, locations[i].hangerPrefab);
                writer.Write(locations[i].position.ToByte());
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(locations.Select((x) => x.hangerPrefab));
        }

        private void AttemptToDestroyRenderer(PointLocation loc)
        {
            ZiplinePointLocation zipLoc = (ZiplinePointLocation)loc;
            if (zipLoc.renderer != null)
            {
                GameObject.Destroy(zipLoc.renderer.gameObject);
            }
        }

    }
}
