using ModLoader;
using SFS.Input;
using SFS.IO;
using SFS.Parsers.Json;
using System.IO;
using System.Net;
using UnityEngine;

namespace ModInstaller
{
	public class Requests 
	{

        public static async Task<string> GetAsync(string endpoint)
        {
            var url = $"http://api.astromods.xyz{endpoint}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    throw new Exception($"Failed to get data. Status code: {response.StatusCode}");
                }
            }
        }

        /* 
        public static async Task Main(string[] args)
        {
        var content = await Requests.GetAsync("/mods?limit=5");
        Console.WriteLine(content); 
        }
        */

        public static void DownloadFile(string url, string destinationFilePath)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, destinationFilePath);
            }
        }
        public static void DeleteFolder(string folderPath)
        {
            Directory.Delete(folderPath, true);
        }


        string modFolderPath = MyMod.modFolder.ToString();
        string modFolderPath2 = modFolderPath.Replace("ModInstaller", "");


        public static async Task ListMods(int limit, int offset)
        {
            var endpoint = $"/mods?limit={limit}&offset={offset}";
            var content = await Requests.GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }



    }
}