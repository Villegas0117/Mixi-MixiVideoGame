using UnityEngine;
// Nombre asignado por los ataques existentes; solo cambia sprite/rotacion local.
public sealed class AspectoProyectil : MonoBehaviour
{
    public Sprite pelo, ropa, bolsa, mapache, tambo, incienso, agua, objeto;
    private SpriteRenderer visual;
    private bool gira;
    private void Start()
    {
        visual=GetComponentInChildren<SpriteRenderer>(); if(visual==null) return;
        string nombre=gameObject.name;
        Sprite elegido=nombre.Contains("Ropa") ? ropa : nombre.Contains("Bolsa") ? bolsa : nombre.Contains("Mapache") ? mapache : nombre.Contains("Tambo") ? tambo : nombre.Contains("Incienso") ? incienso : nombre.Contains("Agua") ? agua : nombre.Contains("lider") ? objeto : pelo;
        if(elegido!=null) { visual.sprite=elegido; visual.transform.localScale=Vector3.one/elegido.bounds.size.y; }
        visual.color=Color.white;
        gira=nombre.Contains("Tambo") || nombre.Contains("Ropa") || nombre.Contains("Objeto");
    }
    private void Update() { if(gira && visual!=null) visual.transform.Rotate(0,0,-240*Time.deltaTime); }
}
