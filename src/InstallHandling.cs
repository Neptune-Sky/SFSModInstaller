using ModInstaller.API;
using System.Collections.Generic;
//using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using System;


namespace ModInstaller
{
    public class InstallHandling
    {
        // Convert modID and versionNumber to versionID
        //Requests.VersionNumberToVersionID(modID, versionNumber);
        // Lets say you store it as versionID in this case

        // Store Reqquest VersionID in a variable

        // Download mod
        //Requests.DownloadMod(versionID);
        // returns links and mod types lets say stored as dldata.links and dldata.types

        // Download mod files
        // Create loop for each item in dldata
        //for (int i = 0; i < 1; i++)
        //{
        //define path

        // if (dldata.types[i] == "mod") path = "/", if (dldata.types[i] == "pack") path = "/Custom Assets/Packs", etc.

        //    Requests.DownloadFile(dldata.links[i],path);
        //}

        //Debug.Log("Mod installed");


        public static async Task InstallMod(string modID, string versionNumber = "latest")
        {
            int versionID = await Requests.VersionNumberToVersionID(modID, versionNumber);

            if (versionID != null)
            {
                // Perform installation using versionID
                UnityEngine.Debug.Log(versionID);
                // ... Theoretical from now on. As I don't know how

                List<(string fileURL, string fileType)> downloadLinks = await Requests.GetDownloadLinks(versionID);

                foreach (var downloadLink in downloadLinks)
                {
                    string fileURL = downloadLink.fileURL;
                    string fileType = downloadLink.fileType;
                    string fileName = fileURL.Substring(fileURL.LastIndexOf('/') + 1);

                    string modFolderPathTemp = Main.modFolder.ToString();
                    string modFolderPath = modFolderPathTemp.Replace("ModInstaller", "");

                    // Download the mod using fileURL and fileType
                    // if file type = plugin,mod,pack,texture
                    if (fileType == "plugin") Requests.DownloadFile(fileURL, modFolderPath + "Plugins" + fileName);
                    if (fileType == "mod") Requests.DownloadFile(fileURL, modFolderPath + "/" + fileName);
                    if (fileType == "pack") Requests.DownloadFile(fileURL, modFolderPath + "/Custom Assets/Packs" + fileName);
                    if (fileType == "texture") Requests.DownloadFile(fileURL, modFolderPath + "/Custom Assets/Textures" + fileName);
                    if (fileType == "mod-zip")
                    {
                        try
                        {
                            Requests.DownloadAndUnzipFile(fileURL, modFolderPath);
                            //Debug.Log("Zip file downloaded and extracted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex);
                        }
                    }

                }
            }
            else
            {
                // Handle the case where versionID is null (error occurred)
                // ...
            }
        }
    }
}