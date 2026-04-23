using System.Collections.Generic;
using UnityEngine;

public class SYS_GameManager : MonoBehaviour
{
    [Header("Données de départ")]
    [SerializeField] private List<SCOBJ_EQUIPE> equipesDeDepart = new();

    [Header("Configuration")]
    [SerializeField] private ExplorationConfig explorationConfig;
    [SerializeField] private CFG_LevelProgression progressionConfigPersonnage;
    [SerializeField] private SYS_DebugEquipesRuntimeView debugEquipesRuntimeView;
    [SerializeField] private SYS_AutoPlayController autoPlayController;
[SerializeField] private CFG_LevelProgression progressionConfigEquipe;
[SerializeField] private VadrouilleConfig vadrouilleConfig;
public VadrouilleConfig VadrouilleConfig => vadrouilleConfig;

    [Header("Références UI")]
    [SerializeField] private HudController hudController;
    [SerializeField] private UI_EQUIPE_DetailController equipeDetailController;
    [SerializeField] private UI_EXPLORATION_RecompensePopup explorationRecompensePopup;

    [Header("Données runtime affichables")]
    [SerializeField] private DATA_JOUEUR_HUD joueurData = new();
    [SerializeField] private DATA_Partie_Hud_Tour partieData = new();

    [Header("Entretien des équipes")]
    [SerializeField] private int coutFixeEquipeParTour = 150;
    [SerializeField] private int coutFixeEquipeAvecMembresParTour = 250;
    [SerializeField] private int surcoutEquipeEnExplorationParTour = 100;

    [Header("Joueurs")]
    public DATA_JOUEUR Joueur1;
    public DATA_JOUEUR Joueur2;
    public DATA_JOUEUR Joueur3;

    [Header("Recrutement")]
    [SerializeField] private List<SCOBJ_Personnage> poolRecrutementInitial = new();
    [SerializeField] private Sprite[] spritesGeneriquesPersonnages;

    [Header("Effets automatiques d'affinité")]
    [SerializeField] private SCOBJ_PERSONNAGE_EFFET effetAffiniteRespectee;
    [SerializeField] private SCOBJ_PERSONNAGE_EFFET effetAffiniteNonRespectee;

    [Header("Création d'équipes")]
    [SerializeField] private SCOBJ_EQUIPE modeleEquipeVide;
    [SerializeField] private int maxEquipesParJoueur = 5;
    [SerializeField] private List<int> coutsCreationEquipes = new() { 100, 15000, 45000, 60000, 100000 };

    [Header("Ordre du tour")]
    [SerializeField] private List<ENUM_Compagnie> ordreTourCourant = new();
    [SerializeField] private int indexJoueurActifTour = 0;

    private readonly HashSet<ENUM_Compagnie> compagniesAyantRecruteCeTour = new();

    public SYS_AutoPlayController AutoPlayController => autoPlayController;
    public ExplorationConfig ExplorationConfig => explorationConfig;
    public CFG_LevelProgression ProgressionConfigPersonnage => progressionConfigPersonnage;

    public List<STATE_EQUIPE> EquipesRuntime { get; private set; } = new();
    public List<STATE_PROVINCE> ProvincesRuntime { get; } = new();

    public DATA_JOUEUR_HUD JoueurData => joueurData;
    public DATA_Partie_Hud_Tour PartieData => partieData;
public SYS_VadrouilleSystem VadrouilleSystem { get; private set; }
    public IReadOnlyList<ENUM_Compagnie> OrdreTourCourant => ordreTourCourant;
    public int IndexJoueurActifTour => indexJoueurActifTour;
    public int MaxEquipesParJoueur => maxEquipesParJoueur;
public CFG_LevelProgression ProgressionConfigEquipe => progressionConfigEquipe;
    public HudController HudController
    {
        get => hudController;
        set => hudController = value;
    }

    public UI_EQUIPE_DetailController EquipeDetailController
    {
        get => equipeDetailController;
        set => equipeDetailController = value;
    }

    public UI_EXPLORATION_RecompensePopup ExplorationRecompensePopup
    {
        get => explorationRecompensePopup;
        set => explorationRecompensePopup = value;
    }

    public bool PartieTerminee { get; set; }

    public SYS_RecrutementSystem SYS_RecrutementSystem { get; private set; }
    public SYS_RevenusSystem RevenusSystem { get; private set; }
    public SYS_VictoireSystem VictoireSystem { get; private set; }

    public SYS_GameInitializationService InitializationService { get; private set; }
    public SYS_TurnSystem TurnSystem { get; private set; }
    public ExplorationSystem ExplorationSystem { get; private set; }
    public SYS_InfluenceSystem InfluenceSystem { get; private set; }
    public SYS_GameUiRefreshService UiSystem { get; private set; }
    public SYS_FailliteSystem FailliteSystem { get; private set; }
    public SYS_EquipeManagementService EquipeManagementService { get; private set; }
    public SYS_GameRulesService RulesService { get; private set; }

    public SYS_PlayerAccessService PlayerAccessService { get; private set; }
    public SYS_TurnOrderService TurnOrderService { get; private set; }
    public SYS_HudSyncService HudSyncService { get; private set; }
    public SYS_PlayerInitializationService PlayerInitializationService { get; private set; }
    public SYS_RecruitmentTurnStateService RecruitmentTurnStateService { get; private set; }

    public int CoutFixeEquipeParTour => coutFixeEquipeParTour;
    public int CoutFixeEquipeAvecMembresParTour => coutFixeEquipeAvecMembresParTour;
    public int SurcoutEquipeEnExplorationParTour => surcoutEquipeEnExplorationParTour;

    public List<SCOBJ_EQUIPE> GetEquipesDeDepart()
    {
        return equipesDeDepart;
    }

    private void Awake()
    {
        Debug.Log("GM Awake start " + Time.realtimeSinceStartup);

        InitialiserServices();
        InitialiserJoueurs();

        partieData ??= new DATA_Partie_Hud_Tour();
        partieData.tourMax = 999;
        FailliteSystem = new SYS_FailliteSystem();

        ConfigurerGenerateurPersonnages();
        AssignerEffetsAffiniteAutomatiques();
        InitialiserRecrutement();

        InitializationService.InitialiserPartie(this, equipesDeDepart);

        SynchroniserCompagniesJoueursDepuisDirigeants();
        SynchroniserHudAvecJoueurHumain();
        ReinitialiserDirigeants();

        Debug.Log($"SYS_RecrutementSystem initialisé | Pool: {poolRecrutementInitial.Count}");
        Debug.Log("GM Awake end " + Time.realtimeSinceStartup);
    }

    private void Start()
    {
        Debug.Log("GM Start begin " + Time.realtimeSinceStartup);

        SynchroniserCompagniesJoueursDepuisDirigeants();
        ConstruireOrdreTourSelonPrestige();
        RevenusSystem?.RecalculerRevenusSeulement(this);

        foreach (STATE_PROVINCE province in ProvincesRuntime)
        {
            InfluenceSystem?.MettreAJourClaimProvince(this, province);
        }

        if (debugEquipesRuntimeView == null)
        {
            debugEquipesRuntimeView = GetComponent<SYS_DebugEquipesRuntimeView>();
        }

        if (autoPlayController == null)
        {
            autoPlayController = GetComponent<SYS_AutoPlayController>();
        }

        SynchroniserHudAvecJoueurHumain();
        RefreshToutLeHUD();

        Debug.Log("GM Start end " + Time.realtimeSinceStartup);
    }

    private void OnDestroy()
    {
        hudController = null;
        equipeDetailController = null;
        explorationRecompensePopup = null;
    }

    private void OnValidate()
    {
        Joueur1?.SynchroniserCompagnieDepuisDirigeant();
        Joueur2?.SynchroniserCompagnieDepuisDirigeant();
        Joueur3?.SynchroniserCompagnieDepuisDirigeant();
    }

    private void InitialiserServices()
    {
        PlayerAccessService = new SYS_PlayerAccessService();
        TurnOrderService = new SYS_TurnOrderService();
        HudSyncService = new SYS_HudSyncService();
        PlayerInitializationService = new SYS_PlayerInitializationService();
        RecruitmentTurnStateService = new SYS_RecruitmentTurnStateService();

        RulesService = new SYS_GameRulesService();
        EquipeManagementService = new SYS_EquipeManagementService();

        RevenusSystem = new SYS_RevenusSystem();
        VictoireSystem = new SYS_VictoireSystem();

       InfluenceSystem = new SYS_InfluenceSystem();
UiSystem = new SYS_GameUiRefreshService();

ExplorationSystem = new ExplorationSystem(InfluenceSystem, UiSystem);
VadrouilleSystem = new SYS_VadrouilleSystem(InfluenceSystem, UiSystem);

InitializationService = new SYS_GameInitializationService();
TurnSystem = new SYS_TurnSystem(ExplorationSystem, VadrouilleSystem, InfluenceSystem, UiSystem);
    }

    private void InitialiserJoueurs()
    {
        Joueur1 ??= new DATA_JOUEUR();
        Joueur2 ??= new DATA_JOUEUR();
        Joueur3 ??= new DATA_JOUEUR();

        PlayerInitializationService.InitialiserJoueurs(Joueur1, Joueur2, Joueur3);
    }

    private void ConfigurerGenerateurPersonnages()
    {
        CALC_PERSONNAGE_Generator.spritesGeneriques = spritesGeneriquesPersonnages;
        CALC_PERSONNAGE_Generator.effetAffiniteRespectee = effetAffiniteRespectee;
        CALC_PERSONNAGE_Generator.effetAffiniteNonRespectee = effetAffiniteNonRespectee;
        CALC_PERSONNAGE_Generator.progressionConfigParDefaut = progressionConfigPersonnage;
    }

    public void RefreshDebugEquipesRuntimeView()
    {
        if (debugEquipesRuntimeView == null)
        {
            debugEquipesRuntimeView = GetComponent<SYS_DebugEquipesRuntimeView>();
        }

        debugEquipesRuntimeView?.RefreshDebugEquipesAdverses();
    }

    private void InitialiserRecrutement()
    {
        SYS_RecrutementSystem = new SYS_RecrutementSystem();
        SYS_RecrutementSystem.InitialiserDistributions();
        SYS_RecrutementSystem.InitialiserPool(poolRecrutementInitial);
        SYS_RecrutementSystem.GenererMarche(5);
    }

    public DATA_JOUEUR GetDATA_JOUEURByCompagnie(ENUM_Compagnie compagnie)
    {
        return PlayerAccessService.GetByCompagnie(compagnie, Joueur1, Joueur2, Joueur3);
    }

    public List<DATA_JOUEUR> GetAllPlayers()
    {
        return PlayerAccessService.GetAllPlayers(Joueur1, Joueur2, Joueur3);
    }

    public DATA_JOUEUR GetHumanPlayer()
    {
        Debug.Log(
    $"[HUMAIN] J1={Joueur1.nomJoueur} humain={Joueur1.estHumain} comp={Joueur1.compagnie} dirigeant={(Joueur1.Dirigeant != null ? Joueur1.Dirigeant.compagnie.ToString() : "null")} | " +
    $"J2={Joueur2.nomJoueur} humain={Joueur2.estHumain} comp={Joueur2.compagnie} dirigeant={(Joueur2.Dirigeant != null ? Joueur2.Dirigeant.compagnie.ToString() : "null")} | " +
    $"J3={Joueur3.nomJoueur} humain={Joueur3.estHumain} comp={Joueur3.compagnie} dirigeant={(Joueur3.Dirigeant != null ? Joueur3.Dirigeant.compagnie.ToString() : "null")}"
);
        return PlayerAccessService.GetHumanPlayer(Joueur1, Joueur2, Joueur3);
    }

    public DATA_JOUEUR GetJoueurActifTour()
    {
        return PlayerAccessService.GetJoueurActifTour(
            ordreTourCourant,
            indexJoueurActifTour,
            Joueur1,
            Joueur2,
            Joueur3
        );
    }

    public SCOBJ_DIRIGEANT GetDirigeantHumain()
    {
        return PlayerAccessService.GetDirigeantHumain(Joueur1, Joueur2, Joueur3);
    }

    public void SynchroniserHudAvecJoueurHumain()
    {
        DATA_JOUEUR humain = GetHumanPlayer();
        SCOBJ_DIRIGEANT dirigeantHumain = GetDirigeantHumain();

        HudSyncService.SynchroniserHudAvecJoueurHumain(
            joueurData,
            humain,
            dirigeantHumain
        );
    }

    public void SynchroniserCompagniesJoueursDepuisDirigeants()
    {
        PlayerAccessService.SynchroniserCompagniesDepuisDirigeants(Joueur1, Joueur2, Joueur3);
    }

    private void ReinitialiserDirigeants()
    {
        PlayerAccessService.ReinitialiserDirigeants(Joueur1, Joueur2, Joueur3);
    }

    public void RegisterProvince(STATE_PROVINCE province)
    {
        if (province == null)
            return;

        if (!ProvincesRuntime.Contains(province))
        {
            ProvincesRuntime.Add(province);
        }
    }

    private void AssignerEffetsAffiniteAutomatiques()
    {
        DATA_JOUEUR humain = GetHumanPlayer();

        if (poolRecrutementInitial != null && humain != null)
        {
            foreach (SCOBJ_Personnage personnage in poolRecrutementInitial)
            {
                if (personnage == null)
                    continue;

                PERSONNAGE_EFFET_AutoAssigner.AssignerEffetsAffinite(
                    personnage,
                    humain.compagnie,
                    effetAffiniteRespectee,
                    effetAffiniteNonRespectee
                );
            }
        }

        if (equipesDeDepart != null)
        {
            foreach (SCOBJ_EQUIPE equipe in equipesDeDepart)
            {
                if (equipe == null || equipe.membres == null)
                    continue;

                foreach (SCOBJ_Personnage personnage in equipe.membres)
                {
                    if (personnage == null)
                        continue;

                    PERSONNAGE_EFFET_AutoAssigner.AssignerEffetsAffinite(
                        personnage,
                        humain != null ? humain.compagnie : ENUM_Compagnie.Aucune,
                        effetAffiniteRespectee,
                        effetAffiniteNonRespectee
                    );
                }
            }
        }
    }

    public void TourSuivant()
    {
        if (TurnSystem == null)
        {
            Debug.LogWarning("TurnSystem introuvable.");
            return;
        }

        TurnSystem.TourSuivant(this);
    }

    public void DemarrerExploration(STATE_EQUIPE equipe, int dureeTours)
    {
        if (ExplorationSystem == null)
        {
            Debug.LogWarning("ExplorationSystem introuvable.");
            return;
        }

        ExplorationSystem.DemarrerExploration(this, equipe, equipe.compagnie, dureeTours);
        SynchroniserHudAvecJoueurHumain();
        RefreshToutLeHUD();
    }
public void DemarrerVadrouille(STATE_EQUIPE equipe)
{
    if (VadrouilleSystem == null)
    {
        Debug.LogWarning("VadrouilleSystem introuvable.");
        return;
    }

    if (equipe == null)
    {
        Debug.LogWarning("DemarrerVadrouille : équipe null.");
        return;
    }

    VadrouilleSystem.DemarrerVadrouille(this, equipe, equipe.compagnie);
    SynchroniserHudAvecJoueurHumain();
    RefreshToutLeHUD();
}
    public void RefreshToutLeHUD()
    {
        if (UiSystem == null)
        {
            Debug.LogWarning("UiSystem introuvable.");
            return;
        }

        SynchroniserCompagniesJoueursDepuisDirigeants();
        SynchroniserHudAvecJoueurHumain();
        UiSystem.RefreshToutLeHUD(this);
    }

    public int GetNombreEquipesJoueur(DATA_JOUEUR joueur)
    {
        return RulesService.GetNombreEquipesJoueur(joueur);
    }

    public int GetCoutCreationEquipe(DATA_JOUEUR joueur)
    {
        if (coutsCreationEquipes == null || coutsCreationEquipes.Count == 0)
            return 100;

        if (joueur == null)
            return coutsCreationEquipes[0];

        int nombreEquipesActuelles = GetNombreEquipesJoueur(joueur);
        int index = Mathf.Clamp(nombreEquipesActuelles, 0, coutsCreationEquipes.Count - 1);
        return coutsCreationEquipes[index];
    }

    public bool PeutCreerEquipe(DATA_JOUEUR joueur)
    {
        if (joueur == null)
            return false;

        return RulesService.PeutCreerEquipe(
            joueur,
            maxEquipesParJoueur,
            GetCoutCreationEquipe(joueur)
        );
    }

    public bool PeutCreerEquipeCeTour(DATA_JOUEUR joueur)
    {
        if (joueur == null)
            return false;

        return RulesService.PeutCreerEquipeCeTour(
            joueur,
            maxEquipesParJoueur,
            GetCoutCreationEquipe(joueur)
        );
    }

   public bool CreerEquipePourJoueurHumain()
{
    DATA_JOUEUR humain = GetHumanPlayer();

    if (humain == null)
    {
        Debug.LogWarning("Aucun joueur humain trouvé.");
        return false;
    }

    bool success = EquipeManagementService.CreerEquipePourJoueur(
        humain,
        EquipesRuntime,
        modeleEquipeVide,
        GetCoutCreationEquipe(humain),
        maxEquipesParJoueur,
        RulesService,
        ProgressionConfigEquipe,
        out STATE_EQUIPE _
    );

    if (!success)
        return false;

    SynchroniserHudAvecJoueurHumain();
    RefreshToutLeHUD();
    return true;
}

  public bool CreerEquipePourIA(DATA_JOUEUR joueur)
{
    if (joueur == null)
    {
        Debug.LogWarning("CreerEquipePourIA : joueur null.");
        return false;
    }

    if (EquipeManagementService == null || RulesService == null)
    {
        Debug.LogWarning("CreerEquipePourIA : services de gestion d'équipe indisponibles.");
        return false;
    }

    int coutCreation = GetCoutCreationEquipe(joueur);

    bool success = EquipeManagementService.CreerEquipePourJoueur(
        joueur,
        EquipesRuntime,
        modeleEquipeVide,
        coutCreation,
        maxEquipesParJoueur,
        RulesService,
        ProgressionConfigEquipe,
        out STATE_EQUIPE nouvelleEquipe
    );

    if (!success)
        return false;

    Debug.Log(
        $"[IA] Nouvelle équipe créée | joueur={joueur.nomJoueur} | " +
        $"compagnie={joueur.compagnie} | " +
        $"équipe={nouvelleEquipe?.data?.nomEquipe} | " +
        $"coût={coutCreation}"
    );

    return true;
}

    public bool PeutRecruterCeTour(DATA_JOUEUR joueur)
    {
        return RecruitmentTurnStateService.PeutRecruterCeTour(
            joueur,
            compagniesAyantRecruteCeTour
        );
    }

    public bool PeutRecruterCeTour()
    {
        DATA_JOUEUR humain = GetHumanPlayer();
        return PeutRecruterCeTour(humain);
    }

    public void MarquerRecrutementEffectue(DATA_JOUEUR joueur)
    {
        RecruitmentTurnStateService.MarquerRecrutementEffectue(
            joueur,
            compagniesAyantRecruteCeTour
        );
    }

    public void MarquerRecrutementEffectue()
    {
        DATA_JOUEUR humain = GetHumanPlayer();
        MarquerRecrutementEffectue(humain);
    }

    public void ResetRecrutementTour()
    {
        RecruitmentTurnStateService.ResetRecrutementTour(
            compagniesAyantRecruteCeTour
        );
    }

    public bool EstPersonnageDansUneEquipe(SCOBJ_Personnage personnage)
    {
        return RulesService.EstPersonnageDansUneEquipe(personnage, EquipesRuntime);
    }

    public void ConstruireOrdreTourSelonPrestige()
    {
        TurnOrderService.ConstruireOrdreTourSelonPrestige(
            GetAllPlayers(),
            ordreTourCourant,
            ref indexJoueurActifTour
        );
    }

    public bool PasserAuJoueurSuivantDansLeTour()
    {
        return TurnOrderService.PasserAuJoueurSuivantDansLeTour(
            ordreTourCourant,
            ref indexJoueurActifTour
        );
    }
    public DATA_JOUEUR GetJoueurProprietaireEquipe(STATE_EQUIPE equipe)
{
    if (equipe == null)
        return null;

    if (Joueur1 != null && Joueur1.equipes != null && Joueur1.equipes.Contains(equipe))
        return Joueur1;

    if (Joueur2 != null && Joueur2.equipes != null && Joueur2.equipes.Contains(equipe))
        return Joueur2;

    if (Joueur3 != null && Joueur3.equipes != null && Joueur3.equipes.Contains(equipe))
        return Joueur3;

    return GetDATA_JOUEURByCompagnie(equipe.compagnie);
}
}