using UnityEngine;
public sealed class PresentacionRescate : MonoBehaviour
{
    public GestorNivel nivel;
    public SpriteRenderer personaje;
    public Sprite cautiva, rescatada;
    public GameObject jaula;
    private void LateUpdate()
    {
        if(nivel==null || nivel.Datos==null) return;
        bool libre=nivel.Datos.rescatada;
        if(personaje!=null) personaje.sprite=libre ? rescatada : cautiva;
        if(jaula!=null) jaula.SetActive(!libre);
    }
}
