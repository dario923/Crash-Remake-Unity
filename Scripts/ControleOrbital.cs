using UnityEngine;

public class ControleOrbital : MonoBehaviour {

    // Corregido el nombre de la variable de 'velcoidade' a 'velocidade'
    public float velocidadeDeGiro = 4.0f;
    private float vertical;

    void Start ()
    {
        // Capturamos la rotación inicial en X
        vertical = transform.eulerAngles.x;
        
        // Ajuste para ángulos negativos si los hay al inicio
        if (vertical > 180) vertical -= 360;
    }
	
    // Cambiamos Update por LateUpdate para evitar el "jitter" (temblor) de cámara
	void LateUpdate ()
    {
        // Obtenemos el movimiento vertical del mouse
        float mouseVertical = Input.GetAxis("Mouse Y");
        
        // Calculamos la nueva rotación
        vertical -= mouseVertical * velocidadeDeGiro;
        
        // Limitamos el ángulo para que la cámara no dé la vuelta completa (Clamping)
        // -30 grados es mirar un poco hacia arriba, 60 es mirar hacia abajo
        vertical = Mathf.Clamp(vertical, -30f, 60f);
        
        // Aplicamos la rotación local solo en el eje Right (X)
        transform.localRotation = Quaternion.Euler(vertical, 0, 0);
    }
}