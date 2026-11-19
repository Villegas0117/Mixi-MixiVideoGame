using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Dos fuentes permiten transiciones sin cortar la musica anterior.
public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }
    public AudioClip musicaNivel1, musicaNivel2, musicaNivel3;
    public AudioClip jefeNivel1, jefeNivel2, jefeNivel3;
    public AudioClip victoria;
    [Range(0f, 1f)] public float volumen = .35f;
    [Min(.05f)] public float tiempoCrossfade = 1.2f;

    private AudioSource fuenteA, fuenteB, actual;
    private Coroutine transicion;
    private AudioClip solicitado;
    private bool pausado, aplicacionPausada;
    private int nivelActual;

    public static AudioManager ObtenerOCrear()
    {
        if (Instancia != null) return Instancia;
        var existente = FindFirstObjectByType<AudioManager>();
        if (existente != null) return existente;
        return new GameObject("Musica").AddComponent<AudioManager>();
    }

    private void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
        musicaNivel1 = musicaNivel1 != null ? musicaNivel1 : Cargar(1, "exploracion");
        musicaNivel2 = musicaNivel2 != null ? musicaNivel2 : Cargar(2, "exploracion");
        musicaNivel3 = musicaNivel3 != null ? musicaNivel3 : Cargar(3, "exploracion");
        jefeNivel1 = jefeNivel1 != null ? jefeNivel1 : Cargar(1, "boss");
        jefeNivel2 = jefeNivel2 != null ? jefeNivel2 : Cargar(2, "boss");
        jefeNivel3 = jefeNivel3 != null ? jefeNivel3 : Cargar(3, "boss");
        fuenteA = gameObject.AddComponent<AudioSource>();
        fuenteB = gameObject.AddComponent<AudioSource>();
        foreach (var fuente in new[] { fuenteA, fuenteB })
        {
            fuente.loop = true; fuente.playOnAwake = false;
            fuente.spatialBlend = 0f; fuente.volume = 0f;
        }
        actual = fuenteA;
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private static AudioClip Cargar(int nivel, string tipo)
    {
        var clip = Resources.Load<AudioClip>("Music/Nivel" + nivel + "/" + tipo);
        if (clip == null) Debug.LogError("Falta musica: Nivel" + nivel + "/" + tipo);
        return clip;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        // El menu y otras escenas sin nivel no deben heredar la pista previa.
        if (System.Array.IndexOf(SesionPartida.Escenas, escena.name) <= 0) DetenerTodo();
    }

    public void ReproducirExploracion(int nivel)
    {
        nivelActual = nivel;
        Reproducir(nivel == 1 ? musicaNivel1 : nivel == 2 ? musicaNivel2 : musicaNivel3);
    }

    public void ReproducirJefe(int nivel)
    {
        nivelActual = nivel;
        Reproducir(nivel == 1 ? jefeNivel1 : nivel == 2 ? jefeNivel2 : jefeNivel3);
    }

    public void ReproducirVictoria()
    {
        // No hay una pista de victoria en el proyecto: regresar a exploracion.
        if (victoria == null) ReproducirExploracion(nivelActual);
        else Reproducir(victoria, false);
    }

    private void Reproducir(AudioClip clip, bool bucle = true)
    {
        if (clip == null || solicitado == clip) return;
        solicitado = clip;
        if (transicion != null)
        {
            StopCoroutine(transicion);
            // Conservar la fuente mas audible al interrumpir una transicion.
            actual = fuenteA.volume >= fuenteB.volume ? fuenteA : fuenteB;
            var otra = actual == fuenteA ? fuenteB : fuenteA;
            otra.Stop(); otra.volume = 0f;
        }
        if (actual.clip == clip && actual.isPlaying) { transicion = null; return; }
        transicion = StartCoroutine(Crossfade(clip, bucle));
    }

    private IEnumerator Crossfade(AudioClip clip, bool bucle)
    {
        var salida = actual;
        var entrada = actual == fuenteA ? fuenteB : fuenteA;
        entrada.Stop(); entrada.clip = clip; entrada.loop = bucle;
        entrada.volume = 0f; entrada.Play();
        if (pausado) entrada.Pause();
        float volumenInicial = salida.volume;
        float duracion = Mathf.Max(.05f, tiempoCrossfade), tiempo = 0f;
        while (tiempo < duracion)
        {
            if (!pausado) tiempo += Time.unscaledDeltaTime;
            float proporcion = Mathf.Clamp01(tiempo / duracion);
            salida.volume = volumenInicial * (1f - proporcion);
            entrada.volume = volumen * proporcion;
            yield return null;
        }
        salida.Stop(); salida.volume = 0f;
        entrada.volume = volumen; actual = entrada; transicion = null;
    }

    private void Update() => SincronizarPausa();
    private void SincronizarPausa()
    {
        bool nueva = aplicacionPausada || Time.timeScale <= 0f;
        if (nueva == pausado) return;
        pausado = nueva;
        if (pausado) { fuenteA.Pause(); fuenteB.Pause(); }
        else { fuenteA.UnPause(); fuenteB.UnPause(); }
    }
    private void OnApplicationPause(bool pausa) { aplicacionPausada = pausa; SincronizarPausa(); }
    public void DetenerTodo()
    {
        if (transicion != null) { StopCoroutine(transicion); transicion = null; }
        fuenteA.Stop(); fuenteB.Stop(); fuenteA.volume = fuenteB.volume = 0f;
        solicitado = null;
    }
    private void OnDestroy()
    {
        if (Instancia != this) return;
        SceneManager.sceneLoaded -= AlCargarEscena;
        Instancia = null;
    }
}
