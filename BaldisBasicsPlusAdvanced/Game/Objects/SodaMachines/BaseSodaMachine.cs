using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines
{
    //TODO: REFACTOR!!! 
    //USE BASE SODA MACHINE CLASS
    //REFLECTION.
    //EXPLICIT IMPLEMENTATION OF INTERFACE.
    [Obsolete("Idk what are you doing here, but i will inherit SodaMachine class when Pixelguy will fix own api...")]
    public class BaseSodaMachine : EnvironmentObject, IItemAcceptor//SodaMachine, IItemAcceptor
    {
        //ALL FIELDS TO REMOVE
        [SerializeField]
        private WeightedItemObject[] potentialItems;

        [SerializeField]
        private ItemObject item;

        [SerializeField]
        private ItemObject requiredItem;

        [SerializeField]
        private MeshRenderer meshRenderer;

        private Material[] _materials = new Material[0];

        [SerializeField]
        private Material outOfStockMat;

        [SerializeField]
        private int usesLeft = 1;

        //void IItemAcceptor.InsertItem(PlayerManager pm, EnvironmentController ec)
        public void InsertItem(PlayerManager pm, EnvironmentController ec)
        {
            StartCoroutine(Delay(pm));
            int usesLeft = ReflectionHelper.getValue<int>(this, "usesLeft");
            usesLeft--;
            ReflectionHelper.setValue<int>(this, "usesLeft", usesLeft);
            MeshRenderer meshRenderer = ReflectionHelper.getValue<MeshRenderer>(this, "meshRenderer");
            if (usesLeft <= 0 && meshRenderer != null)
            {
                Material[] _materials = meshRenderer.sharedMaterials;
                _materials[1] = ReflectionHelper.getValue<Material>(this, "outOfStockMat");
                meshRenderer.sharedMaterials = _materials;
                ReflectionHelper.setValue<Material[]>(this, "_materials", _materials);
            }
        }

        //REMOVE WHEN INHERIT!!
        public bool ItemFits(Items checkItem)
        {
            if (ReflectionHelper.getValue<ItemObject>(this, "requiredItem").itemType == checkItem)
            {
                return ReflectionHelper.getValue<int>(this, "usesLeft") > 0;
            }

            return false;
        }

        protected IEnumerator Delay(PlayerManager pm)
        {
            WeightedSelection<ItemObject>[] potentialItems = ReflectionHelper.getValue<WeightedSelection<ItemObject>[]>
                    (this, "potentialItems");
            yield return null;
            if (potentialItems.Length != 0)
            {
                ItemManager itm = pm.itm;
                itm.AddItem(WeightedSelection<ItemObject>.RandomSelection(potentialItems));
            }
            else
            {
                pm.itm.AddItem(ReflectionHelper.getValue<ItemObject>
                    (this, "item"));
            }
        }
    }
}
