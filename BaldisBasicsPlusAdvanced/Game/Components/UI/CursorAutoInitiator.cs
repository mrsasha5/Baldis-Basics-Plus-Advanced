using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI
{
    public class CursorAutoInitiator : MonoBehaviour
    {

        public CursorInitiator initiator;

        public bool correctCursorPriority = true;

        public int indexAddend;

        private void Update()
        {
            if (Time.deltaTime != 0f && initiator.currentCursor == null)
            {
                initiator.Inititate();
                if (correctCursorPriority) initiator.currentCursor.transform.SetSiblingIndex(initiator.currentCursor.transform.parent.childCount + indexAddend);
            }
        }


    }
}
