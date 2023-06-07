using System;
using System.Collections.Generic;
using System.Linq;
using ModInstaller.API;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using Object = UnityEngine.Object;
using Type = SFS.UI.ModGUI.Type;

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

            Box modListBox = CreateBox(window.gameObject.transform, 1100, 1175, 0, -windowDimensions.y / 2 - 25);
            ModList.Setup(modListBox);
        }
    }
    internal class ModListEntry
    {
        private Box box;
        private Button showInfo;
        private Label name;
        private Label author;
        private Label version;
        private Label tags;
        private Box iconBox;
        private WebImage icon;

        private static Label LabelHelper(Transform parent, int width, int height, TextAlignmentOptions alignment, string text = "",
            int maxFontSize = 25)
        {
            Label toReturn = CreateLabel(parent, width, height, text: text);
            toReturn.TextAlignment = alignment;
            toReturn.gameObject.GetComponent<TextMeshProUGUI>().fontSizeMax = maxFontSize;
            return toReturn;
        }
        public void Init(Window window)
        {
            box = CreateBox(window, (int)window.Size.x - 25, 140);
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var layout = box.CreateLayoutGroup(Type.Horizontal, spacing: 15);
            
            iconBox = CreateBox(box, (int)(box.Size.y - 20), (int)(box.Size.y - 20));
            
            icon = CreateWebImage(iconBox, (int)iconBox.Size.x, (int)iconBox.Size.y);
            icon.gameObject.SetActive(false);
            
            // Used on labels to allow them to shrink/grow if icon box or mod entry box size changes.
            var itemSizes = new Vector2Int((int)((box.Size.x - iconBox.Size.x * 1.666) / 2 - layout.spacing * 2), (int)iconBox.Size.y / 2);
            
            Container container1 = CreateContainer(box);
            container1.CreateLayoutGroup(Type.Vertical, spacing: 0);
            name = LabelHelper(container1, itemSizes.x, itemSizes.y, TextAlignmentOptions.TopLeft);
            author = LabelHelper(container1, itemSizes.x, itemSizes.y, TextAlignmentOptions.BottomLeft);

            Container container2 = CreateContainer(box);
            container2.CreateLayoutGroup(Type.Vertical, spacing: 0);
            version = LabelHelper(container2, itemSizes.x, itemSizes.y, TextAlignmentOptions.TopRight);
            tags = LabelHelper(container2, itemSizes.x, itemSizes.y, TextAlignmentOptions.BottomRight);

            showInfo = CreateButton(box, (int)(iconBox.Size.x / 1.5), (int)iconBox.Size.y, text: "+");
            showInfo.gameObject.GetComponentInChildren<TextMeshProUGUI>().fontSizeMin = 60;
        }

        public void ChangeListing(ModData modData, Action buttonOnClick)
        {
            name.Text = modData.modName;
            author.Text = modData.modAuthor;
            version.Text = modData.modVersion;
            tags.Text = modData.modTags;
            showInfo.OnClick = buttonOnClick;
            Debug.Log(modData.modIcon);
            if (modData.modIcon == null)
            {
                icon.gameObject.SetActive(false);
                return;
            }
            icon.gameObject.SetActive(true);
            icon.ImageLink = modData.modIcon;
        }

        public void SetButtonSelected(bool selected = true)
        {
            showInfo.gameObject.GetComponent<ButtonPC>().SetSelected(selected);
        }

        public void SetActive(bool active = true)
        {
            box.gameObject.SetActive(active);
        }
        
        
    }
    internal static class ModList
    {
        private static Window window;

        private static readonly List<ModListEntry> entries = new();
        private static ModListEntry activeEntry;
        
        private static Label noResults;
        private static Label error;
        private static Label loading;

        public static void Setup(Box box)
        {
            if (window != null) Object.Destroy(window);

            window = CreateWindow(box.gameObject.transform, GetRandomID(), (int)box.Size.x, (int)box.Size.y + 40, 0, (int)box.Size.y / 2 + 40);
            
            // Disable the visual parts of the window. These are not needed, as we are only using the window for the scroll
            // element.
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

            for (var i = 0; i < InstallerMenu.maxModsPerPage; i++)
            {
                var entry = new ModListEntry();
                entry.Init(window);
                entry.SetActive(false);
                entries.Add(entry);
            }
            
            Regenerate();
        }

        public static async void Regenerate(string searchTags = "", string searchQuery = "", int offset = 0)
        {
            window.gameObject.SetActive(false);
            activeEntry?.SetButtonSelected(false);
            error.gameObject.SetActive(false);
            noResults.gameObject.SetActive(false);
            
            loading.gameObject.SetActive(true);
            SearchPane.PageButtonsEnabled(false);
            await Requests.PullMods(searchTags, searchQuery, offset);
            loading.gameObject.SetActive(false);

            switch (Requests.results)
            {
                case null:
                    // results is set to null when an error occurs during the API request or
                    // deserialization of the received JSON.
                    error.gameObject.SetActive(true);
                    return;
                case { Count: 0 }:
                    noResults.gameObject.SetActive(true);
                    return;
            }
            SearchPane.PageButtonsEnabled(true);
            
            // define i outside of the for to use it afterwards. 
            var i = 0;
            
            for (; i < Requests.results.Count; i++)
            {
                ModListEntry entry = entries[i];
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

                entry.SetActive();
                entry.ChangeListing(mod, () =>
                {
                    ModInfoPane.Regenerate(mod);
                    activeEntry?.SetButtonSelected(false);
                    entry.SetButtonSelected();
                    activeEntry = entry;
                });
            }
            
            // hides any entries that weren't updated
            for (; i < entries.Count; i++)
            {
                entries[i].SetActive(false);
            }
            window.gameObject.SetActive(true);
        }
    }
}