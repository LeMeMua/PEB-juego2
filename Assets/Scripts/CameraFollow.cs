
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.5f;
    [SerializeField] Vector3 offset;
    private void LateUpdate()
    {
        if (target == null) return;

        // Solo calcular la posición deseada en X, mantener Y de la cámara
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Solo actualizar X, mantener Y y Z actuales
        transform.position = new Vector3(smoothPosition.x, transform.position.y, transform.position.z);
    }

}
