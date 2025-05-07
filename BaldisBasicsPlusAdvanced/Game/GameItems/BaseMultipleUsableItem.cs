using System;
using System.Collections.Generic;
using System.Text;
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

        public void initialize(params ItemObject[] allVersions)
        {
            this.allVersions = allVersions;
            this.uses = allVersions.Length;
        }

        protected bool onUse()
        {
            if (uses > 0)
            {
                pm.itm.SetItem(allVersions[uses - 1], pm.itm.selectedItem);
                return true;
            }
            return false;
        }
    }
}
