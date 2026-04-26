using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Para usar Dictionary estándar

public class GameManagerKeeper : MonoBehaviour
{
    private GameManager GM;

    // Borramos la definición manual de InspectorField porque ya existe en tu proyecto
    [System.Serializable]
    public class TextureField : InspectorField<string, Texture> { }

    [SerializeField]
    public TextureField[] texture_list;

    public Font rendering_font;

    void Awake()
    {
        // Evita que el objeto se destruya al cambiar de escena
        DontDestroyOnLoad(gameObject);

        GM = GameManager.getInstance();

        // Usamos Dictionary estándar para coincidir con el GameManager actualizado
        Dictionary<string, Texture> new_textures_dictionary = new Dictionary<string, Texture>();
        
        if (texture_list != null)
        {
            foreach (TextureField texture_data in texture_list)
            {
                if (!string.IsNullOrEmpty(texture_data.key))
                {
                    new_textures_dictionary.Add(texture_data.key, texture_data.value);
                }
            }
        }

        // Configuración inicial mediante el Singleton
        GM.setActiveScene(SceneType.MENU);
        GM.setActiveMenu(MenuType.MENU_MAIN);
        GM.setTextures(new_textures_dictionary);
        GM.setRenderingFont(rendering_font);

        GM.showFPS = true; 
    }

    void OnGUI()
    {
        if (GM != null) GM.call_onGUI();
    }

    void Update()
    {
        // Alternar GUI extra con la tecla 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            GM.showGUIExtra = !GM.showGUIExtra;
        }
    }
}