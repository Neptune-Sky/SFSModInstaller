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

            {
                // Perform installation using versionID
                Debug.Log(versionID);
                // ... Theoretical from now on. As I don't know how

                List<(string fileURL, string fileType)> downloadLinks = await Requests.GetDownloadLinks(versionID);

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
                            Requests.DownloadFile(fileURL, modFolderPath + "Plugins" + fileName);
                            break;
                        case "mod":
                            Requests.DownloadFile(fileURL, modFolderPath + "/" + fileName);
                            break;
                        case "pack":
                            Requests.DownloadFile(fileURL, modFolderPath + "/Custom Assets/Packs" + fileName);
                            break;
                        case "texture":
                            Requests.DownloadFile(fileURL, modFolderPath + "/Custom Assets/Textures" + fileName);
                            break;
                        case "mod-zip":
                            try
                            {
                                Requests.DownloadAndUnzipFile(fileURL, modFolderPath);
                                //Debug.Log("Zip file downloaded and extracted successfully.");
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex);
                            }

                            break;
                    }
                }
            }
        }
    }
}