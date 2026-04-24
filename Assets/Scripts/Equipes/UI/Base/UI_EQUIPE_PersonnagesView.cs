using System.Collections.Generic;
using UnityEngine;

public class UI_EQUIPE_PersonnagesView : MonoBehaviour
{
    [SerializeField] private Transform personnagesContent;
    [SerializeField] private UI_PERSONNAGE_Slot UI_PERSONNAGE_SlotTemplate;

    private readonly List<UI_PERSONNAGE_Slot> slotsInstancies = new();

    public void Refresh(STATE_EQUIPE equipe, UI_PERSONNAGE_Detail_Controller UI_PERSONNAGE_Detail_Controller)
    {
        ClearSlots();

        if (equipe == null || equipe.membresActuels == null)
            return;

        if (personnagesContent == null)
        {
            Debug.LogWarning("personnagesContent est null");
            return;
        }

        if (UI_PERSONNAGE_SlotTemplate == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_SlotTemplate est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(UI_PERSONNAGE_SlotTemplate);

        DATA_PERSONNAGE_DisplayContext contexte = new(equipe.compagnie);

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage == null)
                continue;

            DATA_PERSONNAGE_Detail hudData = MAP_PERSONNAGE_DetailMapper.ToHudData(personnage, contexte);
            if (hudData == null)
                continue;

            UI_PERSONNAGE_Slot slot = UTIL_UiSlotListUtility.CreateSlot(UI_PERSONNAGE_SlotTemplate, personnagesContent);
   

slot.SetOnClick(data =>
{
    if (UI_PERSONNAGE_Detail_Controller != null)
    {
        UI_PERSONNAGE_Detail_Controller.OpenPersonnageMenu(data, contexte);
    }
});
            slot.Refresh(hudData);
            slotsInstancies.Add(slot);
        }
    }

    public void ClearSlots()
    {
        UTIL_UiSlotListUtility.ClearSlots(slotsInstancies);
    }
}