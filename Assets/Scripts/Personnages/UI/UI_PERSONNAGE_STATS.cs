using TMPro;
using UnityEngine;

public class UI_PERSONNAGE_STATS : MonoBehaviour
{
    [Header("UI - Stats")]
    [SerializeField] private TMP_Text curiositeText;
    [SerializeField] private TMP_Text ingeniositeText;
    [SerializeField] private TMP_Text combativiteText;
    [SerializeField] private TMP_Text enduranceText;

    [Header("Tooltips")]
    [SerializeField] private UI_StatTooltip_Trigger curiositeTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger ingeniositeTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger combativiteTooltipTrigger;
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
        UTIL_UiStatTextHelper.SetStatValue(ingeniositeText, data.ingeniosite, data.ingeniositeDelta);
        UTIL_UiStatTextHelper.SetStatValue(combativiteText, data.combativite, data.combativiteDelta);
        UTIL_UiStatTextHelper.SetStatValue(enduranceText, data.endurance, data.enduranceDelta);
    }

    private void RefreshTooltips(DATA_PERSONNAGE_Detail data)
    {
        UTIL_UiTooltipHelper.SetTooltip(curiositeTooltipTrigger, data.curiositeTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(ingeniositeTooltipTrigger, data.ingeniositeTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(combativiteTooltipTrigger, data.combativiteTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(enduranceTooltipTrigger, data.enduranceTooltipDetail);
    }
}