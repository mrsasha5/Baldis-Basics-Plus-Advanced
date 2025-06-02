using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static UnityEngine.Networking.UnityWebRequest;
using System.Threading;
using UnityEngine.Networking;
using UnityEngine;
using System.IO.Compression;
using System.Linq;
using BaldisBasicsPlusAdvanced.Game.Components.UI;
using static BaldisBasicsPlusAdvanced.Game.Components.UI.NotificationManager;
using BaldisBasicsPlusAdvanced.Cache.AssetsManagment;
using BaldisBasicsPlusAdvanced.SaveSystem;
using System.IO;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI.AssetTools;
using UnityCipher;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;

namespace BaldisBasicsPlusAdvanced.AutoUpdate
{
    public class AutoUpdateManager// : Singleton<AutoUpdateManager>
    {

        internal static Thread thread;

        /*private static bool updateFound;

        private float cooldown;

        private float time;

        internal void Initialize(float cooldown)
        {
            this.cooldown = cooldown;
            time = 1f;
        }

        private void Update()
        {
            if (time > 0f)
            {
                time -= Time.unscaledDeltaTime;

                if (time <= 0f)
                {
                    time = cooldown;

                    Check();
                }
            }
        }*/

        public static void Check()
        {
            //Instance?.gameObject.SetActive(false);

            if (Singleton<NotificationManager>.Instance.GenericNotificationsHidden) return;

            AdvancedCore.Instance.StartCoroutine(Checker());
        }

        private static IEnumerator Checker()
        {
            string url = "https://api.github.com/repos/mrsasha5/Baldis-Basics-Plus-Advanced/releases?page=";
            int page = 1;
            int attempts = 5;

            List<JToken> jsons = new List<JToken>();

            UnityWebRequest webRequest = Get(url + page);
            
            yield return webRequest.SendWebRequest();

            while (true)
            {
                if (webRequest.result == Result.Success)
                {
                    try
                    {
                        jsons.Add(JToken.Parse(webRequest.downloadHandler.text));
                    }
                    catch
                    {
                        AdvancedCore.Logging.LogWarning("Failed to convert information!");
                        yield break;
                    }
                    

                    if (jsons[jsons.Count - 1].Count() == 0)
                    {
                        jsons.RemoveAt(jsons.Count - 1);
                        break;
                    }

                    page++;

                    webRequest = Get(url + page);

                    yield return webRequest.SendWebRequest();
                }
                else if (attempts > 0)
                {
                    attempts--;

                    webRequest = Get(url + page);

                    yield return webRequest.SendWebRequest();
                } else
                {
                    AdvancedCore.Logging.LogWarning("Failed to check releases page on the Github repository!");
                    yield break;
                }
                
            }

            List<BuildDataStandard> datas = new List<BuildDataStandard>();

            foreach (JToken info in jsons)
            {
                foreach (JToken element in info)
                {
                    BuildDataStandard data = null;

                    try
                    {
                        data = BuildDataStandard.GetFrom(element["body"].Value<string>());
                    }
                    catch { };

                    if (data == null)
                    {
                        yield return null;
                        continue;
                    }

                    datas.Add(data);

                    data.downloadUrl = element["assets"][0]["browser_download_url"].Value<string>();
                    data.fileExtension = Path.GetExtension(element["assets"][0]["name"].Value<string>());

                    yield return null;

                    //test
                    /*Debug.Log("--------------------------");
                    Debug.Log($"Actual release date: {data.releaseDate.ToString()}");
                    Debug.Log($"Version: {data.modVersion}");
                    Debug.Log("Game versions: ");
                    foreach (string version in data.gameVersions)
                    {
                        Debug.Log(version);
                    }
                    Debug.Log("Dependencies:");
                    foreach (BuildDataStandard.Dependency dependency in data.dependencies)
                    {
                        Debug.Log("-----");
                        Debug.Log($"GUID: {dependency.GUID}");
                        Debug.Log($"Is forced: {dependency.forced}");
                        Debug.Log($"Versions:");
                        foreach (string version in dependency.versions)
                        {
                            Debug.Log(version);
                        }
                        Debug.Log("-----");
                    }
                    Debug.Log($"Changelogs:");
                    foreach (string changelogLink in data.changelogLinks)
                    {
                        Debug.Log(changelogLink);
                    }
                    Debug.Log($"Source code: {data.sourceCodeAvailable}");*/
                }
            }

            jsons.Clear();
            jsons = null;

            CompatibilityAnalyzer.Result result = 
                CompatibilityAnalyzer.GetLastCompatibleBuildData(datas, out BuildDataStandard selectedData);

            if (result == CompatibilityAnalyzer.Result.Failed) yield break;

            Notification notif = 
                Singleton<NotificationManager>.Instance.Queue("", AssetsStorage.sounds["ytp_pickup_0"], 
                time: result == CompatibilityAnalyzer.Result.Success ? 15f : 20f);

            void SetExceptionMessage(string text, float time = 10f)
            {
                notif.tmpText.text = text;
                notif.time = time;
                notif.sound = AssetsStorage.sounds["buzz_elv"];
            }

            while (!notif.active) yield return null;

            bool installUpdate = false;

            while (notif.time > 0f)
            {
                switch (result)
                {
                    case CompatibilityAnalyzer.Result.Success:
                        notif.tmpText.text =
                            "Found compatible version of the BB+ Advanced Edition! Press " +
                            $"{KeyBindingsManager.Keys["update_mod"].Button.ToString()} " +
                            $"to download a new version!\nAbility ends in: {(int)notif.time}s";
                        break;
                    case CompatibilityAnalyzer.Result.ForcedDependenciesFits:
                        notif.tmpText.text =
                            "Found a new version of the BB+ Advanced Edition! Some non-forced dependencies may need updating. " +
                            $"Continue? ({KeyBindingsManager.Keys["update_mod"].Button.ToString()})\n" +
                            $"Ability ends in: {(int)notif.time}s";
                        break;
                    case CompatibilityAnalyzer.Result.NewForcedDependenciesAdded:
                        notif.tmpText.text =
                            "Found a new version of the BB+ Advanced Edition! But you should install new dependencies yourself. " +
                            $"Continue? ({KeyBindingsManager.Keys["update_mod"].Button.ToString()})\n" +
                            $"Ability ends in: {(int)notif.time}s";
                        break;
                }
                

                if (notif.active && Input.GetKeyDown(KeyBindingsManager.Keys["update_mod"].Button))
                {
                    installUpdate = true;
                    notif.time = 0f;
                    break;
                }

                yield return null;
            }

            if (installUpdate)
            {
                notif.sound = AssetsStorage.sounds["ytp_pickup_2"];

                UnityWebRequest downloadRequest = UnityWebRequest.Get(selectedData.downloadUrl);
                downloadRequest.SendWebRequest();

                bool retry = false;

                while (true)
                {
                    while (!downloadRequest.downloadHandler.isDone)
                    {
                        notif.tmpText.text = $"Downloading {(int)(downloadRequest.downloadProgress * 100f)}%";
                        yield return null;
                    }

                    if (downloadRequest.result == Result.Success) break;

                    retry = false;

                    SetExceptionMessage("", 15f);

                    while (notif.time > 0f)
                    {
                        notif.tmpText.text =
                            $"Something went wrong! Retry? ({KeyBindingsManager.Keys["update_mod"].Button.ToString()})\n" +
                            $"Ability ends in: {(int)notif.time}";

                        if (Input.GetKeyDown(KeyBindingsManager.Keys["update_mod"].Button))
                        {
                            notif.time = 0f;
                            retry = true;

                            downloadRequest = UnityWebRequest.Get(selectedData.downloadUrl);
                            downloadRequest.SendWebRequest();

                            break;
                        }

                        yield return null;
                    }

                    if (notif.time <= 0f && !retry) yield break;
                }

                try
                {
                    using (FileStream fs = new FileStream($"Adv_TEMP/build{selectedData.fileExtension}", 
                        FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Write(downloadRequest.downloadHandler.data, 0, downloadRequest.downloadHandler.data.Length);
                    }
                } 
                catch (Exception e)
                {
                    SetExceptionMessage("Something went wrong during creating file!");
                    AdvancedCore.Logging.LogError(e);
                }

                if (selectedData.fileExtension != ".zip")
                {
                    SetExceptionMessage($"Extension {selectedData.fileExtension} is unsupported!");
                    yield break;
                }

                thread = new Thread(ExtractArchive);
                thread.Start();

                notif.tmpText.text = "Extracting...";

                while (thread != null)
                {
                    yield return null;
                }

                List<PlatformID> supportedPlatforms = new List<PlatformID>() 
                { PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.Win32NT, PlatformID.WinCE};

                if (!supportedPlatforms.Contains(Environment.OSVersion.Platform))
                {
                    AdvancedCore.preparedToInstalling = false;
                    SetExceptionMessage(
                        "Auto-installation is not supported for your OS! Install it yourself (all mod files stored in Adv_TEMP).");
                    yield break;
                }

                try
                {
                    foreach (string filePath in Directory.GetFiles("Adv_TEMP/AutoUpdater"))
                    {
                        File.Move(filePath, AdvancedCore.tempPath + Path.GetFileName(filePath));
                    }
                }
                catch (Exception e)
                {
                    SetExceptionMessage("Something went wrong during moving files!");
                    AdvancedCore.Logging.LogError(e);
                }

                try
                {
                    File.WriteAllText(AdvancedCore.tempPath + "AutoUpdater_MainPath.txt",
                        Directory.GetCurrentDirectory() + "/");
                }
                catch (Exception e)
                {
                    SetExceptionMessage("Something went wrong during moving files!");
                    AdvancedCore.Logging.LogError(e);
                }
                

                if (AdvancedCore.preparedToInstalling)
                {
                    notif.sound = AssetsStorage.sounds["adv_bal_super_wow"];
                    notif.time = 10f;
                    notif.tmpText.text = "Close game to get auto-installation! If something go wrong, you can do it yourself (all mod files stored in Adv_TEMP).";
                }
                else
                {
                    AdvancedCore.preparedToInstalling = false;
                    SetExceptionMessage("Something went wrong during extracting! Continue installing by yourself.");
                }

            }
            
        }

        private static void ExtractArchive()
        {
            try
            {
                /*using (FileStream encodedFs = new FileStream("Adv_TEMP/build.zip", FileMode.Open, FileAccess.ReadWrite))
                {
                    byte[] decodedBytes = RijndaelEncryption.Decrypt(encodedFs.ToByteArray(), "");
                    encodedFs.Write(decodedBytes, 0, decodedBytes.Length);
                }*/

                ZipFile.ExtractToDirectory("Adv_TEMP/build.zip", "Adv_TEMP");
                AdvancedCore.preparedToInstalling = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                Thread _thr = thread;
                thread = null;
                _thr.Abort();
            }

        }
    }
}
