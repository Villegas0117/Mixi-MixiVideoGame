using System;
using System.IO;
using UnityEngine;
[Serializable]
public sealed class DatosPartida
{
    public int version=1, nivel=1, desbloqueado=1, checkpoint;
    public int dificultad=1; // 0 Facil, 1 Normal, 2 Dificil
    public bool pista, jefeDerrotado, rescatada;
    public float vida=100, comida=100;
    public float velocidadRestante, fuerzaRestante;
    public int monedas, pollos, arrojables, velocidades, fuerzas;
    public string[] recogidos = Array.Empty<string>();
}
public static class GestorGuardado
{
    #if UNITY_EDITOR
    public static string RutaPruebas;
#endif
    public static string Ruta {
        get {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(RutaPruebas)) return RutaPruebas;
#endif
            return Path.Combine(Application.persistentDataPath, "mixi-v01.json");
        }
    }
    public static string Error { get; private set; }
    public static bool Valido(DatosPartida d) => d != null && d.version == 1 && d.nivel >= 1 && d.nivel <= 3 && d.desbloqueado >= d.nivel && d.desbloqueado <= 3 && d.checkpoint >= 0 && d.checkpoint <= 2 && d.dificultad >= 0 && d.dificultad <= 2 && d.vida > 0 && d.vida <= 100 && !float.IsNaN(d.vida) && d.comida >= 0 && d.comida <= 100 && !float.IsNaN(d.comida) && d.monedas >= 0 && d.pollos >= 0 && d.arrojables >= 0 && d.velocidades >= 0 && d.fuerzas >= 0 && d.recogidos != null && d.velocidadRestante >= 0 && d.velocidadRestante <= 20 && d.fuerzaRestante >= 0 && d.fuerzaRestante <= 20;
    public static DatosPartida Leer(string ruta = null)
    {
        Error = null; ruta = ruta ?? Ruta;
        try { if (!File.Exists(ruta)) return null; var d=JsonUtility.FromJson<DatosPartida>(File.ReadAllText(ruta)); if (Valido(d)) return d; Error="Guardado incompatible o invalido."; }
        catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is ArgumentException) { Error="No se pudo leer la partida: " + e.Message; }
        return null;
    }
    public static bool Guardar(DatosPartida d, string ruta = null)
    {
        Error=null; ruta=ruta ?? Ruta;
        if (!Valido(d)) { Error="Se rechazo un estado de guardado invalido."; return false; }
        try {
            Directory.CreateDirectory(Path.GetDirectoryName(ruta));
            string temporal=ruta+".tmp";
            using (var f=new FileStream(temporal, FileMode.Create, FileAccess.Write))
            using (var w=new StreamWriter(f)) { w.Write(JsonUtility.ToJson(d, true)); w.Flush(); f.Flush(true); }
            if (File.Exists(ruta)) File.Replace(temporal,ruta,ruta+".bak"); else File.Move(temporal,ruta);
            return true;
        } catch (Exception e) when (e is IOException || e is UnauthorizedAccessException) { Error="No se pudo guardar: " + e.Message; return false; }
    }
}
