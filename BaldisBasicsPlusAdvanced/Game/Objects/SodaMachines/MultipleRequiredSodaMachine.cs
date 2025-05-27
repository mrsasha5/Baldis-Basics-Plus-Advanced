using BaldisBasicsPlusAdvanced.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.SodaMachines
{
    public class MultipleRequiredSodaMachine : BaseSodaMachine, IItemAcceptor
    {
        public int requiredAmmount;

        void IItemAcceptor.InsertItem(PlayerManager pm, EnvironmentController ec)
        {
            requiredAmmount--;
            if (requiredAmmount < 1)
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
        }
    }
}
