using System.Diagnostics;
using SFS.UI.ModGUI;
using SFS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = SFS.UI.ModGUI.Button;

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
        public static GUI main;
        public static GameObject menuHolder;
        public Window leftPane;
        private Window rightBottomPane;
        
        private void Awake()
        {
            main = this;
            if (menuHolder != null) Destroy(menuHolder);
            menuHolder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "");
            LeftPane();
            ModList.Setup();
            RightBottomPane();
            RightTopPane.Generate(menuHolder.transform);
        }
        
        // KEEP THIS HERE INDEFINETLY

        /*private void OnGUI()
        {
            GUILayout.Label("Fuck");
            var position = menuHolder.transform.position;
            UnityEngine.GUI.Box(new Rect(position.x, position.y, 100, 100), "asdkjfhhj");
        }*/

        private void LeftPane()
        {
            if (leftPane != null) Destroy(leftPane);
            
            Vector2Int windowDimensions = new Vector2Int(1500, 1250);
            leftPane = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 - 10 + 250, windowDimensions.y / 2, titleText: "Mod List");
            Builder.CreateBox(leftPane.gameObject.transform, 1100, 1100, 160, -675);
        }


        private void RightBottomPane()
        {
            if (rightBottomPane != null) Destroy(rightBottomPane);
            
            Vector2Int windowDimensions = new Vector2Int(1000, 750);
            rightBottomPane = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y, windowDimensions.x / 2 + 260, 125);
        }
    }
}