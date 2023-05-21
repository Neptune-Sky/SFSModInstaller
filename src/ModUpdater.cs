using Cysharp.Threading.Tasks;
using ModLoader;
using SFS.Input;
using SFS.IO;
using SFS.UI;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System;

namespace ModInstaller
{
    internal static class ModsUpdater
    {
        private static readonly HttpClient Http = new();
        private static readonly MD5 MD5 = MD5.Create();
        private static int loadedFiles;

        public static async UniTask Update(IUpdatable mod)
        {
            foreach ((string fileKey, FilePath filePath) in mod.UpdatableFiles)
            {
                HttpRequestMessage request = new(HttpMethod.Head, fileKey);
                HttpResponseMessage response = await Http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    continue;

                byte[] md5HashLocal =
                    filePath.FileExists() ? MD5.ComputeHash(filePath.ReadBytes()) : Array.Empty<byte>();
                byte[] md5HashRemote = response.Content.Headers.ContentMD5;

                if (md5HashLocal.SequenceEqual(md5HashRemote))
                    continue;

                request = new HttpRequestMessage(HttpMethod.Get, fileKey);
                response = await Http.SendAsync(request);

                byte[] data = await response.Content.ReadAsByteArrayAsync();
                if (data == null) continue;
                filePath.WriteBytes(data);
                loadedFiles += 1;
            }
        }

    }

    /// <summary>
    ///     Implement this interface on main mod class if you want it to be updated at game start
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>
        ///     Returns dictionary of files that should be updated
        ///     string is web link, FilePath is path where file will be downloaded
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, FilePath> UpdatableFiles { get; }
    }
}
