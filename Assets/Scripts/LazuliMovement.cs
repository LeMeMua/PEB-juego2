using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 1.5f;
    public float bulletSpeed = 10f;
    bool isFacingRight = true;
    float jumpPower = 3f;
    bool isJumping = false;
    bool isGrounded = true;
    bool isVarita = false;
    float anguloRadianes;
    float anguloGrados;

    int cantidad_agua = 0;
    float time_cooldown = 0.1f;

    private Animator animator; // Cambié el nombre a minúscula por convención
    Rigidbody2D rb;

    private Vector3 objetivo;
    [SerializeField] private Camera cam;

    public GameObject agua;
    public GameObject varita;
    public Transform marcador;
    public Transform marcador2;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // CORRECCIÓN: Obtener el Animator, no Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        objetivo=cam.ScreenToWorldPoint(Input.mousePosition);
        anguloRadianes= Mathf.Atan2(objetivo.y-transform.position.y, objetivo.x-transform.position.x);
        anguloGrados= (180/Mathf.PI)*anguloRadianes-90;

        FlipSprite();

        // Animaciones
        animator.SetBool("Running", horizontalInput != 0.0f && isGrounded);
        animator.SetBool("Jumping", isJumping);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("Varita", isVarita);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isJumping = true;
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y); // Corrección para Unity 6.0
    }

    void FlipSprite()
    {
        if (isFacingRight && anguloGrados > 0f || !isFacingRight && anguloGrados < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    void Shoot()
    {
        StartCoroutine(ShootBurst());
    }

    IEnumerator ShootBurst()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bullet = Instantiate(agua, marcador.position, marcador.rotation);
            GameObject bullet2 = Instantiate(agua, marcador2.position, marcador2.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
            rb.linearVelocity = (isFacingRight ? Vector2.right : Vector2.left) * bulletSpeed;
            rb2.linearVelocity = (isFacingRight ? Vector2.right : Vector2.left) * bulletSpeed;

            cantidad_agua += 2;

            Gamemanager.instance.puntos_totales(cantidad_agua);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si está tocando el suelo
        if (collision.gameObject.CompareTag("Ground") || collision.contacts[0].normal.y > 0.7f)
        {
            isJumping = false;
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Verificar si deja de tocar el suelo
        if (collision.gameObject.CompareTag("Ground") || collision.contacts[0].normal.y > 0.7f)
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            isVarita = true;
            Destroy(collision.gameObject); // destruye el objeto con el que chocaste
        }
    }
}