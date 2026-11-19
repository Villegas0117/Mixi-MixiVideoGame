using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class EntregaV01
{
    private static readonly string[] Escenas={"MenuPrincipal","Nivel_01_Barrio","Nivel_02_Iglesia","Nivel_03_Almacen"};
    public static void PrepararYFisica()
    {
        const string ruta="Assets/Prefabs/Personajes/ChipotleJugable.prefab";
        var raiz=PrefabUtility.LoadPrefabContents(ruta);
        try {
            var audio=raiz.GetComponent<AudioJugador>();
            if(audio==null) audio=raiz.AddComponent<AudioJugador>();
            audio.salto=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Salto.wav");
            audio.garra=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Garra.wav");
            audio.pelo=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Pelo.wav");
            audio.recoger=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Recoger.wav");
            audio.dano=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Dano.wav");
            audio.mejora=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Efectos/Mejora.wav");
            raiz.GetComponent<AudioSource>().playOnAwake=false;
            PrefabUtility.SaveAsPrefabAsset(raiz,ruta);
        } finally { PrefabUtility.UnloadPrefabContents(raiz); }
        Validar(); ValidacionFisicaJugador.Ejecutar();
    }
    public static void Validar()
    {
        int objetos=0;
        foreach(var nombre in Escenas) {
            var escena=EditorSceneManager.OpenScene("Assets/Escenas/"+nombre+".unity");
            foreach(var raiz in escena.GetRootGameObjects()) foreach(var t in raiz.GetComponentsInChildren<Transform>(true)) {
                objetos++;
                if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0) throw new Exception("Script faltante: "+t.name);
                foreach(var c in t.GetComponents<Component>()) {
                    if(c==null) throw new Exception("Componente nulo");
                    var so=new SerializedObject(c); var prop=so.GetIterator();
                    while(prop.NextVisible(true)) if(prop.propertyType==SerializedPropertyType.ObjectReference && prop.objectReferenceValue==null && prop.objectReferenceInstanceIDValue!=0) throw new Exception("Referencia rota: "+t.name+"/"+prop.propertyPath);
                }
            }
            var nivel=UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
            if(nombre!="MenuPrincipal") {
                if(nivel==null || nivel.jugador==null || nivel.interfaz==null || nivel.interfaz.nivel!=nivel || nivel.jefe==null || nivel.barrera==null || nivel.salida==null || nivel.puntosControl.Length!=3) throw new Exception("Nivel incompleto "+nombre);
                if(nivel.jugador.GetComponent<CombateJugador>().proyectilPrefab==null) throw new Exception("Sin proyectil");
                foreach(var r in UnityEngine.Object.FindObjectsByType<Recogible>(FindObjectsSortMode.None)) if(r.nivel!=nivel || string.IsNullOrEmpty(r.identificador)) throw new Exception("Objeto sin contexto");
                var ids=UnityEngine.Object.FindObjectsByType<Recogible>(FindObjectsSortMode.None).Select(r=>r.identificador).ToArray();
                if(ids.Distinct().Count()!=ids.Length) throw new Exception("Ids duplicados");
            }
        }
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Personajes/ChipotleJugable.prefab");
        if(PrefabUtility.GetPrefabAssetType(prefab)!=PrefabAssetType.Variant) throw new Exception("Jugador no es variante");
        Debug.Log("MIXI_ESTATICO_OK: 4 escenas; "+objetos+" objetos; referencias e ids comprobados");
    }
    public static void CapturarEscenas()
    {
        for(int n=1;n<=3;n++) {
            EditorSceneManager.OpenScene("Assets/Escenas/"+Escenas[n]+".unity");
            var camara=Camera.main;
            camara.transform.position=new Vector3(n==1 ? 7 : 68,2.65f,-10);
            var rt=new RenderTexture(1280,720,24);
            camara.targetTexture=rt;
            camara.Render();
            RenderTexture.active=rt;
            var imagen=new Texture2D(1280,720,TextureFormat.RGB24,false);
            imagen.ReadPixels(new Rect(0,0,1280,720),0,0); imagen.Apply();
            File.WriteAllBytes("/tmp/mixi-v01-escena"+n+".png",imagen.EncodeToPNG());
            RenderTexture.active=null; camara.targetTexture=null;
            UnityEngine.Object.DestroyImmediate(imagen); UnityEngine.Object.DestroyImmediate(rt);
        }
        Debug.Log("MIXI_CAPTURAS_OK: render de escenas, no HUD IMGUI");
    }
    public static void ConstruirLinux() { Validar(); Construir(BuildTarget.StandaloneLinux64,"Builds/Linux/Mixi-Mixi.x86_64"); }
    public static void ConstruirWindows() { Validar(); Construir(BuildTarget.StandaloneWindows64,"Builds/Windows/Mixi-Mixi.exe"); }
    private static void Construir(BuildTarget target,string destino)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destino));
        var resultado=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes=Escenas.Select(n=>"Assets/Escenas/"+n+".unity").ToArray(),locationPathName=destino,target=target,options=BuildOptions.None
        });
        if(resultado.summary.result!=BuildResult.Succeeded) throw new Exception("Build fallo: "+resultado.summary.result);
        Debug.Log("MIXI_BUILD_OK: "+target+" bytes="+resultado.summary.totalSize+" errores="+resultado.summary.totalErrors+" warnings="+resultado.summary.totalWarnings);
    }
}
