using BepInEx;

namespace TestModForAdvanced
{
    // This project exists only for API tests.
    [BepInPlugin("mrsasha5.baldiplus.advancedapitest", "Advanced API test", "0.0.0.0")]
    public class TestPlugin : BaseUnityPlugin 
    {
        private void Awake()
        {

        }

        // This is required for finding your event listener.
        // Ps: I did not implement it yet.
        private object AdvancedEdition_GetEventListener()
        {
            return new AdvancedEdition_EventListener();
        }
    }
}
