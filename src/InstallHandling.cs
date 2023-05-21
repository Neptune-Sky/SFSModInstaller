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

namespace ModInstaller
{
    public static class InstallHandling
    {
        public static readonly List<string> modsAwaitingInstall = new();
        public static async Task InstallMod(string modID, string versionNumber = "latest")
        {
            int versionID = await Requests.VersionNumberToVersionID(modID, versionNumber);

            {
                // Perform installation using versionID
                Debug.Log(versionID);
                // ... Theoretical from now on. As I don't know how

                List<(string fileURL, string fileType)> downloadLinks = await Requests.GetDownloadLinks(versionID);
                modsAwaitingInstall.Add(modID);
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
                            DownloadFile(fileURL, modFolderPath + "Plugins" + fileName);
                            break;
                        case "mod":
                            DownloadFile(fileURL, new FolderPath(modFolderPath + "/" + Regex.Replace(fileName,".dll$", "")).ExtendToFile(fileName));
                            break;
                        case "pack":
                            DownloadFile(fileURL, modFolderPath + "/Custom Assets/Parts/" + fileName);
                            break;
                        case "texture":
                            DownloadFile(fileURL, modFolderPath + "/Custom Assets/Texture Packs/" + fileName);
                            break;
                        case "mod-zip":
                            try
                            {
                                DownloadAndUnzipFile(fileURL, modFolderPath);
                                //Debug.Log("Zip file downloaded and extracted successfully.");
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex);
                            }

                            break;
                    }
                }

                modsAwaitingInstall.Remove(modID);
            }
        }
        private static async void DownloadFile(string url, string destinationFilePath)
        {
            using var client = new WebClient();
            await Task.Run(() => client.DownloadFile(url, destinationFilePath));
        }

        private static async void DownloadAndUnzipFile(string url, string destinationFolderPath)
        {
            using var client = new WebClient();
            string tempZipFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zip");
            await Task.Run(() =>
            {
                client.DownloadFile(url, tempZipFilePath);
                ZipFile.ExtractToDirectory(tempZipFilePath, destinationFolderPath);
                File.Delete(tempZipFilePath);
            });
        }
    }
}