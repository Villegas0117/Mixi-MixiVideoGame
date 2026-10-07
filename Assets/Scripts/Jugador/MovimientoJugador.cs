using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EntradaJugador), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public sealed class MovimientoJugador : MonoBehaviour
{
    [Header("Movimiento (unidades por segundo)")]
    [SerializeField, Min(0f)] private float velocidadCaminar = 4f;
    [SerializeField, Min(0f)] private float velocidadCorrer = 7f;
    [SerializeField, Min(0f)] private float velocidadSalto = 10f;
    [Header("Suelo")]
    [SerializeField] private LayerMask capasSuelo = 1;
    [SerializeField, Range(0f, 89f)] private float pendienteMaxima = 50f;
    [SerializeField, Min(0f)] private float toleranciaAscenso = 0.1f;
    [Header("Presentacion temporal")]
    [SerializeField] private Transform visual;

    private EntradaJugador entrada;
    private Rigidbody2D cuerpo;
    private CapsuleCollider2D colisionador;
    private readonly ContactPoint2D[] contactos = new ContactPoint2D[16];
    public bool EnSuelo { get; private set; }

    private void Awake()
    {
        entrada = GetComponent<EntradaJugador>();
        cuerpo = GetComponent<Rigidbody2D>();
        colisionador = GetComponent<CapsuleCollider2D>();
    }

    private void FixedUpdate()
    {
        ActualizarSuelo();
        float horizontal = entrada.Horizontal;
        Vector2 velocidad = cuerpo.linearVelocity;
        velocidad.x = horizontal * (entrada.Corriendo ? velocidadCorrer : velocidadCaminar);
        // Consumir siempre: una pulsacion en el aire no queda retenida hasta aterrizar.
        if (entrada.ConsumirSalto() && EnSuelo)
        {
            velocidad.y = velocidadSalto;
            EnSuelo = false;
        }
        cuerpo.linearVelocity = velocidad;
        if (visual != null && !Mathf.Approximately(horizontal, 0f))
        {
            Vector3 escala = visual.localScale;
            escala.x = Mathf.Abs(escala.x) * Mathf.Sign(horizontal);
            visual.localScale = escala;
        }
    }

    private void ActualizarSuelo()
    {
        EnSuelo = false;
        if (cuerpo.linearVelocity.y > toleranciaAscenso) return;
        ContactFilter2D filtro = new ContactFilter2D();
        filtro.SetLayerMask(capasSuelo);
        filtro.useTriggers = false;
        int cantidad = colisionador.GetContacts(filtro, contactos);
        float normalMinima = Mathf.Cos(pendienteMaxima * Mathf.Deg2Rad);
        for (int i = 0; i < cantidad; i++)
        {
            // Las paredes y los techos no habilitan el salto.
            if (contactos[i].normal.y >= normalMinima)
            {
                EnSuelo = true;
                break;
            }
        }
    }

    private void OnDisable() => EnSuelo = false;
}
