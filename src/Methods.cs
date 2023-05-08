using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace ModInstaller
{
	public static class Requests 
	{
        private static readonly string modFolderPath = ModInstaller.Main.inst.ModFolder;
        
        private static async Task<string> GetAsync(string endpoint)
        {
            var url = $"https://api.astromods.xyz{endpoint}";
            // Debug.Log(url);
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

        public static async Task CheckHealth()
        {
            string content = await GetAsync("/health");
            Debug.Log(content);
        }

        public static async Task GetModCount(string tags, string query)
        {
            // Currently takes input with tags already seperated by commas
            string content = await GetAsync($"/total/mods?tags={tags}&q={query}");
            Debug.Log(content);
        }
        public static async Task ListMods(int limit, int offset)
        {
            var endpoint = $"/mods?limit={limit}&offset={offset}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }

        public static async Task GetMod(string modId)
        {
            // The mod ID is the ID of the mod not a number, e.g. "VanUp","UITools","SFS_ESM_UTILITY"
            var endpoint = $"/mods/{modId}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }

        public static async Task GetModVersions(string modId, string version)
        {
            // The mod ID is the ID of the mod not a number, e.g. "VanUp","UITools","SFS_ESM_UTILITY"
            var endpoint = $"/mods/{modId}/versions/";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }

        public static async Task GetModVersion(string modId, string version)
        {
            // The mod ID is the ID of the mod not a number, e.g. "VanUp","UITools","SFS_ESM_UTILITY"
            // Version number: e.g. "1.0.0"
            var endpoint = $"/mods/{modId}/versions/{version}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }

        public static async Task SearchMods(string query, int limit, int offset)
        {
            var endpoint = $"/search/{query}?limit={limit}&offset={offset}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }

        public static async Task SearchTags(string query, int limit, int offset)
        {
            var endpoint = $"/tags/{query}?limit={limit}&offset={offset}";
            string content = await GetAsync(endpoint);

            // Do something with the content, e.g. parse the JSON
            Debug.Log(content);
        }
    }
}