using UnityEngine;

public class SYS_ExplorationAction : SYS_EquipeActionBase
{
    private readonly SYS_InfluenceSystem influenceSystem;

    public SYS_ExplorationAction(
        SYS_InfluenceSystem influenceSystem,
        SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
        this.influenceSystem = influenceSystem;
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Exploration;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null || gameManager.ExplorationConfig == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        if (joueur == null)
            return;

        ExplorationConfig config = gameManager.ExplorationConfig;
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
        }

        int toursModifies = SVC_EQUIPE_ExplorationEffects.GetToursBaseModifies(
            equipe,
            joueur,
            config.toursBase
        );

        float chanceArtefact = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactModifiee(
            equipe,
            joueur,
            config.chanceArtefactBase
        );

        float chanceArtefactRare = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactRareModifiee(
            equipe,
            joueur,
            config.chanceArtefactRareBase
        );

        ENUM_EXPLORATION_Resultat resultat = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            chanceArtefact,
            chanceArtefactRare,
            enclavement
        );

        if (resultat == null)
            return;

        if (joueur.etrinium < resultat.coutTotal)
            return;

        joueur.etrinium -= resultat.coutTotal;
        equipe.resultatExploration = resultat;

        InitialiserAction(equipe, TypeAction, resultat.toursFinaux);

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);
    }

    public override void MettreAJour(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.EquipesRuntime == null)
            return;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (!PeutTraiter(equipe))
                continue;

            equipe.actionToursRestants--;

            if (equipe.actionToursRestants <= 0)
                Terminer(gameManager, equipe);
        }
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        // Branche ici ta logique actuelle de fin d'exploration :
        // gain influence / prestige / artefacts / popup.
        // Je garde le point d’extension pour ne pas te casser du code métier existant.

        CloturerAction(equipe);
        equipe.resultatExploration = null;

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);
    }
}