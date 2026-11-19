using UnityEngine;
[RequireComponent(typeof(MovimientoJugador))]
public sealed class RecursosJugador : MonoBehaviour
{
    public float Comida { get; private set; } = 100f;
    public int Monedas { get; private set; }
    public int Pollos { get; private set; }
    public int Arrojables { get; private set; }
    public int Velocidades { get; private set; }
    public int Fuerzas { get; private set; }
    private float velocidadHasta, fuerzaHasta, lentoHasta;
    private MovimientoJugador movimiento;
    public float MultiplicadorDano => (Time.time < fuerzaHasta ? 1.7f : 1f) * (Comida < 15 ? .7f : 1f);
    public string Efectos => (Time.time < velocidadHasta ? "Velocidad  " : "") + (Time.time < fuerzaHasta ? "Fuerza  " : "") + (Time.time < lentoHasta ? "Agua: lento  " : "") + (Comida < 15 ? "Hambre" : "");
    private void Awake() => movimiento = GetComponent<MovimientoJugador>();
    private void Update()
    {
        Comida = Mathf.Max(0, Comida - Time.deltaTime * .16f);
        movimiento.MultiplicadorVelocidad = (Time.time < velocidadHasta ? 1.35f : 1f) * (Time.time < lentoHasta ? .6f : 1f) * (Comida < 15 ? .85f : 1f);
    }
    public bool ConsumirComida(float cantidad) { if (Comida < cantidad) return false; Comida -= cantidad; return true; }
    public bool ConsumirArrojable() { if (Arrojables <= 0) return false; Arrojables--; return true; }
    public void Ralentizar(float segundos) => lentoHasta = Mathf.Max(lentoHasta, Time.time + segundos);
    public void Recoger(TipoRecogible tipo)
    {
        switch (tipo) {
            case TipoRecogible.Moneda: Monedas++; break;
            case TipoRecogible.Pollo: Pollos++; break;
            case TipoRecogible.Ropa: Arrojables++; break;
            case TipoRecogible.Velocidad: Velocidades++; break;
            case TipoRecogible.Fuerza: Fuerzas++; break;
        }
    }
    public void UsarPollo() { if (Pollos <= 0 || Comida >= 100) return; Pollos--; Comida = Mathf.Min(100, Comida + 50); }
    public void UsarVelocidad() { if (Velocidades <= 0) return; Velocidades--; velocidadHasta = Time.time + 20; }
    public void UsarFuerza() { if (Fuerzas <= 0) return; Fuerzas--; fuerzaHasta = Time.time + 20; }
    public void Cargar(DatosPartida d) { Comida=d.comida; Monedas=d.monedas; Pollos=d.pollos; Arrojables=d.arrojables; Velocidades=d.velocidades; Fuerzas=d.fuerzas; velocidadHasta=Time.time+d.velocidadRestante; fuerzaHasta=Time.time+d.fuerzaRestante; }
    public void Guardar(DatosPartida d) { d.comida=Comida; d.monedas=Monedas; d.pollos=Pollos; d.arrojables=Arrojables; d.velocidades=Velocidades; d.fuerzas=Fuerzas; d.velocidadRestante=Mathf.Clamp(velocidadHasta-Time.time,0,20); d.fuerzaRestante=Mathf.Clamp(fuerzaHasta-Time.time,0,20); }
}
