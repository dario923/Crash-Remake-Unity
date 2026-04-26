using UnityEngine;

public class EnemigoExplosion : MonoBehaviour
{
    [Header("Efectos")]
    public GameObject efectoExplosion;
    public GameObject efectoChispas;

    [Header("Ajustes de Detección")]
    public float distanciaDeActivacion = 5.0f; // Ajustable desde el Inspector

    private bool yaExplotó = false;

    void Update()
    {
        if (yaExplotó) return;

        // Buscamos a Crash en la escena
        PlayerController crash = Object.FindFirstObjectByType<PlayerController>();
        
        if (crash != null && crash.isAttacking)
        {
            // Calculamos la distancia entre el centro de Crash y el Mecha
            float distancia = Vector3.Distance(transform.position, crash.transform.position);

            if (distancia < distanciaDeActivacion)
            {
                Explotar();
            }
        }
    }

    public void Explotar()
    {
        yaExplotó = true;
        Debug.Log("¡BOOM! Mecha destruido por proximidad.");

        // 1. Instanciar efectos en la posición actual
        if (efectoExplosion != null) Instantiate(efectoExplosion, transform.position, Quaternion.identity);
        if (efectoChispas != null) Instantiate(efectoChispas, transform.position, Quaternion.identity);

        // 2. Desactivar el modelo visual inmediatamente (hijos incluidos)
        Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders) r.enabled = false;

        // 3. Gestionar física y colisiones
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; // Evita que salga volando como globo
        }

        // Desactivar el collider para que Crash pase a través del humo sin chocar
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. Limpieza de la jerarquía
        Destroy(gameObject, 0.15f);
    }
}