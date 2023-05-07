using System;
using SFS.UI.ModGUI;
using SFS.UI;
using UnityEngine;
namespace ModInstaller
{
    public class InstallerMenu : BasicMenu
    {
        public static InstallerMenu main;

        private void Awake()
        {
            main = this;
            menuHolder = new GameObject("Installer Menu");
            menuHolder.AddComponent<GUI>();
            menuHolder.SetActive(false);
        }
    }
    public class GUI : MonoBehaviour
    {
        private static GameObject menuHolder = InstallerMenu.main.menuHolder;

        private void Awake()
        {
            
        }

        private void Start()
        {
            
        }
    }
}