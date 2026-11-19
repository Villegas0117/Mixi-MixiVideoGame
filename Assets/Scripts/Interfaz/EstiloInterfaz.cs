using UnityEngine;

// Solo presentacion: marcos de pixel, jerarquia y colores compartidos.
internal sealed class EstiloInterfaz
{
    internal static readonly Color Carbon = new Color32(28, 25, 39, 255);
    internal static readonly Color Panel = new Color32(43, 35, 49, 248);
    internal static readonly Color Crema = new Color32(255, 232, 198, 255);
    internal static readonly Color Tenue = new Color32(203, 178, 166, 255);
    internal static readonly Color Coral = new Color32(237, 119, 132, 255);
    internal static readonly Color Magenta = new Color32(173, 63, 104, 255);
    internal static readonly Color Ambar = new Color32(245, 183, 92, 255);
    internal GUIStyle Titulo, Subtitulo, Texto, Pequeno, Micro, Centro, Boton;

    internal EstiloInterfaz()
    {
        Titulo = Estilo(38, Crema, TextAnchor.MiddleLeft, true);
        Subtitulo = Estilo(23, Crema, TextAnchor.MiddleLeft, true);
        Texto = Estilo(20, Crema, TextAnchor.UpperLeft);
        Pequeno = Estilo(17, Tenue, TextAnchor.MiddleLeft);
        Micro = Estilo(14, Tenue, TextAnchor.MiddleLeft, true);
        Centro = Estilo(19, Crema, TextAnchor.MiddleCenter, true);
        Boton = Estilo(21, Crema, TextAnchor.MiddleCenter, true);
        Boton.normal.background = null;
        Boton.hover.background = null;
        Boton.active.background = null;
        Boton.focused.background = null;
        Boton.hover.textColor = Crema;
        Boton.active.textColor = Crema;
        Boton.focused.textColor = Crema;
    }

    private static GUIStyle Estilo(int tamano, Color color, TextAnchor alineacion, bool negrita = false)
    {
        var s = new GUIStyle(GUI.skin.label) { fontSize = tamano, alignment = alineacion,
            fontStyle = negrita ? FontStyle.Bold : FontStyle.Normal, wordWrap = true,
            padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };
        s.normal.textColor = color;
        return s;
    }

    internal static void ColorPlano(Rect r, Color color)
    {
        Color anterior = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = anterior;
    }

    internal static void Marco(Rect r, Color borde, Color fondo, bool sombra = true)
    {
        if (sombra) ColorPlano(new Rect(r.x + 5, r.y + 6, r.width, r.height), new Color(0, 0, 0, .35f));
        // Esquinas escalonadas, nunca bordes redondeados o degradados web.
        ColorPlano(new Rect(r.x + 4, r.y, r.width - 8, r.height), borde);
        ColorPlano(new Rect(r.x, r.y + 4, r.width, r.height - 8), borde);
        ColorPlano(new Rect(r.x + 3, r.y + 6, r.width - 6, r.height - 12), fondo);
        ColorPlano(new Rect(r.x + 6, r.y + 3, r.width - 12, r.height - 6), fondo);
        ColorPlano(new Rect(r.x + 9, r.y + 7, 22, 2), new Color(1, 1, 1, .12f));
    }

    internal bool Pulsador(Rect r, string etiqueta, bool principal = false)
    {
        bool sobre = r.Contains(Event.current.mousePosition) && GUI.enabled;
        Color fondo = !GUI.enabled ? (Color)new Color32(54, 47, 57, 255) :
            sobre ? Magenta : principal ? (Color)new Color32(134, 53, 81, 255) : Panel;
        Marco(r, !GUI.enabled ? (Color)new Color32(88, 75, 82, 255) : sobre ? Crema : principal ? Coral : Tenue, fondo);
        bool pulsado = GUI.Button(r, etiqueta, Boton);
        if (sobre) ColorPlano(new Rect(r.x + 10, r.center.y - 3, 6, 6), Ambar);
        return pulsado;
    }

    internal void Icono(Rect r, Texture2D imagen, string alternativa, Color acento)
    {
        if (imagen != null) GUI.DrawTexture(r, imagen, ScaleMode.ScaleToFit, true);
        else
        {
            Marco(r, acento, Carbon, false);
            GUI.Label(r, alternativa, Centro);
        }
    }

    internal static void Huella(float x, float y, float escala, Color color)
    {
        ColorPlano(new Rect(x + 4 * escala, y + 7 * escala, 8 * escala, 6 * escala), color);
        ColorPlano(new Rect(x + 2 * escala, y + 9 * escala, 12 * escala, 3 * escala), color);
        ColorPlano(new Rect(x, y + 3 * escala, 3 * escala, 4 * escala), color);
        ColorPlano(new Rect(x + 4 * escala, y, 3 * escala, 5 * escala), color);
        ColorPlano(new Rect(x + 9 * escala, y, 3 * escala, 5 * escala), color);
        ColorPlano(new Rect(x + 13 * escala, y + 3 * escala, 3 * escala, 4 * escala), color);
    }

    internal static void Barra(Rect r, float fraccion, Color color)
    {
        ColorPlano(r, Carbon);
        Rect relleno = new Rect(r.x + 2, r.y + 2, (r.width - 4) * Mathf.Clamp01(fraccion), r.height - 4);
        if (relleno.width > 0) ColorPlano(relleno, color);
        for (int i = 1; i < 10; i++)
            ColorPlano(new Rect(r.x + r.width * i / 10f, r.y + 2, 2, r.height - 4), new Color(0, 0, 0, .16f));
    }
}
