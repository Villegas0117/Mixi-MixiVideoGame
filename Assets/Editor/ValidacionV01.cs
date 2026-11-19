using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

// Prueba de integracion en Play real con teclado virtual. No equivale a una partida manual.
public static class ValidacionV01
{
    private static readonly Stack<IEnumerator> pasos=new Stack<IEnumerator>();
    private static Keyboard teclado;
    private static InputSettings ajustesOriginales, ajustesPrueba;
    private static Key[] sostenidas=Array.Empty<Key>();
    private static int eventos;
    private static void AntesEntrada() { if(InputState.currentUpdateType!=InputUpdateType.Editor && teclado!=null) { eventos++; InputSystem.QueueStateEvent(teclado,new KeyboardState(sostenidas)); } }
    private static double limite;
    private static int aprobadas;
    private static string error;
    private static bool terminado;
    private const string Ruta="/tmp/mixi-v01-prueba-guardado.json";
    private static bool recorrido;
    public static void EjecutarRecorrido() { recorrido=true; EjecutarPlay(); }
    public static void EjecutarPlay()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Solo batchmode: sale del editor al terminar.");
        aprobadas=0; error=null; terminado=false;
        EditorSceneManager.OpenScene("Assets/Escenas/MenuPrincipal.unity");
        EditorApplication.playModeStateChanged+=AlCambiar;
        Application.logMessageReceived+=Log;
        limite=EditorApplication.timeSinceStartup+(recorrido ? 300 : 150);
        EditorApplication.update+=Actualizar;
        EditorApplication.EnterPlaymode();
    }
    private static void AlCambiar(PlayModeStateChange estado)
    {
        if (estado==PlayModeStateChange.EnteredPlayMode) {
            GestorGuardado.RutaPruebas=Ruta;
            ajustesOriginales=InputSystem.settings; ajustesPrueba=UnityEngine.Object.Instantiate(ajustesOriginales);
            ajustesPrueba.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            ajustesPrueba.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            InputSystem.settings=ajustesPrueba;
            teclado=InputSystem.AddDevice<Keyboard>("TecladoPruebaMixi");
            InputSystem.onBeforeUpdate+=AntesEntrada;
            pasos.Push(recorrido ? Recorrido() : Pruebas());
        }
    }
    private static void Log(string mensaje,string pila,LogType tipo)
    { if(tipo==LogType.Exception || tipo==LogType.Error || tipo==LogType.Assert) error=mensaje+"\n"+pila; }
    private static void Actualizar()
    {
        if(terminado) return;
        try {
            if(error!=null) throw new Exception(error);
            if(EditorApplication.timeSinceStartup>limite) throw new Exception("Timeout Play Mode");
            if(pasos.Count==0) return;
            var paso=pasos.Peek();
            if(!paso.MoveNext()) { pasos.Pop(); if(pasos.Count==0) Finalizar(0); }
            else if(paso.Current is IEnumerator siguiente) pasos.Push(siguiente);
        } catch(Exception e) { Debug.Log("MIXI_PLAY_FAIL: "+e); Finalizar(1); }
    }
    private static void Finalizar(int codigo)
    {
        terminado=true; EditorApplication.update-=Actualizar; EditorApplication.playModeStateChanged-=AlCambiar; Application.logMessageReceived-=Log;
        InputSystem.onBeforeUpdate-=AntesEntrada;
        if(teclado!=null) InputSystem.RemoveDevice(teclado);
        GestorGuardado.RutaPruebas=null; Time.timeScale=1;
        if(ajustesOriginales!=null) InputSystem.settings=ajustesOriginales;
        if(ajustesPrueba!=null) UnityEngine.Object.DestroyImmediate(ajustesPrueba);
        Debug.Log("MIXI_PLAY_RESULTADO: "+aprobadas+" comprobaciones aprobadas; salida="+codigo);
        EditorApplication.Exit(codigo);
    }
    private static void Exigir(bool condicion,string nombre) { if(!condicion) throw new Exception(nombre); aprobadas++; Debug.Log("MIXI_TEST_OK: "+nombre); }
    private static IEnumerator Esperar(float segundos) { double fin=EditorApplication.timeSinceStartup+segundos; while(EditorApplication.timeSinceStartup<fin) yield return null; }
    private static void Teclas(params Key[] teclas) => sostenidas=teclas;
    private static IEnumerator Pulsar(Key tecla) { Teclas(tecla); yield return Esperar(.12f); Teclas(); yield return Esperar(.12f); }
    private static void Captura(string nombre) {
        if(SystemInfo.graphicsDeviceType!=UnityEngine.Rendering.GraphicsDeviceType.Null) ScreenCapture.CaptureScreenshot("/tmp/mixi-v01-"+nombre+".png");
    }
    private static GestorNivel Nivel() => UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
    private static void Cerrar(GestorNivel n) { for(int i=0;i<3 && n.interfaz.Estado=="Dialogo";i++) n.interfaz.CerrarDialogo(); }
    private static void Mover(GestorNivel n,float x,float y=.65f) { var rb=n.jugador.GetComponent<Rigidbody2D>(); rb.position=new Vector2(x,y); rb.linearVelocity=Vector2.zero; Physics2D.SyncTransforms(); }
    private static IEnumerator Recorrido()
    {
        yield return Esperar(.3f); SesionPartida.Nueva(); yield return Esperar(.5f);
        int ultimoNivel=0;
        while(true) {
            var n=Nivel();
            if(n==null) { yield return Esperar(.2f); continue; }
            if(n.interfaz.Estado=="Creditos") { Exigir(n.Datos.rescatada,"Recorrido completo sin teletransporte ni dano inyectado"); Teclas(); yield break; }
            if(n.numero!=ultimoNivel) { ultimoNivel=n.numero; Debug.Log("MIXI_BOT_NIVEL: "+ultimoNivel); }
            if(n.interfaz.Estado=="Dialogo") { Teclas(); Cerrar(n); yield return Esperar(.3f); continue; }
            if(n.interfaz.Estado=="Derrota") throw new Exception("Recorrido: derrota en nivel "+n.numero+" x="+n.jugador.transform.position.x);
            var r=n.jugador.GetComponent<RecursosJugador>();
            if(r.Comida<50 && r.Pollos>0) r.UsarPollo();
            if(n.jugador.transform.position.x>60 && r.Fuerzas>0) r.UsarFuerza();
            var teclas=new List<Key>();
            float x=n.jugador.transform.position.x;
            bool combatir=!n.Datos.jefeDerrotado && x>n.jefe.transform.position.x-1.1f;
            bool recogerPista=n.numero==1 && !n.Datos.pista && x>9.5f;
            if(!combatir && !recogerPista && x<81) teclas.Add(Key.D);
            if(Time.time%0.48f<.22f) { teclas.Add(Key.J); teclas.Add(Key.E); }
            if(!recogerPista && x<78 && Time.time%1.0f<.13f) teclas.Add(Key.Space);
            if(!n.Datos.jefeDerrotado && x>60 && Time.time%1.8f<1.35f) teclas.Add(Key.K);
            Teclas(teclas.ToArray());
            yield return Esperar(.055f);
        }
    }
    private static IEnumerator Pruebas()
    {
        yield return Esperar(.5f);
        Exigir(UnityEngine.Object.FindFirstObjectByType<InterfazPartida>().Estado=="Menu","Menu inicial"); Captura("menu"); yield return Esperar(.3f);
        SesionPartida.Nueva(); yield return Esperar(.6f);
        var n=Nivel(); Exigir(n!=null && n.numero==1,"Nueva partida abre nivel 1");
        Exigir(n.interfaz.Estado=="Dialogo" && Time.timeScale==0,"Narrativa inicial pausa controles");
        Cerrar(n); yield return Esperar(.3f);
        var j=n.jugador; var rb=j.GetComponent<Rigidbody2D>(); var vida=j.GetComponent<SistemaVida>(); var recursos=j.GetComponent<RecursosJugador>();
        Exigir(n.interfaz.Estado=="Juego" && Time.timeScale==1,"Cerrar narrativa reanuda juego");
        Captura("barrio"); yield return Esperar(.3f);
        float x=rb.position.x; Teclas(Key.D); yield return Esperar(.6f);
        var action=(InputAction)typeof(EntradaJugador).GetField("mover",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(j.GetComponent<EntradaJugador>());
        Debug.Log($"MIXI_DEVICE: eventos={eventos} enabled={teclado.enabled} pressed={teclado.dKey.isPressed} mode={InputSystem.settings.updateMode} action={action.enabled} controls={action.controls.Count} focus={Application.isFocused} blocked={j.GetComponent<EntradaJugador>().Bloqueada}");
        Teclas();
        Debug.Log($"MIXI_INPUT_DIAG: x0={x} x={rb.position.x} vx={rb.linearVelocity.x} timeScale={Time.timeScale} estado={n.interfaz.Estado} horizontal={j.GetComponent<EntradaJugador>().Horizontal} frames={Time.frameCount}");
        Exigir(rb.position.x>x+1.2f && Mathf.Abs(rb.linearVelocity.x-4)<.2f,"Caminar mediante Input System real");
        Exigir(j.GetComponent<PresentacionJugador>().animador.GetCurrentAnimatorStateInfo(0).IsName("Walk"),"Animator Walk responde al movimiento");
        yield return Esperar(.15f); x=rb.position.x; Teclas(Key.D,Key.LeftShift); yield return Esperar(.5f); Teclas();
        Exigir(rb.position.x>x+2f,"Correr mantiene velocidad validada");
        Exigir(j.GetComponent<PresentacionJugador>().animador.GetCurrentAnimatorStateInfo(0).IsName("Run"),"Animator Run responde a carrera");
        yield return Esperar(.15f); Mover(n,5); yield return Esperar(.2f);
        Teclas(Key.Space); yield return Esperar(.15f); Teclas();
        Exigir(rb.position.y>1.25f,"Salto con teclado virtual");
        Exigir(j.GetComponent<PresentacionJugador>().animador.GetCurrentAnimatorStateInfo(0).IsName("Jump"),"Animator Jump responde a fisica");
        float vy=rb.linearVelocity.y; yield return Pulsar(Key.Space);
        Exigir(rb.linearVelocity.y<vy,"No doble salto"); yield return Esperar(.8f);
        Exigir(j.GetComponent<MovimientoJugador>().EnSuelo,"Aterrizaje en suelo");
        Mover(n,7); yield return Esperar(.2f); yield return Pulsar(Key.E);
        Exigir(recursos.Monedas==1,"E recoge moneda");
        yield return Pulsar(Key.B); Exigir(n.interfaz.Estado=="Mochila" && Time.timeScale==0,"B abre mochila pausada");
        x=rb.position.x; Teclas(Key.D,Key.J); yield return Esperar(.2f); Teclas(); Exigir(Mathf.Abs(rb.position.x-x)<.01f,"Mochila bloquea movimiento");
        yield return Pulsar(Key.B); yield return Pulsar(Key.Escape); Exigir(n.interfaz.Estado=="Pausa","Esc pausa"); yield return Pulsar(Key.Escape);
        recursos.Recoger(TipoRecogible.Pollo); recursos.ConsumirComida(60); float comida=recursos.Comida; recursos.UsarPollo(); Exigir(recursos.Comida>comida+49,"Pollo restaura comida");
        recursos.Recoger(TipoRecogible.Fuerza); recursos.UsarFuerza(); Exigir(recursos.MultiplicadorDano>1,"Power-up fuerza");
        recursos.Recoger(TipoRecogible.Velocidad); recursos.UsarVelocidad(); yield return Esperar(.1f); Exigir(j.GetComponent<MovimientoJugador>().MultiplicadorVelocidad>1,"Power-up velocidad");
        recursos.Ralentizar(.3f); yield return Esperar(.1f); Exigir(j.GetComponent<MovimientoJugador>().MultiplicadorVelocidad<1,"Agua ralentiza temporalmente"); yield return Esperar(.4f); Exigir(j.GetComponent<MovimientoJugador>().MultiplicadorVelocidad>1,"Ralentizacion expira");
        Mover(n,10); yield return Esperar(.15f); yield return Pulsar(Key.E); Exigir(n.Datos.pista,"Pista de la placa");
        var enemigo=UnityEngine.Object.FindObjectsByType<EnemigoBase>(FindObjectsSortMode.None).OrderBy(e=>e.transform.position.x).First();
        Mover(n,enemigo.transform.position.x-.9f); Teclas(Key.D); yield return Esperar(.03f); Teclas(); float salud=enemigo.GetComponent<SistemaVida>().Actual;
        Teclas(Key.J); yield return Esperar(.12f);
        Exigir(j.GetComponent<PresentacionJugador>().animador.GetCurrentAnimatorStateInfo(0).IsName("Attack"),"Animator Attack responde a garra real");
        Teclas(); yield return Esperar(.12f); Exigir(enemigo==null || enemigo.GetComponent<SistemaVida>().Actual<salud,"J aplica dano de garra");
        Mover(n,5); yield return Esperar(.3f); comida=recursos.Comida;
        Teclas(Key.K); yield return Esperar(1.3f); Exigir(j.GetComponent<CombateJugador>().Carga01>.95f,"K carga bola"); Teclas(); yield return Esperar(.1f);
        Exigir(recursos.Comida<comida-5 && UnityEngine.Object.FindObjectsByType<Proyectil>(FindObjectsSortMode.None).Any(p=>p.name=="Bola de pelo"),"Soltar K dispara y consume comida");
        recursos.Recoger(TipoRecogible.Ropa); int ropa=recursos.Arrojables; yield return Esperar(.4f); yield return Pulsar(Key.L); Exigir(recursos.Arrojables==ropa-1,"L consume arrojable");
        Mover(n,30); yield return Esperar(.3f); Exigir(n.Datos.checkpoint==1,"Checkpoint por trigger");
        var guardado=GestorGuardado.Leer(); Exigir(guardado!=null && guardado.checkpoint==1 && guardado.pista && guardado.monedas==1,"Guardado local de checkpoint");
        vida.RecibirDano(10); Exigir(!vida.RecibirDano(10),"Inmunidad evita doble dano inmediato"); yield return Esperar(.9f); vida.RecibirDano(1000); Exigir(n.interfaz.Estado=="Derrota","Vida cero muestra derrota");
        n.Reintentar(); yield return Esperar(.5f); n=Nivel(); Cerrar(n); Exigir(n.Datos.checkpoint==1 && n.jugador.GetComponent<SistemaVida>().Actual==100,"Reintento restaura checkpoint y vida");
        Exigir(n.jugador.GetComponent<RecursosJugador>().MultiplicadorDano>1,"Power-up activo sobrevive checkpoint");
        Exigir(!UnityEngine.Object.FindObjectsByType<Recogible>(FindObjectsSortMode.None).Any(r=>r.identificador=="N1_Moneda_0"),"Moneda guardada no reaparece");
        SesionPartida.Menu(); yield return Esperar(.3f); SesionPartida.Continuar(); yield return Esperar(.5f); n=Nivel(); Cerrar(n); Exigir(n.numero==1 && n.Datos.checkpoint==1,"Continuar carga disco desde menu");
        for(int numero=1;numero<=3;numero++) {
            n=Nivel(); Cerrar(n); Exigir(n.numero==numero,"Escena nivel "+numero);
            Captura("nivel"+numero); yield return Esperar(.3f);
            n.Salir(); Exigir(n.interfaz.Estado=="Juego","Salida bloqueada con jefe vivo N"+numero);
            Mover(n,64); yield return Esperar(2.7f); Exigir(!string.IsNullOrEmpty(n.jefe.Aviso),"Jefe activo con ataque telegrafiado N"+numero);
            if(numero==2) { n.jefe.GetComponent<SistemaVida>().RecibirDano(130); yield return Esperar(.2f); Exigir(n.jefe.SegundaFase,"Fidel segunda fase"); }
            n.jefe.GetComponent<SistemaVida>().RecibirDano(10000); yield return Esperar(.2f);
            Exigir(n.Datos.jefeDerrotado && !n.barrera.activeSelf,"Derrota jefe abre barrera N"+numero);
            Exigir(GestorGuardado.Leer().jefeDerrotado,"Guarda derrota jefe N"+numero);
            Mover(n,81); yield return Esperar(.2f); yield return Pulsar(Key.E);
            Exigir(n.interfaz.Estado=="Dialogo","E activa salida narrativa N"+numero);
            n.interfaz.CerrarDialogo(); yield return Esperar(.5f);
        }
        n=Nivel(); Exigir(n.interfaz.Estado=="Creditos" && n.Datos.rescatada,"Rescate conduce a creditos");
        Exigir(GestorGuardado.Leer().rescatada,"Rescate persistido"); Captura("creditos"); yield return Esperar(.3f);
        SesionPartida.Menu(); yield return Esperar(.3f); SesionPartida.Continuar(); yield return Esperar(.4f);
        Exigir(Nivel().interfaz.Estado=="Creditos","Continuar partida terminada");
        var copia=GestorGuardado.Leer(); Exigir(GestorGuardado.Guardar(copia,Ruta+".roundtrip"),"Escritura JSON aislada");
        File.WriteAllText(Ruta+".corrupto","no json"); Exigir(GestorGuardado.Leer(Ruta+".corrupto")==null,"Archivo corrupto rechazado sin excepcion");
        copia.version=99; Exigir(!GestorGuardado.Guardar(copia,Ruta+".invalido"),"Version incompatible rechazada");
    }
}
