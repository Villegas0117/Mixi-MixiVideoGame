using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Herramienta explicita: modifica presentacion, conserva fisica, referencias y layout jugable.
public static class PasadaVisualEditor
{
    private const string Arte="Assets/Arte/";
    private const string Anim="Assets/Animaciones/Visual/";
    private static readonly string[] Niveles={"Nivel_01_Barrio","Nivel_02_Iglesia","Nivel_03_Almacen"};
    [MenuItem("Mixi-Mixi/Integrar pasada visual")]
    public static void Integrar()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Salir de Play antes de integrar.");
        if(!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Directory.CreateDirectory(Anim); AssetDatabase.Refresh();
        Importar(); Jugador(); Rivales(); Proyectiles(); HUD();
        for(int i=1;i<=3;i++) Nivel(i);
        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene("Assets/Escenas/MenuPrincipal.unity");
        Debug.Log("MIXI_VISUAL_INTEGRADA: presentacion de tres niveles; gameplay conservado");
    }
    public static void IntegrarYValidar() { Integrar(); ValidacionVisualEditor.Validar(); }
    public static void ActualizarHUD() { AssetDatabase.Refresh(); Importar(); HUD(); AssetDatabase.SaveAssets(); }
    public static void FinalizarYConstruirLinux()
    {
        AssetDatabase.Refresh();Importar();HUD();
        Prefab("Assets/Prefabs/Objetos/Recogible.prefab",g=>{g.GetComponentInChildren<SpriteRenderer>().sprite=S("Objetos/Moneda/Moneda");});
        foreach(string nombre in Niveles) {
            var escena=EditorSceneManager.OpenScene("Assets/Escenas/"+nombre+".unity");
            foreach(var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None)) {
                if(sr.sprite!=null && AssetDatabase.GetAssetPath(sr.sprite).Contains("/Provisional/CuadradoV01")) {sr.sprite=S("Efectos/Pixel");EditorUtility.SetDirty(sr);}
            }
            EditorSceneManager.SaveScene(escena);
        }
        AssetDatabase.SaveAssets();ValidacionVisualEditor.ConstruirLinux();
    }
    public static void Importar()
    {
        foreach(string ruta in Directory.GetFiles(Arte,"*.png",SearchOption.AllDirectories)) {
            string p=ruta.Replace('\\','/');
            if(p.Contains("/Provisional/")) continue;
            var t=AssetImporter.GetAtPath(p) as TextureImporter; if(t==null) continue;
            t.textureType=TextureImporterType.Sprite; t.spriteImportMode=SpriteImportMode.Single;
            t.filterMode=FilterMode.Point; t.textureCompression=TextureImporterCompression.Uncompressed;
            t.mipmapEnabled=false; t.alphaIsTransparency=true; t.npotScale=TextureImporterNPOTScale.None;
            t.spritePixelsPerUnit=64; t.wrapMode=TextureWrapMode.Clamp;
            var st=new TextureImporterSettings(); t.ReadTextureSettings(st);
            st.spriteMeshType=SpriteMeshType.FullRect; st.spriteAlignment=(int)SpriteAlignment.Center; st.spritePivot=new Vector2(.5f,.5f);
            if(p.Contains("/Chipotle/Chipotle_")) { st.spriteAlignment=(int)SpriteAlignment.Custom; st.spritePivot=new Vector2(.5f,.125f); }
            t.SetTextureSettings(st); t.SaveAndReimport();
        }
    }
    private static Sprite S(string ruta) { var s=AssetDatabase.LoadAssetAtPath<Sprite>(Arte+ruta+".png"); if(s==null) throw new Exception("Sprite ausente: "+ruta); return s; }
    private static T Obtener<T>(GameObject g) where T:Component { var existente=g.GetComponent<T>(); return existente!=null ? existente : g.AddComponent<T>(); }
    private static void Prefab(string ruta,Action<GameObject> accion)
    {
        var g=PrefabUtility.LoadPrefabContents(ruta);
        try { accion(g); PrefabUtility.SaveAsPrefabAsset(g,ruta); } finally { PrefabUtility.UnloadPrefabContents(g); }
    }
    private static AnimationClip Clip(string nombre,Sprite[] frames,float fps,bool bucle=true)
    {
        string ruta=Anim+nombre+".anim";
        var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>(ruta);
        if(clip==null) { clip=new AnimationClip(); AssetDatabase.CreateAsset(clip,ruta); }
        clip.ClearCurves(); clip.frameRate=fps;
        var binding=new EditorCurveBinding { path="Sprite", type=typeof(SpriteRenderer), propertyName="m_Sprite" };
        var keys=new ObjectReferenceKeyframe[frames.Length+1];
        for(int i=0;i<frames.Length;i++) keys[i]=new ObjectReferenceKeyframe { time=i/fps,value=frames[i] };
        keys[frames.Length]=new ObjectReferenceKeyframe { time=frames.Length/fps,value=frames[frames.Length-1] };
        AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
        var settings=AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=bucle;AnimationUtility.SetAnimationClipSettings(clip,settings);
        EditorUtility.SetDirty(clip); return clip;
    }
    private static Sprite[] Frames(string estado) => Enumerable.Range(0,6).Select(i=>S("Personajes/Chipotle/Chipotle_"+estado+"_"+i.ToString("00"))).ToArray();
    private static AnimatorController Controlador(string nombre)
    {
        string ruta=Anim+nombre+".controller";
        var c=AssetDatabase.LoadAssetAtPath<AnimatorController>(ruta);
        if(c==null)c=AnimatorController.CreateAnimatorControllerAtPath(ruta);
        c.parameters=Array.Empty<AnimatorControllerParameter>();
        var sm=c.layers[0].stateMachine;
        foreach(var tr in sm.anyStateTransitions)sm.RemoveAnyStateTransition(tr);
        foreach(var s in sm.states)sm.RemoveState(s.state);
        return c;
    }
    private static AnimatorState Estado(AnimatorStateMachine sm,string n,AnimationClip clip) { var e=sm.AddState(n);e.motion=clip;return e; }
    private static void Cond(AnimatorStateTransition t,string n,AnimatorConditionMode m,float v=0) => t.AddCondition(m,v,n);
    private static AnimatorStateTransition Hacia(AnimatorStateMachine sm,AnimatorState e)
    {var t=sm.AddAnyStateTransition(e);t.hasExitTime=false;t.duration=0;t.canTransitionToSelf=false;return t;}
    private static void Jugador()
    {
        var c=Controlador("ChipotleVisual"); c.AddParameter("Velocidad",AnimatorControllerParameterType.Float);c.AddParameter("Vertical",AnimatorControllerParameterType.Float);
        foreach(string b in new[]{"EnSuelo","Atacando","Dano","Pelo","Lanzar"})c.AddParameter(b,AnimatorControllerParameterType.Bool);
        var sm=c.layers[0].stateMachine;
        var idle=Estado(sm,"Idle",Clip("Chipotle_Idle",Frames("Idle"),5));
        var walk=Estado(sm,"Walk",Clip("Chipotle_Walk",Frames("Walk"),9));
        var run=Estado(sm,"Run",Clip("Chipotle_Run",Frames("Run"),13));
        var jump=Estado(sm,"Jump",Clip("Chipotle_Jump",new[]{Frames("Jump")[1],Frames("Jump")[2],Frames("Jump")[3]},9,false));
        var fall=Estado(sm,"Fall",Clip("Chipotle_Fall",new[]{Frames("Jump")[4]},1));
        var attack=Estado(sm,"Attack",Clip("Chipotle_Attack",Frames("Attack"),27,false));
        var hurt=Estado(sm,"Hurt",Clip("Chipotle_Hurt",new[]{Frames("Jump")[5]},1));
        var hair=Estado(sm,"Hairball",Clip("Chipotle_Hairball",new[]{Frames("Attack")[0],Frames("Attack")[1]},5));
        var toss=Estado(sm,"Throw",Clip("Chipotle_Throw",new[]{Frames("Attack")[1],Frames("Attack")[3]},10,false));
        sm.defaultState=idle;
        var t=Hacia(sm,hurt);Cond(t,"Dano",AnimatorConditionMode.If);
        t=Hacia(sm,attack);Cond(t,"Dano",AnimatorConditionMode.IfNot);Cond(t,"Atacando",AnimatorConditionMode.If);
        t=Hacia(sm,hair);Cond(t,"Dano",AnimatorConditionMode.IfNot);Cond(t,"Atacando",AnimatorConditionMode.IfNot);Cond(t,"Pelo",AnimatorConditionMode.If);
        t=Hacia(sm,toss);Cond(t,"Dano",AnimatorConditionMode.IfNot);Cond(t,"Atacando",AnimatorConditionMode.IfNot);Cond(t,"Pelo",AnimatorConditionMode.IfNot);Cond(t,"Lanzar",AnimatorConditionMode.If);
        foreach(var e in new[]{idle,walk,run,jump,fall}) {
            t=Hacia(sm,e); foreach(string b in new[]{"Dano","Atacando","Pelo","Lanzar"})Cond(t,b,AnimatorConditionMode.IfNot);
            Cond(t,"EnSuelo",e==jump||e==fall ? AnimatorConditionMode.IfNot : AnimatorConditionMode.If);
            if(e==idle)Cond(t,"Velocidad",AnimatorConditionMode.Less,.1f);
            if(e==walk) {Cond(t,"Velocidad",AnimatorConditionMode.Greater,.1f);Cond(t,"Velocidad",AnimatorConditionMode.Less,5.5f);}
            if(e==run)Cond(t,"Velocidad",AnimatorConditionMode.Greater,5.5f);
            if(e==jump)Cond(t,"Vertical",AnimatorConditionMode.Greater,0);
            if(e==fall)Cond(t,"Vertical",AnimatorConditionMode.Less,.01f);
        }
        Prefab("Assets/Prefabs/Personajes/ChipotleJugable.prefab",g=>{
            var p=g.GetComponent<PresentacionJugador>();p.animador.runtimeAnimatorController=c;
            var v=p.animador.transform.Find("Sprite").GetComponent<SpriteRenderer>();v.sprite=Frames("Idle")[0];v.transform.localScale=Vector3.one;v.transform.localPosition=new Vector3(0,-.6f,0);v.sortingOrder=20;
            var anterior=p.animador.transform.Find("GarraVisual");if(anterior!=null) UnityEngine.Object.DestroyImmediate(anterior.gameObject);
            var garra=new GameObject("GarraVisual").AddComponent<SpriteRenderer>();garra.transform.SetParent(p.animador.transform,false);garra.sprite=S("Efectos/Garra");garra.transform.localPosition=new Vector3(.9f,.15f,0);garra.sortingOrder=24;garra.enabled=false;p.efectoGarra=garra;
        });EditorUtility.SetDirty(c);
    }
    private static void Rival(string prefab,string nombre,Sprite reposo,Sprite ataque,float alto)
    {
        var c=Controlador(nombre+"Visual");c.AddParameter("Velocidad",AnimatorControllerParameterType.Float);
        foreach(string b in new[]{"Atacando","Dano","Muerto"})c.AddParameter(b,AnimatorControllerParameterType.Bool);
        var sm=c.layers[0].stateMachine;
        var idle=Estado(sm,"Idle",Clip(nombre+"_Idle",new[]{reposo},1));
        var pasos=File.Exists(Arte+"Enemigos/Generados/"+nombre+"_Walk1.png") ? new[]{S("Enemigos/Generados/"+nombre+"_Walk1"),S("Enemigos/Generados/"+nombre+"_Walk2")} : new[]{reposo};
        var move=Estado(sm,"Move",Clip(nombre+"_Move",pasos,6));
        var strike=Estado(sm,"Attack",Clip(nombre+"_Attack",new[]{ataque},1));
        var hurt=Estado(sm,"Hurt",Clip(nombre+"_Hurt",new[]{reposo},1));
        var dead=Estado(sm,"Defeat",Clip(nombre+"_Defeat",new[]{reposo},1));sm.defaultState=idle;
        var t=Hacia(sm,dead);Cond(t,"Muerto",AnimatorConditionMode.If);
        t=Hacia(sm,hurt);Cond(t,"Muerto",AnimatorConditionMode.IfNot);Cond(t,"Dano",AnimatorConditionMode.If);
        t=Hacia(sm,strike);Cond(t,"Muerto",AnimatorConditionMode.IfNot);Cond(t,"Dano",AnimatorConditionMode.IfNot);Cond(t,"Atacando",AnimatorConditionMode.If);
        foreach(var e in new[]{idle,move}) {t=Hacia(sm,e);Cond(t,"Muerto",AnimatorConditionMode.IfNot);Cond(t,"Dano",AnimatorConditionMode.IfNot);Cond(t,"Atacando",AnimatorConditionMode.IfNot);Cond(t,"Velocidad",e==idle?AnimatorConditionMode.Less:AnimatorConditionMode.Greater,.05f);}
        Prefab(prefab,g=>{
            var v=g.transform.Find("Sprite").GetComponent<SpriteRenderer>();v.sprite=reposo;v.sortingOrder=20;
            AjustarPies(v,alto,g.GetComponent<ControladorJefe>()!=null ? -1f : -.65f);
            var a=Obtener<Animator>(g);a.runtimeAnimatorController=c;a.applyRootMotion=false;
            var p=Obtener<PresentacionRival>(g);p.animador=a;p.visual=v;
            var old=g.transform.Find("AvisoVisual");if(old!=null)UnityEngine.Object.DestroyImmediate(old.gameObject);
            var aviso=new GameObject("AvisoVisual").AddComponent<SpriteRenderer>();aviso.transform.SetParent(g.transform,false);aviso.sprite=S("Efectos/Aviso");aviso.transform.localPosition=new Vector3(0,alto-.25f,0);aviso.transform.localScale=Vector3.one*.45f;aviso.sortingOrder=25;aviso.enabled=false;p.aviso=aviso;
        });EditorUtility.SetDirty(c);
    }
    private static float AlturaAlpha(Sprite s)
    {
        var tex=new Texture2D(2,2);tex.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(s)));
        var pixels=tex.GetPixels32();int min=tex.height,max=0;
        for(int y=0;y<tex.height;y++)for(int x=0;x<tex.width;x++)if(pixels[y*tex.width+x].a>32){min=Mathf.Min(min,y);max=Mathf.Max(max,y);}
        UnityEngine.Object.DestroyImmediate(tex);return Mathf.Max(1,max-min+1)/s.pixelsPerUnit;
    }
    private static void AjustarPies(SpriteRenderer v,float alto,float baseLocal)
    {
        float escala=alto/AlturaAlpha(v.sprite);
        var tex=new Texture2D(2,2);tex.LoadImage(File.ReadAllBytes(AssetDatabase.GetAssetPath(v.sprite)));
        var pix=tex.GetPixels32();int minimo=tex.height;
        for(int y=0;y<tex.height;y++)for(int x=0;x<tex.width;x++)if(pix[y*tex.width+x].a>32)minimo=Mathf.Min(minimo,y);
        float pie=(minimo-v.sprite.pivot.y)/v.sprite.pixelsPerUnit;
        v.transform.localScale=Vector3.one*escala;v.transform.localPosition=new Vector3(0,baseLocal-pie*escala,0);
        UnityEngine.Object.DestroyImmediate(tex);
    }
    public static void PilotoChipotle()
    {
        Directory.CreateDirectory(Anim);AssetDatabase.Refresh();Importar();Jugador();AssetDatabase.SaveAssets();
        Debug.Log("MIXI_PILOTO_CHIPOTLE_OK: atlas, clips y Animator importados");
    }
    private static void Rivales()
    {
        Rival("Assets/Prefabs/Enemigos/EnemigoBandido.prefab","Bandido",S("Enemigos/Generados/Bandido_Idle"),S("Enemigos/Generados/Bandido_Attack"),1.5f);
        Rival("Assets/Prefabs/Enemigos/EnemigoMonaguillo.prefab","Monaguillo",S("Enemigos/Generados/Monaguillo_Idle"),S("Enemigos/Generados/Monaguillo_Attack"),1.5f);
        foreach(string n in new[]{"Vagabundo","Fidel","Lider"}) {
            bool generado=File.Exists(Arte+"Jefes/Generados/"+n+"_Idle.png");
            Rival("Assets/Prefabs/Jefes/"+n+".prefab",n,S(generado?"Jefes/Generados/"+n+"_Idle":"Jefes/"+n+"/"+n),S(generado?"Jefes/Generados/"+n+"_Attack":"Jefes/"+n+"/"+n),2.25f);
        }
    }
    private static void Proyectiles()
    {
        Prefab("Assets/Prefabs/Objetos/Proyectil.prefab",g=>{
            var v=g.GetComponentInChildren<SpriteRenderer>();v.sprite=S("Objetos/Pelo/Pelo");v.sortingOrder=23;
            var a=Obtener<AspectoProyectil>(g);a.pelo=S("Objetos/Pelo/Pelo");a.ropa=S("Objetos/Ropa/Ropa");a.bolsa=S("Efectos/Bolsa");a.mapache=S("Efectos/Mapache");a.tambo=S("Efectos/Tambo");a.incienso=S("Efectos/Incienso");a.agua=S("Efectos/Agua");a.objeto=S("Efectos/Caja");
        });
    }
    private static void HUD()
    {
        Prefab("Assets/Prefabs/Interfaz/HUD.prefab",g=>{
            var so=new SerializedObject(g.GetComponent<InterfazPartida>());
            foreach(string campo in new[]{"retratoChipotle","retratoPiyu","iconoVida","iconoComida","iconoMoneda","iconoRopa","iconoVelocidad","iconoFuerza"}) {
                string nombre=char.ToUpperInvariant(campo[0])+campo.Substring(1);
                so.FindProperty(campo).objectReferenceValue=AssetDatabase.LoadAssetAtPath<Texture2D>(Arte+"Interfaz/"+nombre+".png");
            }
            foreach(var par in new[]{new[]{"escenaChipotle","Interfaz/ChipotleNarrativa"},new[]{"escenaPiyuNormal","Personajes/Piyu/FuenteAlpha/Piyu_Normal"},new[]{"escenaPiyuAsustada","Personajes/Piyu/FuenteAlpha/Piyu_Asustada"},new[]{"escenaPiyuRescatada","Personajes/Piyu/FuenteAlpha/Piyu_Rescatada"},new[]{"escenaIguana","Personajes/Iguana/FuenteAlpha/Iguana"},new[]{"escenaBandido","Enemigos/Bandido/FuenteAlpha/Bandido_Variante_01"}})
                so.FindProperty(par[0]).objectReferenceValue=AssetDatabase.LoadAssetAtPath<Texture2D>(Arte+par[1]+".png");
            so.FindProperty("fondoMenu").objectReferenceValue=AssetDatabase.LoadAssetAtPath<Texture2D>(Arte+"Interfaz/FondoMenu.png");so.ApplyModifiedPropertiesWithoutUndo();
        });
    }
    private static SpriteRenderer Pieza(Transform padre,string ruta,float x,float suelo,float alto,int orden)
    {
        var g=new GameObject(Path.GetFileName(ruta));g.transform.SetParent(padre,false);
        var v=g.AddComponent<SpriteRenderer>();v.sprite=S(ruta);v.sortingOrder=orden;
        float factor=alto/v.sprite.bounds.size.y;g.transform.localScale=Vector3.one*factor;g.transform.position=new Vector3(x,suelo+alto*.5f,0);return v;
    }
    private static Transform Capa(Transform padre,string nombre) { var g=new GameObject(nombre);g.transform.SetParent(padre,false);return g.transform; }
    private static void Nivel(int numero)
    {
        var escena=EditorSceneManager.OpenScene("Assets/Escenas/"+Niveles[numero-1]+".unity");
        var n=UnityEngine.Object.FindFirstObjectByType<GestorNivel>();
        var viejo=GameObject.Find("Decoracion_SinColisiones");if(viejo!=null)viejo.SetActive(false);
        var anterior=GameObject.Find("PresentacionVisual");if(anterior!=null)UnityEngine.Object.DestroyImmediate(anterior);
        var raiz=new GameObject("PresentacionVisual").transform;
        var fondo=Capa(raiz,"Background");var medio=Capa(raiz,"Midground");var detalle=Capa(raiz,"Decoracion");var frente=Capa(raiz,"Foreground");
        string ruta="Escenarios/Nivel"+numero+"/";
        for(int i=-1;i<7;i++) {
            var cielo=Pieza(fondo,ruta+"Cielo_Pixelado",i*20,-2,14,-100);cielo.size=new Vector2(20,14);cielo.drawMode=SpriteDrawMode.Sliced;cielo.transform.localScale=Vector3.one;
            var monte=Pieza(fondo,ruta+(numero==3?"Silueta_Urbana":"Montanas_Lejanas"),i*20,1.2f,5.5f,-80);monte.color=new Color(.8f,.8f,.85f,1);
        }
        fondo.gameObject.AddComponent<ParallaxVisual>().factor=.12f;
        var prado=Pieza(medio,ruta+"TerrenoLejano_Auxiliar",41.5f,0,1.65f,-65);prado.drawMode=SpriteDrawMode.Tiled;prado.size=new Vector2(88,1.65f);prado.transform.localScale=Vector3.one;
        var baseVisual=Pieza(detalle,ruta+"Suelo_Tile",41.5f,-4.2f,3,4);baseVisual.drawMode=SpriteDrawMode.Tiled;baseVisual.size=new Vector2(88,3);baseVisual.transform.localScale=Vector3.one;
        if(numero==1) Barrio(medio,detalle,frente,ruta); else if(numero==2) Iglesia(medio,detalle,frente,ruta); else Almacen(medio,detalle,frente,ruta);
        var suelo=GameObject.Find("SueloContinuo").GetComponent<SpriteRenderer>();suelo.sprite=S(ruta+"Suelo_Tile");suelo.color=Color.white;suelo.drawMode=SpriteDrawMode.Tiled;suelo.size=new Vector2(88,1.2f);suelo.sortingOrder=5;
        var borde=GameObject.Find("BordeSuelo");if(borde!=null)borde.GetComponent<SpriteRenderer>().enabled=false;
        foreach(string plataforma in new[]{"PlataformaBaja","PlataformaLigera"}) {var v=GameObject.Find(plataforma).GetComponent<SpriteRenderer>();v.sprite=S(ruta+"Plataforma_Tile");v.color=Color.white;v.drawMode=SpriteDrawMode.Tiled;v.sortingOrder=8;}
        Puerta(n.barrera);
        foreach(var r in UnityEngine.Object.FindObjectsByType<Recogible>(FindObjectsSortMode.None)) {
            var v=r.GetComponentInChildren<SpriteRenderer>();string objeto=r.tipo.ToString();
            if(r.tipo!=TipoRecogible.Salida) {v.sprite=S("Objetos/"+objeto+"/"+objeto);v.transform.localScale=Vector3.one*(.62f/AlturaAlpha(v.sprite));v.sortingOrder=22;}
            else if(numero==1) {v.sprite=S("Personajes/Iguana/Iguana");v.transform.localScale=Vector3.one*(.8f/AlturaAlpha(v.sprite));v.transform.localPosition=new Vector3(0,-.1f,0);v.sortingOrder=22;}
            else if(numero==2) {v.sprite=S("Objetos/Pista/Pista");v.transform.localScale=Vector3.one*(.65f/AlturaAlpha(v.sprite));v.sortingOrder=22;}
            else {
                v.sprite=S("Personajes/Piyu/Piyu_Cautiva");v.transform.localScale=Vector3.one*(1.3f/AlturaAlpha(v.sprite));v.sortingOrder=22;
                var rescate=Obtener<PresentacionRescate>(r.gameObject);rescate.nivel=n;rescate.personaje=v;rescate.cautiva=v.sprite;rescate.rescatada=S("Personajes/Piyu/Piyu_Rescatada");
            }
        }
        if(n.piyuInicio!=null) {var v=n.piyuInicio.GetComponentInChildren<SpriteRenderer>();v.sprite=S("Personajes/Piyu/Piyu_Normal");AjustarPies(v,1f,-.5f);v.sortingOrder=20;}
        foreach(var bandera in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))if(bandera.name=="BanderaCheckpoint") {bandera.sprite=S("Efectos/BanderaCheckpoint");bandera.drawMode=SpriteDrawMode.Sliced;bandera.color=Color.white;}
        foreach(var rotulo in UnityEngine.Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None)) if(rotulo.transform.parent!=null && rotulo.transform.parent.name=="Terreno") {
            if(rotulo.text=="Derrota al jefe") rotulo.gameObject.SetActive(false);
            else { rotulo.characterSize=rotulo.text=="CHECKPOINT" ? .07f : .065f;rotulo.color=new Color(1,.92f,.72f); }
        }
        foreach(var root in escena.GetRootGameObjects())foreach(var c in root.GetComponentsInChildren<Component>(true))if(c!=null && PrefabUtility.IsPartOfPrefabInstance(c))PrefabUtility.RecordPrefabInstancePropertyModifications(c);
        EditorSceneManager.SaveScene(escena);
    }
    private static void Puerta(GameObject barrera)
    {
        barrera.GetComponent<SpriteRenderer>().enabled=false;
        var t=barrera.transform.Find("PuertaVisual");
        if(t==null) {t=new GameObject("PuertaVisual").transform;t.SetParent(barrera.transform,false);}
        t.localPosition=new Vector3(0,-.8f,0);
        var v=Obtener<SpriteRenderer>(t.gameObject);v.sprite=S("Efectos/Jaula");v.drawMode=SpriteDrawMode.Sliced;v.size=new Vector2(2.1f,3.4f);v.sortingOrder=15;
    }
    public static void PulirPuertasYConstruirLinux()
    {
        foreach(string nombre in Niveles) {
            var escena=EditorSceneManager.OpenScene("Assets/Escenas/"+nombre+".unity");
            Puerta(UnityEngine.Object.FindFirstObjectByType<GestorNivel>().barrera);
            EditorSceneManager.SaveScene(escena);
        }
        ValidacionVisualEditor.ConstruirLinux();
    }
    private static void Barrio(Transform m,Transform d,Transform f,string p)
    {
        Pieza(m,p+"Casa_Chipotle",4,0,4.8f,-40);Pieza(m,p+"Fachada_Barrio",13,0,3.8f,-39);Pieza(m,p+"Casa_Chipotle",23,0,4.3f,-42);
        Pieza(m,p+"Callejon_Modulo",28,0,4.5f,-45);
        for(int i=0;i<5;i++)Pieza(m,p+"Arbol_Parque",33+i*5,0,7.6f,-44);
        Pieza(m,p+"Entrada_Parque",37,0,4.1f,-30);
        for(int i=0;i<7;i++)Pieza(m,p+"Reja_Alpha_Auxiliar",31+i*4,0,1.7f,-20);
        Pieza(d,p+"Puesto_Elotes",48,0,2.5f,-10);Pieza(d,p+"Banca",42,0,1.3f,-9);
        Pieza(m,p+"Callejon_Modulo",62,0,5,-40);Pieza(m,p+"Fachada_Barrio",72,0,3.8f,-38);Pieza(m,p+"Arbol_Barrio",77,0,6,-45);
        foreach(float x in new[]{58f,63f,66f,74f,77f,83f})Pieza(d,p+"Basurero",x,0,1.4f,-8);
        foreach(float x in new[]{9f,18f,34f,46f,60f,80f})Pieza(d,p+"Lampara_Parque",x,0,3.5f,-12);
        Pieza(d,"Efectos/Bolsa",69,0,.65f,-7);Pieza(d,"Efectos/Tambo",75,0,1.4f,-7);
        Pieza(f,p+"Reja_Alpha_Auxiliar",-1,-.8f,1.2f,28);Pieza(f,p+"Reja_Alpha_Auxiliar",84,-.8f,1.2f,28);
    }
    private static void Iglesia(Transform m,Transform d,Transform f,string p)
    {
        Pieza(m,p+"Veterinaria",7,0,5.3f,-35);Pieza(m,p+"Arbol_Florido",14,0,5.5f,-37);Pieza(m,p+"Pasaje_Lateral",22,0,4.4f,-42);
        Pieza(m,p+"Iglesia_Monumental",40,0,6.7f,-32);Pieza(m,p+"Escalera_Iglesia",40,0,1.1f,-26);Pieza(d,p+"Estatua_Plaza",30,0,3.8f,-24);
        Pieza(m,p+"Patio_Arcadas",55,0,4.8f,-35);Pieza(m,p+"Iglesia_Monumental",73,0,6.4f,-39);
        foreach(float x in new[]{16f,26f,48f,62f,81f})Pieza(m,p+"Arbol_Cementerio",x,0,8.2f,-45);
        for(int i=0;i<8;i++) {Pieza(d,p+"Lapida_Auxiliar",57+i*3.5f,0,1.3f,-15);if(i%2==0)Pieza(d,p+"Cruz_Cementerio_Auxiliar",58+i*3.5f,.2f,1.7f,-16);}
        for(int i=0;i<6;i++)Pieza(m,p+"Reja_Alpha_Auxiliar",57+i*5,0,1.9f,-20);
        foreach(float x in new[]{4f,18f,28f,35f,46f,61f,77f})Pieza(d,p+"Lampara_Iglesia",x,0,3.8f,-12);
        Pieza(d,p+"Senales_Cementerio",51,0,2.3f,-10);Pieza(f,p+"Reja_Alpha_Auxiliar",84,-.7f,1.3f,28);
    }
    private static void Almacen(Transform m,Transform d,Transform f,string p)
    {
        Pieza(m,p+"Escuela_Urbana_Auxiliar",5,0,5.2f,-35);Pieza(m,p+"Callejon_Industrial",20,0,5.1f,-37);Pieza(m,p+"Callejon_Marco_Auxiliar",31,0,5.4f,-30);
        Pieza(m,p+"Torre_Vigilancia",44,0,6.3f,-40);Pieza(m,p+"Almacen_Monumental",66,0,6.5f,-33);Pieza(m,p+"Torre_Vigilancia",83,0,6,-41);
        Pieza(d,p+"Camion_Carga",47,0,3.4f,-22);Pieza(d,p+"Montacargas",58,0,2.1f,-15);
        foreach(float x in new[]{12f,26f,36f,53f,61f,74f,83f})Pieza(d,p+"Cajas_Madera",x,0,1.5f,-9);
        foreach(float x in new[]{16f,33f,56f,69f,77f})Pieza(d,p+"Barril_Azul",x,0,1.2f,-8);
        foreach(float x in new[]{22f,40f,63f,82f})Pieza(d,p+"Barril_Rojo",x,0,1.2f,-8);
        foreach(float x in new[]{10f,30f,51f,70f,81f})Pieza(d,p+"Poste_Reflectores",x,0,4.5f,-14);
        Pieza(d,p+"Lampara_Industrial",81,2.5f,.7f,-10);
        Pieza(f,p+"Barrera_Industrial",-1,-.3f,1.2f,27);Pieza(f,p+"Barrera_Industrial",84,-.3f,1.2f,27);
    }
}
