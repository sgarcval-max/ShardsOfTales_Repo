public static class GameState
{
    public static bool tieneFlauta = false;
    public static bool tieneLlave = false;
    public static bool tienePartitura = false;
    public static bool ratonesCompletados = false;
    public static bool tienePapel = false;
    public static bool tieneBoligrafo = false;
    public static bool ninoSordoAyudado = false;
    public static bool ninaCiegaAyudada = false;
    public static bool ninoCojoAyudado = false;
    public static bool flautistaCompletado = false;
    public static bool finalFlautistaBueno = false;

    // La Bella y la Bestia
    public static bool dinDonHablado = false;
    public static bool velasCompletadas = false;
    public static bool cocinaCompletada = false;
    public static bool bestiaHablada = false;
    public static bool rosaSalvada = false;
    public static bool bellaBestiaCompletado = false;
    public static bool finalBellaBestiaBueno = false;

    // Dónde aparece el jugador al volver al castillo:
    // 0 = inicio, 1 = puerta del comedor, 2 = puerta de la cocina, 3 = puerta de la rosa
    public static int spawnCastillo = 0;
}