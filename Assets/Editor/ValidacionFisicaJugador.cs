using System;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine;

// Prueba por simulacion explicita de Physics2D en batchmode; NO es Play Mode.
public static class ValidacionFisicaJugador
{
    public static void Ejecutar()
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Ejecutar solo en batchmode.");
        const string ruta = "Assets/Escenas/PruebasJugador.unity";
        var modoAnterior = Physics2D.simulationMode;
        try
        {
            var escena = EditorSceneManager.OpenScene(ruta);
            GameObject jugador = null;
            foreach (var raiz in escena.GetRootGameObjects())
                if (raiz.GetComponent<MovimientoJugador>() != null) jugador = raiz;
            Exigir(jugador != null, "Jugador en escena");
            var movimiento = jugador.GetComponent<MovimientoJugador>();
            var entrada = jugador.GetComponent<EntradaJugador>();
            var cuerpo = jugador.GetComponent<Rigidbody2D>();
            var pendiente = typeof(EntradaJugador).GetField("saltoPendiente", BindingFlags.NonPublic | BindingFlags.Instance);
            Invocar(movimiento, "Awake");
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.SyncTransforms();
            for (int i = 0; i < 120; i++) Paso(movimiento);
            Exigir(movimiento.EnSuelo && Mathf.Abs(cuerpo.position.y - 0.1f) < 0.06f, "Caida, colision y suelo");
            float alturaInicial = cuerpo.position.y;
            pendiente.SetValue(entrada, true); // Intencion inyectada; no prueba dispositivos/Input System.
            Paso(movimiento);
            Exigir(cuerpo.linearVelocity.y > 8f && !movimiento.EnSuelo, "Salto desde suelo");
            float velocidadAnterior = cuerpo.linearVelocity.y;
            pendiente.SetValue(entrada, true);
            Paso(movimiento);
            Exigir(cuerpo.linearVelocity.y < velocidadAnterior, "Sin doble salto; gravedad activa");
            float alturaMaxima = cuerpo.position.y;
            for (int i = 0; i < 120; i++)
            {
                Paso(movimiento);
                alturaMaxima = Mathf.Max(alturaMaxima, cuerpo.position.y);
            }
            Exigir(alturaMaxima > alturaInicial + 1f && movimiento.EnSuelo, "Arco y aterrizaje");
            Paso(movimiento);
            Exigir(movimiento.EnSuelo && Mathf.Abs(cuerpo.linearVelocity.y) < 0.1f, "Sin salto retenido al aterrizar");
            cuerpo.position = new Vector2(-11f, 0.1f);
            cuerpo.linearVelocity = Vector2.zero;
            Physics2D.SyncTransforms();
            for (int i = 0; i < 5; i++) Paso(movimiento);
            pendiente.SetValue(entrada, true);
            float alturaTecho = cuerpo.position.y;
            for (int i = 0; i < 45; i++)
            {
                Paso(movimiento);
                alturaTecho = Mathf.Max(alturaTecho, cuerpo.position.y);
            }
            Exigir(alturaTecho < 1.42f, "Colision con techo");
            cuerpo.position = new Vector2(17.1f, 4f);
            cuerpo.linearVelocity = new Vector2(7f, 0f);
            Physics2D.SyncTransforms();
            Physics2D.Simulate(0.02f);
            Invocar(movimiento, "FixedUpdate");
            Exigir(!movimiento.EnSuelo && cuerpo.position.x < 17.15f, "Pared no cuenta como suelo");
            Debug.Log("MIXI_FISICA_OK: 7 comprobaciones; sin Play Mode ni dispositivos reales.");
        }
        finally
        {
            Physics2D.simulationMode = modoAnterior;
            // Descartar posiciones de prueba; nunca guardar el estado simulado.
            EditorSceneManager.OpenScene(ruta);
        }
    }

    private static void Paso(MovimientoJugador movimiento)
    {
        Invocar(movimiento, "FixedUpdate");
        Physics2D.Simulate(0.02f);
    }

    private static void Invocar(MovimientoJugador movimiento, string metodo)
    {
        typeof(MovimientoJugador).GetMethod(metodo, BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(movimiento, null);
    }

    private static void Exigir(bool condicion, string nombre)
    {
        if (!condicion) throw new InvalidOperationException("FALLO: " + nombre);
        Debug.Log("MIXI_PRUEBA_OK: " + nombre);
    }
}
