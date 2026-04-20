using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI_PERSONNAGE_EQUIPEMENT_Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject ghostInstance;
    private RectTransform ghostRectTransform;

    public SCOBJ_OBJET_EQUIPPABLE Objet { get; private set; }
    public bool VientEquipement { get; private set; }
    public ENUM_OBJET_EQUIPPABLE? TypeEquipementSource { get; private set; }

    public void Setup(
        SCOBJ_OBJET_EQUIPPABLE objet,
        Canvas rootCanvas,
        bool vientEquipement = false,
        ENUM_OBJET_EQUIPPABLE? typeEquipementSource = null)
    {
        Objet = objet;
        canvas = rootCanvas;
        VientEquipement = vientEquipement;
        TypeEquipementSource = typeEquipementSource;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Objet == null || canvas == null)
            return;

        EQUIPEMENT_DragContext.ObjetEnCours = Objet;
        EQUIPEMENT_DragContext.SourceUI = this;
        EQUIPEMENT_DragContext.VientEquipement = VientEquipement;
        EQUIPEMENT_DragContext.TypeEquipementSource = TypeEquipementSource;

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;

        CreateGhost();
        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ghostRectTransform == null || canvas == null)
            return;

        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetOriginalVisual();
        DestroyGhost();
        EQUIPEMENT_DragContext.Clear();
    }

    public void ForceStopDragVisual()
    {
        ResetOriginalVisual();
        DestroyGhost();
    }

    private void ResetOriginalVisual()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
    }

    private void CreateGhost()
    {
        if (ghostInstance != null || canvas == null)
            return;

        ghostInstance = new GameObject(
            $"{name}_Ghost",
            typeof(RectTransform),
            typeof(CanvasGroup),
            typeof(Image)
        );

        ghostInstance.transform.SetParent(canvas.transform, false);
        ghostInstance.transform.SetAsLastSibling();

        ghostRectTransform = ghostInstance.GetComponent<RectTransform>();

        CanvasGroup ghostCanvasGroup = ghostInstance.GetComponent<CanvasGroup>();
        ghostCanvasGroup.blocksRaycasts = false;
        ghostCanvasGroup.alpha = 0.85f;

        Image ghostImage = ghostInstance.GetComponent<Image>();
        ghostImage.raycastTarget = false;
        ghostImage.preserveAspect = true;
        ghostImage.sprite = ResolveBestSprite();
        ghostImage.color = Color.white;

        RectTransform sourceRect = transform as RectTransform;
        if (sourceRect != null)
        {
            ghostRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            ghostRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            ghostRectTransform.pivot = new Vector2(0.5f, 0.5f);
            ghostRectTransform.sizeDelta = sourceRect.rect.size;
        }
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect == null)
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            ghostRectTransform.anchoredPosition = localPoint;
        }
    }

    private void DestroyGhost()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
            ghostRectTransform = null;
        }
    }

    private Sprite ResolveBestSprite()
    {
        Image[] images = GetComponentsInChildren<Image>(true);

        foreach (Image img in images)
        {
            if (img == null || img.sprite == null)
                continue;

            string lower = img.name.ToLowerInvariant();
            if (lower.Contains("icon") || lower.Contains("icone") || lower.Contains("sprite"))
                return img.sprite;
        }

        foreach (Image img in images)
        {
            if (img != null && img.sprite != null)
                return img.sprite;
        }

        return Objet != null ? Objet.icone : null;
    }
}