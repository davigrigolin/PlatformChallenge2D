using UnityEngine;

/// <summary>Instruções e estado do desafio exibidos na tela do jogo.</summary>
public class PlatformGameHUD : MonoBehaviour
{
    private TimedMovingPlatform platform;
    private Texture2D panelTexture;
    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle statusStyle;

    private void Start()
    {
        // A busca pelo componente pode acontecer aqui; estilos GUI não.
        // GUI.skin só deve ser acessado durante OnGUI.
        platform = FindFirstObjectByType<TimedMovingPlatform>();
    }

    private void OnGUI()
    {
        EnsureGuiStyles();

        const float panelWidth = 370f;
        const float panelHeight = 154f;
        GUI.Box(new Rect(18f, 18f, panelWidth, panelHeight), GUIContent.none, panelStyle);
        GUI.Label(new Rect(34f, 27f, panelWidth - 30f, 30f), "DESAFIO: PLATAFORMA MÓVEL", titleStyle);
        GUI.Label(new Rect(34f, 61f, panelWidth - 30f, 22f), "A/D ou ← →  andar     •     Espaço / W / ↑  pular", bodyStyle);

        string status = "Encontre a plataforma amarela e toque nela!";
        if (platform != null && platform.HasBeenActivated)
        {
            if (platform.IsMoving)
                status = "PLATAFORMA EM MOVIMENTO — " + platform.RemainingTime.ToString("0.0") + " s";
            else if (platform.HasStartedFalling)
                status = "TEMPO ESGOTADO — gravidade ativada!";
        }

        GUI.Label(new Rect(34f, 92f, panelWidth - 30f, 48f), status, statusStyle);
    }

    /// <summary>
    /// Cria os estilos apenas dentro de OnGUI, quando GUI.skin já está disponível.
    /// Isso evita passar um GUIStyle nulo para GUI.Box/GUI.Label.
    /// </summary>
    private void EnsureGuiStyles()
    {
        if (panelStyle != null && titleStyle != null && bodyStyle != null && statusStyle != null)
            return;

        GUISkin skin = GUI.skin;
        // Fallback defensivo: GUI.skin normalmente existe dentro de OnGUI,
        // mas o HUD nunca deve chamar GUI.Box/GUI.Label com estilo nulo.
        GUIStyle baseBoxStyle = (skin != null && skin.box != null) ? skin.box : new GUIStyle();
        GUIStyle baseLabelStyle = (skin != null && skin.label != null) ? skin.label : new GUIStyle();

        if (panelTexture == null)
        {
            panelTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            panelTexture.SetPixel(0, 0, new Color(0.035f, 0.055f, 0.09f, 0.94f));
            panelTexture.Apply();
        }

        panelStyle = new GUIStyle(baseBoxStyle);
        panelStyle.normal.background = panelTexture;
        panelStyle.border = new RectOffset(10, 10, 10, 10);
        panelStyle.padding = new RectOffset(16, 16, 12, 12);

        titleStyle = new GUIStyle(baseLabelStyle);
        titleStyle.fontSize = 20;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = new Color(0.95f, 0.97f, 1f);

        bodyStyle = new GUIStyle(baseLabelStyle);
        bodyStyle.fontSize = 13;
        bodyStyle.normal.textColor = new Color(0.72f, 0.79f, 0.88f);

        statusStyle = new GUIStyle(baseLabelStyle);
        statusStyle.fontSize = 14;
        statusStyle.fontStyle = FontStyle.Bold;
        statusStyle.wordWrap = true;
        statusStyle.normal.textColor = new Color(1f, 0.76f, 0.25f);
    }

    private void OnDestroy()
    {
        if (panelTexture != null)
        {
            Destroy(panelTexture);
            panelTexture = null;
        }
    }
}
