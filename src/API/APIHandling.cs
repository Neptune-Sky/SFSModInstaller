using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ModInstaller.API
{
    public static class Requests 
	{
        private static readonly string modFolderPath = Main.inst.ModFolder;

        public static List<ModData> results = new();

        private static void SaveResults(string json)
        {
            try
            {
                results = JsonConvert.DeserializeObject<ModData[]>(json).ToList();
            }
            catch (Exception e)
            {
                results = null;
                Debug.LogError(e);
            }
        }

        private static string GenerateEndpoint(string tags = "", string query = "", int offset = 0)
        {
            var endpoint = "/mods?limit=20";
            if (tags is not (null or "")) endpoint += "&tags=" + tags;
            if (query is not (null or "")) endpoint += "&q=" + query;
            if (offset > 0) endpoint += "&offset=" + offset;
            return endpoint;
        }
        
        public static async Task PullMods(string tags = "", string query = "", int offset = 0)
        {
            string content = await GetAsync(GenerateEndpoint(tags, query, offset));

            // Do something with the content, e.g. parse the JSON
            SaveResults(content);
        }
        
        private static async Task<string> GetAsync(string endpoint)
        {
            var url = $"https://api.astromods.xyz{endpoint}";
            // Debug.Log(url);
            string content = null;
            
            using var client = new HttpClient();
            HttpResponseMessage response = null;
            
            try { response = await client.GetAsync(url); }
            catch (Exception e) { Debug.LogError(e); }


            switch (response)
            {
                case { IsSuccessStatusCode: false }:
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        content = "[]";
                    }
                    else
                    {
                        Debug.LogError($"Failed to get data. Status code: {response.StatusCode}");
                    }
                    break;
                }
                case null:
                    return null;
                default:
                    content = await response.Content.ReadAsStringAsync();
                    break;
            }
            return content;
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

        public static async Task<int> GetModCount(string tags = "", string query = "")
        {
            // Currently takes input with tags already seperated by commas
            var count = 0;
            try
            {
                string content = await GetAsync(GenerateEndpoint(tags, query));
                count = int.Parse(content);
            }
            catch (Exception) { /* ignore */ }

            return count;
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
        

        public static ModVersionData versionResults;
        
        public static async Task VersionNumberToVersionID(string modID, string versionNumber = "latest")
        {
            Debug.Log("check1");
            // Convert modID and versionNumber to versionID
            // By requesting /all/:modID/:versionNumber
            var endpoint = $"/version/alternative/{modID}/{versionNumber}";
            string json = await GetAsync(endpoint);

            try
            {
                Debug.Log("check2");
                versionResults = JsonConvert.DeserializeObject<ModVersionData>(json);
                Debug.Log("check3");
            }
            catch (Exception)
            {
                versionResults = null;
                throw;
            }
            Debug.Log("check4");
            Debug.Log(versionResults);
            
        }
    }
}