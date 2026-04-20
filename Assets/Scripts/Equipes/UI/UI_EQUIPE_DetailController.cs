using System.Collections.Generic;
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
    [SerializeField] private Button boutonDemarrerExploration;
    [SerializeField] private TMP_Text boutonDemarrerExplorationText;
    [SerializeField] private Button boutonAjouterPersonnage;
    [SerializeField] private UI_INVENTAIRE_Controller UI_INVENTAIRE_Controller;
    [SerializeField] private int dureeExplorationParDefaut = 3;
    [SerializeField] private Toggle toggleAffectationAutomatique;
    [SerializeField] private Toggle toggleLancementExplorationAutomatique;

    [Header("Couleurs textes boutons")]
    [SerializeField] private Color couleurTexteBoutonActif = Color.white;
    [SerializeField] private Color couleurTexteBoutonInactif = Color.red;

    [Header("Confirmation Exploration")]
    [SerializeField] private GameObject panelConfirmationExploration;
    [SerializeField] private TMP_Text confirmationExplorationText;
    [SerializeField] private Button boutonConfirmerExploration;
    [SerializeField] private Button boutonAnnulerExploration;

    [Header("Etat UI")]
    [SerializeField] private TMP_Text modificationsVerrouilleesText;

    [Header("Sous-vues")]
    [SerializeField] private UI_EQUIPE_HeaderView headerView;
    [SerializeField] private UI_EQUIPE_StatsView statsView;
    [SerializeField] private UI_EQUIPE_PersonnagesView personnagesView;
    [SerializeField] private UI_EQUIPE_ExplorationView explorationView;

    private void Awake()
    {
        AutoBind();
        ResolveDependencies();

        UTIL_UiEventBinder.Bind(boutonAffecterProvince, DemarrerSelectionProvince, this, nameof(boutonAffecterProvince));
        UTIL_UiEventBinder.Bind(toggleAffectationAutomatique, OnToggleAffectationAutomatiqueChanged, this, nameof(toggleAffectationAutomatique));
        UTIL_UiEventBinder.Bind(toggleLancementExplorationAutomatique, OnToggleLancementAutomatiqueChanged, this, nameof(toggleLancementExplorationAutomatique));
        UTIL_UiEventBinder.Bind(boutonDemarrerExploration, DemanderConfirmationExploration, this, nameof(boutonDemarrerExploration));
        UTIL_UiEventBinder.Bind(boutonAjouterPersonnage, OuvrirInventairePourAjout, this, nameof(boutonAjouterPersonnage));
        UTIL_UiEventBinder.Bind(boutonConfirmerExploration, ConfirmerDemarrageExploration, this, nameof(boutonConfirmerExploration));
        UTIL_UiEventBinder.Bind(boutonAnnulerExploration, AnnulerDemarrageExploration, this, nameof(boutonAnnulerExploration));

        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(false);
        }

        ClosePanel();
    }

    private void OnDestroy()
    {
        UTIL_UiEventBinder.Unbind(boutonAffecterProvince, DemarrerSelectionProvince);
        UTIL_UiEventBinder.Unbind(toggleAffectationAutomatique, OnToggleAffectationAutomatiqueChanged);
        UTIL_UiEventBinder.Unbind(toggleLancementExplorationAutomatique, OnToggleLancementAutomatiqueChanged);
        UTIL_UiEventBinder.Unbind(boutonDemarrerExploration, DemanderConfirmationExploration);
        UTIL_UiEventBinder.Unbind(boutonAjouterPersonnage, OuvrirInventairePourAjout);
        UTIL_UiEventBinder.Unbind(boutonConfirmerExploration, ConfirmerDemarrageExploration);
        UTIL_UiEventBinder.Unbind(boutonAnnulerExploration, AnnulerDemarrageExploration);
    }

    private void ResolveDependencies()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<SYS_GameManager>();
        }

        if (mapController == null)
        {
            mapController = FindAnyObjectByType<MapController>();
        }

        if (UI_PERSONNAGE_Detail_Controller == null)
        {
            UI_PERSONNAGE_Detail_Controller = FindAnyObjectByType<UI_PERSONNAGE_Detail_Controller>(FindObjectsInactive.Include);
        }

        if (UI_INVENTAIRE_Controller == null)
        {
            UI_INVENTAIRE_Controller = FindAnyObjectByType<UI_INVENTAIRE_Controller>(FindObjectsInactive.Include);
        }
    }

    public bool EstEnAttenteSelectionProvince()
    {
        return enAttenteSelectionProvince;
    }

    public STATE_EQUIPE GetEquipeActuelle()
    {
        return equipeActuelle;
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
            {
                panelRoot = panelTag.gameObject;
            }
            else
            {
                Debug.LogWarning($"PanelRootTag introuvable dans {name}");
            }
        }
    }

    private void OnToggleAffectationAutomatiqueChanged(bool value)
    {
        if (equipeActuelle == null)
            return;

        equipeActuelle.affectationAutomatique = value;
    }

    private void OnToggleLancementAutomatiqueChanged(bool value)
    {
        if (equipeActuelle == null)
            return;

        equipeActuelle.lancementExplorationAutomatique = value;

        if (value)
        {
            equipeActuelle.affectationAutomatique = true;

            if (toggleAffectationAutomatique != null)
            {
                toggleAffectationAutomatique.SetIsOnWithoutNotify(true);
            }
        }
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

        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(false);
        }

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

        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(false);
        }

        enAttenteSelectionProvince = false;
    }

    private void RefreshVueComplete()
    {
        if (equipeActuelle == null || equipeActuelle.data == null)
            return;

        headerView?.Refresh(equipeActuelle);
        statsView?.Refresh(equipeActuelle);
        personnagesView?.Refresh(equipeActuelle, UI_PERSONNAGE_Detail_Controller);
        explorationView?.Refresh(equipeActuelle, gameManager);
   RefreshToggles();
        RefreshEtatBoutons();
    }

    private void RefreshEtatBoutons()
    {
        bool equipeValide = equipeActuelle != null && equipeActuelle.data != null;

        bool aDesMembres =
            equipeValide &&
            equipeActuelle.membresActuels != null &&
            equipeActuelle.membresActuels.Exists(p => p != null);

        bool explorationEnCours =
            equipeValide &&
            equipeActuelle.explorationEnCours;

        bool provinceAffectee =
            equipeValide &&
            equipeActuelle.provinceAffectee != null &&
            equipeActuelle.provinceAffectee.data != null;

        int coutLancement = CalculerCoutLancementExploration();
        bool aLesFonds = !JoueurHumainAPasAssezFonds(coutLancement);

        bool boutonAffecterInteractable =
            equipeValide &&
            aDesMembres &&
            !enAttenteSelectionProvince &&
            !explorationEnCours;

        bool boutonDemarrerInteractable =
            equipeValide &&
            aDesMembres &&
            provinceAffectee &&
            !enAttenteSelectionProvince &&
            !explorationEnCours &&
            aLesFonds;

        if (boutonAffecterProvince != null)
        {
            boutonAffecterProvince.interactable = boutonAffecterInteractable;
        }

        if (boutonDemarrerExploration != null)
        {
            boutonDemarrerExploration.interactable = boutonDemarrerInteractable;
        }

        if (boutonAffecterProvinceText != null)
        {
            boutonAffecterProvinceText.text = "Affecter à une province";
            boutonAffecterProvinceText.color = boutonAffecterInteractable
                ? couleurTexteBoutonActif
                : couleurTexteBoutonInactif;
        }

        if (boutonDemarrerExplorationText != null)
        {
            boutonDemarrerExplorationText.text = $"{coutLancement}";
            boutonDemarrerExplorationText.color = boutonDemarrerInteractable
                ? couleurTexteBoutonActif
                : couleurTexteBoutonInactif;
        }

        if (boutonAjouterPersonnage != null)
        {
            boutonAjouterPersonnage.interactable = equipeValide && !explorationEnCours;
        }

        if (modificationsVerrouilleesText != null)
        {
            bool afficherMessage =
                explorationEnCours ||
                enAttenteSelectionProvince ||
                (aDesMembres && !provinceAffectee) ||
                (provinceAffectee && !aLesFonds);

            modificationsVerrouilleesText.gameObject.SetActive(afficherMessage);

            if (explorationEnCours)
            {
                modificationsVerrouilleesText.text = "Exploration en cours : modifications verrouillées";
            }
            else if (enAttenteSelectionProvince)
            {
                modificationsVerrouilleesText.text = "Sélectionnez une province à affecter";
            }
            else if (aDesMembres && !provinceAffectee)
            {
                modificationsVerrouilleesText.text = "Affectez une province pour lancer l'exploration";
            }
            else if (provinceAffectee && !aLesFonds)
            {
                modificationsVerrouilleesText.text = $"Fonds insuffisants : {coutLancement} Etrinium requis";
            }
        }
    }

    private int CalculerCoutLancementExploration()
    {
        if (equipeActuelle == null || equipeActuelle.data == null)
            return 0;

        ResolveDependencies();

        if (gameManager == null)
            return 0;

        ExplorationConfig config = gameManager.ExplorationConfig;

        int toursBase = config != null ? config.toursBase : dureeExplorationParDefaut;
        int coutParTourBase = config != null ? config.coutParTourBase : 5;
        int prestigeBase = config != null ? config.prestigeBase : 1;
        float chanceArtefactBase = config != null ? config.chanceArtefactBase : 10f;
        float chanceArtefactRareBase = config != null ? config.chanceArtefactRareBase : 2f;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipeActuelle);

        int enclavement = 0;
        if (equipeActuelle.provinceAffectee != null && equipeActuelle.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipeActuelle.provinceAffectee.data.accesibilite);
        }

        ENUM_EXPLORATION_Resultat result = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursBase,
            coutParTourBase,
            prestigeBase,
            chanceArtefactBase,
            chanceArtefactRareBase,
            enclavement
        );

        return result != null ? result.coutTotal : 0;
    }

    private bool JoueurHumainAPasAssezFonds(int coutLancement)
    {
        ResolveDependencies();

        if (gameManager == null)
            return true;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return true;

        return humain.etrinium < coutLancement;
    }

    private void DemarrerSelectionProvince()
    {
        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return;
        }

        if (equipeActuelle.explorationEnCours)
        {
            Debug.LogWarning("Impossible de modifier l'affectation pendant une exploration.");
            return;
        }

        enAttenteSelectionProvince = true;
        RefreshEtatBoutons();
        ClosePanel();
    }

    public void OnProvinceCliqueePourAffectation(STATE_PROVINCE province)
    {
        if (!enAttenteSelectionProvince)
            return;

        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            enAttenteSelectionProvince = false;
            RefreshEtatBoutons();
            return;
        }

        if (province == null || province.data == null)
        {
            Debug.LogWarning("Province invalide.");
            return;
        }

        equipeActuelle.provinceAffectee = province;
        equipeActuelle.explorationTerminee = false;
        enAttenteSelectionProvince = false;

        OpenPanel();
        RefreshVueComplete();
    }

    private void DemanderConfirmationExploration()
    {
        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return;
        }

        if (equipeActuelle.explorationEnCours)
        {
            Debug.LogWarning("Une exploration est déjà en cours.");
            return;
        }

        if (equipeActuelle.provinceAffectee == null || equipeActuelle.provinceAffectee.data == null)
        {
            Debug.LogWarning("Aucune province affectée.");
            return;
        }

        bool aDesMembres =
            equipeActuelle.membresActuels != null &&
            equipeActuelle.membresActuels.Exists(p => p != null);

        if (!aDesMembres)
        {
            Debug.LogWarning("Impossible de démarrer l'exploration sans personnages.");
            return;
        }

        int coutLancement = CalculerCoutLancementExploration();
        if (JoueurHumainAPasAssezFonds(coutLancement))
        {
            RefreshEtatBoutons();
            return;
        }

        if (confirmationExplorationText != null)
        {
            confirmationExplorationText.text =
                $"Démarrer l'exploration de {equipeActuelle.provinceAffectee.data.nom} " +
                $"avec {equipeActuelle.data.nomEquipe} pour {coutLancement} Etrinium ?";
        }

        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(true);
        }
    }

    private void ConfirmerDemarrageExploration()
    {
        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(false);
        }

        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return;
        }

        if (equipeActuelle.provinceAffectee == null || equipeActuelle.provinceAffectee.data == null)
        {
            Debug.LogWarning("Aucune province affectée.");
            return;
        }

        bool aDesMembres =
            equipeActuelle.membresActuels != null &&
            equipeActuelle.membresActuels.Exists(p => p != null);

        if (!aDesMembres)
        {
            Debug.LogWarning("Impossible de démarrer l'exploration sans personnages.");
            return;
        }

        int coutLancement = CalculerCoutLancementExploration();
        if (JoueurHumainAPasAssezFonds(coutLancement))
        {
            RefreshEtatBoutons();
            return;
        }

        ResolveDependencies();

        if (gameManager != null)
        {
            gameManager.DemarrerExploration(equipeActuelle, dureeExplorationParDefaut);
            gameManager.RefreshToutLeHUD();
        }
        else
        {
            equipeActuelle.explorationEnCours = true;
            equipeActuelle.toursTotaux = dureeExplorationParDefaut;
            equipeActuelle.toursRestants = dureeExplorationParDefaut;
        }

        RefreshVueComplete();
    }

    private void AnnulerDemarrageExploration()
    {
        if (panelConfirmationExploration != null)
        {
            panelConfirmationExploration.SetActive(false);
        }
    }

    private void OuvrirInventairePourAjout()
    {
        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe ouverte.");
            return;
        }

        if (equipeActuelle.explorationEnCours)
        {
            Debug.LogWarning("Impossible d'ajouter un personnage pendant une exploration.");
            return;
        }

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
        if (equipeActuelle == null || equipeActuelle.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return false;
        }

        if (equipeActuelle.explorationEnCours)
        {
            Debug.LogWarning("Impossible d'ajouter un personnage pendant une exploration.");
            return false;
        }

        if (personnage == null)
        {
            Debug.LogWarning("Personnage null.");
            return false;
        }

        ResolveDependencies();

        if (gameManager != null && gameManager.EstPersonnageDansUneEquipe(personnage))
        {
            Debug.LogWarning("Le personnage est déjà dans une autre équipe.");
            return false;
        }

        if (equipeActuelle.membresActuels == null)
        {
            equipeActuelle.membresActuels = new List<SCOBJ_Personnage>();
        }

        equipeActuelle.membresActuels.RemoveAll(p => p == null);

        if (equipeActuelle.membresActuels.Contains(personnage))
        {
            Debug.LogWarning("Le personnage est déjà dans cette équipe.");
            return false;
        }

        if (equipeActuelle.membresActuels.Count >= 12)
        {
            Debug.LogWarning("L'équipe est complète.");
            return false;
        }

        equipeActuelle.membresActuels.Add(personnage);

        RefreshVueComplete();

        if (gameManager != null)
        {
            gameManager.RefreshToutLeHUD();
        }

        return true;
    }
    private void RefreshToggles()
{
    if (equipeActuelle == null)
        return;

    if (toggleAffectationAutomatique != null)
    {
        toggleAffectationAutomatique.SetIsOnWithoutNotify(
            equipeActuelle.affectationAutomatique
        );
    }

    if (toggleLancementExplorationAutomatique != null)
    {
        toggleLancementExplorationAutomatique.SetIsOnWithoutNotify(
            equipeActuelle.lancementExplorationAutomatique
        );
    }
}
}