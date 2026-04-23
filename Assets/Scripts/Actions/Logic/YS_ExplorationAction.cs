using UnityEngine;

public class SYS_ExplorationAction : SYS_EquipeActionBase
{
    public SYS_ExplorationAction(SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Exploration;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        ExplorationConfig config = gameManager.ExplorationConfig;
        if (config == null)
            return;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
        }

        ENUM_EXPLORATION_Resultat resultat = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            config.toursBase,
            config.coutParTourBase,
            config.prestigeBase,
            config.chanceArtefactBase,
            config.chanceArtefactRareBase,
            enclavement
        );

        if (resultat == null)
            return;

        equipe.resultatExploration = resultat;
        InitialiserAction(equipe, TypeAction, resultat.toursFinaux);

        Debug.Log(
            $"[DEMARRAGE_EXPLORATION] equipe={equipe.data?.nomEquipe} | " +
            $"tours={equipe.actionToursRestants}/{equipe.actionToursTotaux}"
        );

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

        uiSystem?.RefreshToutLeHUD(gameManager);
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        // TODO: déplacer ici la récompense complète d’exploration existante
        CloturerAction(equipe);
    }
}