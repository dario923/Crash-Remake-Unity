using UnityEngine;
using System.Collections;

public class ControladorDePersonagem : MonoBehaviour {

    [Header("Variables de Estado")]
    public int estado; 

    private float velocidadDeGiro = 4.0f;
    private float horizontal;
    private Rigidbody rb;
    
    [Header("Configuración de Animación")]
    public Animator animacao; 
    
    [Header("Ajustes de Velocidad Física")]
    public float velocidadCaminar = 5f; 
    public float velocidadCorrer = 8f;
    
    private bool estaAtacando = false;
    public float duracionAtaque = 0.7f;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        
        // Congelamos la rotación física para que no se caiga al chocar
        if (rb != null) rb.freezeRotation = true;

        // Buscamos el Animator automáticamente si no está asignado
        if (animacao == null) animacao = GetComponent<Animator>();
        
        if (animacao == null) {
            Debug.LogError("¡No se encontró un Animator en Crash!");
        }

        horizontal = transform.eulerAngles.y;
    }

    void Update ()
    {
        // Bloqueo del mouse para controlar la cámara
        if (Cursor.lockState != CursorLockMode.Locked) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        // Leemos las teclas
        Controle();
        
        // Ataque (Spin)
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Mouse0)) && !estaAtacando)
        {
            StartCoroutine(EjecutarGiro());
        }
    }

    void FixedUpdate()
    {
        // Movimiento físico y actualización de animaciones
        MovePersonagem();
        Anima_Personagem();
    }

    private void Controle()
    {
        // Importante: No hay condiciones de bloqueo aquí para que siempre responda
        estado = 0;

        if (Input.GetKey(KeyCode.W)) 
        {
            estado = 1; // Caminar
            if (Input.GetKey(KeyCode.LeftShift)) estado = 2; // Correr
        }
        else if (Input.GetKey(KeyCode.S)) 
        {
            estado = 3; // Atrás
        }
        else if (Input.GetKey(KeyCode.D)) 
        {
            estado = 4; // Derecha
        }
        else if (Input.GetKey(KeyCode.A)) 
        {
            estado = 5; // Izquierda
        }
    }

    private void MovePersonagem()
    {
        if (rb == null) return;

        // Rotación con el Mouse
        float mouseHorizontal = Input.GetAxis("Mouse X");
        horizontal += mouseHorizontal * velocidadDeGiro;
        rb.MoveRotation(Quaternion.Euler(0, horizontal, 0));

        // Cálculo de dirección
        Vector3 dir = Vector3.zero;
        float velActual = 0;

        if (estado == 1 || estado == 2) {
            dir = transform.forward;
            velActual = (estado == 2) ? velocidadCorrer : velocidadCaminar;
        } 
        else if (estado == 3) {
            dir = -transform.forward;
            velActual = velocidadCaminar;
        }
        else if (estado == 4) {
            dir = transform.right;
            velActual = velocidadCaminar;
        }
        else if (estado == 5) {
            dir = -transform.right;
            velActual = velocidadCaminar;
        }

        // Aplicamos la velocidad manteniendo la gravedad actual
        Vector3 velocidadFinal = dir * velActual;
        velocidadFinal.y = rb.linearVelocity.y; 
        rb.linearVelocity = velocidadFinal;
    }

    private void Anima_Personagem()
    {
        // Enviamos el valor exacto al parámetro "Estado" del Animator
        if (animacao != null) {
            animacao.SetInteger("Estado", estado);
        }
    }

    IEnumerator EjecutarGiro()
    {
        estaAtacando = true;
        if(animacao != null) animacao.SetTrigger("spin"); 
        Debug.Log("Crash Girando!");
        
        yield return new WaitForSeconds(duracionAtaque);
        estaAtacando = false;
    }
}