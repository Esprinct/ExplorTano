using TMPro;
using UnityEngine;

public class UI_PERSONNAGE_EQUIPPEMENT_Stats : MonoBehaviour
{
    [Header("Row Stats")]
    [SerializeField] private TMP_Text forceText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text dexteriteText;
    [SerializeField] private TMP_Text enduranceText;

    [Header("Tooltips")]
    [SerializeField] private UI_StatTooltip_Trigger forceTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger intelligenceTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger dexteriteTooltipTrigger;
    [SerializeField] private UI_StatTooltip_Trigger enduranceTooltipTrigger;

    public void Refresh(DATA_PERSONNAGE_Detail data)
    {
        if (data == null)
            return;

        Debug.Log($"RowStatsEquipement Refresh -> F:{data.force} I:{data.intelligence} D:{data.dexterite} E:{data.endurance}");

        RefreshTexts(data);
        RefreshTooltips(data);
    }

    private void RefreshTexts(DATA_PERSONNAGE_Detail data)
    {
      UTIL_UiStatTextHelper.SetStatValue(forceText, data.force, data.forceDelta);
        UTIL_UiStatTextHelper.SetStatValue(intelligenceText, data.intelligence, data.intelligenceDelta);
        UTIL_UiStatTextHelper.SetStatValue(dexteriteText, data.dexterite, data.dexteriteDelta);
        UTIL_UiStatTextHelper.SetStatValue(enduranceText, data.endurance, data.enduranceDelta);
    }

    private void RefreshTooltips(DATA_PERSONNAGE_Detail data)
    {
        UTIL_UiTooltipHelper.SetTooltip(forceTooltipTrigger, data.forceTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(intelligenceTooltipTrigger, data.intelligenceTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(dexteriteTooltipTrigger, data.dexteriteTooltipDetail);
        UTIL_UiTooltipHelper.SetTooltip(enduranceTooltipTrigger, data.enduranceTooltipDetail);
    }
}