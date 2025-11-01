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
        time=Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            isHit = true;
        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            while (golpes < 3 && time> last_hit + 1f) {
                Gamemanager.instance.desactivar_vida(golpes);
                golpes++;
                last_hit = Time.time;
            }
        }
    }

    void Destroyobject()
    {
        Destroy(gameObject);
    }
}
