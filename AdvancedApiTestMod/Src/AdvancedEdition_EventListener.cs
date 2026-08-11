using System;

namespace TestModForAdvanced
{
    // No dependencies anymore for SIMPLE goals
    internal class AdvancedEdition_EventListener
    {
        private string[] GetSymbolMachineWords()
        {
            // Yeah, you can put literally english words in the code
            // Because Symbol Machine was not designed to be a multi-language support
            return new string[]
            {
                "just",
                "new",
                "words",
                "LOL"
            };
        }

        private string[] GetTipLocalizationKeys()
        {
            return new string[]
            {
                "PLEASE STOP LEAVING HERE ACTUAL TIP TEXT",
                "USE THE LOCALIZATION KEYS OH MY GOD",
                "localization_example_key_1",
                "localization_example_key_2",
                "localization_example_key_3",
            };
        }

        private Type[] GetForbiddenButtonReceivers()
        {
            return new Type[] { typeof(TestPlugin) };
        }
    }
}
