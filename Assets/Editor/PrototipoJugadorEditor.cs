using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

// Herramienta explicita de editor: nunca se ejecuta al importar ni al entrar en Play.
public static class PrototipoJugadorEditor
{
    private const string Escena = "Assets/Escenas/PruebasJugador.unity";
    private const string Prefab = "Assets/Prefabs/Personajes/Chipotle.prefab";
    private const string Material = "Assets/Prefabs/Personajes/ChipotleSinFriccion.physicsMaterial2D";

    [MenuItem("Mixi-Mixi/Crear prototipo de movimiento (solo si no existe)")]
    public static void Crear()
    {
        if (File.Exists(Escena) || File.Exists(Prefab) || File.Exists(Material))
            throw new InvalidOperationException("El prototipo ya existe; no se sobrescribe.");
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var acciones = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Settings/InputSystem_Actions.inputactions");
        if (acciones == null) throw new InvalidOperationException("Falta el asset de entrada.");
        var material = new PhysicsMaterial2D("ChipotleSinFriccion") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(material, Material);
        var jugador = new GameObject("Chipotle");
        jugador.SetActive(false);
        var cuerpo = jugador.AddComponent<Rigidbody2D>();
        cuerpo.gravityScale = 3f;
        cuerpo.constraints = RigidbodyConstraints2D.FreezeRotation;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        var collider = jugador.AddComponent<CapsuleCollider2D>();
        collider.size = new Vector2(0.8f, 1.2f);
        collider.sharedMaterial = material;
        var entrada = jugador.AddComponent<EntradaJugador>();
        Asignar(entrada, "acciones", acciones);
        var movimiento = jugador.AddComponent<MovimientoJugador>();
        var visual = Rectangulo("Placeholder_NoArteFinal", Vector2.zero, new Vector2(0.8f, 1.2f), new Color(1f, 0.45f, 0.12f));
        visual.transform.SetParent(jugador.transform, false);
        var marca = Rectangulo("IndicadorFrente", new Vector2(0.3f, 0.2f), new Vector2(0.18f, 0.18f), Color.white);
        marca.GetComponent<SpriteRenderer>().sortingOrder = 1;
        marca.transform.SetParent(visual.transform, false);
        Asignar(movimiento, "visual", visual.transform);
        jugador.SetActive(true);
        var prefab = PrefabUtility.SaveAsPrefabAsset(jugador, Prefab);
        UnityEngine.Object.DestroyImmediate(jugador);
        jugador = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        jugador.transform.position = new Vector3(-6f, 1f, 0f);
        Plataforma("Suelo", new Vector2(0f, -1f), new Vector2(36f, 1f));
        Plataforma("PlataformaBaja", new Vector2(0f, 0.5f), new Vector2(3f, 0.5f));
        Plataforma("PlataformaAlta", new Vector2(5f, 1.6f), new Vector2(3f, 0.5f));
        Plataforma("TechoPrueba", new Vector2(-11f, 2.2f), new Vector2(4f, 0.5f));
        Plataforma("LimiteIzquierdo", new Vector2(-18f, 4f), new Vector2(1f, 10f));
        Plataforma("LimiteDerecho", new Vector2(18f, 4f), new Vector2(1f, 10f));
        var camara = new GameObject("CamaraPruebas");
        camara.tag = "MainCamera";
        var lente = camara.AddComponent<Camera>();
        lente.orthographic = true;
        lente.orthographicSize = 5f;
        lente.clearFlags = CameraClearFlags.SolidColor;
        lente.backgroundColor = new Color(0.08f, 0.12f, 0.2f);
        camara.AddComponent<UniversalAdditionalCameraData>();
        Asignar(camara.AddComponent<CamaraSeguimiento>(), "objetivo", jugador.transform);
        camara.transform.position = jugador.transform.position + new Vector3(0f, 1.5f, -10f);
        var luz = new GameObject("LuzGlobal2D").AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Global;
        luz.intensity = 1f;
        EditorSceneManager.SaveScene(escena, Escena);
        AssetDatabase.SaveAssets();
        Validar();
        Debug.Log("MIXI_CREACION_OK");
    }

    private static GameObject Rectangulo(string nombre, Vector2 posicion, Vector2 tamano, Color color)
    {
        var objeto = new GameObject(nombre);
        objeto.transform.position = posicion;
        var renderer = objeto.AddComponent<SpriteRenderer>();
        renderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        if (renderer.sprite == null) throw new InvalidOperationException("Sprite integrado no disponible.");
        renderer.drawMode = SpriteDrawMode.Sliced;
        renderer.size = tamano;
        renderer.color = color;
        return objeto;
    }

    private static void Plataforma(string nombre, Vector2 posicion, Vector2 tamano)
    {
        var objeto = Rectangulo(nombre, posicion, tamano, new Color(0.3f, 0.6f, 0.65f));
        objeto.AddComponent<BoxCollider2D>().size = tamano;
    }

    private static void Asignar(UnityEngine.Object destino, string campo, UnityEngine.Object valor)
    {
        var serializado = new SerializedObject(destino);
        serializado.FindProperty(campo).objectReferenceValue = valor;
        serializado.ApplyModifiedPropertiesWithoutUndo();
    }

    [MenuItem("Mixi-Mixi/Validar assets del prototipo")]
    public static void Validar()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Prefab);
        if (prefab == null || prefab.GetComponent<EntradaJugador>() == null ||
            prefab.GetComponent<MovimientoJugador>() == null || prefab.GetComponent<Rigidbody2D>() == null ||
            prefab.GetComponent<CapsuleCollider2D>() == null)
            throw new InvalidOperationException("Prefab incompleto.");
        foreach (string campo in new[] { "acciones" })
            if (new SerializedObject(prefab.GetComponent<EntradaJugador>()).FindProperty(campo).objectReferenceValue == null)
                throw new InvalidOperationException("Entrada sin referencia.");
        if (new SerializedObject(prefab.GetComponent<MovimientoJugador>()).FindProperty("visual").objectReferenceValue == null)
            throw new InvalidOperationException("Presentacion sin referencia.");
        var acciones = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Settings/InputSystem_Actions.inputactions");
        foreach (string nombre in new[] { "Move", "Jump", "Sprint" }) acciones.FindAction("Player/" + nombre, true);
        foreach (var renderer in prefab.GetComponentsInChildren<SpriteRenderer>())
            if (renderer.sprite == null) throw new InvalidOperationException("Placeholder sin sprite.");
        if (!File.Exists(Escena)) throw new InvalidOperationException("Escena ausente.");
        Debug.Log("MIXI_VALIDACION_ASSETS_OK (no equivale a Play Mode)");
    }
}
