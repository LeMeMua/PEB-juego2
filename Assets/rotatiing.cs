using UnityEngine;

public class rotatiing : MonoBehaviour
{
    private Camera main_camera;
    private Vector3 mouse_position;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main_camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mouse_position = main_camera.ScreenToViewportPoint(Input.mousePosition);
    }
}
