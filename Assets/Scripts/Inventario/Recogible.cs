using UnityEngine;
public enum TipoRecogible { Moneda, Pollo, Ropa, Velocidad, Fuerza, Pista, Salida }
public sealed class Recogible : MonoBehaviour
{
    public TipoRecogible tipo;
    public string identificador;
    public string etiqueta;
    public GestorNivel nivel;
    public void Interactuar(RecursosJugador recursos)
    {
        if (nivel==null) return;
        if (tipo==TipoRecogible.Salida) { nivel.Salir(); return; }
        if (tipo==TipoRecogible.Pista) nivel.ObtenerPista(); else recursos.Recoger(tipo);
        nivel.RegistrarRecogido(identificador);
        gameObject.SetActive(false);
    }
}
