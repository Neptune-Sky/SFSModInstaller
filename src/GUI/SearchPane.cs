using ModInstaller.API;
using SFS.UI.ModGUI;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;

namespace ModInstaller.GUI
{
    internal static class SearchPane
    {
        private static Window window;
        
        private static TextInput searchQuery;
        private static TextInput tagsQuery;

        private static bool fieldsChanged = false;
        public static void Setup(Transform menuHolder)
        {
            if (window != null) Object.Destroy(window);
            
            var windowDimensions = new Vector2Int(800, 534);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 156, -92, titleText: "");
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