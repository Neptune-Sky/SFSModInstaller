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
            LeftPane.Setup(menuHolder.transform);
            RightBottomPane.Setup(menuHolder.transform);
            RightPane.Setup(menuHolder.transform);
        }
        
        // KEEP THIS HERE INDEFINETLY

        /*private void OnGUI()
        {
            GUILayout.Label("Fuck");
            var position = menuHolder.transform.position;
            UnityEngine.GUI.Box(new Rect(position.x, position.y, 100, 100), "asdkjfhhj");
        }*/
    }
}