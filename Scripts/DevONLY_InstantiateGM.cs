using UnityEngine;
using UnityEngine.SceneManagement; // Importante para usar SceneManager de forma limpia

public class DevONLY_InstantiateGM : MonoBehaviour 
{
    void Awake()
    {
        /*
         * Este objeto busca al objeto maestro "__app". 
         * Si no existe, significa que no empezamos desde la escena de carga.
         * En ese caso, forzamos la carga de "_preload".
        */
        
        GameObject check = GameObject.Find("__app"); 
        
        if (check == null)
        {
            // Verificamos si la escena existe en el Build Settings para evitar errores
            if (Application.CanStreamedLevelBeLoaded("_preload"))
            {
                Debug.Log("<color=cyan>DevONLY:</color> Cargando escena de precarga para instanciar el GameManager.");
                SceneManager.LoadScene("_preload");
            }
            else
            {
                Debug.LogError("<color=red>Error:</color> No se encuentra la escena '_preload' en el Build Settings. " +
                               "Agrégala para que el sistema funcione.");
            }
        }
    }
}