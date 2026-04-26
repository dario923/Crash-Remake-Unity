using UnityEngine;

public class ControlCrash : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    [Header("Configuración de Movimiento")]
    public float fuerzaSalto = 12f; 
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 14f; 
    
    [Header("Configuración de Patada")]
    public float fuerzaPatadaHaciaArriba = 1.5f; 
    public float tiempoGolpe = 0.6f;
    public float radioDeAtaque = 15f; 
    public float fuerzaEmpujeEnemigo = 25f; 

    [Header("Efectos Visuales (Impacto)")]
    public GameObject efectoExplosion; 
    public GameObject efectoChispas;   

    private string estadoActual = "Idle";
    private bool estaGolpeando = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>(); // Referencia al Rigidbody de NuevoPivote
        
        if (rb != null) 
        {
            rb.mass = 1.5f; 
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        if (!estaGolpeando) 
        {
            ManejarMovimiento();
            if (Input.GetKeyDown(KeyCode.Space)) EmpezarPatada();
            if (Input.GetKeyDown(KeyCode.X)) Saltar();
        }
    }

    void ManejarMovimiento()
    {
        float hW = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        float vW = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        float hF = (Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        float vF = (Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);

        Vector3 direccion = Vector3.zero;
        float velActual = 0f;
        string nuevoEstado = "Idle";

        if (hF != 0 || vF != 0) {
            direccion = new Vector3(hF, 0, vF);
            velActual = velocidadCorrer;
            nuevoEstado = "Running";
        } else if (hW != 0 || vW != 0) {
            direccion = new Vector3(hW, 0, vW);
            velActual = velocidadCaminar;
            nuevoEstado = "Walking";
        }

        if (nuevoEstado != estadoActual) {
            if (nuevoEstado == "Running") animator.SetTrigger("tCorrer");
            else if (nuevoEstado == "Walking") animator.SetTrigger("tCaminar");
            else animator.SetTrigger("tParar");
            estadoActual = nuevoEstado;
        }

        if (rb != null) {
            if (direccion != Vector3.zero) {
                direccion.Normalize();
                rb.linearVelocity = new Vector3(direccion.x * velActual, rb.linearVelocity.y, direccion.z * velActual);
                transform.forward = Vector3.Slerp(transform.forward, direccion, Time.deltaTime * 15f);
            } else {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                rb.angularVelocity = Vector3.zero; 
            }
        }
    }

    void EmpezarPatada()
    {
        estaGolpeando = true;
        animator.SetTrigger("Patear");
        
        if (rb != null) rb.linearVelocity = new Vector3(0, fuerzaPatadaHaciaArriba, 0); 

        // Escaneo de área para detectar al Mecha
        Collider[] hits = Physics.OverlapSphere(transform.position, radioDeAtaque);
        
        foreach (Collider col in hits)
        {
            // Busca el tag "Enemigo" en el objeto o sus padres
            if (col.CompareTag("Enemigo") || (col.transform.parent != null && col.transform.parent.CompareTag("Enemigo")))
            {
                GameObject objetivo = col.CompareTag("Enemigo") ? col.gameObject : col.transform.parent.gameObject;
                ProcesarImpacto(objetivo);
                break; 
            }
        }
        Invoke("TerminarPatada", tiempoGolpe);
    }

    void ProcesarImpacto(GameObject enemigo)
    {
        // 1. Instanciar efectos en la posición del enemigo
        if (efectoExplosion != null) Instantiate(efectoExplosion, enemigo.transform.position, Quaternion.identity);
        if (efectoChispas != null) Instantiate(efectoChispas, enemigo.transform.position, Quaternion.identity);
        
        // 2. Anular física para evitar el efecto globo
        Rigidbody rbEnemigo = enemigo.GetComponent<Rigidbody>();
        if (rbEnemigo != null)
        {
            rbEnemigo.linearVelocity = Vector3.zero;
            rbEnemigo.isKinematic = true; 
        }

        // 3. Desactivar visuales y colisiones inmediatamente
        Renderer[] renders = enemigo.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renders) r.enabled = false;

        Collider colEnemigo = enemigo.GetComponent<Collider>();
        if (colEnemigo != null) colEnemigo.enabled = false;

        // 4. Borrar el objeto del juego
        Destroy(enemigo, 0.05f);
    }

    void TerminarPatada() { estaGolpeando = false; }

    void Saltar()
    {
        if (rb != null) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); 
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
        animator.SetTrigger("Saltar");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeAtaque);
    }
}