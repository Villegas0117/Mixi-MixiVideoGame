using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
public static class ValidacionVisualEditor
{
    private static readonly string[] Escenas={"MenuPrincipal","Nivel_01_Barrio","Nivel_02_Iglesia","Nivel_03_Almacen"};
    public static void Validar()
    {
        EntregaV01.Validar();
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Personajes/ChipotleJugable.prefab");
        var rb=prefab.GetComponent<Rigidbody2D>();var col=prefab.GetComponent<CapsuleCollider2D>();
        if(rb.gravityScale!=3 || col.size!=new Vector2(.8f,1.2f))throw new Exception("Fisica original cambiada");
        var m=new SerializedObject(prefab.GetComponent<MovimientoJugador>());
        foreach(var par in new[]{("velocidadCaminar",4f),("velocidadCorrer",7f),("velocidadSalto",10f)})if(m.FindProperty(par.Item1).floatValue!=par.Item2)throw new Exception("Parametro cambiado: "+par.Item1);
        var ac=prefab.GetComponent<PresentacionJugador>().animador.runtimeAnimatorController as AnimatorController;
        foreach(string estado in new[]{"Idle","Walk","Run","Jump","Fall","Attack","Hurt","Hairball","Throw"})if(!ac.layers[0].stateMachine.states.Any(s=>s.state.name==estado))throw new Exception("Estado ausente: "+estado);
        int cuadros=0;
        foreach(var clip in ac.animationClips) {
            if(AnimationUtility.GetCurveBindings(clip).Any(b=>b.propertyName.Contains("Scale") || b.propertyName.Contains("Position"))) throw new Exception("Animacion de deformacion/desplazamiento no permitida");
            foreach(var b in AnimationUtility.GetObjectReferenceCurveBindings(clip))foreach(var key in AnimationUtility.GetObjectReferenceCurve(clip,b)) {if(!(key.value is Sprite))throw new Exception("Frame nulo");cuadros++;}
        }
        for(int n=1;n<=3;n++) {
            EditorSceneManager.OpenScene("Assets/Escenas/"+Escenas[n]+".unity");
            var root=GameObject.Find("PresentacionVisual");if(root==null)throw new Exception("Escenario no actualizado");
            foreach(string capa in new[]{"Background","Midground","Decoracion","Foreground"})if(root.transform.Find(capa)==null)throw new Exception("Capa ausente");
            if(root.GetComponentsInChildren<Collider2D>(true).Length>0)throw new Exception("Arte introdujo colision");
            foreach(var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) {
                if(!sr.enabled)continue;if(sr.sprite==null)throw new Exception("Sprite nulo: "+sr.name);
                var path=AssetDatabase.GetAssetPath(sr.sprite);var ti=AssetImporter.GetAtPath(path) as TextureImporter;
                if(ti!=null && path.StartsWith("Assets/Arte/") && !path.Contains("Provisional") && (ti.filterMode!=FilterMode.Point || ti.textureCompression!=TextureImporterCompression.Uncompressed || ti.mipmapEnabled))throw new Exception("Importacion inconsistente: "+path);
            }
        }
        Debug.Log("MIXI_VISUAL_VALIDACION_OK: 9 estados Chipotle, "+cuadros+" claves sprite, sin curvas de deformacion, fisica original, capas sin colliders, importacion Point");
    }
    public static void Capturar()
    {
        string destino="docs/evidencia/visual";Directory.CreateDirectory(destino);
        for(int n=1;n<=3;n++) {
            EditorSceneManager.OpenScene("Assets/Escenas/"+Escenas[n]+".unity");var nivel=UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
            foreach(float x in new[]{5f,24f,42f,72f}) {
                nivel.jugador.transform.position=new Vector3(x-3,.65f,0);
                Camera.main.transform.position=new Vector3(x,2.65f,-10);
                var rt=new RenderTexture(1280,720,24);Camera.main.targetTexture=rt;Camera.main.Render();Camera.main.Render();Camera.main.Render();RenderTexture.active=rt;
                var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();
                File.WriteAllBytes(destino+"/nivel"+n+"_x"+x+".png",tex.EncodeToPNG());
                RenderTexture.active=null;Camera.main.targetTexture=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);
            }
        }
        EditorSceneManager.OpenScene("Assets/Escenas/MenuPrincipal.unity");Debug.Log("MIXI_VISUAL_CAPTURAS_OK: 12 vistas sin guardar posiciones simuladas");
    }
    public static void ConstruirLinux()=>Construir(BuildTarget.StandaloneLinux64,"Builds/Visual/Linux/Mixi-Mixi-Visual.x86_64");
    public static void ConstruirWindows()=>Construir(BuildTarget.StandaloneWindows64,"Builds/Visual/Windows/Mixi-Mixi-Visual.exe");
    private static void Construir(BuildTarget target,string path)
    {
        Validar();Directory.CreateDirectory(Path.GetDirectoryName(path));
        var r=BuildPipeline.BuildPlayer(new BuildPlayerOptions{ scenes=Escenas.Select(s=>"Assets/Escenas/"+s+".unity").ToArray(),target=target,locationPathName=path });
        if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build visual fallido");Debug.Log("MIXI_VISUAL_BUILD_OK: "+target+" errors="+r.summary.totalErrors+" warnings="+r.summary.totalWarnings);
    }
}
