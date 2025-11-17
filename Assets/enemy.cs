using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public GameObject player;
    private Vector3 direction;
    private bool isHit=false;
    public int golpes=0;

    private float time;
    private float last_hit;

    private Animator animator;
    public float horizontalAmplitude = 2f;  // Qué tanto se mueve de lado
    public float horizontalSpeed = 1f;      // Qué tan rápido oscila
    private float timeCounter = 0f;
    public float heightAbovePlayer = 0f;
    private float followSpeed = 3f;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = transform.position - player.transform.position;
        if (direction.x > 0.00f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else transform.localScale = new Vector3(-1.0f,1.0f, 1.0f);
        animator.SetBool("hit", isHit);

        timeCounter += Time.deltaTime * horizontalSpeed;

        float oscillation = Mathf.Sin(timeCounter) * horizontalAmplitude;

        Vector3 targetPos = new Vector3(
            player.transform.position.x + oscillation,
            player.transform.position.y + heightAbovePlayer,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            isHit = true;
        }
    }

    void Destroyobject()
    {
        Destroy(gameObject);
    }
}
