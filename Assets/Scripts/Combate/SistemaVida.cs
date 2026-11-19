using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SistemaVida : MonoBehaviour
{
    [Min(1f)] public float maxima = 100f;
    public bool esJugador;
    public float Actual { get; private set; }
    public bool Muerto => Actual <= 0f;
    public event Action Murio;
    public event Action Cambio;
    private float vulnerableDesde = float.NegativeInfinity;

    private void Awake() => Actual = Mathf.Max(1f, maxima);
    public void Establecer(float valor) { Actual=Mathf.Clamp(valor, 1, maxima); vulnerableDesde=float.NegativeInfinity; Cambio?.Invoke(); }

    public bool RecibirDano(float cantidad)
    {
        if (Muerto || cantidad <= 0f || float.IsNaN(cantidad) || Time.time < vulnerableDesde) return false;
        Actual = Mathf.Max(0f, Actual - cantidad);
        vulnerableDesde = Time.time + (esJugador ? .8f : .15f);
        Cambio?.Invoke();
        if (Muerto) Murio?.Invoke();
        return true;
    }

    // Restaura una cantidad, incluso al cargar un checkpoint tras derrota.
    public void Restaurar(float cantidad)
    {
        if (cantidad <= 0f || float.IsNaN(cantidad)) return;
        Actual = Mathf.Clamp(Actual + cantidad, 0f, Mathf.Max(1f, maxima));
        Cambio?.Invoke();
    }
}
