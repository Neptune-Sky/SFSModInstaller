using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using ModLoader.UI;
using SFS.Input;
using SFS.Translations;
using SFS.UI;
using UnityEngine;

namespace ModInstaller
{
    public class Main : Mod
    {

        public override string ModNameID => "0xNim.ModInstaller.Mod";
        public override string DisplayName => "Mod Installer";
        public override string Author => "0xNim";
        public override string MinimumGameVersionNecessary => "1.5.9.8";
        public override string ModVersion => "v1.0.0";
        public override string Description => "Adds a new menu for use with 0xNim's Mod Installer API.";

        // This initializes the patcher. This is required if you use any Harmony patches.
        private static Harmony patcher;
        
        private void insertModsButton()
        {
            Transform buttons = GameObject.Find("Buttons").transform;
            GameObject modsButton = Object.Instantiate(GameObject.Find("Exit Button"), buttons, true);
            ButtonPC buttonPC = modsButton.GetComponent<ButtonPC>();
            TextAdapter textAdapter = modsButton.GetComponentInChildren<TextAdapter>();
            Object.Destroy(modsButton.GetComponent<TranslationSelector>());
            modsButton.name = "Mod Installer Button";
            textAdapter.Text = "Mod Installer";// add text button

            //click events
            buttonPC.holdEvent = new HoldUnityEvent();
            buttonPC.clickEvent = new ClickUnityEvent();
            buttonPC.clickEvent.AddListener(delegate (OnInputEndData data) {
            });

            // screen position
            var transform = buttonPC.transform;
            transform.localScale = new Vector3(1,1,1);
        }
        public override void Load()
        {
        }

        public override void Early_Load()
        {
            SceneHelper.OnHomeSceneLoaded += insertModsButton;
            patcher = new Harmony(ModNameID);
            patcher.PatchAll();
        }
    }
}