using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class CamaraSeguimiento : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector3 desplazamiento = new Vector3(0f, 1.5f, -10f);

    private void LateUpdate()
    {
        // Rigidbody2D interpolado; sin otro sistema de suavizado ni dependencias.
        if (objetivo != null) transform.position = objetivo.position + desplazamiento;
    }
}
