using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SistemaVida), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public sealed class ControladorJefe : MonoBehaviour
{
    [Range(1, 3)] public int nivel = 1;
    public Transform objetivo;
    public Proyectil proyectilPrefab;
    public EnemigoBase refuerzoPrefab;
    public float distanciaActivacion = 13f;
    public float limiteIzquierdo = 63f, limiteDerecho = 76f;
    public float multiplicadorDificultad = 1f; // Lo fija Dificultad.Aplicar al iniciar el nivel.
    public string Aviso { get; private set; } = "";
    // Lectura para presentacion; no altera el patron ni sus temporizadores.
    public bool PreparandoAtaque => preparando;
    public bool Embistiendo => Time.time < finEmbestida;
    public bool SegundaFase => nivel == 2 && vida != null && vida.Actual <= vida.maxima * .5f;
    public event Action CombateIniciado;
    private bool combateIniciado;
    private SistemaVida vida;
    private SistemaVida vidaObjetivo;
    private Rigidbody2D cuerpo;
    private SpriteRenderer visual;
    private Color colorBase;
    private float siguienteAtaque;
    private bool preparando;
    private int turno;
    private int ataque;
    private Vector2 puntoApuntado;
    private float direccion = -1f;
    private float finEmbestida;
    private bool impactoEmbestida;
    private float alturaBase, inicioSalto, siguienteSalto;
    private bool saltando, ataqueCercano;
    private readonly List<EnemigoBase> refuerzos = new List<EnemigoBase>();

    private void Awake()
    {
        vida = GetComponent<SistemaVida>();
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f;
        cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<BoxCollider2D>().isTrigger = true;
        visual = GetComponentInChildren<SpriteRenderer>();
        if (visual != null) colorBase = visual.color;
        siguienteAtaque = Time.time + 1.5f;
        alturaBase = cuerpo.position.y;
        siguienteSalto = Time.time + 3f;
    }

    private void OnEnable()
    {
        if (vida == null) vida = GetComponent<SistemaVida>();
        vida.Murio += Morir;
    }
    private void OnDisable()
    {
        if (vida != null) vida.Murio -= Morir;
        if (cuerpo != null) cuerpo.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        cuerpo.linearVelocity = Vector2.zero;
        cuerpo.position = new Vector2(Mathf.Clamp(cuerpo.position.x, limiteIzquierdo, limiteDerecho), cuerpo.position.y);
        if (vida.Muerto || objetivo == null) return;
        if (vidaObjetivo == null || vidaObjetivo.transform != objetivo)
            vidaObjetivo = objetivo.GetComponent<SistemaVida>();
        if (vidaObjetivo == null || vidaObjetivo.Muerto) return;
        Vector2 diferencia = objetivo.position - transform.position;
        if (Time.time < finEmbestida)
        {
            cuerpo.linearVelocity = Vector2.right * direccion * 6f;
            if (!impactoEmbestida && Mathf.Abs(diferencia.x) < 1.3f && Mathf.Abs(diferencia.y) < 1f)
            { vidaObjetivo.RecibirDano(18f * multiplicadorDificultad); impactoEmbestida = true; }
            return;
        }
        if (Mathf.Abs(diferencia.x) > distanciaActivacion)
        {
            preparando = false;
            Aviso = "";
            if (visual != null) visual.color = colorBase;
            return;
        }
        if (!combateIniciado)
        {
            combateIniciado = true;
            CombateIniciado?.Invoke();
        }
        float velocidadMovimiento = Dificultad.Actual == NivelDificultad.Facil ? 1f :
            Dificultad.Actual == NivelDificultad.Dificil ? 2.7f : 1.7f;
        if (!preparando)
        {
            float separacion = Mathf.Abs(diferencia.x);
            float sentido = separacion > 4f ? Mathf.Sign(diferencia.x) :
                separacion < 2f ? -Mathf.Sign(diferencia.x) : 0f;
            cuerpo.linearVelocity = new Vector2(sentido * velocidadMovimiento, 0f);
        }
        if (!saltando && Time.time >= siguienteSalto)
        {
            saltando = true;
            inicioSalto = Time.time;
            siguienteSalto = Time.time + (Dificultad.Actual == NivelDificultad.Facil ? 6f :
                Dificultad.Actual == NivelDificultad.Dificil ? 2.8f : 4f);
        }
        if (saltando)
        {
            float fase = (Time.time - inicioSalto) / .8f;
            if (fase >= 1f) { saltando = false; cuerpo.position = new Vector2(cuerpo.position.x, alturaBase); }
            else cuerpo.position = new Vector2(cuerpo.position.x, alturaBase + Mathf.Sin(fase * Mathf.PI) * 1.2f);
        }
        if (Time.time < siguienteAtaque) return;
        if (preparando)
        {
            preparando = false;
            if (visual != null) visual.color = colorBase;
            EjecutarAtaque();
            siguienteAtaque = Time.time + (nivel == 1 ? 1.6f : 1.3f);
        }
        else PrepararAtaque();
    }

    private void PrepararAtaque()
    {
        preparando = true;
        puntoApuntado = objetivo.position; // El telegrafo fija el blanco: se puede esquivar.
        direccion = puntoApuntado.x >= transform.position.x ? 1f : -1f;
        ataque = turno++ % 3;
        ataqueCercano = Vector2.Distance(objetivo.position, transform.position) < 2f;
        if (nivel == 1)
            Aviso = ataque == 0 ? "¡Bolsa de basura!" : ataque == 1 ? "¡Mapache!" : "¡Tambo rodante: salta!";
        else if (nivel == 2)
            Aviso = SegundaFase ? "¡Agua santa: ralentiza!" : "¡Nube de incienso!";
        else
            Aviso = ataque == 0 ? "¡Embestida: salta!" : ataque == 1 ? "¡Objeto arrojado!" : "¡Refuerzos!";
        if (ataqueCercano) Aviso = "¡Ataque cercano!";
        if (visual != null) visual.color = new Color(1f, .5f, .5f);
        siguienteAtaque = Time.time + (nivel == 3 ? .85f : .75f);
    }

    private void EjecutarAtaque()
    {
        if (ataqueCercano)
        {
            if (objetivo != null && Vector2.Distance(objetivo.position, transform.position) < 2.2f)
                vidaObjetivo.RecibirDano(14f * multiplicadorDificultad);
            return;
        }
        if (nivel == 1)
        {
            if (ataque == 0) Disparar("Bolsa de basura", Color.green, .55f, 5f, 12f, 0f, false);
            else if (ataque == 1) Disparar("Mapache lanzado", Color.gray, .7f, 6f, 13f, 0f, false);
            else Disparar("Tambo rodante", new Color(.5f, .3f, .1f), .8f, 4f, 15f, 0f, true);
        }
        else if (nivel == 2)
        {
            if (SegundaFase) Disparar("Agua santa", Color.cyan, .55f, 6f, 12f, 3f, false);
            else Disparar("Incienso", new Color(.75f, .5f, .9f), 1.1f, 3.5f, 14f, 0f, false);
        }
        else if (ataque == 0)
        {
            finEmbestida = Time.time + .55f;
            impactoEmbestida = false;
        }
        else if (ataque == 1) Disparar("Objeto del lider", new Color(.8f, .6f, .35f), .7f, 7f, 16f, 0f, false);
        else LlamarRefuerzos();
    }

    private void Disparar(string nombre, Color color, float escala, float rapidez, float dano,
        float lentitud, bool rodante)
    {
        if (proyectilPrefab == null) return;
        Vector3 posicion = transform.position + Vector3.right * direccion * 1f;
        if (rodante) posicion.y = .5f;
        var proyectil = Instantiate(proyectilPrefab, posicion, Quaternion.identity);
        proyectil.name = nombre;
        proyectil.transform.localScale = Vector3.one * escala;
        var sprite = proyectil.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null) sprite.color = color;
        Vector2 trayectoria = rodante ? Vector2.right * direccion : (puntoApuntado - (Vector2)posicion).normalized;
        proyectil.Lanzar(trayectoria, rapidez, dano * multiplicadorDificultad, false, lentitud);
    }

    private void LlamarRefuerzos()
    {
        refuerzos.RemoveAll(enemigo => enemigo == null);
        if (refuerzoPrefab == null || refuerzos.Count >= 2) return;
        Vector3 posicion = transform.position + Vector3.right * -direccion * 2f;
        posicion.y = .65f;
        var refuerzo = Instantiate(refuerzoPrefab, posicion, Quaternion.identity);
        refuerzo.objetivo = objetivo;
        refuerzo.dano *= multiplicadorDificultad;
        refuerzos.Add(refuerzo);
    }

    private void Morir()
    {
        preparando = false;
        finEmbestida = 0f;
        Aviso = "Jefe derrotado";
        cuerpo.linearVelocity = Vector2.zero;
        foreach (var colision in GetComponentsInChildren<Collider2D>()) colision.enabled = false;
        if (visual != null) visual.color = Color.gray;
        foreach (var refuerzo in refuerzos) if (refuerzo != null) Destroy(refuerzo.gameObject);
        refuerzos.Clear();
        // Se conserva la raiz para que el nivel observe vida/Murio y gestione su progresion.
    }
}
