using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum SceneType : byte { LOADING = 0, MENU = 1, LEVEL1 = 2 }
public enum MenuType : byte { MENU_MAIN = 0, MENU_OPTIONS = 1, MENU_PAUSE = 2, MENU_GAMEOVER = 3 }

public class GameManager
{
    private static GameManager instance;
    public static GameManager getInstance() {
        if (instance == null) instance = new GameManager();
        return instance;
    }

    public Settings m_Settings;     
    private SceneType active_scene = SceneType.MENU; 
    private Dictionary<SceneType, string> scenes_strings;
    private Dictionary<string, Texture> textures;
    public Font rendering_font;     

    public bool showFPS = false;
    public bool showGUIExtra = false;
    public bool toggleGUI = true;
    public MenuType active_menu { get; set; }
    public readonly string[] MenuNames = new string[]{ "Main Menu", "Options Menu", "Pause Menu", "Game Over Menu" };
    public readonly GUI.WindowFunction[] MenuFunctions;

    public GameManager() {
        MenuFunctions = new GUI.WindowFunction[] { MainMenu, OptionsMenu, PauseMenu, GameOverMenu };
        scenes_strings = new Dictionary<SceneType, string> {
            { SceneType.MENU, "scene_preload" },
            { SceneType.LEVEL1, "scene1" }
        };
        m_Settings = Settings.getInstance(); 
        SceneManager.sceneLoaded += callback_onLoadScene;
    }

    // --- ESTO ES LO QUE EL KEEPER NECESITA ENCONTRAR ---
    public void setActiveScene(SceneType new_scene) { active_scene = new_scene; }
    public void setActiveMenu(MenuType new_menu) { active_menu = new_menu; }
    public void setTextures(Dictionary<string, Texture> new_textures) { textures = new_textures; }
    public void setRenderingFont(Font new_font) { rendering_font = new_font; }
    // ---------------------------------------------------

    private void MainMenu(int id) {
        if (GUILayout.Button("Start Game")) SceneManager.LoadScene(scenes_strings[SceneType.LEVEL1]);
        if (GUILayout.Button("Options")) active_menu = MenuType.MENU_OPTIONS;
    }

    private void OptionsMenu(int id) {
        if (GUILayout.Button("Back")) active_menu = MenuType.MENU_MAIN; 
    }

    private void PauseMenu(int id) { }
    private void GameOverMenu(int id) { }

    public void call_onGUI() {
        if (active_scene == SceneType.MENU) {
            Rect windowRect = new Rect((Screen.width - 300) / 2, (Screen.height - 200) / 2, 300, 200);
            GUILayout.Window(0, windowRect, MenuFunctions[(byte)active_menu], MenuNames[(byte)active_menu]);
        }
    }

    private void callback_onLoadScene(Scene scene, LoadSceneMode mode) {
        if (scene.name == "scene1") active_scene = SceneType.LEVEL1;
        else if (scene.name == "scene_preload") active_scene = SceneType.MENU;
    }
}