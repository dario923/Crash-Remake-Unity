using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target_transform;
    public float rotation_speed = 4.0f;
    public Vector3 camera_offset = new Vector3(0, 3, -7); // Un offset estándar para Crash

    private float vertical;
    private float horizontal;

    void Start()
    {
        // Inicializamos con la rotación actual para evitar saltos bruscos al arrancar
        Vector3 angles = transform.eulerAngles;
        vertical = angles.x;
        horizontal = angles.y;

        // Si el target no está asignado, intentamos buscar a Crash por tag
        if (target_transform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target_transform = player.transform;
        }
    }

    // Usamos LateUpdate para cámaras: se ejecuta DESPUÉS de que Crash se haya movido en Update
    void LateUpdate()
    {
        if (target_transform == null) return;

        // 1. Obtener el input del mouse
        horizontal += Input.GetAxis("Mouse X") * rotation_speed;
        float mouseVertical = Input.GetAxis("Mouse Y");
        vertical -= mouseVertical * rotation_speed;

        // 2. Limitar la rotación vertical (clamping) para que no dé la vuelta completa
        vertical = Mathf.Clamp(vertical, -10, 40);

        // 3. Crear la rotación y aplicar el offset rotado
        Quaternion rotation = Quaternion.Euler(vertical, horizontal, 0);
        
        // Calculamos la posición final: posición de Crash + la rotación aplicada al offset
        transform.position = target_transform.position + (rotation * camera_offset);

        // 4. Hacer que la cámara siempre mire hacia el jugador
        transform.LookAt(target_transform.position + Vector3.up * 1.5f); // Apuntamos un poco arriba del suelo
    }
}