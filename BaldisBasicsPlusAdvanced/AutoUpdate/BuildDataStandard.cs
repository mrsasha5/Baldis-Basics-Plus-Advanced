using System;
using System.Collections.Generic;
using System.Globalization;
using BaldisBasicsPlusAdvanced.Exceptions;

namespace BaldisBasicsPlusAdvanced.AutoUpdate
{
    public class BuildDataStandard
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

        public string standardVersion;

        public DateTime releaseDate;

        public Version modVersion;

        public string[] gameVersions;

        public List<Dependency> dependencies;

        public string[] changelogLinks;

        public bool sourceCodeAvailable;

        public static BuildDataStandard GetFrom(string data)
        {
            BuildDataStandard convertedData = new BuildDataStandard();

            string[] lines = data.Split('\n');

            bool titleFound = false;

            bool corrupt = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == "Build Data Standard V1.0")
                {
                    convertedData.standardVersion = "1.0";
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
            }

            if (!titleFound || corrupt) return null;

            return convertedData;
        }

    }
}
