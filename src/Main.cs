using HarmonyLib;
using JetBrains.Annotations;
using ModInstaller.GUI;
using ModLoader;
using ModLoader.Helpers;
using SFS.Audio;
using SFS.IO;
using SFS.Translations;
using SFS.UI;
using UnityEngine;

namespace ModInstaller
{
    [UsedImplicitly]
    public class Main : Mod
    {

        public override string ModNameID => "0xNim.ModInstaller.Mod";
        public override string DisplayName => "Mod Installer";
        public override string Author => "0xNim & NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.5.9.8";
        public override string ModVersion => "v1.0.0";
        public override string Description => "Adds a new menu for use with 0xNim's Mod Installer API.";

        // This initializes the patcher. This is required if you use any Harmony patches.
        private static Harmony patcher;

        public static Main inst;

        public static bool DisableModUpdates => true;
        
        public override void Load() {}

        public static FolderPath modFolder;

        public override void Early_Load()
        {
            inst = this;
            SceneHelper.OnHomeSceneLoaded += insertModsButton;
            patcher = new Harmony(ModNameID);
            patcher.PatchAll();

            modFolder = new FolderPath(ModFolder);
        }
        
        private static void insertModsButton()
        {
            Transform buttons = GameObject.Find("Buttons").transform;
            GameObject installerButton = Object.Instantiate(GameObject.Find("Exit Button"), buttons, true);
            var buttonPC = installerButton.GetComponent<ButtonPC>();
            var textAdapter = installerButton.GetComponentInChildren<TextAdapter>();
            textAdapter.Text = "Mod Installer";
            Object.Destroy(installerButton.GetComponent<TranslationSelector>());
            installerButton.name = "Mod Installer Button";
            
            
            installerButton.AddComponent<InstallerMenu>();
            
            buttonPC.holdEvent = new HoldUnityEvent();
            buttonPC.clickEvent = new ClickUnityEvent();
            buttonPC.clickEvent.AddListener(delegate
            {
                SoundPlayer.main.clickSound.Play();
                InstallerMenu.main.Open();
                InstallHandling.InstallMod("UITools");
            });
        }
    }
}