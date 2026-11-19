using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

// Construccion explicita, nunca al importar. No modifica Chipotle ni PruebasJugador.
public static class ConstruccionV01
{
    private const string Arte = "Assets/Arte/Provisional/";
    private const string Original = "Assets/Prefabs/Personajes/Chipotle.prefab";
    private const string Jugador = "Assets/Prefabs/Personajes/ChipotleJugable.prefab";
    private const string Animaciones = "Assets/Animaciones/Chipotle/";
    private static readonly string[] Escenas = {
        "Assets/Escenas/MenuPrincipal.unity", "Assets/Escenas/Nivel_01_Barrio.unity",
        "Assets/Escenas/Nivel_02_Iglesia.unity", "Assets/Escenas/Nivel_03_Almacen.unity"
    };
    private static readonly string[] Imagenes = {
        "Chipotle", "Piyu", "Bandido", "Monaguillo", "Vagabundo", "Fidel", "Lider",
        "Iguana", "Pollo", "Moneda", "Pelo", "Ropa", "Velocidad", "Fuerza", "Pista"
    };
    private static Sprite cuadrado;

    [MenuItem("Mixi-Mixi/Crear v0.1 (solo ausentes)")]
    public static void Crear()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Sal del modo Play antes de construir.");
        // Preflight de TODAS las escenas antes de producir cualquier asset.
        foreach (string ruta in Escenas) ExigirAusente(ruta);
        if (AssetDatabase.LoadAssetAtPath<GameObject>(Original) == null)
            throw new InvalidOperationException("Falta el prefab original: " + Original);
        foreach (string nombre in Imagenes)
            if (!File.Exists(Arte + nombre + ".png"))
                throw new InvalidOperationException("Falta el PNG: " + nombre);
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (Application.isBatchMode) EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        var configuracionAnterior = EditorSceneManager.GetSceneManagerSetup();
        // Las escenas nuevas se construyen aditivamente, sin cerrar ni guardar las del usuario.
        Scene trabajo = default;
        try
        {
            foreach (string carpeta in new[] { "Assets/Escenas", "Assets/Prefabs/Personajes",
                "Assets/Prefabs/Enemigos", "Assets/Prefabs/Jefes", "Assets/Prefabs/Objetos",
                "Assets/Prefabs/Interfaz", "Assets/Animaciones/Chipotle" }) CrearCarpeta(carpeta);
            foreach (string nombre in Imagenes) ImportarSprite(Arte + nombre + ".png");
            PrepararCuadrado();
            trabajo = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(trabajo);
            var proyectil = CrearProyectil();
            var jugador = CrearJugador(proyectil);
            var bandido = CrearEnemigo("Bandido", 44f);
            var monaguillo = CrearEnemigo("Monaguillo", 60f);
            var jefes = new[] {
                CrearJefe("Vagabundo", 1, 180f, proyectil, bandido),
                CrearJefe("Fidel", 2, 240f, proyectil, monaguillo),
                CrearJefe("Lider", 3, 300f, proyectil, bandido)
            };
            var recogible = CrearRecogible();
            var hud = CrearHUD();
            CrearCamara(null, new Color(.07f, .1f, .18f));
            Instanciar(hud.gameObject, Vector2.zero).GetComponent<InterfazPartida>().nivel = null;
            GuardarEscena(trabajo, Escenas[0]);
            EditorSceneManager.CloseScene(trabajo, true);
            for (int numero = 1; numero <= 3; numero++)
            {
                trabajo = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                SceneManager.SetActiveScene(trabajo);
                CrearNivel(numero, jugador, numero == 2 ? monaguillo : bandido, jefes[numero - 1], recogible, hud);
                GuardarEscena(trabajo, Escenas[numero]);
                EditorSceneManager.CloseScene(trabajo, true);
            }
            // Unico ajuste global: anteponer las escenas jugables sin perder SampleScene ni otras entradas.
            var entradas = new List<EditorBuildSettingsScene>();
            foreach (string ruta in Escenas) entradas.Add(new EditorBuildSettingsScene(ruta, true));
            foreach (var entrada in EditorBuildSettings.scenes)
                if (Array.IndexOf(Escenas, entrada.path) < 0) entradas.Add(entrada);
            EditorBuildSettings.scenes = entradas.ToArray();
            AssetDatabase.SaveAssets();
            Debug.Log("MIXI_CONSTRUCCION_V01_OK: cuatro escenas y prefabs. Falta validacion en Play Mode.");
        }
        finally
        {
            if (trabajo.IsValid() && trabajo.isLoaded) EditorSceneManager.CloseScene(trabajo, true);
            if (configuracionAnterior.Length > 0) EditorSceneManager.RestoreSceneManagerSetup(configuracionAnterior);
        }
    }

    private static void CrearCarpeta(string ruta)
    {
        if (AssetDatabase.IsValidFolder(ruta)) return;
        string padre = Path.GetDirectoryName(ruta).Replace(Path.DirectorySeparatorChar, '/');
        CrearCarpeta(padre);
        AssetDatabase.CreateFolder(padre, Path.GetFileName(ruta));
    }

    private static void ExigirAusente(string ruta)
    {
        if (File.Exists(ruta) || AssetDatabase.LoadMainAssetAtPath(ruta) != null)
            throw new InvalidOperationException("No se sobrescribe un asset existente: " + ruta);
    }

    private static void ImportarSprite(string ruta)
    {
        AssetDatabase.ImportAsset(ruta, ImportAssetOptions.ForceSynchronousImport);
        var importador = AssetImporter.GetAtPath(ruta) as TextureImporter;
        if (importador == null) throw new InvalidOperationException("PNG no importable: " + ruta);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.spritePixelsPerUnit = 100f;
        importador.spritePivot = new Vector2(.5f, .5f);
        var ajustes = new TextureImporterSettings();
        importador.ReadTextureSettings(ajustes);
        ajustes.spriteMeshType = SpriteMeshType.FullRect;
        ajustes.spriteAlignment = (int)SpriteAlignment.Center;
        importador.SetTextureSettings(ajustes);
        importador.filterMode = FilterMode.Point;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.mipmapEnabled = false;
        importador.alphaIsTransparency = true;
        importador.npotScale = TextureImporterNPOTScale.None;
        importador.wrapMode = TextureWrapMode.Clamp;
        importador.SaveAndReimport();
    }

    private static void PrepararCuadrado()
    {
        string ruta = Arte + "CuadradoV01.png";
        if (!File.Exists(ruta))
        {
            var textura = new Texture2D(8, 8, TextureFormat.RGBA32, false);
            try
            {
                var pixeles = new Color[64];
                for (int i = 0; i < pixeles.Length; i++) pixeles[i] = Color.white;
                textura.SetPixels(pixeles); textura.Apply();
                File.WriteAllBytes(ruta, textura.EncodeToPNG());
            }
            finally { UnityEngine.Object.DestroyImmediate(textura); }
        }
        ImportarSprite(ruta);
        cuadrado = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        if (cuadrado == null) throw new InvalidOperationException("Falta el sprite auxiliar.");
    }

    private static Sprite CargarSprite(string nombre)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Arte + nombre + ".png");
        if (sprite == null) throw new InvalidOperationException("Sprite ausente: " + nombre);
        return sprite;
    }

    private static GameObject GuardarPrefab(GameObject objeto, string ruta)
    {
        try
        {
            ExigirAusente(ruta);
            var resultado = PrefabUtility.SaveAsPrefabAsset(objeto, ruta);
            if (resultado == null) throw new InvalidOperationException("No se pudo guardar " + ruta);
            return resultado;
        }
        finally { UnityEngine.Object.DestroyImmediate(objeto); }
    }

    private static GameObject Existente(string ruta)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ruta);
        if (prefab == null && File.Exists(ruta)) throw new InvalidOperationException("Prefab no importable: " + ruta);
        return prefab;
    }

    private static T Componente<T>(GameObject objeto) where T : Component
    {
        var componente = objeto.GetComponent<T>();
        if (componente == null) throw new InvalidOperationException(objeto.name + " requiere " + typeof(T).Name);
        return componente;
    }

    private static GameObject Instanciar(GameObject prefab, Vector2 posicion)
    {
        var objeto = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        objeto.transform.position = posicion;
        return objeto;
    }

    private static SpriteRenderer Visual(Transform padre, string imagen, float alto, float baseY = 0f)
    {
        var objeto = new GameObject("Sprite");
        objeto.transform.SetParent(padre, false);
        objeto.transform.localPosition = new Vector3(0f, baseY, 0f);
        var renderer = objeto.AddComponent<SpriteRenderer>();
        renderer.sprite = CargarSprite(imagen);
        renderer.sortingOrder = 10;
        objeto.transform.localScale = Vector3.one * (alto / renderer.sprite.bounds.size.y);
        return renderer;
    }

    private static Proyectil CrearProyectil()
    {
        const string ruta = "Assets/Prefabs/Objetos/Proyectil.prefab";
        var previo = Existente(ruta);
        if (previo != null) return Componente<Proyectil>(previo);
        var objeto = new GameObject("Proyectil");
        Visual(objeto.transform, "Pelo", 1f);
        var componente = objeto.AddComponent<Proyectil>();
        var cuerpo = objeto.GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f;
        cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var collider = objeto.GetComponent<CircleCollider2D>();
        collider.isTrigger = true; collider.radius = .35f;
        return GuardarPrefab(objeto, ruta).GetComponent<Proyectil>();
    }

    private static GameObject CrearJugador(Proyectil proyectil)
    {
        var previo = Existente(Jugador);
        if (previo != null) return previo;
        var controlador = CrearAnimador();
        var objeto = Instanciar(AssetDatabase.LoadAssetAtPath<GameObject>(Original), Vector2.zero);
        objeto.name = "ChipotleJugable";
        objeto.transform.localScale = Vector3.one;
        // No desempaquetar: guardar una instancia del original produce una variante real.
        var movimiento = objeto.GetComponent<MovimientoJugador>();
        var serializado = new SerializedObject(movimiento);
        var orientacion = serializado.FindProperty("visual").objectReferenceValue as Transform;
        if (orientacion == null) throw new InvalidOperationException("Chipotle original no tiene visual.");
        // Conservar Transform/referencia heredados. Quitar renderers solo como overrides de
        // variante: PresentacionJugador busca el primer SpriteRenderer y debe hallar el nuevo.
        foreach (var renderer in orientacion.GetComponentsInChildren<SpriteRenderer>(true))
            UnityEngine.Object.DestroyImmediate(renderer);
        orientacion.localScale = Vector3.one;
        orientacion.localPosition = Vector3.zero;
        Visual(orientacion, "Chipotle", 1.3f);
        var animador = orientacion.gameObject.AddComponent<Animator>();
        animador.runtimeAnimatorController = controlador;
        animador.applyRootMotion = false;
        var vida = objeto.AddComponent<SistemaVida>();
        vida.esJugador = true; vida.maxima = 100f;
        objeto.AddComponent<RecursosJugador>();
        objeto.AddComponent<CombateJugador>().proyectilPrefab = proyectil;
        objeto.AddComponent<InteraccionJugador>();
        objeto.AddComponent<PresentacionJugador>().animador = animador;
        var resultado = GuardarPrefab(objeto, Jugador);
        if (PrefabUtility.GetPrefabAssetType(resultado) != PrefabAssetType.Variant)
            throw new InvalidOperationException("ChipotleJugable debe ser una variante.");
        return resultado;
    }

    private static AnimatorController CrearAnimador()
    {
        string ruta = Animaciones + "Chipotle.controller";
        var existente = AssetDatabase.LoadAssetAtPath<AnimatorController>(ruta);
        if (existente != null) return existente;
        ExigirAusente(ruta);
        var reposo = CrearClip("Reposo", 1f, .018f);
        var caminar = CrearClip("Caminar", .32f, .07f);
        var controlador = AnimatorController.CreateAnimatorControllerAtPath(ruta);
        controlador.AddParameter("Velocidad", AnimatorControllerParameterType.Float);
        var estados = controlador.layers[0].stateMachine;
        var idle = estados.AddState("Reposo"); idle.motion = reposo;
        var walk = estados.AddState("Caminar"); walk.motion = caminar;
        estados.defaultState = idle;
        var andando = idle.AddTransition(walk);
        andando.hasExitTime = false; andando.duration = .08f;
        andando.AddCondition(AnimatorConditionMode.Greater, .1f, "Velocidad");
        var quieto = walk.AddTransition(idle);
        quieto.hasExitTime = false; quieto.duration = .08f;
        quieto.AddCondition(AnimatorConditionMode.Less, .1f, "Velocidad");
        EditorUtility.SetDirty(controlador);
        return controlador;
    }

    private static AnimationClip CrearClip(string nombre, float duracion, float amplitud)
    {
        string ruta = Animaciones + nombre + ".anim";
        var existente = AssetDatabase.LoadAssetAtPath<AnimationClip>(ruta);
        if (existente != null) return existente;
        ExigirAusente(ruta);
        var clip = new AnimationClip { name = nombre, frameRate = 12f };
        clip.SetCurve("Sprite", typeof(Transform), "m_LocalPosition.y",
            new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(duracion * .5f, amplitud), new Keyframe(duracion, 0f)));
        var ajustes = AnimationUtility.GetAnimationClipSettings(clip);
        ajustes.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, ajustes);
        AssetDatabase.CreateAsset(clip, ruta);
        return clip;
    }

    private static EnemigoBase CrearEnemigo(string imagen, float maxima)
    {
        string ruta = "Assets/Prefabs/Enemigos/Enemigo" + imagen + ".prefab";
        var previo = Existente(ruta);
        if (previo != null) return Componente<EnemigoBase>(previo);
        var objeto = new GameObject("Enemigo" + imagen);
        // Centro fisico .65; visual 1.6 con pies al nivel del suelo.
        Visual(objeto.transform, imagen, 1.6f, .15f);
        objeto.AddComponent<SistemaVida>().maxima = maxima;
        objeto.AddComponent<EnemigoBase>();
        ConfigurarRival(objeto, new Vector2(.8f, 1.3f));
        return GuardarPrefab(objeto, ruta).GetComponent<EnemigoBase>();
    }

    private static ControladorJefe CrearJefe(string imagen, int numero, float maxima,
        Proyectil proyectil, EnemigoBase refuerzo)
    {
        string ruta = "Assets/Prefabs/Jefes/" + imagen + ".prefab";
        var previo = Existente(ruta);
        if (previo != null) return Componente<ControladorJefe>(previo);
        var objeto = new GameObject(imagen);
        Visual(objeto.transform, imagen, 2.2f, .1f);
        objeto.AddComponent<SistemaVida>().maxima = maxima;
        var jefe = objeto.AddComponent<ControladorJefe>();
        jefe.nivel = numero; jefe.proyectilPrefab = proyectil; jefe.refuerzoPrefab = refuerzo;
        ConfigurarRival(objeto, new Vector2(1.1f, 2f));
        return GuardarPrefab(objeto, ruta).GetComponent<ControladorJefe>();
    }

    private static void ConfigurarRival(GameObject objeto, Vector2 tamano)
    {
        var cuerpo = objeto.GetComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f; cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;
        var collider = objeto.GetComponent<BoxCollider2D>();
        collider.isTrigger = true; collider.size = tamano;
    }

    private static Recogible CrearRecogible()
    {
        const string ruta = "Assets/Prefabs/Objetos/Recogible.prefab";
        var previo = Existente(ruta);
        if (previo != null) return Componente<Recogible>(previo);
        var objeto = new GameObject("Recogible");
        Visual(objeto.transform, "Moneda", .6f);
        var collider = objeto.AddComponent<CircleCollider2D>();
        collider.isTrigger = true; collider.radius = .4f;
        var recogible = objeto.AddComponent<Recogible>();
        recogible.tipo = TipoRecogible.Moneda; recogible.etiqueta = "Moneda";
        return GuardarPrefab(objeto, ruta).GetComponent<Recogible>();
    }

    private static InterfazPartida CrearHUD()
    {
        const string ruta = "Assets/Prefabs/Interfaz/HUD.prefab";
        var previo = Existente(ruta);
        if (previo != null) return Componente<InterfazPartida>(previo);
        var objeto = new GameObject("HUD");
        objeto.AddComponent<InterfazPartida>();
        return GuardarPrefab(objeto, ruta).GetComponent<InterfazPartida>();
    }

    private static void CrearNivel(int numero, GameObject prefabJugador, EnemigoBase enemigo,
        ControladorJefe jefe, Recogible recogible, InterfazPartida hud)
    {
        var nivel = new GameObject("GestorNivel").AddComponent<GestorNivel>();
        nivel.numero = numero;
        nivel.jugador = Instanciar(prefabJugador, new Vector2(2f, .65f));
        nivel.interfaz = Instanciar(hud.gameObject, Vector2.zero).GetComponent<InterfazPartida>();
        nivel.interfaz.nivel = nivel;
        CrearCamara(nivel.jugador.transform, numero == 1 ? new Color(.42f, .7f, .88f) :
            numero == 2 ? new Color(.64f, .34f, .32f) : new Color(.1f, .16f, .23f));
        var luz = new GameObject("LuzGlobal2D").AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Global; luz.intensity = 1f;
        var terreno = new GameObject("Terreno").transform;
        Solido("SueloContinuo", new Vector2(41.5f, -.6f), new Vector2(88f, 1.2f),
            numero == 1 ? new Color(.38f, .25f, .18f) : new Color(.23f, .24f, .29f), terreno);
        Rectangulo("BordeSuelo", new Vector2(41.5f, -.08f), new Vector2(88f, .16f),
            numero == 1 ? new Color(.35f, .57f, .24f) : new Color(.48f, .43f, .38f), terreno, 2);
        Solido("LimiteIzquierdo", new Vector2(-2f, 5f), new Vector2(.5f, 10f), Color.clear, terreno);
        Solido("LimiteDerecho", new Vector2(85f, 5f), new Vector2(.5f, 10f), Color.clear, terreno);
        // Plataformas opcionales: superficies a .8/1.2, por debajo del salto de ~1.7.
        Solido("PlataformaBaja", new Vector2(13f, .65f), new Vector2(2.5f, .3f), new Color(.49f, .35f, .23f), terreno);
        Solido("PlataformaLigera", new Vector2(38f, 1.05f), new Vector2(2.5f, .3f), new Color(.49f, .35f, .23f), terreno);
        nivel.barrera = Solido("BarreraJefe", new Vector2(78f, 2.5f), new Vector2(.45f, 5f),
            new Color(.76f, .35f, .16f), terreno);
        Rotulo("Derrota al jefe", new Vector2(76f, 5.4f), .11f, Color.white, terreno);
        nivel.puntosControl = new Transform[3];
        float[] puntos = { 2f, 30f, 59f };
        for (int i = 0; i < puntos.Length; i++)
        {
            var punto = new GameObject("PuntoControl_" + i);
            punto.transform.position = new Vector3(puntos[i], .65f, 0f);
            nivel.puntosControl[i] = punto.transform;
            if (i == 0) continue;
            var trigger = punto.AddComponent<BoxCollider2D>(); trigger.isTrigger = true;
            trigger.size = new Vector2(1.4f, 3f);
            var control = punto.AddComponent<PuntoControl>(); control.indice = i; control.nivel = nivel;
            Rectangulo("PosteCheckpoint", new Vector2(puntos[i], 1f), new Vector2(.1f, 2f), Color.white, terreno, -1);
            Rectangulo("BanderaCheckpoint", new Vector2(puntos[i] + .45f, 1.7f), new Vector2(.8f, .45f), Color.cyan, terreno, -1);
            Rotulo("CHECKPOINT", new Vector2(puntos[i], 2.5f), .09f, Color.white, terreno);
        }
        var posiciones = new List<float> { 18f, 26f, 42f, 50f };
        if (numero >= 2) posiciones.AddRange(new[] { 35f, 55f });
        if (numero == 3) posiciones.AddRange(new[] { 22f, 46f });
        foreach (float x in posiciones)
            Instanciar(enemigo.gameObject, new Vector2(x, .65f)).GetComponent<EnemigoBase>().objetivo = nivel.jugador.transform;
        nivel.jefe = Instanciar(jefe.gameObject, new Vector2(72f, 1f)).GetComponent<ControladorJefe>();
        nivel.jefe.objetivo = nivel.jugador.transform;
        int id = 0;
        ColocarObjeto(recogible, nivel, TipoRecogible.Moneda, "Moneda", 7f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Moneda, "Moneda", 13f, 1.2f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Pollo, "Pollo", 15f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Ropa, "Ropa", 23f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Velocidad, "Velocidad", 32f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Moneda, "Moneda", 38f, 1.6f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Pollo, "Pollo", 44f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Ropa, "Ropa", 53f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Fuerza, "Fuerza", 61f, .55f, id++);
        ColocarObjeto(recogible, nivel, TipoRecogible.Pollo, "Pollo", 65f, .55f, id++);
        if (numero == 1) ColocarObjeto(recogible, nivel, TipoRecogible.Pista, "Pista", 10f, .6f, id++);
        nivel.salida = ColocarObjeto(recogible, nivel, TipoRecogible.Salida,
            numero == 1 ? "Iguana" : numero == 2 ? "Pista" : "Piyu", 81f, .7f, id++);
        if (numero == 1)
        {
            nivel.piyuInicio = new GameObject("PiyuInicio");
            nivel.piyuInicio.transform.position = new Vector3(4f, .5f, 0f);
            Visual(nivel.piyuInicio.transform, "Piyu", 1f);
        }
        Decorar(numero);
        if (numero == 1)
        {
            Rotulo("A/D mover · Shift correr · Espacio saltar", new Vector2(5f, 3f), .11f, Color.white, terreno);
            Rotulo("E recoger / pista", new Vector2(10f, 2.1f), .12f, Color.white, terreno);
            Rotulo("J garra · K pelo (mantener: cargar)", new Vector2(20f, 3.1f), .11f, Color.white, terreno);
            Rotulo("L lanzar ropa · B mochila · Esc pausa", new Vector2(31f, 3.6f), .11f, Color.white, terreno);
        }
        ValidarNivel(nivel);
        // Persistir overrides de instancias sin aplicar nada a los assets fuente.
        foreach (var raiz in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var componente in raiz.GetComponentsInChildren<Component>(true))
                if (componente != null && PrefabUtility.IsPartOfPrefabInstance(componente))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(componente);
    }

    private static Recogible ColocarObjeto(Recogible prefab, GestorNivel nivel, TipoRecogible tipo,
        string imagen, float x, float y, int indice)
    {
        var objeto = Instanciar(prefab.gameObject, new Vector2(x, y));
        objeto.name = tipo + "_" + indice;
        var recogible = objeto.GetComponent<Recogible>();
        recogible.tipo = tipo; recogible.nivel = nivel;
        recogible.identificador = "N" + nivel.numero + "_" + tipo + "_" + indice;
        recogible.etiqueta = tipo == TipoRecogible.Salida ?
            (nivel.numero == 1 ? "Liberar iguana" : nivel.numero == 2 ? "Pista del almacen" : "Rescatar a Piyu") : imagen;
        var visual = objeto.GetComponentInChildren<SpriteRenderer>();
        visual.sprite = CargarSprite(imagen);
        visual.transform.localScale = Vector3.one * (.6f / visual.sprite.bounds.size.y);
        return recogible;
    }

    private static void CrearCamara(Transform objetivo, Color fondo)
    {
        var objeto = new GameObject("CamaraPrincipal"); objeto.tag = "MainCamera";
        var camara = objeto.AddComponent<Camera>();
        camara.orthographic = true; camara.orthographicSize = 4.5f;
        camara.clearFlags = CameraClearFlags.SolidColor; camara.backgroundColor = fondo;
        objeto.AddComponent<UniversalAdditionalCameraData>();
        objeto.AddComponent<AudioListener>();
        var desplazamiento = new Vector3(2f, 2f, -10f);
        objeto.transform.position = objetivo != null ? objetivo.position + desplazamiento : new Vector3(0f, 0f, -10f);
        if (objetivo == null) return;
        var seguimiento = new SerializedObject(objeto.AddComponent<CamaraSeguimiento>());
        seguimiento.FindProperty("objetivo").objectReferenceValue = objetivo;
        seguimiento.FindProperty("desplazamiento").vector3Value = desplazamiento;
        seguimiento.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject Rectangulo(string nombre, Vector2 posicion, Vector2 tamano,
        Color color, Transform padre, int orden = 0)
    {
        var objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre, false); objeto.transform.position = posicion;
        var renderer = objeto.AddComponent<SpriteRenderer>(); renderer.sprite = cuadrado;
        renderer.drawMode = SpriteDrawMode.Sliced; renderer.size = tamano;
        renderer.color = color; renderer.sortingOrder = orden;
        return objeto;
    }

    private static GameObject Solido(string nombre, Vector2 posicion, Vector2 tamano, Color color, Transform padre)
    {
        var objeto = Rectangulo(nombre, posicion, tamano, color, padre);
        objeto.AddComponent<BoxCollider2D>().size = tamano;
        return objeto;
    }

    private static void Rotulo(string texto, Vector2 posicion, float escala, Color color, Transform padre)
    {
        var objeto = new GameObject("Rotulo_" + texto); objeto.transform.SetParent(padre, false);
        objeto.transform.position = new Vector3(posicion.x, posicion.y, -1f);
        var rotulo = objeto.AddComponent<TextMesh>(); rotulo.text = texto;
        rotulo.fontSize = 48; rotulo.characterSize = escala;
        rotulo.anchor = TextAnchor.MiddleCenter; rotulo.alignment = TextAlignment.Center; rotulo.color = color;
        objeto.GetComponent<MeshRenderer>().sortingOrder = 20;
    }

    private static void Decorar(int numero)
    {
        var padre = new GameObject("Decoracion_SinColisiones").transform;
        Color pared = numero == 1 ? new Color(.76f, .49f, .35f) : numero == 2 ? new Color(.42f, .35f, .48f) : new Color(.24f, .29f, .34f);
        Color acento = numero == 1 ? new Color(.38f, .59f, .3f) : numero == 2 ? new Color(.89f, .54f, .3f) : new Color(.91f, .43f, .16f);
        for (int i = 0; i < 11; i++)
        {
            float x = i * 8f; float altura = 3.6f + (i % 3) * .65f;
            Rectangulo("Edificio", new Vector2(x, altura * .5f + .1f), new Vector2(7f, altura), pared * (i % 2 == 0 ? 1f : .85f), padre, -20);
            Rectangulo("Cornisa", new Vector2(x, altura + .15f), new Vector2(7.4f, .3f), acento, padre, -19);
            for (int ventana = 0; ventana < 4; ventana++)
            {
                Rectangulo("Marco", new Vector2(x - 2.4f + ventana * 1.6f, altura - 1f), new Vector2(.9f, 1.1f), new Color(.14f, .18f, .24f), padre, -18);
                Rectangulo("Cristal", new Vector2(x - 2.4f + ventana * 1.6f, altura - 1f), new Vector2(.65f, .8f), numero == 3 ? acento : new Color(.62f, .76f, .79f), padre, -17);
            }
        }
        if (numero == 1)
        {
            Fachada("CASA DE CHIPOTLE", 3f, 8f, 4.2f, new Color(.88f, .65f, .44f), padre);
            Fachada("LA COLONIA", 23f, 7f, 4f, pared, padre);
            Rotulo("PARQUE", new Vector2(43f, 4.5f), .18f, Color.white, padre);
            for (int i = 0; i < 4; i++)
            {
                float x = 36f + i * 5f;
                Rectangulo("Tronco", new Vector2(x, 1.5f), new Vector2(.55f, 3f), new Color(.4f, .25f, .14f), padre, -12);
                Rectangulo("CopaPixel", new Vector2(x, 3.1f), new Vector2(3f, 1.7f), acento, padre, -11);
                Rectangulo("CopaAlta", new Vector2(x, 4f), new Vector2(1.7f, .6f), acento, padre, -11);
            }
            Fachada("BASURERO", 70f, 15f, 4.5f, new Color(.37f, .39f, .3f), padre);
            for (int i = 0; i < 5; i++)
                Rectangulo("TamboFondo", new Vector2(63f + i * 3f, .65f), new Vector2(1f, 1.3f), new Color(.28f, .37f, .25f), padre, -5);
        }
        else if (numero == 2)
        {
            Fachada("VETERINARIA", 10f, 12f, 4.5f, new Color(.61f, .52f, .57f), padre);
            Rectangulo("CruzVeterinaria", new Vector2(10f, 3f), new Vector2(1.3f, .3f), Color.white, padre, -7);
            Rectangulo("CruzVeterinaria", new Vector2(10f, 3f), new Vector2(.3f, 1.3f), Color.white, padre, -7);
            Fachada("IGLESIA", 40f, 17f, 5f, new Color(.56f, .49f, .56f), padre);
            Rectangulo("Campanario", new Vector2(40f, 5.2f), new Vector2(2.5f, 2.4f), pared, padre, -10);
            Rectangulo("CruzIglesia", new Vector2(40f, 6.6f), new Vector2(.2f, 1.3f), acento, padre, -9);
            Rectangulo("CruzIglesia", new Vector2(40f, 6.8f), new Vector2(.9f, .2f), acento, padre, -9);
            Rotulo("CEMENTERIO", new Vector2(68f, 4.5f), .18f, Color.white, padre);
            for (int i = 0; i < 8; i++)
            {
                float x = 58f + i * 3f;
                Rectangulo("Lapida", new Vector2(x, .8f), new Vector2(1f, 1.6f), new Color(.45f, .43f, .5f), padre, -6);
                Rectangulo("Inscripcion", new Vector2(x, 1f), new Vector2(.6f, .15f), new Color(.24f, .23f, .3f), padre, -5);
            }
        }
        else
        {
            Fachada("ESCUELA", 9f, 15f, 4.8f, new Color(.36f, .43f, .5f), padre);
            Fachada("CALLEJON", 34f, 13f, 4.6f, pared, padre);
            Fachada("ALMACEN ABANDONADO", 68f, 25f, 5.5f, new Color(.22f, .27f, .32f), padre);
            for (int i = 0; i < 9; i++)
            {
                float x = 56f + i * 3f;
                Rectangulo("VigaIndustrial", new Vector2(x, 2.5f), new Vector2(.2f, 5f), acento, padre, -8);
                Rectangulo("CajaFondo", new Vector2(x, .55f), new Vector2(1.8f, 1.1f), new Color(.48f, .33f, .2f), padre, -6);
                Rectangulo("MarcaCaja", new Vector2(x, .55f), new Vector2(1.5f, .12f), acento, padre, -5);
            }
            Rotulo("PIYU", new Vector2(81f, 2.5f), .18f, Color.white, padre);
        }
    }

    private static void Fachada(string nombre, float x, float ancho, float alto, Color color, Transform padre)
    {
        Rectangulo(nombre, new Vector2(x, alto * .5f), new Vector2(ancho, alto), color, padre, -15);
        Rectangulo("TechoEscalonado", new Vector2(x, alto), new Vector2(ancho + .5f, .3f), color * .7f, padre, -14);
        Rectangulo("Puerta", new Vector2(x, 1f), new Vector2(1.6f, 2f), new Color(.17f, .2f, .24f), padre, -13);
        Rectangulo("PlacaRotulo", new Vector2(x, alto - .55f), new Vector2(ancho - .5f, .7f), new Color(.14f, .18f, .23f), padre, -12);
        Rotulo(nombre, new Vector2(x, alto - .55f), .14f, Color.white, padre);
    }

    private static void ValidarNivel(GestorNivel nivel)
    {
        if (nivel.jugador == null || nivel.jefe == null || nivel.barrera == null || nivel.salida == null ||
            nivel.interfaz == null || nivel.interfaz.nivel != nivel || nivel.puntosControl.Length != 3)
            throw new InvalidOperationException("Referencias incompletas en nivel " + nivel.numero);
        var ids = new HashSet<string>();
        foreach (var raiz in SceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var recogible in raiz.GetComponentsInChildren<Recogible>(true))
                if (recogible.nivel != nivel || string.IsNullOrEmpty(recogible.identificador) || !ids.Add(recogible.identificador))
                    throw new InvalidOperationException("Recogible sin nivel o identificador unico.");
    }

    private static void GuardarEscena(Scene escena, string ruta)
    {
        ExigirAusente(ruta);
        if (!EditorSceneManager.SaveScene(escena, ruta))
            throw new InvalidOperationException("No se pudo guardar la escena: " + ruta);
    }
}
