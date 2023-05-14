using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
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
            window = CreateWindow(menuHolder.transform, GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 + 140, windowDimensions.y / 2, titleText: "Mod List");
            Box modListBox = CreateBox(window.gameObject.transform, 1100, 1100, 0, -675);
            ModList.Setup(modListBox);
        }
    }

    internal static class ModList
    {
        private static Window window;

        private static readonly List<Button> buttons = new();
        private static Label noResults;
        private static Label error;

        public static async void Setup(Box box)
        {
            if (window != null) Object.Destroy(window);

            window = CreateWindow(box.gameObject.transform, GetRandomID(), (int)box.Size.x, (int)box.Size.y + 40, 0, (int)box.Size.y / 2 + 40);
            window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Title").gameObject.SetActive(false);

            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 5f, disableChildSizeControl: true);
            window.EnableScrolling(Type.Vertical);

            noResults = CreateLabel(window, 1050, 30, text: "No results found!");
            error = CreateLabel(window, 1050, 30, text: "An error occurred! Please try again.");
            noResults.gameObject.SetActive(false);
            error.gameObject.SetActive(false);

            for (var i = 0; i < 20; i++)
            {
                Button button = CreateButton(window, 1075, 150);
                button.gameObject.SetActive(false);
                buttons.Add(button);
            }

            // await Requests.PullMods(20, 0);
            await Regenerate();
        }

        public static async Task Regenerate(string searchTags = "" , string searchQuery = "", int offset = 0)
        {
            await Requests.PullMods(searchTags, searchQuery, offset);

            void DisableAllButtons()
            {
                buttons.ForEach((button) =>
                {
                    button.gameObject.SetActive(false);
                });
            }
            
            if (Requests.results == null)
            {
                DisableAllButtons();
                error.gameObject.SetActive(true);
                return;
            }

            if (Requests.results.Count == 0)
            {
                DisableAllButtons();
                noResults.gameObject.SetActive(true);
                return;
            }
            error.gameObject.SetActive(false);
            noResults.gameObject.SetActive(false);
            
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
                    RightPane.Regenerate(mod);
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
        private static Label description;

        private static Button gitButton;
        private static Button forumsButton;
        private static Button installButton;


        public static void Setup(Transform menuHolder)
        {   
            if (window != null) Object.Destroy(window);
            Vector2Int windowDimensions = new (800, 600);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y,
                windowDimensions.x / 2 + 156, windowDimensions.y + 25, titleText: "Details");

            window.CreateLayoutGroup(Type.Vertical, spacing: 10);

            Container labels = CreateContainer(window);
            labels.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 5f);
            modName = CreateLabel(labels, 385, 50, text: "");
            modName.TextAlignment = TextAlignmentOptions.Left;
            modVersion = CreateLabel(labels, 385, 50, text: "");
            modVersion.TextAlignment = TextAlignmentOptions.Right;
            CreateSeparator(window, windowDimensions.x - 20);
            CreateLabel(window, windowDimensions.x - 20, 50, text: "Description:").TextAlignment = TextAlignmentOptions.Left;
            Box box = CreateBox(window, 790, 310);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(10, 5, 20, 5));
            description = CreateLabel(box, 775, 290);
            description.TextAlignment = TextAlignmentOptions.TopLeft;
            description.AutoFontResize = false;
            description.FontSize = 30;

            Container buttons = CreateContainer(window);
            buttons.CreateLayoutGroup(Type.Horizontal, spacing: 8f);
            gitButton = CreateButton(buttons, 228, 60, text: "GitHub");
            gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            forumsButton = CreateButton(buttons, 228, 60, text: "Forums");
            forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            installButton = CreateButton(buttons, 228, 60, text: "Install");
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
        }

        public static void Regenerate(ModData modData)
        {
            modName.Text = modData.modName;
            modVersion.Text = modData.modVersion;
            description.Text = modData.modDescription;


            if (modData.github == null)
            {
                gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            }
            else
            {
                gitButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
                gitButton.OnClick = () => Process.Start(modData.github);
            }

            if (modData.forum == null)
            {
                forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            }
            else
            {
                forumsButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
                forumsButton.OnClick = () => Process.Start(modData.forum);
            }

            /*if ("installURL" == "") installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            else
            {
                installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
            }*/
        }
    }

    internal static class RightBottomPane
    {
        private static Window window;
        public static void Setup(Transform menuHolder)
        {
            if (window != null) Object.Destroy(window);
            
            Vector2Int windowDimensions = new Vector2Int(800, 634);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 156, 8, titleText: "");
        }
    }
}