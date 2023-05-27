using HarmonyLib;
using JetBrains.Annotations;
using ModInstaller.GUI;
using ModLoader;
using ModLoader.Helpers;
using SFS.Audio;
using SFS.IO;
using SFS.Translations;
using SFS.UI;
using System.Collections.Generic;
using UnityEngine;

namespace ModInstaller
{
    [UsedImplicitly]
    public class Main : Mod, IUpdatable
    {
        public override string ModNameID => "0xNim.ModInstaller.Mod";
        public override string DisplayName => "Mod Installer";
        public override string Author => "NeptuneSky & 0xNim";
        public override string MinimumGameVersionNecessary => "1.5.9.8";
        public override string ModVersion => "Beta-0.6.2";
        public override string Description => "Adds a new menu for use with 0xNim's Mod Installer API.";


        public Dictionary<string, FilePath> UpdatableFiles => new()
        {
            {
                "https://github.com/Neptune-Sky/SFSModInstaller/releases/latest/download/ModInstaller.dll",
                new FolderPath(ModFolder).ExtendToFile("ModInstaller.dll")
            }
        };

        private static Harmony patcher;
        public static Main inst;
        public static bool DisableModUpdates => true;
        public override /*async*/ void Load()
        {
            // IUpdatable modToUpdate = new Main(); // Replace MyMod with the actual class implementing IUpdatable
            // await ModsUpdater.Update(modToUpdate);
        }

        public static FolderPath modFolder;

        public override void Early_Load()
        {
            inst = this;
            SceneHelper.OnHomeSceneLoaded += insertModsButton;
            patcher = new Harmony(ModNameID);
            patcher.PatchAll();

            modFolder = new FolderPath(ModFolder);

            // Additional initialization code
        }

        private static void insertModsButton()
        {
            Transform buttons = GameObject.Find("Buttons").transform;
            GameObject modLoaderButton = GameObject.Find("Mod Loader Button");
            GameObject installerButton = Object.Instantiate(modLoaderButton, buttons, true);
            installerButton.GetComponent<RectTransform>().SetSiblingIndex(modLoaderButton.GetComponent<RectTransform>().GetSiblingIndex() + 1);
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
            });
        }
    }
}
