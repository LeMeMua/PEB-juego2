using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 1.5f;
    bool isFacingRight = true;
    float jumpPower = 3f;
    bool isJumping = false;
    bool isGrounded = true;
    bool isVarita = false;
    public bool isHurting = false;
    bool isRecharging = false;
    float anguloRadianes;
    float anguloGrados;
    float max_impulse = 2f;
    Vector2 velocity;

    int golpes = 0;
    float last_hit = 0f;
    float time_enemy = 0f;



    public float distanciaMark = 0.05f;   // distancia desde el player a cada mark
    public float spriteOffset = 0f;

    public waterdropplet pool; // pool de agua
    public float sprayRate = 0.02f; // cada cuántos segundos sale una gota
    public float dropletSpeed = 8f; // velocidad del agua

    public float impulso = 0f;

    float sprayTimer = 0f;
    bool spraying = false;

    int cantidad_agua = 100;
    float time_recharge = 5f;  // Aumentado de 3f a 5f para que tarde más recargando
    float time_charging = 0f;

    public GameObject recarga;
    private Animator animator; // Cambié el nombre a minúscula por convención
    public Animator recharge;
    Rigidbody2D rb;

    private Vector3 objetivo;
    [SerializeField] private Camera cam;
    

    public GameObject agua;
    public GameObject varita;
    public GameObject mark;
    public GameObject mark2;
    public Transform marcador;
    public Transform marcador2;

    // Sistema de checkpoints
    private Vector3 lastSafePosition;
    private bool isOnSafeGround = false;
    
    // Sistema anti-pegado lateral
    private bool isCollidingLaterally = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); // CORRECCIÓN: Obtener el Animator, no Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        mark = transform.Find("Marcador").gameObject;
        recarga.SetActive(false);

        // Guardar posición inicial como checkpoint
        lastSafePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        objetivo=cam.ScreenToWorldPoint(Input.mousePosition);
        anguloRadianes= Mathf.Atan2(objetivo.y-transform.position.y, objetivo.x-transform.position.x);
        anguloGrados= (180/Mathf.PI)*anguloRadianes-90;

        FlipSprite(anguloGrados);
        RotateMarks(anguloGrados);

        // Animaciones
        animator.SetBool("Running", horizontalInput != 0.0f && isGrounded);
        animator.SetBool("Jumping", isJumping);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("Varita", isVarita);
        //animator.SetBool("Hurting", isHurting);
        

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            isJumping = true;
            isGrounded = false;
        }

        if (isVarita)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                spraying = true;
                SprayWater();
            }
            else if (Input.GetKey(KeyCode.R) && isGrounded)  // Solo puede recargar cuando está en el suelo
            {
                cantidad_agua = Gamemanager.instance.Puntos_Totales;
                if (cantidad_agua < 100) {
                    spraying = false;
                    sprayTimer = 0f;
                    recarga.SetActive(true);
                    recharge.SetBool("Recharging", true);
                    
                    // Incrementar el timer de recarga
                    time_charging += Time.deltaTime;
                    
                    // Si el tiempo de recarga se completó, recargar
                    if (time_charging >= time_recharge)
                    {
                        // Buscar el componente recargua y llamar al método Recharge
                        recargua recargaScript = recarga.GetComponent<recargua>();
                        if (recargaScript != null)
                        {
                            recargaScript.Recharge();
                        }
                        time_charging = 0f;  // Resetear el timer
                        recharge.SetBool("Recharging", false);
                        recarga.SetActive(false);
                    }
                }
                
            }
            else if (Input.GetKey(KeyCode.R) && !isGrounded)
            {
                // Si intenta recargar en el aire, cancelar cualquier recarga en progreso
                time_charging = 0f;
                recharge.SetBool("Recharging", false);
                recarga.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                recharge.SetBool("Recharging", false);
                time_charging = 0f;  // Resetear si suelta la tecla antes de completar
            }
            else
            {
                spraying = false;
                sprayTimer = 0f;
                // Si deja de estar en el suelo mientras recarga, cancelar la recarga
                if (!isGrounded && time_charging > 0f)
                {
                    time_charging = 0f;
                    recharge.SetBool("Recharging", false);
                    recarga.SetActive(false);
                }
                else if (!Input.GetKey(KeyCode.R))
                {
                    time_charging = 0f;
                    recarga.SetActive(false);
                }
            }
        }
    }

    
    void SprayWater()
    {
        if (cantidad_agua >= 1)
        {
            sprayTimer += Time.deltaTime;

            while (sprayTimer >= sprayRate)
            {
                FireDroplet(marcador);

                sprayTimer -= sprayRate;
            }
        }
    }

    void FireDroplet(Transform spawnPoint)
    {
        GameObject drop = pool.Get();

        if (drop != null)
        {
            drop.transform.position = spawnPoint.position;

            drop.SetActive(true);

            Rigidbody2D drb = drop.GetComponent<Rigidbody2D>();

            cantidad_agua -= 1;
            Gamemanager.instance.puntos_totales(-1);

            Vector2 dir = (objetivo - transform.position).normalized;
            Debug.Log(dir);
            drb.linearVelocity = dir * dropletSpeed;
            if (dir.y < 0.0f) // apuntando hacia abajo
            {
                // empuje directo hacia arriba
                velocity = rb.linearVelocity;
                velocity.y += impulso;
                velocity.y = Mathf.Clamp(velocity.y, 0, max_impulse);
                rb.linearVelocity = velocity;
            }
        }
    }
    private void FixedUpdate()
    {
        // Solo aplicar movimiento horizontal si no está chocando lateralmente con una plataforma
        if (!isCollidingLaterally)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y); // Corrección para Unity 6.0
        }
        else
        {
            // Si está chocando lateralmente, permitir que caiga pero limitar el movimiento horizontal
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void FlipSprite(float grados)
    {
        if (isFacingRight && anguloGrados > 0f || !isFacingRight && anguloGrados < 0f)
        {
            isFacingRight = !isFacingRight;
            GetComponent<SpriteRenderer>().flipX = !isFacingRight;

        }
    }

    void RotateMarks(float grados)
    {
        // 1) Calcula la dirección desde el player hacia el mouse (NO normalices antes de poner posición si quieres usar distancia fija)
        Vector2 dir = (objetivo - transform.position);
        dir.Normalize();

        // 2) Coloca los marks en la dirección correcta, a una distancia fija
        mark.transform.position = (Vector2)transform.position + dir * distanciaMark;

        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Si tu sprite "marca" apunta hacia arriba en el editor, usa spriteOffset = -90 o +90 según necesites.
        mark.transform.rotation = Quaternion.Euler(0f, 0f, ang + spriteOffset);
    }
    //void Shoot()
    //{
    //    StartCoroutine(ShootBurst());
    //}

    //IEnumerator ShootBurst()
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        GameObject bullet = Instantiate(agua, marcador.position, marcador.rotation);
    //        GameObject bullet2 = Instantiate(agua, marcador2.position, marcador2.rotation);
    //        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //        Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
    //        rb.linearVelocity = (isFacingRight ? Vector2.right : Vector2.left) * bulletSpeed;
    //        rb2.linearVelocity = (isFacingRight ? Vector2.right : Vector2.left) * bulletSpeed;

    //        cantidad_agua += 2;

    //        Gamemanager.instance.puntos_totales(cantidad_agua);

    //        yield return new WaitForSeconds(0.05f);
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si está tocando el suelo o plataforma DESDE ARRIBA
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            // Verificar la dirección de la colisión
            // Si el contacto viene desde arriba (normal apunta hacia arriba), es suelo
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Si la normal apunta hacia arriba (Y > 0.5), es una colisión desde arriba
                if (contact.normal.y > 0.5f)
                {
                    isJumping = false;
                    isGrounded = true;
                    // Guardar posición cuando está en suelo seguro
                    lastSafePosition = transform.position;
                    isOnSafeGround = true;
                    break; // Solo necesitamos un contacto válido
                }
            }
        }
        if (collision.gameObject.CompareTag("Enemies"))
        {
            time_enemy = Time.time;
            if (golpes < 3 && time_enemy > last_hit + 1f)
            {
                Gamemanager.instance.desactivar_vida(golpes);
                golpes++;

                last_hit = time_enemy;
                animator.SetBool("Hurting", true);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Verificar si está tocando el suelo o plataforma DESDE ARRIBA
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            // Verificar la dirección de la colisión
            bool isTouchingFromTop = false;
            float bestNormalY = 0f;
            
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > bestNormalY)
                {
                    bestNormalY = contact.normal.y;
                }
                
                if (contact.normal.y > 0.5f)
                {
                    isTouchingFromTop = true;
                    break;
                }
            }
            
            if (isTouchingFromTop)
            {
                isGrounded = true;
                isJumping = false;
                lastSafePosition = transform.position;
                isOnSafeGround = true;
                isCollidingLaterally = false;
            }
            else
            {
                // Chocando lateralmente
                isGrounded = false;
                isCollidingLaterally = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Verificar si deja de tocar el suelo o plataforma
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
            isOnSafeGround = false;
            isCollidingLaterally = false;
        }
    }
    
    // Método público para establecer checkpoint
    public void SetCheckpoint(Vector3 position)
    {
        lastSafePosition = position;
    }

    // Método público para obtener última posición segura
    public Vector3 GetLastSafePosition()
    {
        return lastSafePosition;
    }
    
    // Método para resetear estado de grounded (útil después de respawn)
    public void ResetGroundedState()
    {
        isGrounded = false;
        isJumping = false;
        isOnSafeGround = false;
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