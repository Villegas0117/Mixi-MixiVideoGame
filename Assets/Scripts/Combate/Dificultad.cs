using UnityEngine;

public enum NivelDificultad { Facil = 0, Normal = 1, Dificil = 2 }

// Escala dano, velocidad y densidad de enemigos segun la eleccion del jugador.
// Se aplica una vez al cargar cada nivel, antes de que los enemigos actuen.
public static class Dificultad
{
    public static NivelDificultad Actual = NivelDificultad.Normal;

    public static float MultiplicadorDano =>
        Actual == NivelDificultad.Facil ? .7f : Actual == NivelDificultad.Dificil ? 1.5f : 1f;
    public static float MultiplicadorVelocidad =>
        Actual == NivelDificultad.Facil ? .85f : Actual == NivelDificultad.Dificil ? 1.2f : 1f;
    public static string Nombre =>
        Actual == NivelDificultad.Facil ? "Fácil" : Actual == NivelDificultad.Dificil ? "Difícil" : "Normal";

    public static void Aplicar()
    {
        var originales = Object.FindObjectsByType<EnemigoBase>(FindObjectsSortMode.None);
        Time.timeScale = 1f;
        for (int i = 0; i < originales.Length; i++)
        {
            var e = originales[i];
            if (e == null) continue;
            e.dano *= MultiplicadorDano;
            e.velocidad *= MultiplicadorVelocidad;
            // Densidad: Normal duplica a 1 de cada 2 enemigos, Dificil duplica a todos.
            bool duplicar = Actual == NivelDificultad.Dificil ||
                (Actual == NivelDificultad.Normal && i % 2 == 0);
            if (!duplicar) continue;
            var copia = Object.Instantiate(e.gameObject, e.transform.parent);
            copia.transform.position = e.transform.position + Vector3.right * 1.8f;
            copia.SetActive(true);
            var ec = copia.GetComponent<EnemigoBase>();
            if (ec != null) { ec.dano = e.dano; ec.velocidad = e.velocidad; ec.objetivo = e.objetivo; }
        }
        foreach (var jefe in Object.FindObjectsByType<ControladorJefe>(FindObjectsSortMode.None))
            if (jefe != null) jefe.multiplicadorDificultad = MultiplicadorDano;
    }
}
