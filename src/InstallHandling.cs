using ModInstaller.API;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using SFS.IO;
using System.IO.Compression;
using ModInstaller.GUI;
using SFS.UI;

namespace ModInstaller
{
    public static class InstallHandling
    {
        public static readonly List<string> modsAwaitingInstall = new();

        public static async void InstallButtonFunc(ModData modData)
        {
            try
            {
                MsgDrawer.main.Log("Installing " + modData.modName + "...");
                await InstallMod(modData.modID);
                MsgDrawer.main.Log("Install Successful: " + modData.modName);
            }
            catch (Exception e)
            {
                MsgDrawer.main.Log("Install Failed: " + modData.modName);
                Debug.LogError(e);
                return;
            }
            InstallerMenu.modsInstalled = true;
        }
        private static async Task InstallMod(string modID, string versionNumber = "latest")
        {
            modsAwaitingInstall.Add(modID);
            ModInfoPane.CheckInstallButton(modID);
            int versionID = await Requests.VersionNumberToVersionID(modID, versionNumber);

            {
                // Perform installation using versionID
                Debug.Log(versionID);
                // ... Theoretical from now on. As I don't know how

                List<(string fileURL, string fileType)> downloadLinks = await Requests.GetDownloadLinks(versionID);
                
                try
                {
                    foreach ((string fileURL, string fileType) in downloadLinks)
                    {
                        string fileName = fileURL[(fileURL.LastIndexOf('/') + 1)..];

                        var modFolderPathTemp = Main.modFolder.ToString();
                        string modFolderPath = modFolderPathTemp.Replace("ModInstaller", "");

                        switch (fileType)
                        {
                            // Download the mod using fileURL and fileType
                            // if file type = plugin,mod,pack,texture
                            case "plugin":

                                await DownloadFile(fileURL, modFolderPath + "/Plugins/" + fileName);
                                break;
                            case "mod":
                                await DownloadFile(fileURL,
                                    new FolderPath(modFolderPath + "/" + Regex.Replace(fileName, ".dll$", ""))
                                        .ExtendToFile(fileName));
                                break;
                            case "pack":
                                await DownloadFile(fileURL, modFolderPath + "/Custom Assets/Parts/" + fileName);
                                break;
                            case "texture":
                                await DownloadAndUnzipFile(fileURL, modFolderPath + "/Custom Assets/Texture Packs/");
                                break;
                            case "mod-zip":
                                await DownloadAndUnzipFile(fileURL, modFolderPath);
                                break;
                            case "root":
                                await DownloadFile(fileURL,
                                    new FolderPath(modFolderPath.Replace("/Mods","") + "/" + Regex.Replace(fileName, ".dll$", ""))
                                        .ExtendToFile(fileName));
                                break;
                        }
                    }
                    modsAwaitingInstall.Remove(modID);
                    ModInfoPane.CheckInstallButton(modID);
                }
                catch (Exception)
                {
                    modsAwaitingInstall.Remove(modID);
                    ModInfoPane.CheckInstallButton(modID);
                    throw;
                }
            }
        }
        private static async Task DownloadFile(string url, string destinationFilePath)
        {
            using var client = new WebClient();
            try
            {
                await Task.Run(() =>
                {
                    string parentDirectory =
                        destinationFilePath[..destinationFilePath.LastIndexOf("/", StringComparison.Ordinal)];
                    if (!Directory.Exists(parentDirectory))
                    {
                        Directory.CreateDirectory(parentDirectory);
                    }
                    client.DownloadFile(url, destinationFilePath);
                });
            }
            catch (Exception)
            {
                File.Delete(destinationFilePath);
                throw;
            }
        }

        private static async Task DownloadAndUnzipFile(string url, string destinationFolderPath)
        {
            using var client = new WebClient();
            string tempZipFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zip");
            try
            {
                await Task.Run(() =>
                {
                    client.DownloadFile(url, tempZipFilePath);
                    ZipFile.ExtractToDirectory(tempZipFilePath, destinationFolderPath);
                    File.Delete(tempZipFilePath);
                });
            }
            catch (Exception)
            {
                File.Delete(tempZipFilePath);
                throw;
            }
        }
    }
}
