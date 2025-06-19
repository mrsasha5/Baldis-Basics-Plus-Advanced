using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.Game.Objects;
using BaldisBasicsPlusAdvanced.Patches;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class PlaceableFanItem : Item, IPrefab
    {
        [SerializeField]
        private CellCoverage cover;

        public void InitializePrefab(int variant)
        {
            cover = CellCoverage.Up | CellCoverage.Center| CellCoverage.Down;
        }

        public override bool Use(PlayerManager pm)
        {
            Cell cell = pm.ec.CellFromPosition(pm.transform.position);

            if (cell != null && !cell.Null && cell.AllCoverageFits(cover))
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
