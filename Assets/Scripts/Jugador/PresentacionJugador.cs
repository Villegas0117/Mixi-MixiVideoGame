using UnityEngine;
// Solo lee gameplay. Las animaciones no aplican movimiento, hitboxes ni daño.
public sealed class PresentacionJugador : MonoBehaviour
{
    public Animator animador;
    public SpriteRenderer efectoGarra;
    private Rigidbody2D cuerpo;
    private MovimientoJugador movimiento;
    private SistemaVida vida;
    private SpriteRenderer sprite;
    private CombateJugador combate;
    private float ultimaVida, destelloHasta;
    private void Start()
    {
        cuerpo=GetComponent<Rigidbody2D>(); movimiento=GetComponent<MovimientoJugador>();
        vida=GetComponent<SistemaVida>(); combate=GetComponent<CombateJugador>();
        sprite=animador!=null ? animador.GetComponentInChildren<SpriteRenderer>() : GetComponentInChildren<SpriteRenderer>();
        ultimaVida=vida.Actual;
    }
    private void Update()
    {
        if(vida.Actual<ultimaVida) destelloHasta=Time.time+.22f;
        ultimaVida=vida.Actual;
        if(animador!=null) {
            animador.SetFloat("Velocidad",Mathf.Abs(cuerpo.linearVelocity.x));
            animador.SetBool("EnSuelo",movimiento.EnSuelo);
            animador.SetFloat("Vertical",cuerpo.linearVelocity.y);
            animador.SetBool("Atacando",combate.Accion=="Garra");
            animador.SetBool("Dano",Time.time<destelloHasta);
            animador.SetBool("Pelo",combate.Carga01>0 || combate.Accion.Contains("Pelo"));
            animador.SetBool("Lanzar",combate.Accion=="Lanzar");
        }
        if(sprite!=null) sprite.color=Time.time<destelloHasta ? new Color(1,.4f,.4f) : combate.Carga01>0 ? Color.Lerp(Color.white,new Color(1,.85f,.45f),combate.Carga01) : Color.white;
        if(efectoGarra!=null) efectoGarra.enabled=combate.Accion=="Garra";
    }
}
