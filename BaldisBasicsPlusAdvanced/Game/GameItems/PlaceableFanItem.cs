using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Patches;
using System.Collections.Generic;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class PlaceableFanItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            //List<Cell> potentialCells = pm.ec.AllTilesNoGarbage(includeOffLimits: false, includeWithHardCoverage: false);
            Cell cell = pm.ec.CellFromPosition(pm.transform.position);

            if (cell != null && !cell.Null && !cell.HasAnyHardCoverage)/*(cell != null && potentialCells.Contains(cell) && cell.room.entitySafeCells.Find(x => x == cell.position) != null
                && cell.room.eventSafeCells.Find(x => x == cell.position) != null)*/
            { 
                Fan fan = Instantiate(ObjectsStorage.Entities["Fan"]).GetComponent<Fan>();

                GameCamera gameCamera = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber);

                fan.Initialize(pm.ec, gameCamera.transform.position, gameCamera.transform.rotation, livingTime: 20f, turnOff: true);

                Destroy(gameObject);
                return true;
            } else
            {
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
                Destroy(gameObject);
                return false;
            }
        }
    }
}
