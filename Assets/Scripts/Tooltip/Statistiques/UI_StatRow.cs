using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatRow : MonoBehaviour
{
    [SerializeField] private ENUM_Stats ENUM_Stats;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Image iconImage;
    [SerializeField] private UI_StatTooltip_Trigger tooltipTrigger;

    public void Setup(ENUM_Stats type, string value, Sprite icon = null)
    {
        ENUM_Stats = type;

        if (labelText != null)
            labelText.text = GetLabel(type);

        if (valueText != null)
            valueText.text = value;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (tooltipTrigger != null)
        {
            tooltipTrigger.SetENUM_Stats(type);
            tooltipTrigger.SetValueTextSource(valueText);
        }
    }

    public void SetValue(string value)
    {
        if (valueText != null)
            valueText.text = value;
    }

    private string GetLabel(ENUM_Stats type)
    {
        switch (type)
        {
            case ENUM_Stats.Curiosite: return "Curiosite";
            case ENUM_Stats.Intelligence: return "Intelligence";
            case ENUM_Stats.Dexterite: return "Dextérité";
            case ENUM_Stats.Endurance: return "Endurance";
            default: return "Stat";
        }
    }
}