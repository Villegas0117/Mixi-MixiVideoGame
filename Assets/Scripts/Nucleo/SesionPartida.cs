using UnityEngine;
using UnityEngine.SceneManagement;
public static class SesionPartida
{
    public static DatosPartida Datos;
    public static bool MostrarInicio;
    public static readonly string[] Escenas = { "MenuPrincipal", "Nivel_01_Barrio", "Nivel_02_Iglesia", "Nivel_03_Almacen" };
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Limpiar() { Datos=null; MostrarInicio=false; Time.timeScale=1; }
    public static void Nueva() { Nueva((int)Dificultad.Actual); }
    public static void Nueva(int dificultad) { Dificultad.Actual=(NivelDificultad)Mathf.Clamp(dificultad,0,2); Datos=new DatosPartida(); Datos.dificultad=(int)Dificultad.Actual; MostrarInicio=true; Cargar(1); }
    public static void Continuar() { Datos=GestorGuardado.Leer(); if (Datos!=null) { Dificultad.Actual=(NivelDificultad)Mathf.Clamp(Datos.dificultad,0,2); Cargar(Datos.nivel); } }
    public static void Cargar(int nivel) { Time.timeScale=1; SceneManager.LoadScene(Escenas[nivel]); }
    public static void Menu() { Time.timeScale=1; SceneManager.LoadScene(Escenas[0]); }
}
