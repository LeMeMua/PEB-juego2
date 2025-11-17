using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class FOV : MonoBehaviour
{
    public int raycount = 2;
    float fov = 140;
    float angle = 0f;
    Vector3 origin = Vector3.zero;
    public float viewDistance = 1f;
    Mesh mesh;
    float watching = 3f;
    float waiting = 3f;
    float timer = 0f;
    float last_time = 0f;
    bool showingFOV = false;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {
        timer += Time.deltaTime;

        if (showingFOV)
        {
            // Mostrar FOV durante 3 segundos
            fov = 120;
            GenerateMesh();

            if (timer >= watching)
            {
                timer = 0f;
                showingFOV = false;
            }
        }
        else
        {
            // Ocultar FOV durante 3 segundos
            fov = 0;
            mesh.Clear(); // Limpia el mesh para que no se vea nada

            if (timer >= waiting)
            {
                timer = 0f;
                showingFOV = true;
            }
        }
    }

    void GenerateMesh()
    {
        Vector3 origin = transform.position;
        float angle = fov / 2;
        float angleIncrease = fov / raycount;

        Vector3[] vertices = new Vector3[raycount + 2];
        int[] triangles = new int[raycount * 3];

        vertices[0] = transform.InverseTransformPoint(origin);

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= raycount; i++)
        {
            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 vertex = Vector3.zero;
            RaycastHit2D raycast = Physics2D.Raycast(origin, new Vector3(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)), viewDistance);
            

            if (raycast.collider == null)
            {
                vertex = origin + new Vector3(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)) * viewDistance;

            }
            else
            {
                if (raycast.collider.CompareTag("Ground") || raycast.collider.CompareTag("Player"))
                {
                    vertex = raycast.point;
                }
                else
                {
                    vertex = origin + new Vector3(Mathf.Sin(angleRad), -Mathf.Cos(angleRad)) * viewDistance;
                }
            }

            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
