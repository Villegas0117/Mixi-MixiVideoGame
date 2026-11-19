using UnityEngine;

// Parallax sutil por capas. Mantener el pixel art nitido (sin interpolacion).
public sealed class ParallaxVisual : MonoBehaviour
{
    [Range(0f, 1f)] public float factor = 0.05f; // compatibilidad con escenas existentes
    [Range(0f, 1f)] public float factorY = 0f;
    public bool repetirHorizontal = false;
    public float anchoRepeticion = 0f;

    private Transform camara;
    private Vector3 origen, inicioCamara;

    private void Start()
    {
        origen = transform.position;
        if (Camera.main != null) { camara = Camera.main.transform; inicioCamara = camara.position; }
    }

    private void LateUpdate()
    {
        if (camara == null) return;
        Vector3 delta = camara.position - inicioCamara;
        float x = origen.x + delta.x * factor;
        float y = origen.y + delta.y * factorY;
        if (repetirHorizontal && anchoRepeticion > 0f)
        {
            float ciclo = anchoRepeticion;
            x = origen.x + Mathf.Repeat(delta.x * factor, ciclo);
        }
        transform.position = new Vector3(x, y, origen.z);
    }
}
