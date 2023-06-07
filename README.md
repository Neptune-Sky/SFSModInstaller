# SFS Mod Installer
Frontend for 0xNim's Mod Installer API.

Adds a new in-game menu that streamlines installing mods.
![UI](https://github.com/Neptune-Sky/SFSModInstaller/blob/master/UI.png?raw=true)

# How to Use
1. Use the `Open Mods Folder` button in the Mod Loader menu of the game.
2. Download `ModInstaller.dll` from the latest release in the pain shown on the right.
3. Drag and drop the DLL into the folder, then close and re-open the game.

# Adding Your Own Mods
Use the [Mod Author Portal](https://portal.astromods.xyz/) if you are a mod creator and you'd like to publish your mod to the installer. 

# Compiling [FOR MOD DEVELOPERS INTERESTED IN FORKING]
Create a `References` folder in the root of the project. Open your game files, navigate to `Spaceflight Simulator_Data > Managed` then copy all the DLLs from this directory and paste them into your references folder. This should automatically resolve missing dependencies.
