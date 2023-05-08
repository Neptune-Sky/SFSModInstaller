using SFS.UI.ModGUI;
using SFS.UI;
using UnityEngine;
using UnityEngine.UI;

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
        private Window leftPane;
        
        private void Awake()
        {
            menuHolder = Builder.CreateHolder(Builder.SceneToAttach.BaseScene, "");
            leftPane = LeftPane();
            GenerateModList();
            RightBottomPane();
            RightTopPane();
        }

        // KEEP THIS HERE INDEFINETLY

        /*private void OnGUI()
        {
            GUILayout.Label("Fuck");
            var position = menuHolder.transform.position;
            UnityEngine.GUI.Box(new Rect(position.x, position.y, 100, 100), "asdkjfhhj");
        }*/

        Window LeftPane()
        {
            Vector2Int windowDimensions = new Vector2Int(1500, 1250);
            Window window = Builder.CreateWindow(menuHolder.transform, Builder.GetRandomID(), windowDimensions.x, windowDimensions.y,  - windowDimensions.x / 2 - 10 + 250, windowDimensions.y / 2, titleText: "Mod List");
            Builder.CreateBox(window.gameObject.transform, 1100, 1100, 160, -675);

            return window;
        }

        async void GenerateModList()
        {
            Window modList = Builder.CreateWindow(leftPane.gameObject.transform, 1100, 1100, 160, -675);
            modList.gameObject.transform.Find("Back (Game)").gameObject.SetActive(false);
            modList.gameObject.transform.Find("Back (InGame)").gameObject.SetActive(false);
            modList.gameObject.transform.Find("Title").gameObject.SetActive(false);
            modList.gameObject.transform.Find("Mask").GetComponent<RectMask2D>().rectTransform.offsetMax += new Vector2(0, 40);
            modList.gameObject.Rect().offsetMax =
                new Vector2(modList.gameObject.Rect().offsetMax.x, 10);

            await Requests.PullMods(1, 0);
            Debug.Log(Requests.results[0].modName);
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