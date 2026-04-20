using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PERSONNAGE_Detail_Controller : BaseDetailController<DATA_PERSONNAGE_Detail>
{
    [Header("Sous-vues")]
    [SerializeField] private UI_PERSONNAGE_Detail_Header headerView;
    [SerializeField] private UI_PERSONNAGE_STATS statsView;
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_Stats rowStatsEquipementView;
    [SerializeField] private UI_PERSONNAGE_EQUIPPEMENT_PanelController equipementPanel;
    [SerializeField] private UI_XP_STATS_AllocationPanel statAllocationPanel;

    [Header("Onglets")]
    [SerializeField] private Button boutonOngletStats;
    [SerializeField] private Button boutonOngletEquipements;
    [SerializeField] private GameObject statsTabRoot;
    [SerializeField] private GameObject equipementTabRoot;

    [Header("Congédiement")]
    [SerializeField] private Button boutonCongedier;
    [SerializeField] private TMP_Text boutonCongedierText;
    [SerializeField] private UI_ConfirmationDialog confirmationDialog;

    private SYS_GameManager gameManager;
    private DATA_PERSONNAGE_Detail personnageCourant;
    private SCOBJ_Personnage personnageSource;
    private DATA_PERSONNAGE_DisplayContext contexteCourant = DATA_PERSONNAGE_DisplayContext.Default;

    private PersonnageDetailTab ongletActif = PersonnageDetailTab.Stats;

    private enum PersonnageDetailTab
    {
        Stats = 0,
        Equipements = 1
    }

    protected override void Awake()
    {
        base.Awake();

        gameManager = FindAnyObjectByType<SYS_GameManager>();

        UTIL_UiEventBinder.Bind(boutonOngletStats, OuvrirOngletStats, this, nameof(boutonOngletStats));
        UTIL_UiEventBinder.Bind(
            boutonOngletEquipements,
            OuvrirOngletEquipements,
            this,
            nameof(boutonOngletEquipements)
        );

        UTIL_UiEventBinder.Bind(boutonCongedier, DemanderConfirmationCongedier, this, nameof(boutonCongedier));

        if (statAllocationPanel != null)
            statAllocationPanel.gameObject.SetActive(false);

        if (equipementPanel != null)
            equipementPanel.Initialize();

        if (confirmationDialog != null)
            confirmationDialog.Close();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UTIL_UiEventBinder.Unbind(boutonOngletStats, OuvrirOngletStats);
        UTIL_UiEventBinder.Unbind(boutonOngletEquipements, OuvrirOngletEquipements);
        UTIL_UiEventBinder.Unbind(boutonCongedier, DemanderConfirmationCongedier);

        if (equipementPanel != null)
            equipementPanel.Cleanup();
    }

    protected override void ValidateReferences()
    {
        base.ValidateReferences();

        UTIL_UiReferenceValidator.Require(headerView, nameof(headerView), this);
        UTIL_UiReferenceValidator.Require(statsView, nameof(statsView), this);
        UTIL_UiReferenceValidator.Require(rowStatsEquipementView, nameof(rowStatsEquipementView), this);
        UTIL_UiReferenceValidator.Require(equipementPanel, nameof(equipementPanel), this);
        UTIL_UiReferenceValidator.Require(statAllocationPanel, nameof(statAllocationPanel), this);
        UTIL_UiReferenceValidator.Require(boutonOngletStats, nameof(boutonOngletStats), this);
        UTIL_UiReferenceValidator.Require(boutonOngletEquipements, nameof(boutonOngletEquipements), this);
        UTIL_UiReferenceValidator.Require(statsTabRoot, nameof(statsTabRoot), this);
        UTIL_UiReferenceValidator.Require(equipementTabRoot, nameof(equipementTabRoot), this);
        UTIL_UiReferenceValidator.Require(boutonCongedier, nameof(boutonCongedier), this);
    }

    public void OpenPersonnageMenu(DATA_PERSONNAGE_Detail data, DATA_PERSONNAGE_DisplayContext contexte)
    {
        if (data == null)
        {
            Debug.LogWarning("OpenPersonnageMenu : data est null");
            return;
        }

        personnageCourant = data;
        contexteCourant = contexte ?? DATA_PERSONNAGE_DisplayContext.Default;
        personnageSource = UTIL_PERSONNAGE_Resolver.ResolveFromDetailData(gameManager, data);

        ongletActif = PersonnageDetailTab.Stats;
        OpenMenu(data);

        if (confirmationDialog != null)
            confirmationDialog.Close();
    }

    protected override void RefreshUI(DATA_PERSONNAGE_Detail data)
    {
        if (data == null)
            return;

        headerView?.Refresh(data);
        statsView?.Refresh(data);
        rowStatsEquipementView?.Refresh(data);

        RefreshAllocationPanel();
        RefreshTabs();
        RefreshEquipementSection();
        RefreshCongedierSection();
    }

    protected override void RefreshPrimaryAction(DATA_PERSONNAGE_Detail data)
    {
        if (primaryActionButton != null)
        {
            primaryActionButton.gameObject.SetActive(false);
            primaryActionButton.interactable = false;
        }

        if (primaryActionText != null)
            primaryActionText.text = string.Empty;
    }

    protected override IReadOnlyList<SCOBJ_EFFET> GetEffets(DATA_PERSONNAGE_Detail data)
    {
        return data != null ? data.effets : null;
    }

    protected override ENUM_PERSONNAGE_Genre? GetGenreForEffets(DATA_PERSONNAGE_Detail data)
    {
        return data != null ? data.genre : null;
    }

    private void RefreshAllocationPanel()
    {
        if (statAllocationPanel == null)
            return;

        if (personnageSource == null)
        {
            statAllocationPanel.gameObject.SetActive(false);
            return;
        }

        statAllocationPanel.gameObject.SetActive(ongletActif == PersonnageDetailTab.Stats);
        statAllocationPanel.Setup(personnageSource, HandleAllocationChanged);
    }

    private void HandleAllocationChanged()
    {
        if (personnageSource == null)
            return;

        RafraichirDepuisSource();
    }

    private void RefreshTabs()
    {
        bool afficherStats = ongletActif == PersonnageDetailTab.Stats;
        bool afficherEquipements = ongletActif == PersonnageDetailTab.Equipements;

        if (statsTabRoot != null)
            statsTabRoot.SetActive(afficherStats);

        if (equipementTabRoot != null)
            equipementTabRoot.SetActive(afficherEquipements);
    }

    private void OuvrirOngletStats()
    {
        ongletActif = PersonnageDetailTab.Stats;
        RefreshCurrentView();
    }

    private void OuvrirOngletEquipements()
    {
        ongletActif = PersonnageDetailTab.Equipements;
        RefreshCurrentView();
    }

    private void RefreshEquipementSection()
    {
        if (equipementPanel == null)
            return;

        Canvas rootCanvas = GetComponentInParent<Canvas>();

        equipementPanel.Setup(
            gameManager,
            personnageSource,
            rootCanvas,
            HandleEquipementChanged
        );

        equipementPanel.RefreshView(ongletActif == PersonnageDetailTab.Equipements);
    }

    private void HandleEquipementChanged()
    {
        RafraichirDepuisSource();

        if (gameManager != null)
        {
            gameManager.SynchroniserHudAvecJoueurHumain();
            gameManager.RefreshToutLeHUD();
        }
    }

    private void RefreshCongedierSection()
    {
        DATA_JOUEUR humain = gameManager != null ? gameManager.GetHumanPlayer() : null;

        bool personnageRecrute =
            humain != null &&
            personnageSource != null &&
            humain.personnagesRecrutes != null &&
            humain.personnagesRecrutes.Contains(personnageSource);

        if (boutonCongedier != null)
            boutonCongedier.gameObject.SetActive(personnageRecrute);

        if (boutonCongedier != null)
            boutonCongedier.interactable = personnageRecrute;

        if (boutonCongedierText != null)
            boutonCongedierText.text = "Congédier";
    }

    private void DemanderConfirmationCongedier()
    {
        if (personnageSource == null)
            return;

        if (confirmationDialog == null)
        {
            Debug.LogWarning("UI_ConfirmationDialog non assigné.");
            return;
        }

        confirmationDialog.Open(
            $"Voulez-vous vraiment congédier {personnageSource.nom} {personnageSource.prenom} ?",
            ConfirmerCongedier,
            "Congédier",
            "Annuler"
        );
    }

    private void ConfirmerCongedier()
    {
        if (gameManager == null || personnageSource == null)
            return;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return;

        bool succes = SVC_PERSONNAGE_DismissService.CongedierPersonnage(
            gameManager,
            humain,
            personnageSource
        );

        if (!succes)
        {
            Debug.LogWarning("Échec du congédiement.");
            return;
        }

        CloseMenu();
    }

    private void RafraichirDepuisSource()
    {
        if (personnageSource == null)
            return;

        personnageCourant = MAP_PERSONNAGE_DetailMapper.ToHudData(personnageSource, contexteCourant);
        currentData = personnageCourant;
        RefreshCurrentView();
    }
}