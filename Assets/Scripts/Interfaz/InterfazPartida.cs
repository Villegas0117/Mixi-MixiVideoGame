using System;
using UnityEngine;
public sealed class InterfazPartida : MonoBehaviour
{
    public GestorNivel nivel;
    public string Estado { get; private set; } = "Juego";
    private EntradaJugador entrada;
    private RecursosJugador recursos;
    private SistemaVida vida;
    private string titulo, texto;
    private Action alCerrar;
    private bool controles, confirmarNueva;
    private DatosPartida guardado;
    private string errorCarga;
    public Texture2D retratoChipotle, retratoPiyu, iconoVida, iconoComida, iconoMoneda;
    public Texture2D iconoRopa, iconoVelocidad, iconoFuerza, fondoMenu;
    public Texture2D escenaChipotle, escenaPiyuNormal, escenaPiyuAsustada, escenaPiyuRescatada, escenaIguana, escenaBandido;
    private EstiloInterfaz ui;
    private Vector2 desplazamientoTexto;
    private void Awake()
    {
        if (nivel==null) { Estado="Menu"; guardado=GestorGuardado.Leer(); errorCarga=GestorGuardado.Error; }
        else { entrada=nivel.jugador.GetComponent<EntradaJugador>(); recursos=nivel.jugador.GetComponent<RecursosJugador>(); vida=nivel.jugador.GetComponent<SistemaVida>(); }
    }
    private void Update()
    {
        if (entrada==null) return;
        if (entrada.PausaPulsada) {
            if (Estado=="Juego") Cambiar("Pausa");
            else if (Estado=="Pausa" || Estado=="Mochila") { controles=false; Cambiar("Juego"); }
        }
        if (entrada.MochilaPulsada && (Estado=="Juego" || Estado=="Mochila")) Cambiar(Estado=="Juego" ? "Mochila" : "Juego");
    }
    public void Cambiar(string estado)
    {
        Estado=estado;
        bool bloqueada=estado!="Juego";
        Time.timeScale=bloqueada ? 0 : 1;
        if (entrada!=null) { entrada.Bloqueada=bloqueada; entrada.LimpiarSalto(); }
    }
    public void Dialogo(string encabezado,string contenido,Action despues)
    { titulo=encabezado; texto=contenido; desplazamientoTexto=Vector2.zero; alCerrar=despues; Cambiar("Dialogo"); }
    public void CerrarDialogo()
    { var accion=alCerrar; alCerrar=null; Cambiar("Juego"); accion?.Invoke(); }
    public void MostrarDerrota() => Cambiar("Derrota");
    public void MostrarFinal() => Cambiar("Creditos");
    private void OnApplicationFocus(bool foco) { if (!foco && nivel!=null && Estado=="Juego") Cambiar("Pausa"); }
    // Presentacion solamente: la maquina de estados y todas las acciones originales se conservan.
    private void OnGUI()
    {
        Matrix4x4 matrizAnterior = GUI.matrix;
        Color colorAnterior = GUI.color;
        bool habilitadaAnterior = GUI.enabled;
        float escala = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
        Vector3 margen = new Vector3((Screen.width - 1280 * escala) * .5f, (Screen.height - 720 * escala) * .5f, 0);
        GUI.matrix = Matrix4x4.TRS(margen, Quaternion.identity, new Vector3(escala, escala, 1));
        GUI.color = Color.white;
        if (ui == null) ui = new EstiloInterfaz();
        try
        {
            if (Estado == "Menu") { FondoMenu(); Menu(); return; }
            if (nivel != null && recursos != null) DibujarHUD();
            if (Estado == "Juego") return;
            EstiloInterfaz.ColorPlano(new Rect(0, 0, 1280, 720), new Color(.06f, .04f, .09f, .7f));
            if (Estado == "Dialogo") DibujarDialogo();
            else if (Estado == "Pausa") Pausa();
            else if (Estado == "Mochila") Mochila();
            else if (Estado == "Derrota") Derrota();
            else if (Estado == "Creditos") Creditos();
        }
        finally { GUI.matrix = matrizAnterior; GUI.color = colorAnterior; GUI.enabled = habilitadaAnterior; }
    }

    private void Cabecera(string nombre, string detalle, Rect panel)
    {
        EstiloInterfaz.Marco(panel, EstiloInterfaz.Coral, EstiloInterfaz.Panel);
        EstiloInterfaz.Huella(panel.x + 30, panel.y + 30, 2, EstiloInterfaz.Coral);
        TextoAjustado(new Rect(panel.x + 82, panel.y + 21, panel.width - 112, 52), nombre, ui.Titulo);
        GUI.Label(new Rect(panel.x + 32, panel.y + 80, panel.width - 64, 42), detalle, ui.Pequeno);
        EstiloInterfaz.ColorPlano(new Rect(panel.x + 32, panel.y + 132, panel.width - 64, 2), EstiloInterfaz.Magenta);
    }

    private void FondoMenu()
    {
        EstiloInterfaz.ColorPlano(new Rect(0, 0, 1280, 720), EstiloInterfaz.Carbon);
        if (fondoMenu != null)
        {
            GUI.DrawTexture(new Rect(0, 0, 1280, 720), fondoMenu, ScaleMode.ScaleAndCrop, true);
            EstiloInterfaz.ColorPlano(new Rect(0, 0, 1280, 720), new Color(.09f, .06f, .14f, .48f));
        }
        else
        {
            // Silueta discreta de colonia; alternativa sin dependencia de recursos.
            for (int i = 0; i < 14; i++)
            {
                float h = 140 + (i * 67 % 170);
                EstiloInterfaz.ColorPlano(new Rect(i * 96, 540 - h, 92, h + 180), new Color32(53, 40, 57, 255));
                for (int j = 0; j < 3; j++)
                    EstiloInterfaz.ColorPlano(new Rect(i * 96 + 18, 555 - h + j * 46, 18, 24), new Color32(112, 72, 67, 255));
            }
        }
        EstiloInterfaz.ColorPlano(new Rect(0, 650, 1280, 70), new Color(.08f, .06f, .1f, .86f));
        GUI.Label(new Rect(48, 667, 700, 28), "PEQUEÑAS HUELLAS. UNA GRAN AVENTURA.", ui.Micro);
        GUI.Label(new Rect(1010, 667, 220, 28), "MIXI-MIXI  /  v0.1", ui.Micro);
    }

    private void Menu()
    {
        if (controles) { Ayuda(true); return; }
        if (confirmarNueva)
        {
            Cabecera("Una nueva aventura", "PROGRESO GUARDADO", new Rect(290, 155, 700, 410));
            GUI.Label(new Rect(330, 320, 620, 86), "Nueva partida reemplazará el progreso guardado al comenzar. ¿Continuar?", ui.Texto);
            if (ui.Pulsador(new Rect(330, 422, 620, 48), "Comenzar nueva partida", true)) SesionPartida.Nueva();
            if (ui.Pulsador(new Rect(330, 486, 620, 44), "Cancelar")) confirmarNueva = false;
            return;
        }
        EstiloInterfaz.Marco(new Rect(60, 60, 515, 585), EstiloInterfaz.Magenta, EstiloInterfaz.Panel);
        EstiloInterfaz.Huella(99, 91, 2, EstiloInterfaz.Coral);
        GUI.Label(new Rect(147, 85, 365, 30), "LA BÚSQUEDA DE PIYU", ui.Micro);
        var logo = new GUIStyle(ui.Titulo) { fontSize = 61 };
        GUI.Label(new Rect(96, 131, 460, 78), "MIXI-MIXI", logo);
        EstiloInterfaz.ColorPlano(new Rect(100, 211, 110, 5), EstiloInterfaz.Coral);
        GUI.Label(new Rect(100, 235, 410, 60), "Sigue las pistas. Encuentra a Piyu.\nVuelve a casa.", ui.Texto);
        GUI.Label(new Rect(100, 300, 430, 26), "DIFICULTAD", ui.Micro);
        var nombresDificultad = new[] { "Fácil", "Normal", "Difícil" };
        for (int i = 0; i < 3; i++)
        {
            bool activa = (int)Dificultad.Actual == i;
            if (ui.Pulsador(new Rect(100 + i * 145, 324, 138, 38), (activa ? "▸ " : "") + nombresDificultad[i])) Dificultad.Actual = (NivelDificultad)i;
        }
        if (ui.Pulsador(new Rect(100, 376, 430, 52), "Nueva partida", true))
        { if (guardado != null) confirmarNueva = true; else SesionPartida.Nueva(); }
        GUI.enabled = guardado != null;
        if (ui.Pulsador(new Rect(100, 442, 430, 50), "Continuar")) SesionPartida.Continuar();
        GUI.enabled = true;
        if (ui.Pulsador(new Rect(100, 506, 430, 46), "Controles")) controles = true;
        if (ui.Pulsador(new Rect(100, 566, 430, 46), "Salir"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        GUI.Label(new Rect(100, 620, 430, 32), guardado != null ? "Tu último checkpoint te espera." : "Comienza tu primera aventura.", ui.Pequeno);
        EstiloInterfaz.Marco(new Rect(651, 235, 272, 326), EstiloInterfaz.Coral, EstiloInterfaz.Panel);
        ui.Icono(new Rect(671, 253, 232, 240), retratoChipotle, "CHIPOTLE", EstiloInterfaz.Coral);
        GUI.Label(new Rect(671, 509, 232, 30), "CHIPOTLE", ui.Centro);
        EstiloInterfaz.Marco(new Rect(955, 308, 224, 253), EstiloInterfaz.Ambar, EstiloInterfaz.Panel);
        ui.Icono(new Rect(975, 328, 184, 174), retratoPiyu, "PIYU", EstiloInterfaz.Ambar);
        GUI.Label(new Rect(975, 517, 184, 28), "PIYU", ui.Centro);
        EstiloInterfaz.Huella(931, 198, 3, EstiloInterfaz.Coral);
        GUI.Label(new Rect(665, 575, 520, 35), "El barrio es grande. Su amistad, más.", ui.Centro);
        if (!string.IsNullOrEmpty(errorCarga))
        {
            EstiloInterfaz.Marco(new Rect(650, 70, 530, 138), EstiloInterfaz.Ambar, EstiloInterfaz.Panel);
            TextoAjustado(new Rect(672, 88, 486, 100), errorCarga + " Puedes iniciar Nueva partida.", ui.Texto);
        }
    }

    private void Pausa()
    {
        if (controles) { Ayuda(false); return; }
        Cabecera("Un respiro", "PAUSA  /  La aventura te espera.", new Rect(340, 110, 600, 500));
        if (ui.Pulsador(new Rect(380, 280, 520, 52), "Continuar", true)) Cambiar("Juego");
        if (ui.Pulsador(new Rect(380, 347, 520, 48), "Reiniciar desde checkpoint")) nivel.Reintentar();
        if (ui.Pulsador(new Rect(380, 410, 520, 48), "Controles")) controles = true;
        if (ui.Pulsador(new Rect(380, 473, 520, 48), "Volver al menú")) SesionPartida.Menu();
        GUI.Label(new Rect(380, 549, 520, 26), "ESC  /  Continuar", ui.Pequeno);
    }

    private void Ayuda(bool menu)
    {
        Cabecera("Cada paso cuenta", "CONTROLES  /  Las herramientas de Chipotle", new Rect(170, 65, 940, 590));
        string[] teclas = { "A / D", "ESPACIO", "SHIFT", "E", "J", "K", "L", "B / ESC" };
        string[] acciones = { "Moverse · también flechas", "Saltar", "Correr", "Recoger / interactuar", "Ataque de garra", "Bola de pelo · mantén y suelta para cargar", "Lanzar ropa equipada", "Mochila / pausa" };
        for (int i = 0; i < teclas.Length; i++)
        {
            float x = i < 4 ? 206 : 655;
            float y = 225 + (i % 4) * 58;
            EstiloInterfaz.Marco(new Rect(x, y, 98, 42), EstiloInterfaz.Magenta, EstiloInterfaz.Carbon, false);
            GUI.Label(new Rect(x + 4, y, 90, 42), teclas[i], ui.Micro);
            GUI.Label(new Rect(x + 110, y, 300, 46), acciones[i], ui.Pequeno);
        }
        GUI.Label(new Rect(206, 470, 864, 88), "La comida baja reduce velocidad y fuerza. La garra no consume comida.\nEnemigos rojos preparan un ataque: aléjate o salta.\nLos checkpoints restauran vida y guardan. E recoge también las monedas.", ui.Pequeno);
        if (ui.Pulsador(new Rect(760, 578, 310, 44), "Volver", true)) controles = false;
        GUI.Label(new Rect(206, 578, 500, 44), menu ? "LISTO PARA LA AVENTURA" : "EL JUEGO SIGUE EN PAUSA", ui.Micro);
    }

    private void Mochila()
    {
        Cabecera("La mochila de Chipotle", "El juego está en pausa. La ropa se equipa automáticamente.", new Rect(150, 75, 980, 570));
        TarjetaObjeto(190, iconoComida, "POLLO", recursos.Pollos, "+50 comida", "Comer pollo", recursos.UsarPollo, EstiloInterfaz.Ambar);
        TarjetaObjeto(500, iconoVelocidad, "VELOCIDAD", recursos.Velocidades, "20 segundos", "Usar velocidad", recursos.UsarVelocidad, EstiloInterfaz.Coral);
        TarjetaObjeto(810, iconoFuerza, "FUERZA", recursos.Fuerzas, "20 segundos", "Usar fuerza", recursos.UsarFuerza, EstiloInterfaz.Coral);
        ui.Icono(new Rect(190, 543, 48, 48), iconoRopa, "L", EstiloInterfaz.Ambar);
        GUI.Label(new Rect(253, 540, 470, 56), "Ropa arrojable: " + recursos.Arrojables + "  ·  L para lanzar", ui.Texto);
        if (ui.Pulsador(new Rect(810, 553, 270, 48), "Cerrar (B)", true)) Cambiar("Juego");
    }

    private void TarjetaObjeto(float x, Texture2D imagen, string nombre, int cantidad, string detalle, string accion, Action usar, Color acento)
    {
        EstiloInterfaz.Marco(new Rect(x, 242, 280, 272), acento, EstiloInterfaz.Carbon);
        ui.Icono(new Rect(x + 93, 260, 94, 92), imagen, nombre.Substring(0, 1), acento);
        GUI.Label(new Rect(x + 14, 359, 252, 30), nombre + "  ×" + cantidad, ui.Centro);
        GUI.Label(new Rect(x + 14, 394, 252, 28), detalle, ui.Centro);
        if (ui.Pulsador(new Rect(x + 16, 448, 248, 44), accion)) usar();
    }

    private void DibujarDialogo()
    {
        EstiloInterfaz.Marco(new Rect(350,95,580,195),EstiloInterfaz.Magenta,EstiloInterfaz.Carbon);
        if(escenaChipotle!=null) GUI.DrawTexture(new Rect(375,113,200,160),escenaChipotle,ScaleMode.ScaleToFit,true);
        bool secuestro=(titulo??"").Contains("llevaron");
        bool rescate=(titulo??"").Contains("libre");
        Texture2D companera=(titulo??"").ToLowerInvariant().Contains("iguana") ? escenaIguana : rescate ? escenaPiyuRescatada : secuestro ? escenaPiyuAsustada : escenaPiyuNormal;
        if(companera!=null) GUI.DrawTexture(new Rect(650,113,180,160),companera,ScaleMode.ScaleToFit,true);
        if(secuestro && escenaBandido!=null) GUI.DrawTexture(new Rect(810,99,90,176),escenaBandido,ScaleMode.ScaleToFit,true);
        Cabecera(titulo, "MIXI-MIXI  /  Una historia de pequeñas huellas", new Rect(160, 310, 960, 346));
        EstiloInterfaz.Huella(211, 476, 4, EstiloInterfaz.Magenta);
        // El contenido narrativo no se trunca: scroll solo si excede el espacio reservado.
        Rect area = new Rect(310, 461, 765, 100);
        float alto = Mathf.Max(area.height, ui.Texto.CalcHeight(new GUIContent(texto ?? ""), area.width - 24));
        desplazamientoTexto = GUI.BeginScrollView(area, desplazamientoTexto, new Rect(0, 0, area.width - 24, alto));
        GUI.Label(new Rect(0, 0, area.width - 24, alto), texto ?? "", ui.Texto);
        GUI.EndScrollView();
        if (ui.Pulsador(new Rect(820, 589, 260, 46), "Continuar", true)) CerrarDialogo();
    }

    private void Derrota()
    {
        Cabecera("Otro intento, otra huella", "CHIPOTLE NECESITA UN RESPIRO", new Rect(260, 110, 760, 490));
        ui.Icono(new Rect(300, 289, 132, 148), retratoChipotle, "CHIPOTLE", EstiloInterfaz.Coral);
        GUI.Label(new Rect(465, 292, 510, 126), "Reintentar restaura el último checkpoint. Los objetos posteriores vuelven con el estado guardado, sin duplicar inventario.", ui.Texto);
        if (ui.Pulsador(new Rect(300, 466, 680, 48), "Reintentar", true)) nivel.Reintentar();
        if (ui.Pulsador(new Rect(300, 533, 680, 44), "Volver al menú")) SesionPartida.Menu();
    }

    private void Creditos()
    {
        Cabecera("Piyu está a salvo", "MIXI-MIXI  /  Fin de la aventura", new Rect(200, 50, 880, 620));
        ui.Icono(new Rect(243, 232, 176, 203), retratoChipotle, "CHIPOTLE", EstiloInterfaz.Coral);
        ui.Icono(new Rect(251, 451, 160, 149), retratoPiyu, "PIYU", EstiloInterfaz.Ambar);
        GUI.Label(new Rect(460, 231, 568, 311), "Proyecto académico de Eber Villegas\n\nDiseño y referencias: material del proyecto Mixi-Mixi\nImplementación v0.1 asistida con OpenClaw\nMotor: Unity 6000.3.23f1\n\nGracias por acompañar a Chipotle.\nArte derivado y sprites asistidos por IA, basados en el concept art. Audio final pendiente.", ui.Texto);
        if (ui.Pulsador(new Rect(460, 584, 568, 48), "Volver al menú", true)) SesionPartida.Menu();
    }

    private void DibujarHUD()
    {
        EstiloInterfaz.Marco(new Rect(20, 20, 420, 128), EstiloInterfaz.Magenta, EstiloInterfaz.Panel);
        ui.Icono(new Rect(31, 31, 84, 104), retratoChipotle, "C", EstiloInterfaz.Coral);
        GUI.Label(new Rect(130, 27, 285, 22), "CHIPOTLE", ui.Micro);
        Medidor(130, 53, "VIDA", vida != null ? vida.Actual : 0, vida != null ? vida.maxima : 100, iconoVida, EstiloInterfaz.Coral);
        Medidor(130, 98, "COMIDA", recursos.Comida, 100, iconoComida, EstiloInterfaz.Ambar);
        EstiloInterfaz.Marco(new Rect(458, 20, 236, 65), EstiloInterfaz.Ambar, EstiloInterfaz.Panel);
        ui.Icono(new Rect(472, 34, 32, 32), iconoMoneda, "$", EstiloInterfaz.Ambar);
        GUI.Label(new Rect(512, 32, 78, 35), recursos.Monedas.ToString(), ui.Subtitulo);
        ui.Icono(new Rect(590, 34, 32, 32), iconoRopa, "L", EstiloInterfaz.Coral);
        GUI.Label(new Rect(632, 32, 50, 35), recursos.Arrojables.ToString(), ui.Subtitulo);
        if (!string.IsNullOrEmpty(recursos.Efectos))
        {
            EstiloInterfaz.Marco(new Rect(458, 97, 260, 51), EstiloInterfaz.Magenta, EstiloInterfaz.Panel);
            Texture2D efecto = recursos.Efectos.Contains("Velocidad") ? iconoVelocidad : recursos.Efectos.Contains("Fuerza") ? iconoFuerza : iconoComida;
            ui.Icono(new Rect(470, 107, 30, 30), efecto, "!", EstiloInterfaz.Ambar);
            TextoAjustado(new Rect(510, 105, 194, 35), recursos.Efectos, ui.Pequeno);
        }
        EstiloInterfaz.Marco(new Rect(858, 20, 402, 116), EstiloInterfaz.Ambar, EstiloInterfaz.Panel);
        GUI.Label(new Rect(878, 32, 365, 24), "NIVEL " + nivel.numero + "  /  " + Dificultad.Nombre.ToUpper(), ui.Micro);
        TextoAjustado(new Rect(878, 63, 360, 57), nivel.Objetivo, ui.Texto);
        // Solo mensajes relevantes: fuera la lista de depuracion permanente.
        var interaccion = nivel.jugador.GetComponent<InteraccionJugador>();
        var cercano = interaccion != null ? interaccion.Cercano : null;
        if (cercano != null && Estado == "Juego")
        {
            EstiloInterfaz.Marco(new Rect(380, 564, 520, 62), EstiloInterfaz.Coral, EstiloInterfaz.Panel);
            EstiloInterfaz.Marco(new Rect(394, 575, 40, 40), EstiloInterfaz.Crema, EstiloInterfaz.Magenta, false);
            GUI.Label(new Rect(394, 575, 40, 40), "E", ui.Centro);
            TextoAjustado(new Rect(450, 577, 431, 37), cercano.etiqueta, ui.Texto);
        }
        string mensaje = !string.IsNullOrEmpty(nivel.ErrorGuardado) ? nivel.ErrorGuardado : nivel.Mensaje;
        if (!string.IsNullOrEmpty(mensaje))
        {
            EstiloInterfaz.Marco(new Rect(224, 637, 832, 61), EstiloInterfaz.Ambar, EstiloInterfaz.Panel);
            TextoAjustado(new Rect(244, 646, 792, 43), mensaje, ui.Texto);
        }
        else
        {
            EstiloInterfaz.Marco(new Rect(1000, 660, 260, 39), EstiloInterfaz.Magenta, EstiloInterfaz.Panel, false);
            GUI.Label(new Rect(1014, 666, 233, 24), "B  Mochila   ·   ESC  Pausa", ui.Micro);
        }
        var combate = nivel.jugador.GetComponent<CombateJugador>();
        if (combate != null && combate.Carga01 > 0 && Estado == "Juego")
        {
            EstiloInterfaz.Marco(new Rect(475, 493, 330, 58), EstiloInterfaz.Coral, EstiloInterfaz.Panel);
            GUI.Label(new Rect(492, 500, 296, 24), "K  ·  CARGA " + Mathf.RoundToInt(combate.Carga01 * 100) + "%", ui.Micro);
            EstiloInterfaz.Barra(new Rect(492, 529, 296, 10), combate.Carga01, EstiloInterfaz.Coral);
        }
        if (nivel.jefe != null && nivel.jefe.gameObject.activeSelf && Vector2.Distance(nivel.jugador.transform.position, nivel.jefe.transform.position) < 15)
        {
            var j = nivel.jefe.GetComponent<SistemaVida>();
            if (j == null || j.Muerto) return;
            EstiloInterfaz.Marco(new Rect(360, 166, 560, 98), EstiloInterfaz.Coral, EstiloInterfaz.Panel);
            TextoAjustado(new Rect(380, 174, 380, 27), nivel.NombreJefe, ui.Subtitulo);
            GUI.Label(new Rect(776, 177, 125, 24), Mathf.CeilToInt(j.Actual) + " / " + Mathf.CeilToInt(j.maxima), ui.Micro);
            EstiloInterfaz.Barra(new Rect(380, 208, 520, 14), j.Actual / Mathf.Max(1, j.maxima), EstiloInterfaz.Coral);
            TextoAjustado(new Rect(380, 230, 520, 25), nivel.jefe.Aviso, ui.Pequeno);
        }
    }

    private void Medidor(float x, float y, string nombre, float valor, float maximo, Texture2D icono, Color color)
    {
        ui.Icono(new Rect(x, y + 2, 25, 25), icono, nombre == "VIDA" ? "+" : "P", color);
        GUI.Label(new Rect(x + 35, y - 5, 115, 23), nombre, ui.Micro);
        GUI.Label(new Rect(x + 162, y - 5, 114, 23), Mathf.CeilToInt(valor) + " / " + Mathf.CeilToInt(maximo), ui.Micro);
        EstiloInterfaz.Barra(new Rect(x + 35, y + 21, 248, 13), valor / Mathf.Max(1, maximo), color);
    }

    private static void TextoAjustado(Rect r, string contenido, GUIStyle baseEstilo)
    {
        var estilo = new GUIStyle(baseEstilo);
        var contenidoGUI = new GUIContent(contenido ?? "");
        while (estilo.fontSize > 12 && estilo.CalcHeight(contenidoGUI, r.width) > r.height) estilo.fontSize--;
        GUI.Label(r, contenidoGUI, estilo);
    }
    private void OnDestroy() { Time.timeScale=1; if(entrada!=null) entrada.Bloqueada=false; }
}
