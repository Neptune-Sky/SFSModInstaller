using System;
using System.Collections.Generic;
using ModInstaller.API;
using SFS.UI;
using SFS.UI.ModGUI;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using Object = UnityEngine.Object;
using Type = SFS.UI.ModGUI.Type;

namespace ModInstaller.GUI
{
    internal static class SearchPane
    {
        private static Window window;
        
        private static TextInput searchQuery;
        private static TextInput tagsQuery;

        private static Label pagesText;
        private static Label modsNumberText;
        private static readonly List<Button> pageButtons = new();

        private static bool fieldsChanged;
        
        private static int results = 0;
        private static int page = 1;
        private static int totalPages = 1;
        
        public static void Setup(Transform menuHolder)
        {
            if (window != null) Object.Destroy(window);

            var windowDimensions = new Vector2Int(800, 534);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 156, -92, titleText: "");
            window.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 10);
            CreateSpace(window, 0, 0);
            Container searchContainer = CreateContainer(window);
            searchContainer.CreateLayoutGroup(Type.Vertical, spacing: 2);
            CreateLabel(searchContainer, 500, 55, text: "Search by Title:");
            searchQuery = CreateTextInput(searchContainer, 500, 60, onChange: _ => fieldsChanged = true, text: "");

            Container tagContainer = CreateContainer(window);
            tagContainer.CreateLayoutGroup(Type.Vertical, spacing: 2);
            CreateLabel(tagContainer, 500, 55, text: "Search by Tag (Separate Tags with Commas):");
            tagsQuery = CreateTextInput(tagContainer, 500, 60, onChange: _ => fieldsChanged = true, text: "");

            Container confirmContainer = CreateContainer(window);
            confirmContainer.CreateLayoutGroup(Type.Horizontal, spacing: 3);
            CreateButton(confirmContainer, 160, 57, onClick: () =>
            {
                if (!fieldsChanged && searchQuery.Text == "" && tagsQuery.Text == "") return;
                searchQuery.Text = "";
                tagsQuery.Text = "";
                fieldsChanged = false;
                ModInfoPane.Regenerate(new ModData());
                ModList.Regenerate();
                UpdatePage(0, false);
            }, text: "Clear");
            CreateButton(confirmContainer, 160, 57, onClick: () =>
            {
                if (!fieldsChanged) return;
                fieldsChanged = false;
                ModInfoPane.Regenerate(new ModData());
                ModList.Regenerate(tagsQuery.Text, searchQuery.Text);
                UpdatePage(0, false);
            }, text: "Search");

            Container pagesContainer = CreateContainer(window);
            pagesContainer.CreateLayoutGroup(Type.Horizontal, spacing: 3);
            pageButtons.Add(CreateButton(pagesContainer, 60, 55, onClick: () => UpdatePage(-2), text: "<<"));
            pageButtons.Add(CreateButton(pagesContainer, 50, 55, onClick: () => UpdatePage(-1), text: "<"));
            pagesText = CreateLabel(pagesContainer, 300, 50, text: "Page 1 of 1");
            pageButtons.Add(CreateButton(pagesContainer, 50, 55, onClick: () => UpdatePage(1), text: ">"));
            pageButtons.Add(CreateButton(pagesContainer, 60, 55, onClick: () => UpdatePage(2), text: ">>"));

            modsNumberText = CreateLabel(window, 400, 35, text: "Displaying 1-20 of 20");
            PageButtonsEnabled(false);
            
            UpdatePage(0, false);
        }

        public static void PageButtonsEnabled(bool enabled)
        {
            pageButtons.ForEach(button =>
            {
                button.gameObject.GetComponent<ButtonPC>().SetEnabled(enabled);
            });
        }
        private static async void UpdatePage(int pageModifier = 0, bool regenerate = true)
        {
            switch (pageModifier)
            {
                case 0:
                    results = await Requests.GetModCount(tagsQuery.Text, searchQuery.Text);
                    page = 1;
                    totalPages = (int)Math.Clamp(Mathf.Ceil((float)results / InstallerMenu.maxModsPerPage), 1, int.MaxValue);
                    break;
                case 1 or -1:
                    int newPage = page + pageModifier;
                    if (newPage < 1 || newPage > totalPages) return;
                    page = newPage;
                    break;
                case 2:
                    if (page == totalPages) return;
                    page = totalPages;
                    break;
                case -2:
                    if (page == 1) return;
                    page = 1;
                    break;
            }
            
            pagesText.Text = "Page " + page + " of " + totalPages;
            modsNumberText.Text = results > 0
                ? $"Displaying {(page - 1) * InstallerMenu.maxModsPerPage + 1} - {Math.Min(page * InstallerMenu.maxModsPerPage, results)} of {results}"
                : "";
            
            if (!regenerate) return;
            ModInfoPane.Regenerate(new ModData());
            ModList.Regenerate(tagsQuery.Text, searchQuery.Text, InstallerMenu.maxModsPerPage * (page - 1));
            
        }
    }
}