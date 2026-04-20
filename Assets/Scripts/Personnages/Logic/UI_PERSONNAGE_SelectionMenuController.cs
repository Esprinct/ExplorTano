using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PERSONNAGE_SelectionMenuController : UTIL_UiPanelControllerBase
{
    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private UI_PERSONNAGE_Slot slotTemplate;

    private readonly List<UI_PERSONNAGE_Slot> slots = new();
    private List<SCOBJ_Personnage> personnagesCourants = new();
    private Action<SCOBJ_Personnage> onPersonnageChoisi;
    private DATA_PERSONNAGE_DisplayContext contexteCourant = DATA_PERSONNAGE_DisplayContext.Default;

    private void Awake()
    {
        AutoBind();
        ClosePanel();
    }

    private void Update()
    {
        if (panelRoot != null &&
            panelRoot.activeSelf &&
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag tag = GetComponentInChildren<PanelRootTag>(true);
            if (tag != null)
            {
                panelRoot = tag.gameObject;
            }
        }
    }

    public void OpenMenu(
        List<SCOBJ_Personnage> personnages,
        Action<SCOBJ_Personnage> onChoisi,
        DATA_PERSONNAGE_DisplayContext contexte = null)
    {
        personnagesCourants = personnages ?? new List<SCOBJ_Personnage>();
        onPersonnageChoisi = onChoisi;
        contexteCourant = contexte ?? DATA_PERSONNAGE_DisplayContext.Default;

        OpenPanel();
        RefreshView();
    }

    public void CloseMenu()
    {
        ClosePanel();
        onPersonnageChoisi = null;
    }

    private void RefreshView()
    {
        Clear();

        if (content == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_SelectionMenuController : content est null");
            return;
        }

        if (slotTemplate == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_SelectionMenuController : slotTemplate est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(slotTemplate);

        foreach (SCOBJ_Personnage personnage in personnagesCourants)
        {
            if (personnage == null)
                continue;

            DATA_PERSONNAGE_Detail detailData = MAP_PERSONNAGE_DetailMapper.ToHudData(personnage, contexteCourant);
            if (detailData == null)
                continue;

            UI_PERSONNAGE_Slot slot = UTIL_UiSlotListUtility.CreateSlot(slotTemplate, content);
            slot.Refresh(detailData);
            slot.SetOnClick(HandlePersonnageClicked);
            slots.Add(slot);
        }
    }

    private void HandlePersonnageClicked(DATA_PERSONNAGE_Detail detailData)
    {
        if (detailData == null || string.IsNullOrWhiteSpace(detailData.idUnique))
            return;

        SCOBJ_Personnage personnage = ResolvePersonnageSource(detailData.idUnique);
        if (personnage == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_SelectionMenuController : personnage source introuvable");
            return;
        }

        onPersonnageChoisi?.Invoke(personnage);
        CloseMenu();
    }

    private SCOBJ_Personnage ResolvePersonnageSource(string idUnique)
    {
        foreach (SCOBJ_Personnage personnage in personnagesCourants)
        {
            if (personnage == null)
                continue;

            if (personnage.idUnique == idUnique)
                return personnage;
        }

        return null;
    }

    private void Clear()
    {
        UTIL_UiSlotListUtility.ClearSlots(slots);
    }
}