using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SistemaVida), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public sealed class EnemigoBase : MonoBehaviour
{
    public Transform objetivo;
    public float velocidad = 1.5f;
    public float dano = 10f;
    public float distanciaActivacion = 9f;
    public string Aviso => preparando ? "¡Ataque!" : "";
    private SistemaVida vida;
    private SistemaVida vidaObjetivo;
    private Rigidbody2D cuerpo;
    private SpriteRenderer visual;
    private Color colorBase;
    private float resolverAtaque;
    private float siguienteAtaque;
    private bool preparando;
    private float contactoDesde;

    private void Awake()
    {
        vida = GetComponent<SistemaVida>();
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f;
        cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<BoxCollider2D>().isTrigger = true;
        visual = GetComponentInChildren<SpriteRenderer>();
        if (visual != null) colorBase = visual.color;
    }

    private void OnEnable()
    {
        if (vida == null) vida = GetComponent<SistemaVida>();
        vida.Murio += Morir;
    }
    private void OnDisable() { if (vida != null) vida.Murio -= Morir; }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        var victima = otro.GetComponentInParent<SistemaVida>();
        if (victima != null && victima.esJugador) contactoDesde = Time.time + .35f;
    }

    private void OnTriggerStay2D(Collider2D otro)
    {
        if (vida == null || vida.Muerto || Time.time < contactoDesde) return;
        var victima = otro.GetComponentInParent<SistemaVida>();
        if (victima != null && victima.esJugador && !victima.Muerto)
            victima.RecibirDano(dano);
    }

    private void FixedUpdate()
    {
        if (vida.Muerto) return;
        cuerpo.linearVelocity = Vector2.zero;
        if (objetivo == null) return;
        if (vidaObjetivo == null || vidaObjetivo.transform != objetivo)
            vidaObjetivo = objetivo.GetComponent<SistemaVida>();
        if (vidaObjetivo == null || vidaObjetivo.Muerto) return;
        Vector2 diferencia = objetivo.position - transform.position;
        if (preparando)
        {
            if (Time.time >= resolverAtaque)
            {
                preparando = false;
                if (visual != null) visual.color = colorBase;
                if (Mathf.Abs(diferencia.x) < 1.5f && Mathf.Abs(diferencia.y) < 1.05f)
                    vidaObjetivo.RecibirDano(dano);
                siguienteAtaque = Time.time + 1.1f;
            }
            return;
        }
        if (Mathf.Abs(diferencia.x) > distanciaActivacion) return;
        if (Mathf.Abs(diferencia.x) <= 1.2f)
        {
            if (Time.time >= siguienteAtaque)
            {
                preparando = true;
                resolverAtaque = Time.time + .55f;
                if (visual != null) visual.color = new Color(1f, .45f, .35f);
            }
        }
        else cuerpo.linearVelocity = new Vector2(Mathf.Sign(diferencia.x) * velocidad, 0f);
    }

    private void Morir()
    {
        preparando = false;
        cuerpo.linearVelocity = Vector2.zero;
        foreach (var colision in GetComponentsInChildren<Collider2D>()) colision.enabled = false;
        if (visual != null) visual.color = Color.gray;
        Destroy(gameObject, .3f);
    }
}
