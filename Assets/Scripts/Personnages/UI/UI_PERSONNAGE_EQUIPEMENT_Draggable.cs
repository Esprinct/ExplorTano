using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UI_PERSONNAGE_EQUIPEMENT_Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Références")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private ScrollRect parentScrollRect;

    [Header("Options")]
    [SerializeField] private bool dragActif = true;
    [SerializeField] private bool autoriserScrollParent = true;

    [Tooltip("Si le mouvement vertical est plus fort que l'horizontal, on scrolle au lieu de drag.")]
    [SerializeField] private float ratioVerticalPourScroll = 1.15f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private GameObject ghostInstance;
    private RectTransform ghostRectTransform;

    private bool scrollEnCours;
    private bool dragEnCours;

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

        SetDragActif(objet != null);
    }

    public void Clear()
    {
        Objet = null;
        VientEquipement = false;
        TypeEquipementSource = null;

        scrollEnCours = false;
        dragEnCours = false;

        ResetOriginalVisual();
        DestroyGhost();

        if (EQUIPEMENT_DragContext.SourceUI == this)
            EQUIPEMENT_DragContext.Clear();
    }

    public void SetDragActif(bool actif)
    {
        dragActif = actif;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        ResetOriginalVisual();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (parentScrollRect == null)
            parentScrollRect = GetComponentInParent<ScrollRect>();

        ResetOriginalVisual();
    }

    private void OnDisable()
    {
        ResetOriginalVisual();
        DestroyGhost();

        scrollEnCours = false;
        dragEnCours = false;

        if (EQUIPEMENT_DragContext.SourceUI == this)
            EQUIPEMENT_DragContext.Clear();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollEnCours = false;
        dragEnCours = false;

        bool doitScroller =
            autoriserScrollParent &&
            parentScrollRect != null &&
            EstGesteDeScroll(eventData);

        if (doitScroller || !dragActif || Objet == null || canvas == null)
        {
            DemarrerScrollParent(eventData);
            return;
        }

        dragEnCours = true;

        EQUIPEMENT_DragContext.ObjetEnCours = Objet;
        EQUIPEMENT_DragContext.SourceUI = this;
        EQUIPEMENT_DragContext.VientEquipement = VientEquipement;
        EQUIPEMENT_DragContext.TypeEquipementSource = TypeEquipementSource;

        if (canvasGroup != null)
        {
            canvasGroup.ignoreParentGroups = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.5f;
        }

        CreateGhost();
        UpdateGhostPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (scrollEnCours)
        {
            parentScrollRect?.OnDrag(eventData);
            return;
        }

        if (!dragEnCours || ghostRectTransform == null || canvas == null)
            return;

        UpdateGhostPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (scrollEnCours)
        {
            parentScrollRect?.OnEndDrag(eventData);
        }

        ResetOriginalVisual();
        DestroyGhost();

        if (dragEnCours && EQUIPEMENT_DragContext.SourceUI == this)
            EQUIPEMENT_DragContext.Clear();

        scrollEnCours = false;
        dragEnCours = false;
    }

    public void CuriositeStopDragVisual()
    {
        ResetOriginalVisual();
        DestroyGhost();

        scrollEnCours = false;
        dragEnCours = false;
    }

    private bool EstGesteDeScroll(PointerEventData eventData)
    {
        if (eventData == null)
            return false;

        Vector2 delta = eventData.delta;

        if (delta.sqrMagnitude <= 0.01f)
            return false;

        float vertical = Mathf.Abs(delta.y);
        float horizontal = Mathf.Abs(delta.x);

        return vertical > horizontal * ratioVerticalPourScroll;
    }

    private void DemarrerScrollParent(PointerEventData eventData)
    {
        if (parentScrollRect == null)
            return;

        scrollEnCours = true;
        dragEnCours = false;

        ResetOriginalVisual();
        parentScrollRect.OnBeginDrag(eventData);
    }

    private void ResetOriginalVisual()
    {
        if (canvasGroup != null)
        {
            canvasGroup.ignoreParentGroups = false;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
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
        ghostCanvasGroup.interactable = false;
        ghostCanvasGroup.ignoreParentGroups = true;
        ghostCanvasGroup.alpha = 0.85f;

        Image ghostImage = ghostInstance.GetComponent<Image>();
        ghostImage.raycastTarget = false;
        ghostImage.maskable = false;
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
        if (ghostRectTransform == null || canvas == null)
            return;

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