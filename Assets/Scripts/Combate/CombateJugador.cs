using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SistemaVida))]
public sealed class CombateJugador : MonoBehaviour
{
    public Proyectil proyectilPrefab;
    [Min(.1f)] public float tiempoCarga = 1.2f;
    [Min(0f)] public float danoGarra = 22f;
    [Min(.1f)] public float alcanceGarra = 1.25f;
    public float Carga01 => cargando ? Mathf.Clamp01(carga / tiempoCarga) : 0f;
    public string Accion { get; private set; } = "";
    private EntradaJugador entrada;
    private MovimientoJugador movimiento;
    private RecursosJugador recursos;
    private SistemaVida vida;
    private bool cargando;
    private float carga;
    private float proximaGarra;
    private float proximoDisparo;
    private float finAccion;
    private readonly Collider2D[] blancos = new Collider2D[32];
    private readonly HashSet<SistemaVida> golpeados = new HashSet<SistemaVida>();

    private void Awake()
    {
        entrada = GetComponent<EntradaJugador>();
        movimiento = GetComponent<MovimientoJugador>();
        recursos = GetComponent<RecursosJugador>();
        vida = GetComponent<SistemaVida>();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || entrada == null || !entrada.isActiveAndEnabled || entrada.Bloqueada || vida.Muerto)
        { CancelarCarga(); return; }
        if (Time.time > finAccion) Accion = "";
        if (entrada.GarraPulsada && Time.time >= proximaGarra) Garra();
        if (entrada.PeloPulsado && Time.time >= proximoDisparo)
        { cargando = true; carga = 0f; }
        if (cargando)
        {
            carga += Time.deltaTime;
            if (entrada.PeloSoltado) SoltarPelo();
            else if (!entrada.PeloMantenido && !entrada.PeloPulsado) CancelarCarga();
        }
        if (entrada.LanzarPulsado && Time.time >= proximoDisparo && proyectilPrefab != null &&
            recursos != null && recursos.ConsumirArrojable())
        {
            Disparar(30f, 11f, .4f, "Ropa");
            MostrarAccion("Lanzar");
        }
    }

    private float Direccion => movimiento != null ? movimiento.Direccion : 1f;
    private float Fuerza => recursos != null ? recursos.MultiplicadorDano : 1f;

    private void Garra()
    {
        proximaGarra = Time.time + .38f;
        MostrarAccion("Garra");
        golpeados.Clear();
        Vector2 centro = (Vector2)transform.position + Vector2.right * Direccion * .65f;
        var filtro = new ContactFilter2D { useTriggers = true };
        int cantidad = Physics2D.OverlapBox(centro, new Vector2(alcanceGarra, 1.3f), 0f, filtro, blancos);
        for (int i = 0; i < cantidad; i++)
        {
            var blanco = blancos[i].GetComponentInParent<SistemaVida>();
            if (blanco != null && !blanco.esJugador && golpeados.Add(blanco))
                blanco.RecibirDano(danoGarra * Fuerza);
        }
    }

    private void SoltarPelo()
    {
        float potencia = Carga01;
        CancelarCarga();
        if (proyectilPrefab == null || (recursos != null && !recursos.ConsumirComida(Mathf.Lerp(2f, 6f, potencia))))
            return;
        // Velocidad reducida ~20% y trayectoria parabolica por gravedad del proyectil.
        float angulo = Mathf.Lerp(.15f, .4f, potencia);
        Vector2 direccion = new Vector2(Direccion, angulo).normalized;
        Disparar(Mathf.Lerp(14f, 42f, potencia), Mathf.Lerp(8f, 12f, potencia),
            Mathf.Lerp(.3f, .7f, potencia), "Bola de pelo", direccion);
        MostrarAccion(potencia >= .95f ? "Pelo cargado" : "Pelo");
    }

    private void Disparar(float dano, float velocidad, float escala, string nombre, Vector2? direccion = null)
    {
        var proyectil = Instantiate(proyectilPrefab, transform.position + Vector3.right * Direccion * .6f,
            Quaternion.identity);
        proyectil.name = nombre;
        proyectil.transform.localScale = Vector3.one * escala;
        proyectil.Lanzar(direccion ?? (Vector2.right * Direccion), velocidad, dano * Fuerza, true);
        proximoDisparo = Time.time + .35f;
    }

    private void MostrarAccion(string nombre) { Accion = nombre; finAccion = Time.time + .22f; }
    private void CancelarCarga() { cargando = false; carga = 0f; }
    private void OnDisable() { CancelarCarga(); Accion = ""; }
    private void OnApplicationFocus(bool foco) { if (!foco) CancelarCarga(); }
}
