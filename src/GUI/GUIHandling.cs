using SFS.UI;
using SFS.UI.ModGUI;
using UnityEngine;

namespace ModInstaller.GUI
{
    public class InstallerMenu : BasicMenu
    {
        public static InstallerMenu main;
        public static int maxModsPerPage = 20;

        private void Awake()
        {
            main = this;
            menuHolder = new GameObject("Installer Menu");
            menuHolder.SetActive(false);
            menuHolder.AddComponent<GUIManager>();
        }

        public override void OnOpen()
        {
            menuHolder.SetActive(true);
            GUIManager.menuHolder.SetActive(true);
        }
        
        public override void OnClose()
        {
            menuHolder.SetActive(false);
            GUIManager.menuHolder.SetActive(false);
        }
    }
    
    
    
    public class GUIManager : MonoBehaviour
    {
        public static GameObject menuHolder;
        
        private void Awake()
        {
            menuHolder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "");
            ModListPane.Setup(menuHolder.transform);
            ModInfoPane.Setup(menuHolder.transform);
            SearchPane.Setup(menuHolder.transform);
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