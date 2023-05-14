using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using Button = SFS.UI.ModGUI.Button;
using Debug = UnityEngine.Debug;

namespace ModInstaller
{
    internal static class LeftPane
    {
        private static Window window;

        public static void Setup(Transform menuHolder)
        {
            var windowDimensions = new Vector2Int(1125, 1250);
            window = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 - 10 + 250, windowDimensions.y / 2, titleText: "Mod List");
            Box modListBox = Builder.CreateBox(window.gameObject.transform, 1100, 1100, 0, -675);
            ModList.Setup(modListBox);
        }
    }

    internal static class ModList
    {
        private static Window window;

        private static readonly List<Button> buttons = new();

        public static async void Setup(Box box)
        {
            if (window != null) Object.Destroy(window);

            window = Builder.CreateWindow(box.gameObject.transform, Builder.GetRandomID(), (int)box.Size.x, (int)box.Size.y + 40, 0, (int)box.Size.y / 2 + 40);
            window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Title").gameObject.SetActive(false);

            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 5f, disableChildSizeControl: true);
            window.EnableScrolling(Type.Vertical);

            for (var i = 0; i < 20; i++)
            {
                Button button = Builder.CreateButton(window, 1050, 150);
                button.gameObject.SetActive(false);
                buttons.Add(button);
            }

            // await Requests.PullMods(20, 0);
            await Regenerate();
        }

        public static async Task Regenerate(string searchTags = "" , string searchQuery = "", int offset = 0)
        {
            await Requests.PullMods(searchTags, searchQuery, offset);
            if (Requests.results == null)
            {
                //Builder.CreateLabel(window, 200, 50, text: "An error occurred! Please try again.");
                return;
            }

            if (Requests.results.Count == 0)
            {
                //Builder.CreateLabel(window, 200, 50, text: "No results");
                return;
            }

            var i = 0;
            for (; i < Requests.results.Count; i++)
            {
                Button button = buttons[i];
                ModData mod = Requests.results[i];
                if (mod.modTags != null)
                {
                    string[] modtags = mod.modTags.Split(',');
                    if (modtags.Contains("test"))
                    {
                        Requests.results.RemoveAt(i);
                        i--;
                        continue;
                    }
                }

                button.gameObject.SetActive(true);
                button.Text = mod.modName;
                button.OnClick = () =>
                {
                    RightPane.Regenerate(mod.modName, mod.modVersion, mod.github, mod.forum);
                    Debug.Log(JsonUtility.ToJson(mod));
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

    internal static class RightPane
    {
        private static Window window;
        private static Label modName;
        private static Label modVersion;

        private static Button gitButton;
        private static Button forumsButton;
        private static Button installButton;


        public static void Setup(Transform menuHolder)
        {   
            if (window != null) Object.Destroy(window);
            Vector2Int windowDimensions = new (800, 600);
            window = Builder.CreateWindow(menuHolder, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,
                windowDimensions.x / 2 + 260, windowDimensions.y + 25);

            window.CreateLayoutGroup(Type.Vertical);

            Container labels = Builder.CreateContainer(window);
            labels.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 5f);
            modName = Builder.CreateLabel(labels, 385, 50, text: "Mod Name");
            modName.TextAlignment = TextAlignmentOptions.Left;
            modVersion = Builder.CreateLabel(labels, 385, 50, text: "");
            modVersion.TextAlignment = TextAlignmentOptions.Right;

            Container buttons = Builder.CreateContainer(window);
            buttons.CreateLayoutGroup(Type.Horizontal, spacing: 8f);
            gitButton = Builder.CreateButton(buttons, 228, 60, text: "GitHub");
            gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            forumsButton = Builder.CreateButton(buttons, 228, 60, text: "Forums");
            forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            installButton = Builder.CreateButton(buttons, 228, 60, text: "Install");
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
        }

        public static void Regenerate(string name = "", string version = "", string gitRepo = "",
            string forumsThread = "", string installURL = "")
        {
            modName.Text = name;
            modVersion.Text = version;

            if (gitRepo == "") gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
                gitButton.OnClick = () => Process.Start(gitRepo);
            }

            if (forumsThread == "") forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
                forumsButton.OnClick = () => Process.Start(forumsThread);
            }

            if (installURL == "") installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
            }
        }
    }
}