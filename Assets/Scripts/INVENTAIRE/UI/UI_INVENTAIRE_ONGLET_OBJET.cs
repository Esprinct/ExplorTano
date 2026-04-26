using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_INVENTAIRE_ONGLET_OBJET : MonoBehaviour, UI_INVENTAIRE_ONGLET_BASE
{
    [SerializeField] private Transform content;

    [Header("Templates")]
    [SerializeField] private UI_OBJET_Slot slotTemplateParDefaut;
    [SerializeField] private UI_OBJET_CONSOMMABLE_Slot slotConsommableTemplate;

    [Header("Détail")]
    [SerializeField] private UI_OBJET_DetailController UI_OBJET_DetailController;
    [SerializeField] private bool ouvrirDetailEnModeCompact = false;

    [Header("Interactions")]
    [SerializeField] private float delaiDoubleClic = 0.28f;

    private readonly List<MonoBehaviour> slots = new();

    private List<SCOBJ_OBJET> objetsCourants = new();
    private List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommablesCourants = new();

    private ENUM_OBJET_EQUIPPABLE? typeEquipableFiltre;
    private Action<SCOBJ_OBJET_EQUIPPABLE> onObjetEquipableChoisi;

    private DATA_JOUEUR joueurCourant;
    private SCOBJ_Personnage personnageCourant;

    private Coroutine clicRoutine;
    private SCOBJ_OBJET dernierObjetClique;

    public void Configure(
        List<SCOBJ_OBJET> objets,
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = null,
        ENUM_OBJET_EQUIPPABLE? typeFiltre = null,
        Action<SCOBJ_OBJET_EQUIPPABLE> onChoisi = null,
        DATA_JOUEUR joueur = null,
        SCOBJ_Personnage personnage = null)
    {
        objetsCourants = objets ?? new List<SCOBJ_OBJET>();
        consommablesCourants = consommables ?? new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();
        typeEquipableFiltre = typeFiltre;
        onObjetEquipableChoisi = onChoisi;
        joueurCourant = joueur;
        personnageCourant = personnage;
    }

    public void SetDetailModeCompact(bool compact)
    {
        ouvrirDetailEnModeCompact = compact;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        StopPendingClick();
    }

    public void RefreshView()
    {
        Clear();

        if (content == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_ONGLET_OBJET : content est null");
            return;
        }

        if (slotTemplateParDefaut == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_ONGLET_OBJET : slotTemplateParDefaut est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(slotTemplateParDefaut);

        if (slotConsommableTemplate != null)
        {
            UTIL_UiSlotListUtility.PrepareTemplate(slotConsommableTemplate);
        }

        int nbObjetsAffiches = 0;
        int nbConsommablesAffiches = 0;

        foreach (SCOBJ_OBJET objet in objetsCourants)
        {
            if (!PeutAfficherObjet(objet))
                continue;

            UI_OBJET_Slot slot = UTIL_UiSlotListUtility.CreateSlot(slotTemplateParDefaut, content);
            slot.Refresh(objet);

            bool estEquipableMaintenant = EstObjetEquipableMaintenant(objet, out string raisonIndisponibilite);

            ConfigurerVisuelSlot(slot, estEquipableMaintenant);
            ConfigurerDescriptionSlot(slot, objet, estEquipableMaintenant, raisonIndisponibilite);
            ConfigurerInteractionsSlot(slot, objet, estEquipableMaintenant);

            slots.Add(slot);
            nbObjetsAffiches++;
        }

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in consommablesCourants)
        {
            if (!PeutAfficherConsommable(stack))
                continue;

            if (slotConsommableTemplate == null)
                continue;

            UI_OBJET_CONSOMMABLE_Slot slot = UTIL_UiSlotListUtility.CreateSlot(slotConsommableTemplate, content);
            slot.RefreshStack(stack);
            slot.SetOnClick(HandleConsommableClicked);

            slots.Add(slot);
            nbConsommablesAffiches++;
        }

        Debug.Log(
            $"UI_INVENTAIRE_ONGLET_OBJET Refresh | objets affichés={nbObjetsAffiches} | " +
            $"consommables affichés={nbConsommablesAffiches} | filtre={typeEquipableFiltre}"
        );
    }

    private bool PeutAfficherObjet(SCOBJ_OBJET objet)
    {
        if (objet == null)
            return false;

        if (EstDejaDansLesConsommables(objet))
            return false;

        if (!PasseFiltreEquipable(objet))
            return false;

        return true;
    }

    private bool PeutAfficherConsommable(DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack)
    {
        if (stack == null || stack.objet == null)
            return false;

        if (typeEquipableFiltre.HasValue)
            return false;

        return true;
    }

    private bool EstObjetEquipableMaintenant(SCOBJ_OBJET objet, out string raisonIndisponibilite)
    {
        raisonIndisponibilite = "";

        SCOBJ_OBJET_EQUIPPABLE equipable = objet as SCOBJ_OBJET_EQUIPPABLE;
        if (equipable == null)
            return true;

        if (typeEquipableFiltre.HasValue && equipable.typeEquipable != typeEquipableFiltre.Value)
        {
            raisonIndisponibilite = "Type d'équipement incompatible";
            return false;
        }

        if (personnageCourant != null)
        {
            if (SVC_OBJET_EquipementRequeteService.EstEquipeParPersonnage(personnageCourant, equipable))
            {
                raisonIndisponibilite = "Déjà équipé";
                return false;
            }

            if (!RespecteConditions(personnageCourant, equipable, out raisonIndisponibilite))
                return false;

            return true;
        }

        if (joueurCourant != null)
        {
            if (SVC_OBJET_EquipementRequeteService.EstEquipeParUnDesPersonnagesDuJoueur(joueurCourant, equipable))
            {
                raisonIndisponibilite = "Déjà équipé";
                return false;
            }
        }

        return true;
    }

    private bool RespecteConditions(
        SCOBJ_Personnage personnage,
        SCOBJ_OBJET_EQUIPPABLE objet,
        out string raisonIndisponibilite)
    {
        raisonIndisponibilite = "";

        if (personnage == null || objet == null)
        {
            raisonIndisponibilite = "Personnage ou objet invalide";
            return false;
        }

        if (objet.conditionsEquipement == null || objet.conditionsEquipement.Count == 0)
            return true;

        foreach (DATA_OBJET_EQUIPPABLE_ConditionEquipement condition in objet.conditionsEquipement)
        {
            if (condition == null)
                continue;

            switch (condition.type)
            {
                case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.NiveauMinimum:
                    {
                        int niveau = personnage.progression != null ? personnage.progression.niveau : 0;
                        if (niveau < condition.valeur)
                        {
                            raisonIndisponibilite = $"Niveau insuffisant ({niveau}/{condition.valeur})";
                            return false;
                        }
                        break;
                    }

                case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CuriositeMinimum:
                    if (personnage.curiosite < condition.valeur)
                    {
                        raisonIndisponibilite = $"Curiosite insuffisante ({personnage.curiosite}/{condition.valeur})";
                        return false;
                    }
                    break;

                case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.IngeniositeMinimum:
                    if (personnage.ingeniosite < condition.valeur)
                    {
                        raisonIndisponibilite = $"Ingeniosite insuffisante ({personnage.ingeniosite}/{condition.valeur})";
                        return false;
                    }
                    break;

                case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CombativiteMinimum:
                    if (personnage.combativite < condition.valeur)
                    {
                        raisonIndisponibilite = $"Dextérité insuffisante ({personnage.combativite}/{condition.valeur})";
                        return false;
                    }
                    break;

                case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.EnduranceMinimum:
                    if (personnage.endurance < condition.valeur)
                    {
                        raisonIndisponibilite = $"Endurance insuffisante ({personnage.endurance}/{condition.valeur})";
                        return false;
                    }
                    break;
            }
        }

        return true;
    }

private void ConfigurerVisuelSlot(UI_OBJET_Slot slot, bool estEquipableMaintenant)
{
    if (slot == null)
        return;

    CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
    if (canvasGroup == null)
        canvasGroup = slot.gameObject.AddComponent<CanvasGroup>();

    canvasGroup.alpha = estEquipableMaintenant ? 1f : 0.45f;
    canvasGroup.interactable = true;
    canvasGroup.blocksRaycasts = true;

    // Très important pour que le ScrollView / RectMask2D continue de fonctionner.
    canvasGroup.ignoreParentGroups = false;

    Image[] images = slot.GetComponentsInChildren<Image>(true);
    foreach (Image image in images)
    {
        if (image == null)
            continue;

        image.maskable = true;

        string lower = image.name.ToLowerInvariant();
        if (lower.Contains("sprite") || lower.Contains("icone") || lower.Contains("icon"))
        {
            image.preserveAspect = true;
            image.raycastTarget = false;
        }
    }
}

    private void ConfigurerDescriptionSlot(
        UI_OBJET_Slot slot,
        SCOBJ_OBJET objet,
        bool estEquipableMaintenant,
        string raisonIndisponibilite)
    {
        if (slot == null || objet == null)
            return;

        if (estEquipableMaintenant)
        {
            slot.SetOverrideDescription(objet.description, false);
        }
        else
        {
            string texte = string.IsNullOrWhiteSpace(raisonIndisponibilite)
                ? "Objet indisponible"
                : raisonIndisponibilite;

            slot.SetOverrideDescription(texte, true);
        }
    }

private void ConfigurerInteractionsSlot(UI_OBJET_Slot slot, SCOBJ_OBJET objet, bool estEquipableMaintenant)
{
    if (slot == null || objet == null)
        return;

    slot.SetOnClick(_ => HandleUI_OBJET_SlotClicked(objet, estEquipableMaintenant));

    // Important :
    // Le drag ne doit être actif que dans l'inventaire embarqué du menu personnage.
    bool modeEquipementPersonnage =
        personnageCourant != null &&
        onObjetEquipableChoisi != null;

    if (modeEquipementPersonnage && estEquipableMaintenant)
    {
        SetupDragIfEquipable(slot, objet);
    }
    else
    {
        DesactiverDragSiPresent(slot);
    }
}
private void DesactiverDragSiPresent(UI_OBJET_Slot slot)
{
    if (slot == null)
        return;

    UI_PERSONNAGE_EQUIPEMENT_Draggable draggable =
        slot.GetComponent<UI_PERSONNAGE_EQUIPEMENT_Draggable>();

    if (draggable != null)
        draggable.Clear();

    CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
    if (canvasGroup == null)
        canvasGroup = slot.gameObject.AddComponent<CanvasGroup>();

    canvasGroup.alpha = 1f;
    canvasGroup.interactable = true;
    canvasGroup.blocksRaycasts = true;
    canvasGroup.ignoreParentGroups = false;
}
    private void HandleUI_OBJET_SlotClicked(SCOBJ_OBJET objet, bool estEquipableMaintenant)
    {
        if (objet == null)
            return;

        bool memeObjet = ReferenceEquals(dernierObjetClique, objet)
            || (
                !string.IsNullOrWhiteSpace(dernierObjetClique?.idUnique) &&
                dernierObjetClique.idUnique == objet.idUnique
            );

        if (clicRoutine != null && memeObjet)
        {
            StopCoroutine(clicRoutine);
            clicRoutine = null;
            dernierObjetClique = null;

            HandleDoubleClick(objet, estEquipableMaintenant);
            return;
        }

        dernierObjetClique = objet;

        if (clicRoutine != null)
        {
            StopCoroutine(clicRoutine);
        }

        clicRoutine = StartCoroutine(AttendreSimpleClic(objet));
    }

    private IEnumerator AttendreSimpleClic(SCOBJ_OBJET objet)
    {
        yield return new WaitForSecondsRealtime(delaiDoubleClic);

        clicRoutine = null;
        dernierObjetClique = null;

        OuvrirDetailObjet(objet);
    }

    private void HandleDoubleClick(SCOBJ_OBJET objet, bool estEquipableMaintenant)
    {
        if (objet == null)
            return;

        SCOBJ_OBJET_EQUIPPABLE equipable = objet as SCOBJ_OBJET_EQUIPPABLE;
        if (equipable == null)
            return;

        if (!estEquipableMaintenant)
            return;

        if (onObjetEquipableChoisi == null)
            return;

        onObjetEquipableChoisi.Invoke(equipable);
    }

    private void HandleConsommableClicked(SCOBJ_OBJET objet)
    {
        if (objet == null)
            return;

        OuvrirDetailObjet(objet);
    }

    private void OuvrirDetailObjet(SCOBJ_OBJET objet)
    {
        if (objet == null)
            return;

        if (UI_OBJET_DetailController == null)
        {
            UI_OBJET_DetailController =
                FindAnyObjectByType<UI_OBJET_DetailController>(FindObjectsInactive.Include);
        }

        if (UI_OBJET_DetailController == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_ONGLET_OBJET : UI_OBJET_DetailController est null");
            return;
        }

        DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stackAssocie = FindConsommableStack(objet);

        DATA_OBJET_Detail detailData = stackAssocie != null
            ? MAP_OBJET_DetailMapper.ToDetailData(stackAssocie)
            : MAP_OBJET_DetailMapper.ToDetailData(objet);

        if (detailData == null)
            return;

        if (ouvrirDetailEnModeCompact)
        {
            UI_OBJET_DetailController.OpenCompactMenu(detailData);
        }
        else
        {
            UI_OBJET_DetailController.OpenMenu(detailData);
        }
    }

   private void SetupDragIfEquipable(UI_OBJET_Slot slot, SCOBJ_OBJET objet)
{
    if (slot == null || objet == null)
        return;

    SCOBJ_OBJET_EQUIPPABLE equipable = objet as SCOBJ_OBJET_EQUIPPABLE;
    if (equipable == null)
    {
        DesactiverDragSiPresent(slot);
        return;
    }

    Canvas rootCanvas = GetComponentInParent<Canvas>();
    if (rootCanvas == null)
    {
        Debug.LogWarning("UI_INVENTAIRE_ONGLET_OBJET : aucun Canvas parent trouvé pour le drag and drop.");
        return;
    }

    UI_PERSONNAGE_EQUIPEMENT_Draggable draggable =
        slot.GetComponent<UI_PERSONNAGE_EQUIPEMENT_Draggable>();

    if (draggable == null)
        draggable = slot.gameObject.AddComponent<UI_PERSONNAGE_EQUIPEMENT_Draggable>();

    draggable.Setup(equipable, rootCanvas, false, null);

    CanvasGroup canvasGroup = slot.GetComponent<CanvasGroup>();
    if (canvasGroup == null)
        canvasGroup = slot.gameObject.AddComponent<CanvasGroup>();

    canvasGroup.alpha = 1f;
    canvasGroup.interactable = true;
    canvasGroup.blocksRaycasts = true;
    canvasGroup.ignoreParentGroups = false;
}
    private bool PasseFiltreEquipable(SCOBJ_OBJET objet)
    {
        if (!typeEquipableFiltre.HasValue)
            return true;

        SCOBJ_OBJET_EQUIPPABLE equipable = objet as SCOBJ_OBJET_EQUIPPABLE;
        if (equipable == null)
            return false;

        return equipable.typeEquipable == typeEquipableFiltre.Value;
    }

    private bool EstDejaDansLesConsommables(SCOBJ_OBJET objet)
    {
        if (objet == null || consommablesCourants == null)
            return false;

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in consommablesCourants)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (ReferenceEquals(stack.objet, objet))
                return true;

            if (!string.IsNullOrWhiteSpace(stack.objet.idUnique) &&
                stack.objet.idUnique == objet.idUnique)
            {
                return true;
            }
        }

        return false;
    }

    private DATA_OBJET_CONSOMMABLE_EQUIPE_Stack FindConsommableStack(SCOBJ_OBJET objet)
    {
        if (objet == null || consommablesCourants == null)
            return null;

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in consommablesCourants)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (ReferenceEquals(stack.objet, objet))
                return stack;

            if (!string.IsNullOrWhiteSpace(stack.objet.idUnique) &&
                stack.objet.idUnique == objet.idUnique)
            {
                return stack;
            }
        }

        return null;
    }

    private void StopPendingClick()
    {
        if (clicRoutine != null)
        {
            StopCoroutine(clicRoutine);
            clicRoutine = null;
        }

        dernierObjetClique = null;
    }

    private void Clear()
    {
        StopPendingClick();

        foreach (MonoBehaviour slot in slots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

        slots.Clear();
    }
}