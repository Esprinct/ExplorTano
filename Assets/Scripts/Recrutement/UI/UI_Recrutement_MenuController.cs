using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Recrutement_MenuController : UTIL_UiPanelControllerBase
{
    [Header("Navigation")]
    [SerializeField] private Button boutonFermer;

    [Header("Contenu")]
    [SerializeField] private Transform recrutementContent;
    [SerializeField] private UI_Recrutement_Slot UI_Recrutement_SlotTemplate;

    [Header("Confirmation offre")]
    [SerializeField] private GameObject panelConfirmationOffre;
    [SerializeField] private TMP_Text confirmationOffreText;
    [SerializeField] private TMP_Text montantOffreText;
    [SerializeField] private Slider sliderOffre;
    [SerializeField] private Button boutonConfirmerOffre;
    [SerializeField] private Button boutonAnnulerOffre;

    [Header("Références")]
    [SerializeField] private SYS_GameManager gameManager;

    private readonly List<UI_Recrutement_Slot> slotsInstancies = new();

    private List<SCOBJ_Personnage> personnagesActuels = new();
    private SCOBJ_Personnage personnageSelectionnePourOffre;

    private void Awake()
    {
        AutoBind();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        UTIL_UiEventBinder.Bind(boutonFermer, CloseMenu, this, nameof(boutonFermer));
        UTIL_UiEventBinder.Bind(boutonConfirmerOffre, ConfirmerOffre, this, nameof(boutonConfirmerOffre));
        UTIL_UiEventBinder.Bind(boutonAnnulerOffre, FermerConfirmationOffre, this, nameof(boutonAnnulerOffre));

        if (sliderOffre != null)
        {
            sliderOffre.onValueChanged.RemoveListener(OnSliderValueChanged);
            sliderOffre.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (panelConfirmationOffre != null)
            panelConfirmationOffre.SetActive(false);

        ClosePanel();
    }

    private void OnDestroy()
    {
        UTIL_UiEventBinder.Unbind(boutonFermer, CloseMenu);
        UTIL_UiEventBinder.Unbind(boutonConfirmerOffre, ConfirmerOffre);
        UTIL_UiEventBinder.Unbind(boutonAnnulerOffre, FermerConfirmationOffre);

        if (sliderOffre != null)
            sliderOffre.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
                panelRoot = panelTag.gameObject;
        }

        if (recrutementContent == null)
        {
            TAG_Recrutement_Content contentTag = GetComponentInChildren<TAG_Recrutement_Content>(true);
            if (contentTag != null)
                recrutementContent = contentTag.transform;
        }

        if (UI_Recrutement_SlotTemplate == null)
        {
            TAG_Recrutement_SlotPrefab slotTag = GetComponentInChildren<TAG_Recrutement_SlotPrefab>(true);
            if (slotTag != null)
                UI_Recrutement_SlotTemplate = slotTag.GetComponent<UI_Recrutement_Slot>();
        }
    }

    public void OpenMenu(List<SCOBJ_Personnage> personnages)
    {
        personnagesActuels = personnages ?? new List<SCOBJ_Personnage>();

        OpenPanel();
        RefreshPersonnages(personnagesActuels);
    }

    public void RefreshCurrentMenu()
    {
        RefreshPersonnages(personnagesActuels);
    }

    public void RefreshPersonnages(List<SCOBJ_Personnage> personnages)
    {
        ClearSlots();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        if (recrutementContent == null)
        {
            Debug.LogWarning("recrutementContent est null");
            return;
        }

        if (UI_Recrutement_SlotTemplate == null)
        {
            Debug.LogWarning("UI_Recrutement_SlotTemplate est null");
            return;
        }

        if (personnages == null)
        {
            Debug.LogWarning("personnages est null");
            return;
        }

        UTIL_UiSlotListUtility.PrepareTemplate(UI_Recrutement_SlotTemplate);

        foreach (SCOBJ_Personnage personnage in personnages)
        {
            if (personnage == null)
                continue;

            UI_Recrutement_Slot slot =
                UTIL_UiSlotListUtility.CreateSlot(UI_Recrutement_SlotTemplate, recrutementContent);

            slot.SetMenuController(this);
            slot.Refresh(personnage);

            slotsInstancies.Add(slot);
        }
    }

    public void OuvrirConfirmationPour(SCOBJ_Personnage personnage)
    {
        if (personnage == null)
            return;

        if (gameManager == null || gameManager.SYS_RecrutementSystem == null)
        {
            Debug.LogWarning("GameManager ou RecrutementSystem introuvable.");
            return;
        }

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
        {
            Debug.LogWarning("Joueur humain introuvable.");
            return;
        }
Debug.Log(
    $"[RECRUTEMENT MENU] humain={humain.nomJoueur} | comp={humain.compagnie} | personnage={personnage.nom} {personnage.prenom}"
);
        if (!gameManager.PeutRecruterCeTour())
        {
            Debug.Log("Recrutement déjà effectué ce tour.");
            return;
        }

        DATA_OffreRecrutement offre = gameManager.SYS_RecrutementSystem.GetOffre(personnage);
        
        if (offre == null)
        {
            Debug.LogWarning("Offre introuvable pour ce personnage.");
            return;
        }

        int minimum = offre.prixMinimum;
        int maximum = Mathf.FloorToInt(humain.etrinium);

        if (maximum < minimum)
        {
            Debug.Log("Fonds insuffisants pour proposer une offre.");
            return;
        }

        personnageSelectionnePourOffre = personnage;

        if (panelConfirmationOffre != null)
            panelConfirmationOffre.SetActive(true);

        if (confirmationOffreText != null)
        {
            confirmationOffreText.text =
                $"Quelle somme d'étrinium voulez-vous offrir pour recruter " +
                $"{personnage.nom} {personnage.prenom} ?";
        }

        int montantInitial = offre.GetMontant(humain.compagnie);
        if (montantInitial < minimum)
            montantInitial = minimum;

        if (sliderOffre != null)
        {
            sliderOffre.minValue = minimum;
            sliderOffre.maxValue = maximum;
            sliderOffre.wholeNumbers = true;
            sliderOffre.SetValueWithoutNotify(montantInitial);
        }

        RefreshMontantOffreText(montantInitial);
    }

    private void ConfirmerOffre()
    {
        if (personnageSelectionnePourOffre == null)
            return;

        if (gameManager == null || gameManager.SYS_RecrutementSystem == null)
            return;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return;

        int montant = sliderOffre != null
            ? Mathf.RoundToInt(sliderOffre.value)
            : 0;

        bool succes = gameManager.SYS_RecrutementSystem.SoumettreOffre(
            personnageSelectionnePourOffre,
            humain,
            montant
        );

        if (!succes)
        {
            Debug.LogWarning("Soumission d'offre échouée.");
            return;
        }

        gameManager.MarquerRecrutementEffectue();
        RefreshCurrentMenu();
        FermerConfirmationOffre();
    }

    private void FermerConfirmationOffre()
    {
        personnageSelectionnePourOffre = null;

        if (panelConfirmationOffre != null)
            panelConfirmationOffre.SetActive(false);
    }

    private void OnSliderValueChanged(float value)
    {
        RefreshMontantOffreText(Mathf.RoundToInt(value));
    }

    private void RefreshMontantOffreText(int montant)
    {
        if (montantOffreText != null)
            montantOffreText.text = montant.ToString();
    }

    public void CloseMenu()
    {
        FermerConfirmationOffre();
        ClosePanel();
    }

    private void ClearSlots()
    {
        UTIL_UiSlotListUtility.ClearSlots(slotsInstancies);
    }
}