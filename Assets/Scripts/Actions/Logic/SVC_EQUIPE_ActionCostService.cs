using UnityEngine;

public static class SVC_EQUIPE_ActionCostService
{
    public static int CalculerCoutActionCourante(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager,
        int dureeExplorationParDefaut)
    {
        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                return CalculerCoutLancementVadrouille(equipe, gameManager);

            case ENUM_EQUIPE_ACTION.Exploration:
                return CalculerCoutLancementExploration(equipe, gameManager, dureeExplorationParDefaut);

            case ENUM_EQUIPE_ACTION.Construction:
            default:
                return 0;
        }
    }

    public static int CalculerCoutLancementExploration(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager,
        int dureeExplorationParDefaut)
    {
        if (equipe == null || equipe.data == null || gameManager == null)
            return 0;

        ExplorationConfig config = gameManager.ExplorationConfig;

        int toursBase = config != null ? config.toursBase : dureeExplorationParDefaut;
        int coutParTourBase = config != null ? config.coutParTourBase : 5;
        int prestigeBase = config != null ? config.prestigeBase : 1;
        float chanceArtefactBase = config != null ? config.chanceArtefactBase : 10f;
        float chanceArtefactRareBase = config != null ? config.chanceArtefactRareBase : 2f;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.enclavement);

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);

        int toursModifies = SVC_EQUIPE_ExplorationEffects.GetToursBaseModifies(
            equipe,
            joueur,
            toursBase
        );

        float chanceArtefactModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactModifiee(
            equipe,
            joueur,
            chanceArtefactBase
        );

        float chanceArtefactRareModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactRareModifiee(
            equipe,
            joueur,
            chanceArtefactRareBase
        );

        DATA_EXPLORATION_Resultat result = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursModifies,
            coutParTourBase,
            prestigeBase,
            chanceArtefactModifiee,
            chanceArtefactRareModifiee,
            enclavement
        );

        return result != null ? result.coutTotal : 0;
    }

    public static int CalculerCoutLancementVadrouille(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager)
    {
        if (equipe == null || gameManager == null || gameManager.VadrouilleConfig == null)
            return 0;

        VadrouilleConfig config = gameManager.VadrouilleConfig;
        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int toursModifies = SVC_EQUIPE_VadrouilleEffects.GetToursVadrouilleFinals(
            equipe,
            joueur,
            config.toursBase
        );

        float gainOccupation = SVC_EQUIPE_VadrouilleEffects.GetGainOccupationFinal(
            equipe,
            joueur,
            config.gainOccupationBase
        );

        float reductionAdverse = SVC_EQUIPE_VadrouilleEffects.GetReductionOccupationAdverseFinal(
            equipe,
            joueur,
            config.reductionOccupationAdverseBase
        );

        DATA_VADROUILLE_Resultat resultat = CALC_VADROUILLE_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            gainOccupation,
            reductionAdverse
        );

        return resultat != null ? resultat.coutTotal : 0;
    }

    public static bool JoueurHumainAPasAssezFonds(
        SYS_GameManager gameManager,
        int coutLancement)
    {
        if (gameManager == null)
            return true;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return true;

        return humain.etrinium < coutLancement;
    }
}