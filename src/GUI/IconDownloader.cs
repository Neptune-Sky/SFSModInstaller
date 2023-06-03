using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ModInstaller.GUI
{
    public static class IconDownloader
    {
        private static readonly Dictionary<string, Sprite> iconCache = new();
        
        public static async Task<Sprite> DownloadImageAsync(string imageUrl)
        {
            imageUrl =
                "https://target.scene7.com/is/image/Target/GUEST_f5d0cfc3-9d02-4ee0-a6c6-ed5dc09971d1?wid=488&hei=488&fmt=pjpeg";
            if (imageUrl == null) return null;
            if (iconCache.TryGetValue(imageUrl, out Sprite icon))
            {
                return icon;
            }
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
            UnityWebRequestAsyncOperation asyncOperation = www.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Failed to download image: " + www.error);
                return null;
            }
            // Create a new texture and assign the downloaded image data to it
            Texture2D downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Debug.Log(downloadedTexture.name + " Success");
            // Create a sprite from the downloaded texture
            var downloadedSprite = Sprite.Create(downloadedTexture, new Rect(0, 0, downloadedTexture.width, downloadedTexture.height), Vector2.zero);
            iconCache.TryAdd(imageUrl, downloadedSprite);
            if (iconCache.Count > 30) iconCache.Remove(iconCache.Keys.ToArray()[0]);
            return downloadedSprite;
        }
    }
}