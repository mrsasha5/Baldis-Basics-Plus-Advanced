using System;
using System.Collections.Generic;
using System.Text;
using BaldisBasicsPlusAdvanced.Helpers;

namespace BaldisBasicsPlusAdvanced.Compats
{
    public class ModsIntegration
    {

        public static bool EndlessInstalled => AssetsHelper.modInstalled("mtm101.rulerp.baldiplus.endlessfloors");

        //public static bool ExtraInstalled => AssetsHelper.modInstalled("rost.moment.baldiplus.extramod");

        //public static bool CarnellInstalled => AssetsHelper.modInstalled("alexbw145.baldiplus.bcarnellchars");

        //public static bool CarnivalInstalled => AssetsHelper.modInstalled("mtm101.rulerp.bbplus.carnivalpackroot");

        public static bool LevelEditorInstalled => AssetsHelper.modInstalled(LevelEditorId);

        public static bool LevelLoaderInstalled => AssetsHelper.modInstalled(LevelLoaderId);

        public const string LevelEditorId = "mtm101.rulerp.baldiplus.leveleditor";

        public const string LevelLoaderId = "mtm101.rulerp.baldiplus.levelloader";

    }
}
