using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_INVENTAIRE_ONGLET_PERSONNAGE : MonoBehaviour, UI_INVENTAIRE_ONGLET_BASE
{
    [SerializeField] private Transform content;
    [SerializeField] private UI_PERSONNAGE_Slot slotTemplate;
    [SerializeField] private UI_PERSONNAGE_Detail_Controller UI_PERSONNAGE_Detail_Controller;

    private readonly List<UI_PERSONNAGE_Slot> slots = new();

    private List<SCOBJ_Personnage> personnagesCourants = new();
    private DATA_PERSONNAGE_DisplayContext contexteCourant = DATA_PERSONNAGE_DisplayContext.Default;
    private UI_EQUIPE_DetailController equipeMenuCible;
    private bool modeAjoutEquipe;
    private Action<SCOBJ_Personnage> onPersonnageChoisi;

    public void Configure(
        List<SCOBJ_Personnage> personnages,
        DATA_PERSONNAGE_DisplayContext contexte,
        UI_EQUIPE_DetailController equipeMenu,
        bool modeAjout,
        Action<SCOBJ_Personnage> onChoisi = null)
    {
        personnagesCourants = personnages ?? new List<SCOBJ_Personnage>();
        contexteCourant = contexte ?? DATA_PERSONNAGE_DisplayContext.Default;
        equipeMenuCible = equipeMenu;
        modeAjoutEquipe = modeAjout;
        onPersonnageChoisi = onChoisi;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void RefreshView()
    {
        Clear();

        if (content == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_ONGLET_PERSONNAGE : content est null");
            return;
        }

        if (slotTemplate == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_ONGLET_PERSONNAGE : slotTemplate est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(slotTemplate);

        foreach (SCOBJ_Personnage personnage in personnagesCourants)
        {
            if (personnage == null)
                continue;

            DATA_PERSONNAGE_Detail hudData = MAP_PERSONNAGE_DetailMapper.ToHudData(personnage, contexteCourant);
            if (hudData == null)
                continue;

            UI_PERSONNAGE_Slot slot = UTIL_UiSlotListUtility.CreateSlot(slotTemplate, content);
            slot.Refresh(hudData);
            slot.SetOnClick(_ => HandlePersonnageClicked(personnage, hudData));
            slots.Add(slot);
        }
    }

    private void HandlePersonnageClicked(SCOBJ_Personnage personnage, DATA_PERSONNAGE_Detail hudData)
    {
        if (personnage == null)
            return;

        if (onPersonnageChoisi != null)
        {
            onPersonnageChoisi.Invoke(personnage);
            return;
        }

        if (modeAjoutEquipe && equipeMenuCible != null)
        {
            bool success = equipeMenuCible.AjouterPersonnageAEquipe(personnage);

            if (!success)
            {
                Debug.LogWarning("Impossible d'ajouter le personnage à l'équipe.");
            }

            return;
        }

        OuvrirDetailPersonnage(hudData);
    }

    private void OuvrirDetailPersonnage(DATA_PERSONNAGE_Detail hudData)
    {
        if (hudData == null)
            return;

        if (UI_PERSONNAGE_Detail_Controller == null)
        {
            UI_PERSONNAGE_Detail_Controller =
                FindAnyObjectByType<UI_PERSONNAGE_Detail_Controller>(FindObjectsInactive.Include);
        }

        if (UI_PERSONNAGE_Detail_Controller == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_Detail_Controller introuvable.");
            return;
        }

        UI_PERSONNAGE_Detail_Controller.OpenPersonnageMenu(hudData, contexteCourant);
    }

    private void Clear()
    {
        UTIL_UiSlotListUtility.ClearSlots(slots);
    }
}