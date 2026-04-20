using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StatTooltip_Trigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("Stat")]
    [SerializeField] private ENUM_Stats ENUM_Stats;
    [SerializeField] private string valeurOverride;

    [Header("Overrides")]
    [TextArea]
    [SerializeField] private string descriptionOverride;

    [SerializeField] private string valeurPrefix = "";
    [SerializeField] private string valeurSuffix = "";

    [Header("Optional Dynamic Value")]
    [SerializeField] private TMP_Text valeurTextSource;
    [SerializeField] private Image iconSource;

    [Header("Selection")]
    [SerializeField] private bool afficherAuSelect = true;

    [Header("Position Tooltip")]
    [SerializeField] private float offsetVertical = 8f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltipUnderRow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!afficherAuSelect)
            return;

        ShowTooltipUnderRow();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltipUnderRow()
    {
        if (UI_StatTooltip.Instance == null)
            return;

        string titre = DATA_StatTooltip.GetTitre(ENUM_Stats);
        string description = string.IsNullOrWhiteSpace(descriptionOverride)
            ? DATA_StatTooltip.GetDescription(ENUM_Stats)
            : descriptionOverride;

        string valeur = BuildValeur();

        RectTransform rowRect = transform as RectTransform;
        if (rowRect == null)
        {
            UI_StatTooltip.Instance.Show(titre, description, valeur);
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            UI_StatTooltip.Instance.Show(titre, description, valeur);
            return;
        }

        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        rowRect.GetWorldCorners(corners);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        screenPoint += new Vector2(0f, -offsetVertical);

        UI_StatTooltip.Instance.ShowAtPosition(titre, description, screenPoint, valeur);
    }

    private void HideTooltip()
    {
        if (UI_StatTooltip.Instance == null)
            return;

        UI_StatTooltip.Instance.Hide();
    }

    private string BuildValeur()
    {
        if (!string.IsNullOrWhiteSpace(valeurOverride))
            return valeurOverride;

        if (valeurTextSource == null)
            return null;

        string raw = valeurTextSource.text;
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        return $"{valeurPrefix}{raw}{valeurSuffix}";
    }

    public void SetValeurOverride(string value)
    {
        valeurOverride = value;
    }

    // Alias de compatibilité
    public void SetText(string value)
    {
        SetValeurOverride(value);
    }

    public void ClearValeurOverride()
    {
        valeurOverride = null;
    }

    public void SetDescriptionOverride(string value)
    {
        descriptionOverride = value;
    }

    public void SetENUM_Stats(ENUM_Stats type)
    {
        ENUM_Stats = type;
    }

    public void SetValueTextSource(TMP_Text source)
    {
        valeurTextSource = source;
    }
}