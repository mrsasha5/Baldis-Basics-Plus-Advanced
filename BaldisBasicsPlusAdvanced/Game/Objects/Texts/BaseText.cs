using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Texts
{
    public class BaseText : MonoBehaviour
    {

        [SerializeField]
        protected TMP_Text text;

        public void Setup(TMP_Text text)
        {
            this.text = text;
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }

    }
}
