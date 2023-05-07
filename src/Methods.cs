using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace ModInstaller
{
	public class Requests 
	{
        private static async Task<string> GetAsync(string endpoint)
        {
            var url = $"https://api.astromods.xyz{endpoint}";
            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get data. Status code: {response.StatusCode}");
            
            string content = await response.Content.ReadAsStringAsync();
            return content;
        }


        private static async Task Main(string[] args)
        {
        string content = await GetAsync("/mods?limit=5");
        Console.WriteLine(content); 
        }
        

        public static void DownloadFile(string url, string destinationFilePath)
        {
            using var client = new WebClient();
            client.DownloadFile(url, destinationFilePath);
        }
        public static void DeleteFolder(string folderPath)
        {
            Directory.Delete(folderPath, true);
        }
        
        private readonly string modFolderPath = ModInstaller.Main.main.ModFolder;


        public static async Task ListMods(int limit, int offset)
        {
            var endpoint = $"/mods?limit={limit}&offset={offset}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }



    }
}