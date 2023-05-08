using System;
using SFS.UI.ModGUI;
using SFS.UI;
using UnityEngine;
namespace ModInstaller
{
    public class InstallerMenu : BasicMenu
    {
        public static InstallerMenu main;
        private GUI gui;

        private void Awake()
        {
            main = this;
            menuHolder = new GameObject("Installer Menu");
            menuHolder.SetActive(false);
            gui = menuHolder.AddComponent<GUI>();
        }

        public override void OnOpen()
        {
            menuHolder.SetActive(true);
            GUI.menuHolder.SetActive(true);
        }
        
        public override void OnClose()
        {
            menuHolder.SetActive(false);
            GUI.menuHolder.SetActive(false);
        }
    }
    public class GUI : MonoBehaviour
    {
        public static GameObject menuHolder;

        private void Awake()
        {
            menuHolder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "");
            LeftPane();
            RightBottomPane();
            RightTopPane();
        }

        /*private void OnGUI()
        {
            GUILayout.Label("Fuck");
            var position = menuHolder.transform.position;
            UnityEngine.GUI.Box(new Rect(position.x, position.y, 100, 100), "asdkjfhhj");
        }*/

        void LeftPane()
        {
            Vector2Int windowDimensions = new Vector2Int(1500, 1250);
            Window window = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 - 10 + 250, windowDimensions.y / 2, titleText: "Mod List");
        }

        void RightTopPane()
        {
            Vector2Int windowDimensions = new Vector2Int(1000, 480);
            Window window = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 260, windowDimensions.y + 145);
        }

        void RightBottomPane()
        {
            Vector2Int windowDimensions = new Vector2Int(1000, 750);
            Window window = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 260, 125);
        }
    }
}