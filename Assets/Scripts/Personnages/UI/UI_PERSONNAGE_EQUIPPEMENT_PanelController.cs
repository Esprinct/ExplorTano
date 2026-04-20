using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PERSONNAGE_EQUIPPEMENT_PanelController : MonoBehaviour
{
    [Header("Équipement - Contents gauche")]
    [SerializeField] private Transform contentOutilEquipe;
    [SerializeField] private Transform contentTenueEquipe;
    [SerializeField] private Transform contentAccessoireEquipe;

    [Header("Équipement - Template gauche")]
    [SerializeField] private UI_OBJET_Slot slotEquipementTemplate;

    [Header("Boutons déséquiper")]
    [SerializeField] private Button boutonDesequiperOutil;
    [SerializeField] private Button boutonDesequiperTenue;
    [SerializeField] private Button boutonDesequiperAccessoire;

    [Header("Inventaire embarqué")]
    [SerializeField] private UI_INVENTAIRE_ONGLET_OBJET equipementInventoryView;
    [SerializeField] private UI_OBJET_DetailController UI_OBJET_DetailController;

    [Header("Drag & Drop")]
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_DropSlotUI dropSlotOutil;
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_DropSlotUI dropSlotTenue;
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_DropSlotUI dropSlotAccessoire;
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_DropZone dropZoneInventaire;
[Header("Interactions")]
[SerializeField] private float delaiDoubleClic = 0.28f;

private Coroutine clicEquipementRoutine;
private SCOBJ_OBJET_EQUIPPABLE dernierObjetEquipeClique;
private ENUM_OBJET_EQUIPPABLE? dernierTypeEquipeClique;    private readonly List<UI_OBJET_Slot> slotsEquipesInstancies = new();

    private SYS_GameManager gameManager;
    private SCOBJ_Personnage personnageSource;
    private Action onChanged;
    private Canvas rootCanvas;

    public void Initialize()
    {
        if (boutonDesequiperOutil != null)
            boutonDesequiperOutil.onClick.AddListener(OnClickDesequiperOutil);

        if (boutonDesequiperTenue != null)
            boutonDesequiperTenue.onClick.AddListener(OnClickDesequiperTenue);

        if (boutonDesequiperAccessoire != null)
            boutonDesequiperAccessoire.onClick.AddListener(OnClickDesequiperAccessoire);
    }

    public void Cleanup()
    {
        if (boutonDesequiperOutil != null)
            boutonDesequiperOutil.onClick.RemoveListener(OnClickDesequiperOutil);

        if (boutonDesequiperTenue != null)
            boutonDesequiperTenue.onClick.RemoveListener(OnClickDesequiperTenue);

        if (boutonDesequiperAccessoire != null)
            boutonDesequiperAccessoire.onClick.RemoveListener(OnClickDesequiperAccessoire);
    }

    public void Setup(
        SYS_GameManager gm,
        SCOBJ_Personnage personnage,
        Canvas canvas,
        Action onDataChanged)
    {
        gameManager = gm;
        personnageSource = personnage;
        rootCanvas = canvas;
        onChanged = onDataChanged;

        if (dropSlotOutil != null)
            dropSlotOutil.Setup(this, ENUM_OBJET_EQUIPPABLE.Outil);

        if (dropSlotTenue != null)
            dropSlotTenue.Setup(this, ENUM_OBJET_EQUIPPABLE.Tenue);

        if (dropSlotAccessoire != null)
            dropSlotAccessoire.Setup(this, ENUM_OBJET_EQUIPPABLE.Accessoire);

        if (dropZoneInventaire != null)
            dropZoneInventaire.Setup(this);
    }

    public void RefreshView(bool visible)
    {
        if (!visible)
        {
            if (equipementInventoryView != null)
                equipementInventoryView.Hide();

            ClearEquipementSlots();
            return;
        }

        RefreshEquipementsGauche();
        RefreshInventaireEquipementDroite();
    }

    private void RefreshEquipementsGauche()
    {
        ClearEquipementSlots();

        if (personnageSource == null)
        {
            SetDesequiperState(boutonDesequiperOutil, false);
            SetDesequiperState(boutonDesequiperTenue, false);
            SetDesequiperState(boutonDesequiperAccessoire, false);
            return;
        }

        if (slotEquipementTemplate == null)
        {
            Debug.LogWarning("UI_PERSONNAGE_EQUIPPEMENT_PanelController : slotEquipementTemplate est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(slotEquipementTemplate);

        SCOBJ_OBJET_EQUIPPABLE outil =
            UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnageSource, ENUM_OBJET_EQUIPPABLE.Outil);

        SCOBJ_OBJET_EQUIPPABLE tenue =
            UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnageSource, ENUM_OBJET_EQUIPPABLE.Tenue);

        SCOBJ_OBJET_EQUIPPABLE accessoire =
            UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnageSource, ENUM_OBJET_EQUIPPABLE.Accessoire);

        CreateEquipementSlot(outil, contentOutilEquipe, ENUM_OBJET_EQUIPPABLE.Outil);
        CreateEquipementSlot(tenue, contentTenueEquipe, ENUM_OBJET_EQUIPPABLE.Tenue);
        CreateEquipementSlot(accessoire, contentAccessoireEquipe, ENUM_OBJET_EQUIPPABLE.Accessoire);

        SetDesequiperState(boutonDesequiperOutil, outil != null);
        SetDesequiperState(boutonDesequiperTenue, tenue != null);
        SetDesequiperState(boutonDesequiperAccessoire, accessoire != null);
    }

    private void CreateEquipementSlot(
        SCOBJ_OBJET_EQUIPPABLE objet,
        Transform parent,
        ENUM_OBJET_EQUIPPABLE typeEquipement)
    {
        if (objet == null || parent == null || slotEquipementTemplate == null)
            return;

        UI_OBJET_Slot slot = UTIL_UiSlotListUtility.CreateSlot(slotEquipementTemplate, parent);
        slot.Refresh(objet);
        SetupEquipementSlotInteractions(slot, objet, typeEquipement);

        if (rootCanvas != null)
        {
            UI_PERSONNAGE_EQUIPEMENT_Draggable draggable = slot.GetComponent<UI_PERSONNAGE_EQUIPEMENT_Draggable>();
            if (draggable == null)
                draggable = slot.gameObject.AddComponent<UI_PERSONNAGE_EQUIPEMENT_Draggable>();

            draggable.Setup(objet, rootCanvas, true, typeEquipement);
        }

        slotsEquipesInstancies.Add(slot);
    }
private void SetupEquipementSlotInteractions(
    UI_OBJET_Slot slot,
    SCOBJ_OBJET_EQUIPPABLE objet,
    ENUM_OBJET_EQUIPPABLE typeEquipement)
{
    if (slot == null || objet == null)
        return;

    slot.SetOnClick(_ => HandleEquipementSlotClicked(objet, typeEquipement));
}
private void HandleEquipementSlotClicked(SCOBJ_OBJET_EQUIPPABLE objet, ENUM_OBJET_EQUIPPABLE typeEquipement)
{
    if (objet == null)
        return;

    bool memeObjet = ReferenceEquals(dernierObjetEquipeClique, objet)
        || (!string.IsNullOrWhiteSpace(dernierObjetEquipeClique?.idUnique)
            && dernierObjetEquipeClique.idUnique == objet.idUnique);

    bool memeType = dernierTypeEquipeClique.HasValue && dernierTypeEquipeClique.Value == typeEquipement;

    if (clicEquipementRoutine != null && memeObjet && memeType)
    {
        StopCoroutine(clicEquipementRoutine);
        clicEquipementRoutine = null;
        dernierObjetEquipeClique = null;
        dernierTypeEquipeClique = null;

        HandleDesequiper(typeEquipement);
        return;
    }

    dernierObjetEquipeClique = objet;
    dernierTypeEquipeClique = typeEquipement;

    if (clicEquipementRoutine != null)
        StopCoroutine(clicEquipementRoutine);

    clicEquipementRoutine = StartCoroutine(AttendreSimpleClicEquipement(objet));
}

private System.Collections.IEnumerator AttendreSimpleClicEquipement(SCOBJ_OBJET_EQUIPPABLE objet)
{
    yield return new WaitForSecondsRealtime(delaiDoubleClic);

    clicEquipementRoutine = null;
    dernierObjetEquipeClique = null;
    dernierTypeEquipeClique = null;

    OuvrirDetailObjet(objet);
}
    private void OuvrirDetailObjet(SCOBJ_OBJET objet)
    {
        if (UI_OBJET_DetailController == null || objet == null)
            return;

        DATA_OBJET_Detail detailData = MAP_OBJET_DetailMapper.ToDetailData(objet);
        if (detailData == null)
            return;

        UI_OBJET_DetailController.OpenMenu(detailData);
    }

   private void RefreshInventaireEquipementDroite()
{
    if (equipementInventoryView == null)
        return;

    if (gameManager == null || personnageSource == null)
    {
        equipementInventoryView.Hide();
        return;
    }

    DATA_JOUEUR joueur = gameManager.GetHumanPlayer();
    if (joueur == null)
    {
        equipementInventoryView.Hide();
        return;
    }

    equipementInventoryView.SetDetailModeCompact(true);

    equipementInventoryView.Configure(
        joueur.objetsPossedes,
        null,
        null,
        objet => HandleObjetChoisiDepuisInventaire(joueur, personnageSource, objet),
        joueur,
        personnageSource
    );

    equipementInventoryView.Show();
    equipementInventoryView.RefreshView();
}

    private void HandleObjetChoisiDepuisInventaire(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (joueur == null || personnage == null || objet == null)
            return;

        if (SVC_OBJET_EquipementRequeteService.EstEquipeParUnDesPersonnagesDuJoueur(joueur, objet))
            return;

        bool success = UTIL_JOUEUR_EQUIPPEMENT.EquiperObjetAuPersonnage(joueur, personnage, objet);
        if (!success)
        {
            Debug.LogWarning("Équipement via clic échoué");
            return;
        }

        onChanged?.Invoke();
    }

    public void TryEquipFromDrag(ENUM_OBJET_EQUIPPABLE slotType, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (personnageSource == null || objet == null || gameManager == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetHumanPlayer();
        if (joueur == null)
            return;

        if (SVC_OBJET_EquipementRequeteService.EstEquipeParUnDesPersonnagesDuJoueur(joueur, objet))
            return;

        if (objet.typeEquipable != slotType)
        {
            Debug.LogWarning($"Objet incompatible avec le slot {slotType}");
            return;
        }

        if (EQUIPEMENT_DragContext.SourceUI != null)
            EQUIPEMENT_DragContext.SourceUI.ForceStopDragVisual();

        EQUIPEMENT_DragContext.Clear();

        bool success = UTIL_JOUEUR_EQUIPPEMENT.EquiperObjetAuPersonnage(joueur, personnageSource, objet);
        if (!success)
        {
            Debug.LogWarning("Équipement échoué");
            return;
        }

        onChanged?.Invoke();
    }

    public void TryDesequipFromDrag(ENUM_OBJET_EQUIPPABLE type)
    {
        if (personnageSource == null || gameManager == null)
            return;

        if (EQUIPEMENT_DragContext.SourceUI != null)
            EQUIPEMENT_DragContext.SourceUI.ForceStopDragVisual();

        EQUIPEMENT_DragContext.Clear();

        HandleDesequiper(type);
    }

    private void OnClickDesequiperOutil()
    {
        HandleDesequiper(ENUM_OBJET_EQUIPPABLE.Outil);
    }

    private void OnClickDesequiperTenue()
    {
        HandleDesequiper(ENUM_OBJET_EQUIPPABLE.Tenue);
    }

    private void OnClickDesequiperAccessoire()
    {
        HandleDesequiper(ENUM_OBJET_EQUIPPABLE.Accessoire);
    }

    private void HandleDesequiper(ENUM_OBJET_EQUIPPABLE type)
    {
        if (personnageSource == null || gameManager == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetHumanPlayer();
        if (joueur == null)
            return;

        bool success = UTIL_JOUEUR_EQUIPPEMENT.DesequiperObjetDuPersonnage(joueur, personnageSource, type);
        if (!success)
        {
            Debug.LogWarning("Déséquipement échoué");
            return;
        }

        onChanged?.Invoke();
    }

    private void SetDesequiperState(Button bouton, bool actif)
    {
        if (bouton == null)
            return;

        bouton.interactable = actif;
    }

    private void ClearEquipementSlots()
    {
        foreach (UI_OBJET_Slot slot in slotsEquipesInstancies)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slotsEquipesInstancies.Clear();
    }
}