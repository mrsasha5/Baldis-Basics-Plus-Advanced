using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects.Portals;
using BaldisBasicsPlusAdvanced.Patches;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class PlaceablePortalItem : BaseMultipleUsableItem
    {

        public override bool Use(PlayerManager pm)
        {
            this.pm = pm;
            //includeWithHardCoverage - with objects
            List<Cell> potentialCells = pm.ec.AllTilesNoGarbage(includeOffLimits: false, includeWithHardCoverage: false);
            Cell cell = pm.ec.CellFromPosition(pm.transform.position);
            if (cell != null && potentialCells.Contains(cell) && cell.room.entitySafeCells.Find(x => x == cell.position) != null
                && cell.room.eventSafeCells.Find(x => x == cell.position) != null)
            {
                MysteriousPortal portalToConnect = null;
                foreach (MysteriousPortal _portal in FindObjectsOfType<MysteriousPortal>())
                {
                    if (_portal.IsPlayerPortal && !_portal.Connected)
                    {
                        portalToConnect = _portal;
                        break;
                    }
                }

                MysteriousPortal portal = Instantiate(ObjectsStorage.Objects["mysterious_portal"].GetComponent<MysteriousPortal>());
                portal.transform.position = cell.TileTransform.position + Vector3.up * 5f;
                portal.PostInitialize(pm.ec, true);
                portal.SetStatic(true);

                if (portalToConnect != null)
                {
                    portalToConnect.ConnectTo(portal);
                    portal.ConnectTo(portalToConnect);
                }

                portal.Activate();

                Destroy(gameObject);
                return ReturnOnUse();
            } else
            {
                pm.ec.GetAudMan().PlaySingle(AssetsStorage.sounds["error_maybe"]);
                Destroy(gameObject);
                return false;
            }
            //return false;
        }

    }
}
