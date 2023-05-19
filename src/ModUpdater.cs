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

    static class ModsUpdater
    {
        static readonly HttpClient Http = new();
        static readonly MD5 MD5 = MD5.Create();
        static int loadedFiles;

        public static async UniTask Update(IUpdatable mod)
        {
            foreach (KeyValuePair<string, FilePath> file in mod.UpdatableFiles)
            {
                HttpRequestMessage request = new(HttpMethod.Head, file.Key);
                HttpResponseMessage response = await Http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    continue;

                byte[] md5HashLocal =
                    file.Value.FileExists() ? MD5.ComputeHash(file.Value.ReadBytes()) : Array.Empty<byte>();
                byte[] md5HashRemote = response.Content.Headers.ContentMD5;

                if (md5HashLocal.SequenceEqual(md5HashRemote))
                    continue;

                request = new HttpRequestMessage(HttpMethod.Get, file.Key);
                response = await Http.SendAsync(request);

                byte[] data = await response.Content.ReadAsByteArrayAsync();
                if (data != null)
                {
                    file.Value.WriteBytes(data);
                    loadedFiles += 1;
                }
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
