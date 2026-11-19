using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public sealed class AudioJugador : MonoBehaviour
{
    public AudioClip salto, garra, pelo, recoger, dano, mejora;
    private AudioSource fuente;
    private Rigidbody2D cuerpo;
    private EntradaJugador entrada;
    private RecursosJugador recursos;
    private SistemaVida vida;
    private float saludAnterior, verticalAnterior, danoAnterior;
    private int objetosAnteriores;
    private void Start()
    {
        fuente=GetComponent<AudioSource>(); fuente.playOnAwake=false; fuente.spatialBlend=0; fuente.volume=.2f;
        cuerpo=GetComponent<Rigidbody2D>(); entrada=GetComponent<EntradaJugador>(); recursos=GetComponent<RecursosJugador>(); vida=GetComponent<SistemaVida>();
        saludAnterior=vida.Actual; objetosAnteriores=Total(); danoAnterior=recursos.MultiplicadorDano;
    }
    private int Total() => recursos.Monedas+recursos.Pollos+recursos.Arrojables+recursos.Velocidades+recursos.Fuerzas;
    private void LateUpdate()
    {
        if(Time.timeScale>0 && !entrada.Bloqueada) {
            if(cuerpo.linearVelocity.y>7 && verticalAnterior<=0) Sonar(salto);
            if(entrada.GarraPulsada) Sonar(garra);
            if(entrada.PeloSoltado || entrada.LanzarPulsado) Sonar(pelo);
        }
        if(vida.Actual<saludAnterior) Sonar(dano);
        if(Total()>objetosAnteriores) Sonar(recoger);
        if(recursos.MultiplicadorDano>danoAnterior) Sonar(mejora);
        verticalAnterior=cuerpo.linearVelocity.y; saludAnterior=vida.Actual; objetosAnteriores=Total(); danoAnterior=recursos.MultiplicadorDano;
    }
    private void Sonar(AudioClip clip) { if(clip!=null) fuente.PlayOneShot(clip); }
}
