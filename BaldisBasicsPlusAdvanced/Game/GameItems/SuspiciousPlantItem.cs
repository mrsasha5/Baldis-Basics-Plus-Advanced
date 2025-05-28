/*using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class SuspiciousPlantItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            Cell cell = pm.ec.CellFromPosition(pm.transform.position);
            
            if (cell != null && !cell.Null && !cell.HasAnyHardCoverage)
            {
                Vector3 offset = default;
                if (cell.AllWallDirections.Count > 0)
                {
                    offset = cell.AllWallDirections[0].ToVector3() * 3f;
                    cell.HardCover(cell.AllWallDirections[0].ToCoverage());
                } else
                {
                    cell.HardCover(CellCoverage.Center | CellCoverage.Down);
                }

                Instantiate(ObjectsStorage.Objects["suspicious_plant"])
                    .transform.position = pm.transform.position.CorrectForCell() + offset + Vector3.up * -5f;
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["slap"]);

                Destroy(gameObject);
                return true;
            }
            
            pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);

            Destroy(gameObject);
            return false;
        }
    }
}
*/