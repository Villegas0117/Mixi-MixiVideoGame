using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ejecucion aislada: Unity -batchmode -executeMethod ValidacionMusicaEditor.Ejecutar
public static class ValidacionMusicaEditor
{
    private static AudioManager gestor;
    private static AudioSource[] fuentes;
    private static int paso;
    private static double espera;
    private static int errores;
    private static AudioClip esperado;
    private static ControladorJefe jefe;
    private static int inicios;
    private static AudioSource fuentePausada;
    private static int muestraPausada;

    public static void Ejecutar()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorApplication.update += Actualizar;
        EditorApplication.isPlaying = true;
    }

    private static void Comprobar(bool condicion, string descripcion)
    {
        Debug.Log((condicion ? "MUSICA OK: " : "MUSICA ERROR: ") + descripcion);
        if (!condicion) errores++;
    }

    private static void Actualizar()
    {
        if (!EditorApplication.isPlaying || EditorApplication.timeSinceStartup < espera) return;
        try
        {
            if (paso == 0)
            {
                gestor = AudioManager.ObtenerOCrear();
                gestor.tiempoCrossfade = .15f;
                new GameObject("AudioListener prueba").AddComponent<AudioListener>();
                fuentes = gestor.GetComponents<AudioSource>();
                var clips = new[] { gestor.musicaNivel1, gestor.jefeNivel1, gestor.musicaNivel2,
                    gestor.jefeNivel2, gestor.musicaNivel3, gestor.jefeNivel3 };
                Comprobar(fuentes.Length == 2, "dos fuentes, sin duplicados");
                for (int i = 0; i < clips.Length; i++)
                {
                    Comprobar(clips[i] != null && clips[i].length >= 60f, "pista " + i + " cargada y duracion >= 60 s");
                    for (int j = 0; j < i; j++) Comprobar(clips[i] != clips[j], "pistas distintas " + i + "/" + j);
                }
                Comprobar(AudioManager.ObtenerOCrear() == gestor, "gestor unico");
                paso = 1;
            }
            else if (paso >= 1 && paso <= 12)
            {
                int nivel = (paso - 1) / 4 + 1;
                int fase = (paso - 1) % 4;
                if (fase == 0)
                {
                    gestor.ReproducirExploracion(nivel);
                    esperado = nivel == 1 ? gestor.musicaNivel1 : nivel == 2 ? gestor.musicaNivel2 : gestor.musicaNivel3;
                }
                else if (fase == 1) VerificarFuente("exploracion nivel " + nivel);
                else if (fase == 2)
                {
                    gestor.ReproducirJefe(nivel);
                    esperado = nivel == 1 ? gestor.jefeNivel1 : nivel == 2 ? gestor.jefeNivel2 : gestor.jefeNivel3;
                }
                else VerificarFuente("jefe nivel " + nivel);
                paso++;
                espera = EditorApplication.timeSinceStartup + .4;
            }
            else if (paso == 13)
            {
                gestor.ReproducirVictoria(); // Sin pista de victoria vuelve a exploracion.
                esperado = gestor.musicaNivel3; paso++; espera = EditorApplication.timeSinceStartup + .4;
            }
            else if (paso == 14)
            {
                VerificarFuente("retorno tras jefe final");
                fuentePausada = fuentes[0].isPlaying ? fuentes[0] : fuentes[1];
                int antes = fuentePausada.timeSamples;
                gestor.ReproducirExploracion(3);
                Comprobar(fuentePausada.timeSamples >= antes, "solicitud repetida no reinicia la pista");
                Time.timeScale = 0f; paso++; espera = EditorApplication.timeSinceStartup + .2;
            }
            else if (paso == 15)
            {
                Comprobar(!fuentes[0].isPlaying && !fuentes[1].isPlaying, "pausa de ambas fuentes");
                muestraPausada = fuentePausada.timeSamples;
                Time.timeScale = 1f; paso++; espera = EditorApplication.timeSinceStartup + .2;
            }
            else if (paso == 16)
            {
                VerificarFuente("continuacion tras pausa");
                Comprobar(fuentePausada.timeSamples > muestraPausada, "reproduccion avanza desde la posicion pausada");
                // Crear un jefe real aislado para verificar su evento de activacion.
                var jugador = new GameObject("Jugador prueba"); jugador.AddComponent<SistemaVida>();
                var rival = new GameObject("Jefe prueba");
                jefe = rival.AddComponent<ControladorJefe>();
                jefe.limiteIzquierdo = -10; jefe.limiteDerecho = 10;
                jefe.objetivo = jugador.transform;
                jefe.CombateIniciado += () => inicios++;
                paso++; espera = EditorApplication.timeSinceStartup + .5;
            }
            else if (paso == 17)
            {
                Comprobar(inicios == 1, "evento real de jefe se emite una sola vez");
                UnityEngine.Object.Destroy(jefe.gameObject);
                GestorGuardado.RutaPruebas = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(), "opencode", "claw-musica-test.json");
                paso++;
            }
            else if (paso >= 18 && paso < 33)
            {
                int nivel = (paso - 18) / 5 + 1;
                int fase = (paso - 18) % 5;
                if (fase == 0)
                {
                    SesionPartida.Datos = new DatosPartida { nivel = nivel, desbloqueado = nivel };
                    SceneManager.LoadScene(SesionPartida.Escenas[nivel]);
                }
                else if (fase == 1)
                {
                    var nivelReal = UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
                    if (nivelReal.interfaz.Estado == "Dialogo") nivelReal.interfaz.CerrarDialogo();
                    esperado = nivel == 1 ? gestor.musicaNivel1 : nivel == 2 ? gestor.musicaNivel2 : gestor.musicaNivel3;
                }
                else if (fase == 2)
                {
                    VerificarFuente("escena real exploracion " + nivel);
                    var nivelReal = UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
                    var posicion = nivelReal.jefe.transform.position + Vector3.left * 2f;
                    nivelReal.jugador.GetComponent<Rigidbody2D>().position = posicion;
                    esperado = nivel == 1 ? gestor.jefeNivel1 : nivel == 2 ? gestor.jefeNivel2 : gestor.jefeNivel3;
                }
                else if (fase == 3)
                {
                    VerificarFuente("escena real jefe " + nivel);
                    var nivelReal = UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
                    nivelReal.jefe.GetComponent<SistemaVida>().RecibirDano(10000f);
                    esperado = nivel == 1 ? gestor.musicaNivel1 : nivel == 2 ? gestor.musicaNivel2 : gestor.musicaNivel3;
                }
                else VerificarFuente("escena real tras derrota jefe " + nivel);
                paso++; espera = EditorApplication.timeSinceStartup + .5;
            }
            else if (paso == 33)
            {
                VerificarFuente("escena real tras derrota del jefe final");
                SceneManager.LoadScene("MenuPrincipal");
                paso++; espera = EditorApplication.timeSinceStartup + .5;
            }
            else
            {
                Comprobar(!fuentes[0].isPlaying && !fuentes[1].isPlaying, "menu detiene musica");
                Debug.Log("MUSICA RESULTADO: " + errores + " errores");
                EditorApplication.update -= Actualizar;
                EditorApplication.Exit(errores == 0 ? 0 : 1);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            EditorApplication.update -= Actualizar;
            EditorApplication.Exit(2);
        }
    }

    private static void VerificarFuente(string nombre)
    {
        int activas = 0;
        foreach (var fuente in fuentes)
            if (fuente.isPlaying && fuente.clip == esperado && fuente.loop) activas++;
        Comprobar(activas == 1, nombre + ": una pista correcta reproduciendose en bucle");
        int total = 0;
        foreach (var fuente in fuentes) if (fuente.isPlaying) total++;
        Comprobar(total == 1, nombre + ": sin duplicados al terminar transicion");
    }
}
