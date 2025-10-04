using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using BaldisBasicsPlusAdvanced.Game.Builders;
using PlusLevelStudio;
using PlusLevelStudio.Editor;
using PlusStudioLevelFormat;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Compats.LevelStudio.Editor.Locations.Zipline
{
    public class ZiplineStructureLocation : StructureLocation
    {

        public const byte formatVersion = 0;

        public List<ZiplinePointLocation> locations = new List<ZiplinePointLocation>();

        public virtual ZiplinePointLocation CreateNewChild(
            EditorLevelData levelData, string hangerPrefab, IntVector2 pos, bool disableChecks = false)
        {
            if (!disableChecks)
            {
                for (int i = 0; i < locations.Count; i++)
                {
                    if (locations[i].position == pos)
                    {
                        return null;
                    }
                }
            }

            ZiplinePointLocation loc = new ZiplinePointLocation(this)
            {
                hangerPrefab = hangerPrefab, //Prefab for Structure Builder
                prefab = "adv_zipline_pillar", //Prefab for editor only
                position = pos,
                deleteAction = OnDeleteLocation
            };

            if (!disableChecks && !loc.ValidatePosition(levelData, ignoreSelf: false)) return null;

            locations.Add(loc);

            Structure_Zipline builderPre = 
                (Structure_Zipline)PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type].structure;

            if (locations.Count % 2 == 0)
            {
                loc.renderer = locations[locations.Count - 2].renderer;
                builderPre.UpdateRenderer(loc.renderer, 
                    locations[locations.Count - 2].position.ToWorld(),
                    locations[locations.Count - 1].position.ToWorld());
            }
            else
            {
                loc.renderer = builderPre.GetLineRenderer();
            }
            
            return loc;
        }


        private bool OnDeleteLocation(EditorLevelData level, PointLocation loc)
        {
            EditorController.Instance.RemoveVisual(loc);
            return true;
        }

        public override void CleanupVisual(GameObject visualObject)
        {
            
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
                    direction = PlusDirection.North,
                    data = locations[i].EncodeData()
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
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.AddVisual(locations[i]);
            }
        }

        public override void UpdateVisual(GameObject visualObject)
        {
            Structure_Zipline builderPre =
                    (Structure_Zipline)PlusStudioLevelLoader.LevelLoaderPlugin.Instance.structureAliases[type].structure;
            for (int i = 0; i < locations.Count; i++)
            {
                EditorController.Instance.UpdateVisual(locations[i]);

                if (i % 2 != 0)
                {
                    builderPre.UpdateRenderer(locations[i].renderer,
                        locations[i - 1].position.ToWorld(),
                            locations[i].position.ToWorld());
                }
            }
        }

        public override void ShiftBy(Vector3 worldOffset, IntVector2 cellOffset, IntVector2 sizeDifference)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                locations[i].position -= cellOffset;
            }
        }

        public override bool ValidatePosition(EditorLevelData data)
        {
            if (locations.Count == 0) return false;

            for (int i = 0; i < locations.Count; i++)
            {
                if (!locations[i].ValidatePosition(data, ignoreSelf: false))
                {
                    EditorController.Instance.RemoveVisual(locations[i]);
                    if ((i + 1) % 2 == 0) i -= 2;
                    else i--;
                }
            }

            return true;
        }

        public override void ReadInto(EditorLevelData data, BinaryReader reader, StringCompressor compressor)
        {
            byte ver = reader.ReadByte();

#warning Postponed change for testers. Also related to other structure locations.
            //if (ver > formatVersion)
                //throw new System.Exception(LevelStudioIntegration.standardMsg_StructureVersionException);

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string hangerPrefab = compressor.ReadStoredString(reader);
                IntVector2 position = reader.ReadByteVector2().ToInt();
                int parametersData = reader.ReadInt32();

                ZiplinePointLocation loc = CreateNewChild(data, hangerPrefab, position, disableChecks: true);
                loc.LoadEncodedData(parametersData);
            }

            for (int i = 0; i < locations.Count; i += 2)
            {
                locations[i].ConnectTo(locations[i + 1]);
            }
        }

        public override void Write(EditorLevelData data, BinaryWriter writer, StringCompressor compressor)
        {
            writer.Write(formatVersion);
            writer.Write(locations.Count);
            for (int i = 0; i < locations.Count; i++)
            {
                compressor.WriteStoredString(writer, locations[i].hangerPrefab);
                writer.Write(locations[i].position.ToByte());
                writer.Write(locations[i].EncodeData());
            }
        }

        public override void AddStringsToCompressor(StringCompressor compressor)
        {
            compressor.AddStrings(locations.Select((x) => x.hangerPrefab));
        }

    }
}
