/*using System;
using System.Collections.Generic;
using System.Linq;
using BaldisBasicsPlusAdvanced.Helpers;
using BepInEx.Bootstrap;

namespace BaldisBasicsPlusAdvanced.AutoUpdate
{
    internal class CompatibilityAnalyzer
    {

        public enum Result
        {
            Success,
            ForcedDependenciesFits,
            NewForcedDependenciesAdded,
            Failed
        }

        public static Result GetLastCompatibleBuildData(List<BuildDataStandard> datas, out BuildDataStandard data)
        {
            Result result = Result.Failed;
            data = null;

            Dictionary<BuildDataStandard, Result> potentialVariants = new Dictionary<BuildDataStandard, Result>();

            foreach (BuildDataStandard info in datas)
            {
                if (!info.gameVersions.Contains(AdvancedCore.GameVersion)
                    || info.modVersion <= Chainloader.PluginInfos[AdvancedCore.modId].Metadata.Version) continue;

                bool forcedDepsAdded = false;
                bool foundInstalledDepConflict = false;

                foreach (BuildDataStandard.Dependency dependency in info.dependencies)
                {
                    if (dependency.forced)
                    {
                        if (!AssetsHelper.ModInstalled(dependency.guid))
                        {
                            forcedDepsAdded = true;
                            continue;
                        }

                        if (!dependency.versions.Contains(Chainloader.PluginInfos[dependency.guid].Metadata.Version.ToString()))
                        {
                            foundInstalledDepConflict = true;
                        }
                    }
                }

                if (foundInstalledDepConflict)
                {
                    result = Result.Failed;
                    continue;
                }
                else if (forcedDepsAdded) result = Result.NewForcedDependenciesAdded;
                else result = Result.ForcedDependenciesFits;

                foundInstalledDepConflict = false;

                foreach (BuildDataStandard.Dependency dependency in info.dependencies)
                {
                    if (!dependency.forced)
                    {
                        if (!AssetsHelper.ModInstalled(dependency.guid)) continue;

                        if (!dependency.versions.Contains(Chainloader.PluginInfos[dependency.guid].Metadata.Version.ToString()))
                        {
                            foundInstalledDepConflict = true;
                        }
                    }
                }

                if (result == Result.ForcedDependenciesFits && !foundInstalledDepConflict) result = Result.Success;

                potentialVariants.Add(info, result);
            }

            Version version = new Version("0.0.0.0");
            BuildDataStandard resultData = null;

            foreach (BuildDataStandard info in potentialVariants.Keys)
            {
                if (version < info.modVersion)
                {
                    version = info.modVersion;
                    resultData = info;
                }
            }

            if (resultData == null) return Result.Failed;

            data = resultData;

            return potentialVariants[data];
        }

    }
}*/