using UnityEngine;

public class agua : MonoBehaviour
{
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
