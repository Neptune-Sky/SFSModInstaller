using System;
using System.Diagnostics;
using System.Linq;
using ModInstaller.API;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using TMPro;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Button = SFS.UI.ModGUI.Button;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Type = SFS.UI.ModGUI.Type;

namespace ModInstaller.GUI
{
    internal static class ModInfoPane
    {
        private static Window window;
        private static Label name;
        private static Label version;
        private static Label description;
        private static Label author;
        private static Label tags;

        private static Button gitButton;
        private static Button forumsButton;
        private static Button installButton;

        private static ModData currentMod;

        public static void Setup(Transform menuHolder)
        {   
            if (window != null) Object.Destroy(window);
            Vector2Int windowDimensions = new (800, 700);
            window = CreateWindow(menuHolder, GetRandomID(), windowDimensions.x, windowDimensions.y,
                windowDimensions.x / 2 + 156, windowDimensions.y -75, titleText: "Details");

            window.CreateLayoutGroup(Type.Vertical, spacing: 10);

            Container labels = CreateContainer(window);
            labels.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 5f);
            name = CreateLabel(labels, 385, 50, text: "");
            name.TextAlignment = TextAlignmentOptions.Left;
            version = CreateLabel(labels, 385, 50, text: "");
            version.TextAlignment = TextAlignmentOptions.Right;
            CreateSeparator(window, windowDimensions.x - 20);

            Container authorContainer = CreateContainer(window);
            authorContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            CreateLabel(authorContainer, 95, 40, text: "Author:").TextAlignment = TextAlignmentOptions.Left;
            author = CreateLabel(authorContainer, windowDimensions.x - 140, 40, text: "");
            author.TextAlignment = TextAlignmentOptions.Left;
            
            Container tagsContainer = CreateContainer(window);
            tagsContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleCenter, 10);
            CreateLabel(tagsContainer, 95, 40, text: "Tags:").TextAlignment = TextAlignmentOptions.Left;
            tags = CreateLabel(tagsContainer, windowDimensions.x - 140, 40, text: "None");
            tags.TextAlignment = TextAlignmentOptions.Left;
            
            CreateLabel(window, windowDimensions.x - 30, 40, text: "Description:").TextAlignment = TextAlignmentOptions.Left;
            Box box = CreateBox(window, 780, 330);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperLeft, 0, new RectOffset(10, 5, 20, 5));
            description = CreateLabel(box, 765, 300);
            description.TextAlignment = TextAlignmentOptions.TopLeft;
            description.gameObject.GetComponent<TextMeshProUGUI>().fontSizeMax = 30;

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
            currentMod = modData;
            name.Text = modData.modName;
            version.Text = modData.modVersion;
            author.Text = modData.modAuthor;
            description.Text = modData.modDescription;
            tags.Text = "None";
            
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
            
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            // bool downloadable = await Requests.CheckInstallable(modData.modID);
            var nodownload = false;
            if (modData.modTags != null)
            {
                var modTags = modData.modTags.Split(',').ToList();
                if (modTags.Contains("nodownload"))
                {
                    modTags.Remove("nodownload");
                    nodownload = true;
                }
                tags.Text = string.Join(", ", modTags);
            }

            if (nodownload || InstallHandling.modsAwaitingInstall.Contains(modData.modID)) return;

            
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
            installButton.OnClick = () => InstallHandling.InstallButtonFunc(modData);

            /*if (installed)
            {
                installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(true);
            }
            else
            {
                installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(false);
            } */
        }

        public static void CheckInstallButton(string modID)
        {
            installButton.gameObject.GetComponent<ButtonPC>().SetEnabled(!InstallHandling.modsAwaitingInstall.Contains(modID));
        }
    }
}