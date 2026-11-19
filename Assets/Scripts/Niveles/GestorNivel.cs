using System;
using System.Collections.Generic;
using UnityEngine;
public sealed class GestorNivel : MonoBehaviour
{
    public int numero=1;
    public GameObject jugador;
    public ControladorJefe jefe;
    public Transform[] puntosControl;
    public GameObject barrera, piyuInicio;
    public Recogible salida;
    public InterfazPartida interfaz;
    public DatosPartida Datos { get; private set; }
    public string Mensaje { get; private set; }
    public string ErrorGuardado { get; private set; }
    private SistemaVida vida;
    private RecursosJugador recursos;
    private readonly HashSet<string> recogidos=new HashSet<string>();
    private DatosPartida ultimoCheckpoint;
    private float mensajeHasta;
    public string Objetivo => Datos==null ? "" : Datos.rescatada ? "Piyu esta a salvo" : !Datos.pista && numero==1 ? "Encuentra la pista de la placa (E)" : !Datos.jefeDerrotado ? "Avanza hacia " + NombreJefe + " y derrotalo" : numero==1 ? "Libera a la iguana (E)" : numero==2 ? "Recoge la pista del almacen (E)" : "Libera a Piyu (E)";
    public string NombreJefe => numero==1 ? "Vagabundo" : numero==2 ? "Sacerdote Fidel" : "Lider de los secuestradores";
    private void Start()
    {
        Time.timeScale=1;
        Datos=SesionPartida.Datos;
        if (Datos==null || Datos.nivel!=numero) Datos=new DatosPartida { nivel=numero, desbloqueado=numero };
        SesionPartida.Datos=Datos;
        Dificultad.Actual=(NivelDificultad)Mathf.Clamp(Datos.dificultad,0,2);
        Dificultad.Aplicar();
        vida=jugador.GetComponent<SistemaVida>(); recursos=jugador.GetComponent<RecursosJugador>();
        recursos.Cargar(Datos); vida.Establecer(Datos.vida);
        jugador.transform.position=puntosControl[Mathf.Clamp(Datos.checkpoint,0,puntosControl.Length-1)].position;
        foreach (var id in Datos.recogidos) recogidos.Add(id);
        foreach (var r in FindObjectsByType<Recogible>(FindObjectsSortMode.None))
            if (r.tipo!=TipoRecogible.Salida && recogidos.Contains(r.identificador)) r.gameObject.SetActive(false);
        vida.Murio+=Derrota;
        jefe.GetComponent<SistemaVida>().Murio+=JefeDerrotado;
        if (jefe != null) jefe.CombateIniciado += AlIniciarCombateJefe;
        AudioManager.ObtenerOCrear().ReproducirExploracion(numero);
        if (Datos.jefeDerrotado) { jefe.gameObject.SetActive(false); barrera.SetActive(false); }
        if (piyuInicio!=null) piyuInicio.SetActive(numero==1 && SesionPartida.MostrarInicio);
        Guardar();
        if (Datos.rescatada) interfaz.MostrarFinal();
        else if (numero==1 && SesionPartida.MostrarInicio) {
            SesionPartida.MostrarInicio=false;
            interfaz.Dialogo("Chipotle y Piyu", "Chipotle vive con Piyu, una pequena chihuahua negra. Mientras sus duenos estan fuera, tres secuestradores irrumpen en casa...", () => {
                if (piyuInicio!=null) piyuInicio.SetActive(false);
                interfaz.Dialogo("¡Se llevaron a Piyu!", "Chipotle alcanza a identificar la placa del coche. Sigue su rastro por la colonia y el parque hasta el basurero. Busca la pista marcada y pulsa E.", null);
            });
        }
        else if (Datos.checkpoint==0 && !Datos.jefeDerrotado) interfaz.Dialogo(numero==1 ? "La placa del coche" : numero==2 ? "La pista de la iglesia" : "El almacen abandonado", numero==1 ? "Piyu fue secuestrada. Sigue la placa hasta el basurero." : numero==2 ? "La iguana vio a los secuestradores cerca de la iglesia. Cruza las calles y busca al Sacerdote Fidel." : "La pista conduce al almacen tras la escuela. Piyu esta cautiva. Derrota al lider y rescatala.",null);
    }
    private void Update()
    {
        if (Time.time>mensajeHasta) Mensaje="";
        if (jugador.transform.position.y < -6 && !vida.Muerto) vida.RecibirDano(1000);
    }
    public void Avisar(string texto) { Mensaje=texto; mensajeHasta=Time.time+5; }
    public void RegistrarRecogido(string id) { if (!string.IsNullOrEmpty(id)) recogidos.Add(id); }
    public void ObtenerPista() { Datos.pista=true; Avisar("La placa lleva al basurero. ¡Busca al Vagabundo!"); }
    public void ActivarCheckpoint(int indice)
    {
        if (indice<=Datos.checkpoint || indice>=puntosControl.Length || vida.Muerto) return;
        Datos.checkpoint=indice; vida.Restaurar(100); Guardar(); Avisar(ErrorGuardado==null ? "Checkpoint: vida restaurada y partida guardada" : "Checkpoint en memoria; no se pudo guardar en disco");
    }
    private void AlIniciarCombateJefe()
    {
        if (vida != null && vida.Muerto) return;
        AudioManager.Instancia?.ReproducirJefe(numero);
    }
    private void JefeDerrotado()
    {
        // La derrota del jugador tiene prioridad en un intercambio letal simultaneo.
        if (vida.Muerto) return;
        Datos.jefeDerrotado=true; barrera.SetActive(false); vida.Restaurar(35);
        Guardar(); Avisar("¡"+NombreJefe+" derrotado! "+Objetivo);
        if (numero == 3) AudioManager.Instancia?.ReproducirVictoria();
        else AudioManager.Instancia?.ReproducirExploracion(numero);
    }
    public void Guardar()
    {
        Datos.vida=Mathf.Max(1,vida.Actual); recursos.Guardar(Datos);
        Datos.recogidos=new List<string>(recogidos).ToArray();
        ultimoCheckpoint=JsonUtility.FromJson<DatosPartida>(JsonUtility.ToJson(Datos));
        ErrorGuardado=GestorGuardado.Guardar(Datos) ? null : GestorGuardado.Error;
    }
    private void Derrota() => interfaz.MostrarDerrota();
    public void Reintentar()
    {
        SesionPartida.Datos=JsonUtility.FromJson<DatosPartida>(JsonUtility.ToJson(ultimoCheckpoint));
        SesionPartida.Cargar(numero);
    }
    public void Salir()
    {
        if (!Datos.jefeDerrotado) { Avisar("Primero derrota a "+NombreJefe); return; }
        if (numero==1 && !Datos.pista) { Avisar("Falta la pista de la placa en la colonia. Regresa a buscarla."); return; }
        if (numero==3) {
            Datos.rescatada=true; Guardar();
            interfaz.Dialogo("Piyu esta libre", "Chipotle rompe las cadenas. Piyu, la pequena chihuahua negra, vuelve a estar a salvo junto a su amigo. Juntos pueden regresar a casa.", interfaz.MostrarFinal);
            return;
        }
        interfaz.Dialogo(numero==1 ? "La pista de la iguana" : "La pista del almacen", numero==1 ? "Gracias por liberarme, Chipotle. Vi a los secuestradores dirigirse hacia la iglesia. ¡Encontraras a Piyu!" : "Entre las pertenencias de Fidel encuentras la pista: un almacen abandonado tras la escuela. Piyu debe estar alli.", () => {
            recursos.Guardar(Datos);
            Datos.nivel=numero+1; Datos.desbloqueado=Mathf.Max(Datos.desbloqueado,Datos.nivel);
            Datos.checkpoint=0; Datos.pista=false; Datos.jefeDerrotado=false; Datos.recogidos=Array.Empty<string>(); Datos.vida=100;
            SesionPartida.Datos=Datos; SesionPartida.Cargar(Datos.nivel);
        });
    }
    private void OnDestroy()
    {
        if (vida!=null) vida.Murio-=Derrota;
        if (jefe!=null) { jefe.GetComponent<SistemaVida>().Murio-=JefeDerrotado; jefe.CombateIniciado -= AlIniciarCombateJefe; }
        Time.timeScale=1;
    }
}
