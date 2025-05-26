using System;
using System.Linq;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.GameItems
{
    public class BaseMultipleUsableItem : Item
    {
        [SerializeField]
        protected ItemObject[] allVersions;

        [SerializeField] 
        protected int uses;

        public ItemObject[] AllVersions => allVersions;

        public void Initialize(params ItemObject[] allVersions)
        {
            this.allVersions = allVersions;
            this.uses = allVersions.Length;
            for (int i = 0; i < allVersions.Length; i++)
            {
                BaseMultipleUsableItem item = (BaseMultipleUsableItem)allVersions[i].item;
                item.allVersions = allVersions;
                item.uses = i;
            }
        }

        protected bool ReturnOnUse()
        {
            if (uses > 0)
            {
                pm.itm.SetItem(allVersions[uses - 1], pm.itm.selectedItem);
                return false;
            }
            return true;
        }
    }
}
