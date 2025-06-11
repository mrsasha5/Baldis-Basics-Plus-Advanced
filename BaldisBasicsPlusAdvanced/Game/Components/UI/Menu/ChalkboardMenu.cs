using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaldisBasicsPlusAdvanced.Game.Components.UI.Menu
{
    public class ChalkboardMenu : MonoBehaviour
    {
        public Canvas canvas;

        public Image chalkboard;

        public List<TMP_Text> texts = new List<TMP_Text>();

        public List<StandardMenuButton> buttons = new List<StandardMenuButton>();

        public TMP_Text GetText(string name)
        {
            return texts.Find(x => x.name == name);
        }

        public StandardMenuButton GetButton(string name)
        {
            return buttons.Find(x => x.name == name);
        }
    }
}
