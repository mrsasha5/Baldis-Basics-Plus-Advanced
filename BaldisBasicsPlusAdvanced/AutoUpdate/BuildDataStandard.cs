using System;
using System.Collections.Generic;
using System.Globalization;
using BaldisBasicsPlusAdvanced.Exceptions;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.AutoUpdate
{
    internal class BuildDataStandard
    {

        //not a part of build information on Github, taken from Github jsons
        public string downloadUrl;

        public string fileExtension;
        //this part ends

        public class Dependency
        {

            public string guid;

            public string[] versions;

            public bool forced;

            public static Dependency GetFrom(string line)
            {
                string _line = line.Trim();

                Dependency dependency = new Dependency();

                if (_line.StartsWith("FORCED ")) dependency.forced = true;
                else if (!_line.StartsWith("NON-FORCED "))
                    throw new StandardViolationException("Any dependency always should start with \"FORCED\" or \"NON-FORCED\" text.");

                if (dependency.forced) _line = _line.Replace("FORCED ", "");
                else _line = _line.Replace("NON-FORCED ", "");

                string[] values = _line.Split(':');

                dependency.guid = values[0].Replace("\"", "");
                dependency.versions = values[1].Trim().Replace("\",", "").Replace("\"", "").Split(' ');

                return dependency; 
            }

        }

        public Version standardVersion;

        public DateTime releaseDate;

        public Version modVersion;

        public string[] gameVersions;

        public string[] tags;

        public List<Dependency> dependencies;

        public string[] changelogLinks;

        public bool? sourceCodeAvailable;

        public static BuildDataStandard GetFrom(string data)
        {
            BuildDataStandard convertedData = new BuildDataStandard();

            string[] lines = data.Split('\n');

            bool titleFound = false;

            bool corrupt = false;

            for (int i = 0; i < lines.Length; i++)
            {
                #region V1.0
                if (lines[i].Trim() == "Build Data Standard V1.0")
                {
                    convertedData.standardVersion = new Version("1.0");
                    titleFound = true;

                    try
                    {
                        string actualReleaseDate = lines[i + 1].Trim().Replace("Actual release date: ", "");

                        if (actualReleaseDate != "no")
                        {
                            convertedData.releaseDate = DateTime.ParseExact(actualReleaseDate,
                                "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                        }
                        
                        convertedData.modVersion = new Version(lines[i + 2].Trim().Replace("Version: ", "").Replace("\"", ""));

                        convertedData.gameVersions =
                            lines[i + 3].Trim().Replace("Game versions: ", "").Replace("\",", "").Replace("\"", "").Split(' ');

                        int offset = 1;
                        if (lines[i + 4].Trim() != "Dependencies: no")
                        {
                            convertedData.dependencies = new List<Dependency>();
                            while (lines[i + 4 + offset].TrimStart().StartsWith("FORCED") 
                                || lines[i + 4 + offset].TrimStart().StartsWith("NON-FORCED"))
                            {
                                convertedData.dependencies.Add(Dependency.GetFrom(lines[i + 4 + offset]));
                                offset++;
                            }
                        }
                        else offset = 0;

                        string changelogs = lines[i + 5 + offset].Trim().Replace("Changelogs: ", "");

                        if (changelogs != "no")
                        {
                            convertedData.changelogLinks = changelogs.Replace("\",", "").Replace("\"", "").Split(' ');
                        }

                        string srcAvailable = lines[i + 5 + offset].Trim().Replace("Source code: ", "");
                        if (srcAvailable == "yes") convertedData.sourceCodeAvailable = true;
                        else if (srcAvailable != "no")
                            throw new StandardViolationException("Source code field should contain only \"yes\" or \"no\" value.");
                    }
                    catch (Exception e)
                    {
                        AdvancedCore.Logging.LogError(e.ToString());
                        corrupt = true;
                    }

                    break;
                }
                #endregion
                #region V1.1
                else if (lines[i].Trim() == "Build Data Standard V1.1")
                {
                    convertedData.standardVersion = new Version("1.1");
                    titleFound = true;

                    try
                    {
                        int offset = 1;

                        if (lines[i + offset].Trim().StartsWith("Actual release date: "))
                        {
                            string actualReleaseDate = lines[i + offset].Trim().Replace("Actual release date: ", "");

                            if (actualReleaseDate != "no")
                            {
                                convertedData.releaseDate = DateTime.ParseExact(actualReleaseDate,
                                    "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                            }

                            offset++;
                        }

                        convertedData.modVersion = new Version(lines[i + offset].Trim().Replace("Version: ", "").Replace("\"", ""));
                        offset++;

                        convertedData.gameVersions =
                            lines[i + offset].Trim().Replace("Game versions: ", "").Replace("\",", "").Replace("\"", "").Split(' ');
                        offset++;

                        if (lines[i + offset].Trim().StartsWith("Dependencies:"))
                        {
                            if (lines[i + offset].Trim() != "Dependencies: no")
                            {
                                offset++;
                                convertedData.dependencies = new List<Dependency>();
                                while (true)
                                {
                                    if (lines.Length <= i + offset) break;

                                    if (!lines[i + offset].TrimStart().StartsWith("FORCED") &&
                                        !lines[i + offset].TrimStart().StartsWith("NON-FORCED"))
                                    {
                                        break;
                                    }

                                    convertedData.dependencies.Add(Dependency.GetFrom(lines[i + offset]));
                                    offset++;
                                }
                            }
                            else offset++;
                        }

                        if (lines.Length <= i + offset) break;

                        if (lines[i + offset].Trim().StartsWith("Tags:"))
                        {
                            convertedData.tags =
                                lines[i + offset].Trim().Replace("Tags: ", "").Replace("\",", "").Replace("\"", "").Split(' ');
                            offset++;
                        }

                        if (lines.Length <= i + offset) break;

                        if (lines[i + offset].Trim().StartsWith("Changelogs:"))
                        {
                            string changelogs = lines[i + offset].Trim().Replace("Changelogs: ", "");

                            if (changelogs != "no")
                            {
                                convertedData.changelogLinks = changelogs.Replace("\",", "").Replace("\"", "").Split(' ');
                            }
                            offset++;
                        }

                        if (lines.Length <= i + offset) break;

                        if (lines[i + offset].Trim().StartsWith("Source code:"))
                        {
                            string srcAvailable = lines[i + offset].Trim().Replace("Source code: ", "");
                            if (srcAvailable == "yes") convertedData.sourceCodeAvailable = true;
                            else if (srcAvailable != "no")
                                throw new StandardViolationException("Source code field should contain only \"yes\" or \"no\" value.");
                        }
                    }
                    catch (Exception e)
                    {
                        AdvancedCore.Logging.LogError(e.ToString());
                        corrupt = true;
                    }

                    break;
                }
                #endregion
            }

            if (!titleFound || corrupt) return null;

            return convertedData;
        }

        public void LogDebugInfo()
        {
            Debug.Log("--------------------------");
            Debug.Log($"Actual release date: {releaseDate.ToString()}");
            Debug.Log($"Version: {modVersion}");
            Debug.Log("Game versions: ");
            foreach (string version in gameVersions)
            {
                Debug.Log(version);
            }
            Debug.Log("Dependencies:");
            if (dependencies != null)
            {
                foreach (Dependency dependency in dependencies)
                {
                    Debug.Log("-----");
                    Debug.Log($"GUID: {dependency.guid}");
                    Debug.Log($"Is forced: {dependency.forced}");
                    Debug.Log($"Versions:");
                    foreach (string version in dependency.versions)
                    {
                        Debug.Log(version);
                    }
                    Debug.Log("-----");
                }
            }
            Debug.Log("Tags:");
            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    Debug.Log(tag);
                }
            }
            Debug.Log($"Changelogs:");
            if (changelogLinks != null)
            {
                foreach (string changelogLink in changelogLinks)
                {
                    Debug.Log(changelogLink);
                }
            }
            Debug.Log($"Source code: {sourceCodeAvailable}");
        }

    }
}
