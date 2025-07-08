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
using BaldisBasicsPlusAdvanced.Cache.AssetsManagement;
using System.IO;
using BaldisBasicsPlusAdvanced.SaveSystem.Managers;
using BaldisBasicsPlusAdvanced.Patches;
using System.Diagnostics;

namespace BaldisBasicsPlusAdvanced.AutoUpdate
{
    public class AutoUpdateManager : Singleton<AutoUpdateManager>
    {

        internal static Thread thread;

        private bool archiveIsExtracted;

        private bool updateFound;

        public void Check()
        {
            if (NotificationManager.Instance.GenericNotificationsHidden) return;

            StartCoroutine(Checker());
        }

        private IEnumerator Checker()
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
                text = text.Localize();
                AdvancedCore.Logging.LogWarning(text);
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
                            string.Format(
                                "Adv_Notif_UpdateIsFound".Localize(), 
                                KeyBindingsManager.Keys["update_mod"].Button.ToString(), 
                                (int)notif.time);
                        break;
                    case CompatibilityAnalyzer.Result.ForcedDependenciesFits:
                        notif.tmpText.text =
                            string.Format(
                                "Adv_Notif_UpdateWithFittingForcedDeps".Localize(),
                                KeyBindingsManager.Keys["update_mod"].Button.ToString(),
                                (int)notif.time);
                        break;
                    case CompatibilityAnalyzer.Result.NewForcedDependenciesAdded:
                        notif.tmpText.text =
                            string.Format(
                                "Adv_Notif_UpdateWithNewDeps".Localize(),
                                KeyBindingsManager.Keys["update_mod"].Button.ToString(),
                                (int)notif.time);
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
                        notif.tmpText.text = string.Format("Adv_Notif_DownloadingProgress".Localize(),
                            (int)(downloadRequest.downloadProgress * 100f));
                        yield return null;
                    }

                    if (downloadRequest.result == Result.Success) break;

                    retry = false;

                    SetExceptionMessage("", 15f);

                    while (notif.time > 0f)
                    {
                        notif.tmpText.text =
                            string.Format(
                                "Adv_Notif_RetryQuestion".Localize(),
                                KeyBindingsManager.Keys["update_mod"].Button.ToString(),
                                (int)notif.time);

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
                    if (Directory.Exists("Adv_TEMP")) Directory.Delete("Adv_TEMP", recursive: true);
                    Directory.CreateDirectory("Adv_TEMP");

                    using (FileStream fs = new FileStream($"Adv_TEMP/build{selectedData.fileExtension}", 
                        FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Write(downloadRequest.downloadHandler.data, 0, downloadRequest.downloadHandler.data.Length);
                    }
                } 
                catch (Exception e)
                {
                    SetExceptionMessage("Adv_Notif_WritingFilesError");
                    AdvancedCore.Logging.LogError(e);
                    yield break;
                }

                if (selectedData.fileExtension != ".zip")
                {
                    SetExceptionMessage(string.Format("Adv_Notif_ExtensionError", selectedData.fileExtension));
                    yield break;
                }

                thread = new Thread(ExtractArchive);
                thread.Start();

                notif.tmpText.text = "Extracting...";

                while (thread != null)
                {
                    yield return null;
                }

                List<PlatformID> windowsIds = new List<PlatformID>() 
                { PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.Win32NT, PlatformID.WinCE};

                if (!windowsIds.Contains(Environment.OSVersion.Platform))
                {
                    SetExceptionMessage("Adv_Notif_AutoInstallerOsSupportError");
                    yield break;
                }

                if (archiveIsExtracted)
                {
                    try
                    {
                        File.Move("Adv_TEMP/AutoUpdater/Windows/Installer.bat", "Adv_Installer.bat");
                    } 
                    catch (Exception e)
                    {
                        SetExceptionMessage("Adv_Notif_MovingFilesError");
                        AdvancedCore.Logging.LogError(e);
                        yield break;
                    }
                    ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe",
                        $"/c Adv_Installer.bat");
                    processInfo.CreateNoWindow = false;
                    processInfo.WindowStyle = ProcessWindowStyle.Minimized;

                    Process.Start(processInfo);

                    notif.sound = AssetsStorage.sounds["adv_bal_super_wow"];
                    notif.time = 10f;
                    notif.tmpText.text = "Adv_Notif_InstallerIsReady".Localize();
                }
                else SetExceptionMessage("Adv_Notif_ExtractingUpdateError");

            }
            
        }

        private static void ExtractArchive()
        {
            try
            {
                ZipFile.ExtractToDirectory("Adv_TEMP/build.zip", "Adv_TEMP");
                Instance.archiveIsExtracted = true;
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
