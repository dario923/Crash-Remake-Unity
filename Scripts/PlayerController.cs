using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator _animator; 
    private Rigidbody _rigidbody;
    private Vector3 movement_vector;

    [Header("Movimiento")]
    public float movement_speed = 5.0f;
    public float jump_factor = 15.0f; 
    private float rotation_angle;

    [Header("Ataque")]
    // CAMBIO CLAVE: Ahora es 'public' para que EnemigoExplosion pueda leerlo
    public bool isAttacking = false; 
    public float attackDuration = 0.8f; 

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        
        if (_animator == null) _animator = GetComponentInChildren<Animator>();

        if (_rigidbody != null)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void Update()
    {
        movement_vector = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) movement_vector.z = 1.0f;
        if (Input.GetKey(KeyCode.S)) movement_vector.z = -1.0f;
        if (Input.GetKey(KeyCode.A)) movement_vector.x = -1.0f;
        if (Input.GetKey(KeyCode.D)) movement_vector.x = 1.0f;

        if (_animator != null && !isAttacking)
        {
            if (movement_vector != Vector3.zero) 
                _animator.SetInteger("Status", 1); 
            else
                _animator.SetInteger("Status", 0); 
        }

        if (Input.GetKeyDown(KeyCode.Space) && _rigidbody != null && Mathf.Abs(_rigidbody.linearVelocity.y) < 0.1f)
        {
            _rigidbody.AddForce(Vector3.up * jump_factor, ForceMode.Impulse);
        }

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && !isAttacking)
        {
            StartCoroutine(PerformSpinAttack());
        }
    }

    IEnumerator PerformSpinAttack()
    {
        isAttacking = true;
        if (_animator != null) _animator.SetTrigger("spin"); 
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (movement_vector != Vector3.zero && _rigidbody != null)
        {
            movement_vector = movement_vector.normalized * movement_speed;
            _rigidbody.MovePosition(_rigidbody.position + movement_vector * Time.fixedDeltaTime);
            
            rotation_angle = Mathf.Atan2(movement_vector.x, movement_vector.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, rotation_angle, 0);
            _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, 0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Solo manejamos cajas aquí para no interferir con el script del Mecha
        if (isAttacking)
        {
            if (collision.gameObject.name.ToLower().Contains("crashbox"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}