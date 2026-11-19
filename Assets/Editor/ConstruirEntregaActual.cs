using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Compila en un directorio nuevo indicado por CLAW_BUILD_DIR.
public static class ConstruirEntregaActual
{
    public static void Linux() => Construir(BuildTarget.StandaloneLinux64, "Linux", "Claw.x86_64");
    public static void Windows() => Construir(BuildTarget.StandaloneWindows64, "Windows", "Claw.exe");

    private static void Construir(BuildTarget plataforma, string carpeta, string ejecutable)
    {
        string raiz = Environment.GetEnvironmentVariable("CLAW_BUILD_DIR");
        if (string.IsNullOrEmpty(raiz) || !Path.IsPathRooted(raiz))
            throw new Exception("CLAW_BUILD_DIR debe ser una ruta absoluta.");
        string destino = Path.Combine(raiz, carpeta);
        if (Directory.Exists(destino) || File.Exists(destino))
            throw new Exception("El destino ya existe, no se sobrescribe: " + destino);
        ValidacionVisualEditor.Validar();
        Directory.CreateDirectory(destino);
        string[] escenas = { "MenuPrincipal", "Nivel_01_Barrio", "Nivel_02_Iglesia", "Nivel_03_Almacen" };
        var resultado = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = escenas.Select(n => "Assets/Escenas/" + n + ".unity").ToArray(),
            locationPathName = Path.Combine(destino, ejecutable),
            target = plataforma,
            options = BuildOptions.None
        });
        Debug.Log("CLAW_BUILD: " + plataforma + " resultado=" + resultado.summary.result +
            " errores=" + resultado.summary.totalErrors + " avisos=" + resultado.summary.totalWarnings +
            " bytes=" + resultado.summary.totalSize);
        if (resultado.summary.result != BuildResult.Succeeded)
            throw new Exception("No se pudo generar " + plataforma);
    }
}
