using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_StatTooltip : MonoBehaviour
{
    public static UI_StatTooltip Instance { get; private set; }

    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("Texts")]
    [SerializeField] private TMP_Text titreText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valeurText;

    [Header("Layout")]
    [SerializeField] private RectTransform tooltipRect;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private Vector2 mouseOffset = new Vector2(20f, -20f);
    [SerializeField] private Vector2 paddingToScreen = new Vector2(16f, 16f);

    [Header("Mode")]
    [SerializeField] private bool suivreSouris = true;

    private RectTransform canvasRect;
    private Camera canvasCamera;
    private bool visible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (tooltipRect == null)
            tooltipRect = transform as RectTransform;

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.transform as RectTransform;
            canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera;
        }

        Hide();
    }

private void Update()
{
    if (!visible || !suivreSouris)
        return;

    if (Mouse.current == null)
        return;

    RepositionToMouse();
}

    public void Show(string titre, string description, string valeur = null)
    {
        if (titreText != null)
            titreText.text = titre;

        if (descriptionText != null)
            descriptionText.text = description;

        if (valeurText != null)
        {
            bool hasValeur = !string.IsNullOrWhiteSpace(valeur);
            valeurText.gameObject.SetActive(hasValeur);
            if (hasValeur)
                valeurText.text = valeur;
        }

        if (root != null)
            root.SetActive(true);

        Canvas.ForceUpdateCanvases();

        visible = true;

        if (suivreSouris)
            RepositionToMouse();
    }

    public void ShowAtPosition(string titre, string description, Vector2 screenPosition, string valeur = null)
{
    suivreSouris = false;
    transform.SetAsLastSibling();

    if (titreText != null)
        titreText.text = titre;

    if (descriptionText != null)
        descriptionText.text = description;

    if (valeurText != null)
    {
        bool hasValeur = !string.IsNullOrWhiteSpace(valeur);
        valeurText.gameObject.SetActive(hasValeur);
        if (hasValeur)
            valeurText.text = valeur;
    }

    if (root != null)
        root.SetActive(true);

    Canvas.ForceUpdateCanvases();
    visible = true;

    RepositionToScreenPoint(screenPosition);
}
    public void Hide()
    {
        visible = false;

        if (root != null)
            root.SetActive(false);
    }

private void RepositionToMouse()
{
    if (Mouse.current == null)
        return;

    Vector2 targetScreenPos = Mouse.current.position.ReadValue() + mouseOffset;
    RepositionToScreenPoint(targetScreenPos);
}

    private void RepositionToScreenPoint(Vector2 screenPoint)
    {
        if (tooltipRect == null || canvasRect == null)
            return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPoint,
                canvasCamera,
                out localPoint))
        {
            return;
        }

        tooltipRect.anchoredPosition = ClampToCanvas(localPoint);
    }

    private Vector2 ClampToCanvas(Vector2 desiredAnchoredPos)
    {
        if (tooltipRect == null || canvasRect == null)
            return desiredAnchoredPos;

        Vector2 size = tooltipRect.rect.size;
        Vector2 canvasSize = canvasRect.rect.size;

        float minX = -canvasSize.x * 0.5f + size.x * tooltipRect.pivot.x + paddingToScreen.x;
        float maxX = canvasSize.x * 0.5f - size.x * (1f - tooltipRect.pivot.x) - paddingToScreen.x;

        float minY = -canvasSize.y * 0.5f + size.y * tooltipRect.pivot.y + paddingToScreen.y;
        float maxY = canvasSize.y * 0.5f - size.y * (1f - tooltipRect.pivot.y) - paddingToScreen.y;

        float clampedX = Mathf.Clamp(desiredAnchoredPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredAnchoredPos.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }
}