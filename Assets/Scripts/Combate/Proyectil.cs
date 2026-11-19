using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public sealed class Proyectil : MonoBehaviour
{
    [Min(.1f)] public float duracion = 4f;
    [Min(0f)] public float gravedad = 1f;
    private Rigidbody2D cuerpo;
    private float dano;
    private bool delJugador;
    private float ralentizacion;
    private bool activo;
    private float caduca;
    private readonly RaycastHit2D[] impactos = new RaycastHit2D[24];

    private void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = gravedad;
        cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    public void Lanzar(Vector2 direccion, float velocidad, float dano, bool delJugador, float ralentizacion = 0f)
    {
        if (cuerpo == null) Awake();
        this.dano = Mathf.Max(0f, dano);
        this.delJugador = delJugador;
        this.ralentizacion = Mathf.Max(0f, ralentizacion);
        cuerpo.gravityScale = delJugador ? gravedad : 0f;
        cuerpo.linearVelocity = direccion.normalized * Mathf.Max(0f, velocidad);
        activo = true;
        caduca = Time.time + duracion;
    }

    private void FixedUpdate()
    {
        if (!activo) return;
        if (Time.time >= caduca) { Retirar(); return; }
        // Barrido suplementario: los triggers rapidos no deben atravesar paredes delgadas.
        Vector2 velocidad = cuerpo.linearVelocity;
        var filtro = new ContactFilter2D { useTriggers = true };
        int cantidad = Physics2D.CircleCast(cuerpo.position, .1f, velocidad.normalized,
            filtro, impactos, velocidad.magnitude * Time.fixedDeltaTime);
        for (int i = 0; i < cantidad && activo; i++) Impactar(impactos[i].collider);
    }

    private void OnTriggerEnter2D(Collider2D otro) => Impactar(otro);

    private void Impactar(Collider2D otro)
    {
        if (!activo || otro == null || otro.transform.IsChildOf(transform)) return;
        if (otro.GetComponentInParent<Proyectil>() != null) return;
        var vida = otro.GetComponentInParent<SistemaVida>();
        if (vida != null)
        {
            if (vida.esJugador == delJugador || vida.Muerto) return;
            bool recibido = vida.RecibirDano(dano);
            if (recibido && ralentizacion > 0f)
                vida.GetComponent<RecursosJugador>()?.Ralentizar(ralentizacion);
            Retirar();
        }
        else if (!otro.isTrigger) Retirar(); // Las pistas/recogibles trigger no bloquean.
    }

    private void Retirar()
    {
        activo = false;
        cuerpo.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject);
    }
}
