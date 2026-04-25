using TMPro;
using UnityEngine;

public class UI_PERSONNAGE_STATS : MonoBehaviour
{
    [Header("UI - Stats")]
    [SerializeField] private TMP_Text curiositeText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text dexteriteText;
    [SerializeField] private TMP_Text enduranceText;

    [Header("Tooltips")]
    [SerializeField] private UI_StatTooltip_Trigger curiositeTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger intelligenceTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger dexteriteTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger enduranceTooltipTrigger;

    public void Refresh(DATA_PERSONNAGE_Detail data)
    {
        if (data == null)
            return;

        RefreshStats(data);
        RefreshTooltips(data);
    }

    private void RefreshStats(DATA_PERSONNAGE_Detail data)
    {
        UTIL_UiStatTextHelper.SetStatValue(curiositeText, data.curiosite, data.curiositeDelta);
        UTIL_UiStatTextHelper.SetStatValue(intelligenceText, data.intelligence, data.intelligenceDelta);
        UTIL_UiStatTextHelper.SetStatValue(dexteriteText, data.dexterite, data.dexteriteDelta);
        UTIL_UiStatTextHelper.SetStatValue(enduranceText, data.endurance, data.enduranceDelta);
    }

    private void RefreshTooltips(DATA_PERSONNAGE_Detail data)
    {
        UTIL_UiTooltipHelper.SetTooltip(curiositeTooltipTrigger, data.curiositeTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(intelligenceTooltipTrigger, data.intelligenceTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(dexteriteTooltipTrigger, data.dexteriteTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(enduranceTooltipTrigger, data.enduranceTooltipDetail);
    }
}