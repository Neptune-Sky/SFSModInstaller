namespace ModInstaller
{
    public class InstallHandling
    {
        public static void InstallMod(string modID, string versionNumber)
        {
            // Convert modID and versionNumber to versionID
            //Requests.VersionNumberToVersionID(modID, versionNumber);
            // Lets say you store it as versionID in this case

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



            
        }
    }
}