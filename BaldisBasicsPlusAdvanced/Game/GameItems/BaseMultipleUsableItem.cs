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

        public void Initialize(params ItemObject[] allVersions)
        {
            this.allVersions = allVersions;
            this.uses = allVersions.Length;
        }

        protected bool OnUse()
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
