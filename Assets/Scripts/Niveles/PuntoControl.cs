using UnityEngine;
public sealed class PuntoControl : MonoBehaviour
{
    public int indice;
    public GestorNivel nivel;
    private void OnTriggerEnter2D(Collider2D otro) { if (otro.GetComponent<RecursosJugador>()!=null) nivel.ActivarCheckpoint(indice); }
}
