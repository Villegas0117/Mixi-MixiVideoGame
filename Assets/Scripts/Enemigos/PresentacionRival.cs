using UnityEngine;
// Adaptador exclusivamente visual: observa estados existentes, sin cambiar IA ni daño.
public sealed class PresentacionRival : MonoBehaviour
{
    public Animator animador;
    public SpriteRenderer visual;
    public SpriteRenderer aviso;
    private SistemaVida vida;
    private Rigidbody2D cuerpo;
    private EnemigoBase enemigo;
    private ControladorJefe jefe;
    private float ultimaVida, heridoHasta;
    private void Start()
    {
        vida=GetComponent<SistemaVida>(); cuerpo=GetComponent<Rigidbody2D>();
        enemigo=GetComponent<EnemigoBase>(); jefe=GetComponent<ControladorJefe>(); ultimaVida=vida.Actual;
    }
    private void LateUpdate()
    {
        if(vida.Actual<ultimaVida) heridoHasta=Time.time+.18f;
        ultimaVida=vida.Actual;
        Transform objetivo=enemigo!=null ? enemigo.objetivo : jefe!=null ? jefe.objetivo : null;
        bool atacando=enemigo!=null ? !string.IsNullOrEmpty(enemigo.Aviso) : jefe!=null && (jefe.PreparandoAtaque || jefe.Embistiendo) && !vida.Muerto;
        if(visual!=null) {
            if(objetivo!=null) visual.flipX=objetivo.position.x<transform.position.x;
            visual.color=vida.Muerto ? Color.gray : Time.time<heridoHasta ? new Color(1,.7f,.7f) : atacando ? new Color(1,.55f,.5f) : Color.white;
        }
        if(animador!=null) { animador.SetFloat("Velocidad",cuerpo.linearVelocity.magnitude); animador.SetBool("Atacando",atacando); animador.SetBool("Dano",Time.time<heridoHasta); animador.SetBool("Muerto",vida.Muerto); }
        if(aviso!=null) { aviso.enabled=atacando && !vida.Muerto; aviso.color=Time.time<heridoHasta ? Color.white : new Color(1,.5f,.25f,.75f); }
    }
}
