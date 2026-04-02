using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Données de départ")]
    [SerializeField] private List<EquipeData> equipesDeDepart = new();

    [Header("Références UI")]
    [SerializeField] private HudController hudController;
    [SerializeField] private EquipeMenuController equipeMenuController;

    [Header("Données runtime affichables")]
    [SerializeField] private JoueurHudData joueurData = new();
    [SerializeField] private PartieHudData partieData = new();

    [Header("Joueurs")]
    public PlayerData Joueur1;
    public PlayerData Joueur2;
    public PlayerData Joueur3;

    public List<EquipeState> EquipesRuntime { get; private set; } = new();
    public List<ProvinceState> ProvincesRuntime { get; } = new();

    public JoueurHudData JoueurData => joueurData;
    public PartieHudData PartieData => partieData;

    public HudController HudController
    {
        get => hudController;
        set => hudController = value;
    }

    public EquipeMenuController EquipeMenuController
    {
        get => equipeMenuController;
        set => equipeMenuController = value;
    }

    public bool PartieTerminee { get; set; }

    public GameInitializationService InitializationService { get; private set; }
    public TurnSystem TurnSystem { get; private set; }
    public ExplorationSystem ExplorationSystem { get; private set; }
    public InfluenceSystem InfluenceSystem { get; private set; }
    public GameUiSystem UiSystem { get; private set; }

    private void Awake()
    {
        Debug.Log("GM Awake start " + Time.realtimeSinceStartup);
        InitialiserJoueurs();
        partieData.tourMax = 999; 
        InfluenceSystem = new InfluenceSystem();
        UiSystem = new GameUiSystem();
        ExplorationSystem = new ExplorationSystem(InfluenceSystem, UiSystem);
        InitializationService = new GameInitializationService();
        TurnSystem = new TurnSystem(ExplorationSystem, InfluenceSystem, UiSystem);

        InitializationService.InitialiserPartie(this, equipesDeDepart);
        SynchroniserHudAvecJoueurHumain();
         Debug.Log("GM Awake end " + Time.realtimeSinceStartup);
    }

    private void Start()
    {
        Debug.Log("GM Start begin " + Time.realtimeSinceStartup);
        if (TurnSystem != null)
        {
            TurnSystem.RecalculerRevenusSeulement(this);
        }

        SynchroniserHudAvecJoueurHumain();
        RefreshToutLeHUD();
         Debug.Log("GM Start end " + Time.realtimeSinceStartup);
    }

    private void OnDestroy()
    {
        hudController = null;
        equipeMenuController = null;
    }

    private void InitialiserJoueurs()
    {
        Joueur1 = new PlayerData
        {
            nomJoueur = "Joueur 1",
            estHumain = true,
            compagnie = Compagnie.Maizin,
            etrinium = joueurData != null ? joueurData.etriniumTotal : 0f,
            prestige = joueurData != null ? joueurData.prestige : 0f
        };

        Joueur2 = new PlayerData
        {
            nomJoueur = "Joueur 2",
            estHumain = false,
            compagnie = Compagnie.Kinia,
            etrinium = 0f,
            prestige = 0f
        };

        Joueur3 = new PlayerData
        {
            nomJoueur = "Joueur 3",
            estHumain = false,
            compagnie = Compagnie.Joho,
            etrinium = 0f,
            prestige = 0f
        };
    }

    public PlayerData GetPlayerDataByCompagnie(Compagnie compagnie)
    {
        if (Joueur1 != null && Joueur1.compagnie == compagnie)
            return Joueur1;

        if (Joueur2 != null && Joueur2.compagnie == compagnie)
            return Joueur2;

        if (Joueur3 != null && Joueur3.compagnie == compagnie)
            return Joueur3;

        return null;
    }

    public PlayerData GetHumanPlayer()
    {
        if (Joueur1 != null && Joueur1.estHumain) return Joueur1;
        if (Joueur2 != null && Joueur2.estHumain) return Joueur2;
        if (Joueur3 != null && Joueur3.estHumain) return Joueur3;

        return null;
    }

    public void SynchroniserHudAvecJoueurHumain()
    {
        PlayerData humain = GetHumanPlayer();

        if (humain == null || joueurData == null)
        {
            return;
        }

        joueurData.etriniumTotal = Mathf.RoundToInt(humain.etrinium);
        joueurData.prestige = Mathf.RoundToInt(humain.prestige);
        joueurData.provincesControlees = humain.provincesControlees;
    }

    public void RegisterProvince(ProvinceState province)
    {
        if (province == null)
        {
            return;
        }

        if (!ProvincesRuntime.Contains(province))
        {
            ProvincesRuntime.Add(province);
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

    public void DemarrerExploration(EquipeState equipe, int dureeTours)
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

    public void RefreshToutLeHUD()
    {
        if (UiSystem == null)
        {
            Debug.LogWarning("UiSystem introuvable.");
            return;
        }

        SynchroniserHudAvecJoueurHumain();
        UiSystem.RefreshToutLeHUD(this);
    }
}