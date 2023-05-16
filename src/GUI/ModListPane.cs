using System.Collections.Generic;
using System.Linq;
using ModInstaller.API;
using SFS.UI;
using SFS.UI.ModGUI;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;

namespace ModInstaller.GUI
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
            SearchPane.PageButtonsEnabled(false);
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
            SearchPane.PageButtonsEnabled(true);
            
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
}