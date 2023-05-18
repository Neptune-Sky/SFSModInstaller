using System;

namespace ModInstaller.API
{
    [Serializable]
    public class ModData
    {
        public string modID;
        public string modName = "";
        public string modDescription = "";
        public string modAuthor = "";
        public string modVersion = "";
        public string modReleaseDate;
        public string modTags;
        public string modIcon;
        public string github;
        public string forum;
        public string donation;
    }

    [Serializable]
    public class ModVersionData
    {
        public int modVersionID;
        public string modID;
        public string versionNumber;
        public string releaseDate;
        public string changelog;
    }

    [Serializable]
    public class DownloadData
    {
        public string fileType;
        public string fileURL;
    }
}