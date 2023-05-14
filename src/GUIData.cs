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
    internal static class ModListPane
    {
        private static Window window;

        public static void Setup(Transform menuHolder)
        {
            var windowDimensions = new Vector2Int(1125, 1250);
            window = CreateWindow(menuHolder.transform, GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 + 140, windowDimensions.y / 2, titleText: "Mod List");
            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter);
            // window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            // window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);

            Box modListBox = CreateBox(window.gameObject.transform, 1100, 1175, 0, -windowDimensions.y / 2 - 25);
            ModList.Setup(modListBox);
        }
    }

    internal static class ModList
    {
        private static Window window;

        private static readonly List<Button> buttons = new();
        private static Button activeButton;
        
        private static Label noResults;
        private static Label error;
        private static Label loading;

        public static void Setup(Box box)
        {
            if (window != null) Object.Destroy(window);

            window = CreateWindow(box.gameObject.transform, GetRandomID(), (int)box.Size.x, (int)box.Size.y + 40, 0, (int)box.Size.y / 2 + 40);
            window.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            window.gameObject.transform.Find("Title").gameObject.SetActive(false);

            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 5f, disableChildSizeControl: true);
            window.EnableScrolling(Type.Vertical);

            noResults = CreateLabel(box, 1050, 60, text: "No results found!");
            error = CreateLabel(box, 1050, 60, text: "An error occurred! Please try again.");
            loading = CreateLabel(box, 1050, 60, text: "Loading...");
            noResults.gameObject.SetActive(false);
            error.gameObject.SetActive(false);

            for (var i = 0; i < 20; i++)
            {
                Button button = CreateButton(window, 1075, 140);
                button.gameObject.SetActive(false);
                buttons.Add(button);
            }
            
            Regenerate();
        }

        public static async void Regenerate(string searchTags = "", string searchQuery = "", int offset = 0)
        {
            window.gameObject.SetActive(false);
            activeButton?.gameObject.GetComponent<ButtonPC>().SetSelected(false);
            error.gameObject.SetActive(false);
            noResults.gameObject.SetActive(false);
            
            loading.gameObject.SetActive(true);
            await Requests.PullMods(searchTags, searchQuery, offset);
            loading.gameObject.SetActive(false);
            
            if (Requests.results == null)
            {
                error.gameObject.SetActive(true);
                return;
            }

            if (Requests.results.Count == 0)
            {
                noResults.gameObject.SetActive(true);
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
                    ModInfoPane.Regenerate(mod);
                    // Debug.Log(JsonUtility.ToJson(mod));
                    activeButton?.gameObject.GetComponent<ButtonPC>().SetSelected(false);
                    activeButton = button;
                    activeButton.gameObject.GetComponent<ButtonPC>().SetSelected(true);
                };
                // button.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            }
            
            if (i >= buttons.Count - 1) return;

            for (; i < buttons.Count; i++)
            {
                Button button = buttons[i];
                button.gameObject.SetActive(false);
            }
            window.gameObject.SetActive(true);
        }
    }

    internal static class ModInfoPane
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
            CreateLabel(window, windowDimensions.x - 30, 50, text: "Description:").TextAlignment = TextAlignmentOptions.Left;
            Box box = CreateBox(window, 780, 310);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(10, 5, 20, 5));
            description = CreateLabel(box, 765, 290);
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
        
        private static TextInput searchQuery;
        private static TextInput tagsQuery;

        private static bool fieldsChanged = false;
        public static void Setup(Transform menuHolder)
        {
            if (window != null) Object.Destroy(window);
            
            var windowDimensions = new Vector2Int(800, 634);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 156, 8, titleText: "");
            window.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10);

            Container searchContainer = CreateContainer(window);
            searchContainer.CreateLayoutGroup(Type.Vertical, spacing: 2);
            CreateLabel(searchContainer, 450, 45, text: "Search by Title:");
            searchQuery = CreateTextInput(searchContainer, 450, 50, onChange: _ => fieldsChanged = true, text: "");

            Container tagContainer = CreateContainer(window);
            tagContainer.CreateLayoutGroup(Type.Vertical, spacing: 2);
            CreateLabel(tagContainer, 450, 45, text: "Search by Tag (Separate Tags with Commas):");
            tagsQuery = CreateTextInput(tagContainer, 450, 50, onChange: _ => fieldsChanged = true, text: "");

            Container confirmContainer = CreateContainer(window);
            confirmContainer.CreateLayoutGroup(Type.Horizontal, spacing: 3);
            CreateButton(confirmContainer, 125, 47, onClick: () =>
            {
                if (!fieldsChanged && searchQuery.Text == "" && tagsQuery.Text == "") return;
                searchQuery.Text = "";
                tagsQuery.Text = "";
                fieldsChanged = false;
                ModInfoPane.Regenerate(new ModData());
                ModList.Regenerate();
            }, text: "Clear");
            CreateButton(confirmContainer, 125, 47, onClick: () =>
            {
                if (!fieldsChanged) return;
                fieldsChanged = false;
                ModInfoPane.Regenerate(new ModData());
                ModList.Regenerate(tagsQuery.Text, searchQuery.Text);
            }, text: "Search");
        }
    }
}