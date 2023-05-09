using System.Collections.Generic;
using System.Diagnostics;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;

namespace ModInstaller
{
    static class RightTopPane
    {
        public static Window window;
        public static Label modName;
        public static Label modVersion;

        public static Button gitButton;
        public static Button forumsButton;
        public static Button installButton;


        public static void Generate(Transform menuHolder)
        {   
            if (window != null) Object.Destroy(window);
            Vector2Int windowDimensions = new Vector2Int(1000, 480);
            window = Builder.CreateWindow(menuHolder, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,
                windowDimensions.x / 2 + 260, windowDimensions.y + 145);

            window.CreateLayoutGroup(Type.Vertical);

            Container labels = Builder.CreateContainer(window);
            labels.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 5f);
            modName = Builder.CreateLabel(labels, 475, 50, text: "Mod Name");
            modName.TextAlignment = TextAlignmentOptions.Left;
            modVersion = Builder.CreateLabel(labels, 475, 50, text: "");
            modVersion.TextAlignment = TextAlignmentOptions.Right;

            Container buttons = Builder.CreateContainer(window);
            buttons.CreateLayoutGroup(Type.Horizontal, spacing: 5f);
            gitButton = Builder.CreateButton(buttons, 325, 60, text: "GitHub");
            gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            forumsButton = Builder.CreateButton(buttons, 325, 60, text: "Forums");
            forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            installButton = Builder.CreateButton(buttons, 325, 60, text: "Install");
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
        }

        public static void Regenerate(string name = "", string version = "", string gitRepo = null,
            string forumsThread = null, string installURL = null)
        {
            modName.Text = name;
            modVersion.Text = version;

            if (gitRepo == null) gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
                gitButton.OnClick = () => Process.Start(gitRepo);
            }

            if (forumsThread == null) forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
                forumsButton.OnClick = () => Process.Start(forumsThread);
            }

            if (installURL == null) installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
            }
        }
    }

    public static class ModList
    {
        private static Window window;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static List<Button> buttons = new();

        public static void Setup()
        {
            if (window != null) Object.Destroy(window);
            
            window = Builder.CreateWindow(GUI.main.leftPane.gameObject.transform, Builder.GetRandomID() ,1100, 1140, 160, -85);
            window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Title").gameObject.SetActive(false);
            
            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 5f, disableChildSizeControl: true);
            window.EnableScrolling(Type.Vertical);

            for (var i = 0; i < 20; i++)
            {
                Button button = Builder.CreateButton(window, 1050, 200);
                button.gameObject.SetActive(false);
                buttons.Add(button);
            }
            Regenerate();
        }

        public static async void Regenerate()
        {
            await Requests.PullMods(20, 0);
            if (Requests.results == null)
            {
                Builder.CreateLabel(window, 200, 50, text: "An error occurred! Please try again.");
                return;
            }

            if (Requests.results.Length == 0)
            {
                Builder.CreateLabel(window, 200, 50, text: "No results");
                return;
            }
            var i = 0;
            for (; i < Requests.results.Length; i++)
            {
                Button button = buttons[i];
                ModData mod = Requests.results[i];
                button.gameObject.SetActive(true);
                button.Text = mod.modName;
                button.OnClick = () =>
                {
                    RightTopPane.Regenerate(mod.modName, mod.modVersion);
                };
                // button.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            }

            if (i >= buttons.Count - 1) return;
            
            for (; i < buttons.Count; i++)
            {
                Button button = buttons[i];
                button.gameObject.SetActive(false);
            }
            
        }
    }
}