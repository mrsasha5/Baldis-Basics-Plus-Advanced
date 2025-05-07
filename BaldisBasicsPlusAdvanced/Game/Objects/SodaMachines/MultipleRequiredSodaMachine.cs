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
        }
    }
}
