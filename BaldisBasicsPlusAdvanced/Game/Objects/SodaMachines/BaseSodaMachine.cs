using BaldisBasicsPlusAdvanced.Helpers;
using System.Collections;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines
{
    public class BaseSodaMachine : SodaMachine, IItemAcceptor//EnvironmentObject, IItemAcceptor//SodaMachine, IItemAcceptor
    {
        void IItemAcceptor.InsertItem(PlayerManager pm, EnvironmentController ec)
        {
            StartCoroutine(Delay(pm));
            int usesLeft = ReflectionHelper.GetValue<int>(this, "usesLeft");
            usesLeft--;
            ReflectionHelper.SetValue<int>(this, "usesLeft", usesLeft);

            MeshRenderer meshRenderer = ReflectionHelper.GetValue<MeshRenderer>(this, "meshRenderer");
            if (usesLeft <= 0 && meshRenderer != null)
            {
                Material[] _materials = meshRenderer.sharedMaterials;
                _materials[1] = ReflectionHelper.GetValue<Material>(this, "outOfStockMat");
                meshRenderer.sharedMaterials = _materials;
                ReflectionHelper.SetValue<Material[]>(this, "_materials", _materials);
            }
        }

        protected IEnumerator Delay(PlayerManager pm)
        {
            WeightedSelection<ItemObject>[] potentialItems = ReflectionHelper.GetValue<WeightedSelection<ItemObject>[]>(this, "potentialItems");
            yield return null;
            if (potentialItems.Length != 0)
            {
                ItemManager itm = pm.itm;
                WeightedSelection<ItemObject>[] items = potentialItems;
                itm.AddItem(WeightedSelection<ItemObject>.RandomSelection(items));
            }
            else
            {
                pm.itm.AddItem(ReflectionHelper.GetValue<ItemObject>(this, "item"));
            }
        }
    }
}
