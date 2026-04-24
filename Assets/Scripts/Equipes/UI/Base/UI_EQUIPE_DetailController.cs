using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EQUIPE_DetailController : UTIL_UiPanelControllerBase
{
    private UI_PERSONNAGE_Detail_Controller UI_PERSONNAGE_Detail_Controller;
    private STATE_EQUIPE equipeActuelle;
    private MapController mapController;
    private SYS_GameManager gameManager;

    private bool enAttenteSelectionProvince = false;

    [Header("Actions")]
    [SerializeField] private Button boutonAffecterProvince;
    [SerializeField] private TMP_Text boutonAffecterProvinceText;
    [SerializeField] private Button boutonDemarrerAction;
    [SerializeField] private TMP_Text boutonDemarrerActionText;
    [SerializeField] private Button boutonAjouterPersonnage;
    [SerializeField] private UI_INVENTAIRE_Controller UI_INVENTAIRE_Controller;
    [SerializeField] private int dureeExplorationParDefaut = 3;
    [SerializeField] private Toggle toggleAffectationAutomatique;
    [SerializeField] private Toggle toggleLancementActionAutomatique;

    [Header("Spécialisation équipe")]
    [SerializeField] private Button boutonSpecialisation;
    [SerializeField] private TMP_Text boutonSpecialisationText;
    [SerializeField] private UI_EQUIPE_SpecialisationTreeController specialisationTreeController;

    [Header("Couleurs textes boutons")]
    [SerializeField] private Color couleurTexteBoutonActif = Color.white;
    [SerializeField] private Color couleurTexteBoutonInactif = Color.red;

    [Header("Confirmation Action")]
    [SerializeField] private GameObject panelConfirmationAction;
    [SerializeField] private TMP_Text confirmationActionText;
    [SerializeField] private Button boutonConfirmerAction;
    [SerializeField] private Button boutonAnnulerAction;

    [Header("Etat UI")]
    [SerializeField] private TMP_Text modificationsVerrouilleesText;

    [Header("Sous-vues")]
    [SerializeField] private UI_EQUIPE_HeaderView headerView;
    [SerializeField] private UI_EQUIPE_StatsView statsView;
    [SerializeField] private UI_EQUIPE_PersonnagesView personnagesView;
    [SerializeField] private UI_EQUIPE_ActionView actionView;

    public bool EstEnAttenteSelectionProvince => enAttenteSelectionProvince;

    private void Awake()
    {
        AutoBind();
        ResolveDependencies();

        UTIL_UiEventBinder.Bind(boutonAffecterProvince, DemarrerSelectionProvince, this, nameof(boutonAffecterProvince));
        UTIL_UiEventBinder.Bind(toggleAffectationAutomatique, OnToggleAffectationAutomatiqueChanged, this, nameof(toggleAffectationAutomatique));
        UTIL_UiEventBinder.Bind(toggleLancementActionAutomatique, OnToggleLancementAutomatiqueChanged, this, nameof(toggleLancementActionAutomatique));
        UTIL_UiEventBinder.Bind(boutonDemarrerAction, DemanderConfirmationAction, this, nameof(boutonDemarrerAction));
        UTIL_UiEventBinder.Bind(boutonAjouterPersonnage, OuvrirInventairePourAjout, this, nameof(boutonAjouterPersonnage));
        UTIL_UiEventBinder.Bind(boutonConfirmerAction, ConfirmerDemarrageAction, this, nameof(boutonConfirmerAction));
        UTIL_UiEventBinder.Bind(boutonAnnulerAction, AnnulerDemarrageAction, this, nameof(boutonAnnulerAction));

        if (boutonSpecialisation != null)
            boutonSpecialisation.onClick.AddListener(OnBoutonSpecialisationClicked);

        if (panelConfirmationAction != null)
            panelConfirmationAction.SetActive(false);

        ClosePanel();
    }

    private void OnDestroy()
    {
        UTIL_UiEventBinder.Unbind(boutonAffecterProvince, DemarrerSelectionProvince);
        UTIL_UiEventBinder.Unbind(toggleAffectationAutomatique, OnToggleAffectationAutomatiqueChanged);
        UTIL_UiEventBinder.Unbind(toggleLancementActionAutomatique, OnToggleLancementAutomatiqueChanged);
        UTIL_UiEventBinder.Unbind(boutonDemarrerAction, DemanderConfirmationAction);
        UTIL_UiEventBinder.Unbind(boutonAjouterPersonnage, OuvrirInventairePourAjout);
        UTIL_UiEventBinder.Unbind(boutonConfirmerAction, ConfirmerDemarrageAction);
        UTIL_UiEventBinder.Unbind(boutonAnnulerAction, AnnulerDemarrageAction);

        if (boutonSpecialisation != null)
            boutonSpecialisation.onClick.RemoveListener(OnBoutonSpecialisationClicked);
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
                panelRoot = panelTag.gameObject;
            else
                Debug.LogWarning($"PanelRootTag introuvable dans {name}");
        }
    }

    private void ResolveDependencies()
    {
        if (UI_PERSONNAGE_Detail_Controller == null)
        {
            UI_PERSONNAGE_Detail_Controller =
                FindAnyObjectByType<UI_PERSONNAGE_Detail_Controller>(FindObjectsInactive.Include);
        }

        if (mapController == null)
            mapController = FindAnyObjectByType<MapController>();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();
    }

    private ENUM_EQUIPE_ACTION GetActionCourante()
    {
        return SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipeActuelle);
    }

    private int CalculerCoutActionCourante()
    {
        return SVC_EQUIPE_ActionCostService.CalculerCoutActionCourante(
            equipeActuelle,
            gameManager,
            dureeExplorationParDefaut
        );
    }

    private bool JoueurHumainAPasAssezFonds(int coutLancement)
    {
        return SVC_EQUIPE_ActionCostService.JoueurHumainAPasAssezFonds(
            gameManager,
            coutLancement
        );
    }

    public void OpenEquipeMenu(STATE_EQUIPE equipe)
    {
        ResolveDependencies();

        equipeActuelle = equipe;
        enAttenteSelectionProvince = false;

        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("STATE_EQUIPE ou SCOBJ_EQUIPE est null");
            return;
        }

        OpenPanel();

        if (panelConfirmationAction != null)
            panelConfirmationAction.SetActive(false);

        RefreshVueComplete();
    }

    public void RefreshCurrentEquipe()
    {
        if (equipeActuelle == null)
            return;

        ResolveDependencies();
        RefreshVueComplete();
    }

    public void CloseMenu()
    {
        ClosePanel();

        if (panelConfirmationAction != null)
            panelConfirmationAction.SetActive(false);

        enAttenteSelectionProvince = false;
    }

    public void RefreshVueComplete()
    {
        SVC_EQUIPE_DetailRefreshService.RefreshVueComplete(
            equipeActuelle,
            gameManager,
            UI_PERSONNAGE_Detail_Controller,
            specialisationTreeController,
            headerView,
            statsView,
            personnagesView,
            actionView,
            toggleAffectationAutomatique,
            toggleLancementActionAutomatique,
            boutonAffecterProvince,
            boutonAffecterProvinceText,
            boutonDemarrerAction,
            boutonDemarrerActionText,
            boutonAjouterPersonnage,
            boutonSpecialisation,
            boutonSpecialisationText,
            modificationsVerrouilleesText,
            couleurTexteBoutonActif,
            couleurTexteBoutonInactif,
            enAttenteSelectionProvince,
            dureeExplorationParDefaut
        );
    }

    private void OnBoutonSpecialisationClicked()
    {
        SVC_EQUIPE_SpecialisationUiService.OuvrirArbreSpecialisation(
            equipeActuelle,
            specialisationTreeController
        );
    }

    private void OnToggleAffectationAutomatiqueChanged(bool value)
    {
        SVC_EQUIPE_ToggleUiService.OnToggleAffectationAutomatiqueChanged(
            equipeActuelle,
            value
        );
    }

    private void OnToggleLancementAutomatiqueChanged(bool value)
    {
        SVC_EQUIPE_ToggleUiService.OnToggleLancementAutomatiqueChanged(
            equipeActuelle,
            value,
            toggleAffectationAutomatique
        );

        RefreshVueComplete();
    }

    private void DemarrerSelectionProvince()
    {
        DATA_EQUIPE_ProvinceAssignmentResult result =
            SVC_EQUIPE_ProvinceAssignmentUiService.DemarrerSelectionProvince(equipeActuelle);

        if (!string.IsNullOrWhiteSpace(result.messageErreur) && modificationsVerrouilleesText != null)
            modificationsVerrouilleesText.text = result.messageErreur;

        if (!result.succes)
            return;

        enAttenteSelectionProvince = true;

        if (result.resterFerme)
            ClosePanel();

        if (result.refreshBoutons)
            RefreshVueComplete();
    }

    public void OnProvinceCliqueePourAffectation(STATE_PROVINCE province)
    {
        if (!enAttenteSelectionProvince)
            return;

        DATA_EQUIPE_ProvinceAssignmentResult result =
            SVC_EQUIPE_ProvinceAssignmentUiService.AffecterProvince(equipeActuelle, province);

        if (!result.succes)
        {
            if (!string.IsNullOrWhiteSpace(result.messageErreur))
                Debug.LogWarning(result.messageErreur);

            enAttenteSelectionProvince = false;

            if (result.refreshBoutons)
                RefreshVueComplete();

            return;
        }

        enAttenteSelectionProvince = false;

        if (result.reouvrirPanel)
            OpenPanel();

        RefreshVueComplete();
    }

    private void DemanderConfirmationAction()
    {
        ENUM_EQUIPE_ACTION action = GetActionCourante();

        if (!SVC_EQUIPE_ActionLaunchService.PeutLancerAction(equipeActuelle, action))
            return;

        int coutLancement = CalculerCoutActionCourante();
        if (JoueurHumainAPasAssezFonds(coutLancement))
        {
            RefreshVueComplete();
            return;
        }

        SVC_EQUIPE_ActionLaunchService.RemplirConfirmation(
            equipeActuelle,
            action,
            coutLancement,
            confirmationActionText,
            panelConfirmationAction
        );
    }

    private void ConfirmerDemarrageAction()
    {
        if (panelConfirmationAction != null)
            panelConfirmationAction.SetActive(false);

        ENUM_EQUIPE_ACTION action = GetActionCourante();

        if (!SVC_EQUIPE_ActionLaunchService.PeutLancerAction(equipeActuelle, action))
            return;

        int coutLancement = CalculerCoutActionCourante();
        if (JoueurHumainAPasAssezFonds(coutLancement))
        {
            RefreshVueComplete();
            return;
        }

        ResolveDependencies();

        SVC_EQUIPE_ActionLaunchService.DemarrerAction(
            gameManager,
            equipeActuelle,
            action,
            dureeExplorationParDefaut
        );

        RefreshVueComplete();
    }

    private void AnnulerDemarrageAction()
    {
        if (panelConfirmationAction != null)
            panelConfirmationAction.SetActive(false);
    }

    private void OuvrirInventairePourAjout()
    {
        if (!SVC_EQUIPE_RecruitmentUiService.PeutOuvrirInventairePourAjout(equipeActuelle))
            return;

        ResolveDependencies();

        if (UI_INVENTAIRE_Controller == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_Controller introuvable.");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogWarning("SYS_GameManager introuvable.");
            return;
        }

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
        {
            Debug.LogWarning("Joueur humain introuvable.");
            return;
        }

        DATA_PERSONNAGE_DisplayContext contexte = new(equipeActuelle.compagnie);

        UI_INVENTAIRE_Controller.OpenMenuPourEquipe(
            humain.personnagesRecrutes,
            this,
            contexte,
            humain.objetsPossedes,
            humain.consommablesPossedes
        );
    }

    public bool AjouterPersonnageAEquipe(SCOBJ_Personnage personnage)
    {
        ResolveDependencies();

        if (!SVC_EQUIPE_RecruitmentUiService.PeutAjouterPersonnageAEquipe(
            equipeActuelle,
            personnage,
            gameManager))
        {
            return false;
        }

        bool ajoute = SVC_EQUIPE_RecruitmentUiService.AjouterPersonnageAEquipe(
            equipeActuelle,
            personnage
        );

        if (!ajoute)
            return false;

        RefreshVueComplete();

        if (gameManager != null)
            gameManager.RefreshToutLeHUD();

        return true;
    }
}