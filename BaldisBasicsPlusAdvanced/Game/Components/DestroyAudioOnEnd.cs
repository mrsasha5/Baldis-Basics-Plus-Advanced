using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Components
{
    public class DestroyAudioOnEnd : MonoBehaviour
    {
        [SerializeField]
        private AudioManager audioManager;

        [SerializeField]
        private bool destroyWithObject;

        public void Assign(AudioManager audMan, bool destroyWithObject)
        {
            audioManager = audMan;
            this.destroyWithObject = destroyWithObject;
        }

        private void Update()
        {
            if (!audioManager.AnyAudioIsPlaying)
            {
                if (destroyWithObject)
                {
                    Destroy(this);
                    Destroy(audioManager.gameObject);
                } else
                {
                    Destroy(this);
                    Destroy(audioManager);
                }
            }
        }

    }
}
