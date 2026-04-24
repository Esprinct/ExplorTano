using UnityEngine;

public static class SVC_IA_ActionExecutionService
{
    public static void LancerActions(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = SVC_IA_EquipeRosterService.GetTailleMinEquipePourAction(joueur);
        float ratioMinimalBudget = SVC_IA_PersonnaliteResolver.GetRatioMinimalBudgetPourAction(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.AUneActionEnCours)
                continue;

            if (equipe.provinceAffectee == null || equipe.provinceAffectee.data == null)
                continue;

            if (equipe.membresActuels == null || equipe.membresActuels.Count < tailleMinEquipe)
                continue;

            ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

            if (action == ENUM_EQUIPE_ACTION.Aucune)
                continue;

            bool peutLancer = SVC_EQUIPE_ActionLaunchService.PeutLancerAction(equipe, action);
            if (!peutLancer)
                continue;

            int coutAction = CalculerCoutAction(gameManager, equipe, action);
            if (coutAction <= 0 || joueur.etrinium < coutAction)
                continue;

            if (joueur.etriniumParTour < 0f && joueur.etrinium < coutAction * ratioMinimalBudget)
                continue;

            switch (action)
            {
                case ENUM_EQUIPE_ACTION.Vadrouille:
                    gameManager.DemarrerVadrouille(equipe);
                    break;

                case ENUM_EQUIPE_ACTION.Exploration:
                    gameManager.DemarrerExploration(equipe, 0);
                    break;

                case ENUM_EQUIPE_ACTION.Construction:
                    break;
            }
        }
    }

    private static int CalculerCoutAction(SYS_GameManager gameManager, STATE_EQUIPE equipe, ENUM_EQUIPE_ACTION action)
    {
        if (gameManager == null || equipe == null)
            return 0;

        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Exploration:
                return CalculerCoutExploration(gameManager, equipe);

            case ENUM_EQUIPE_ACTION.Vadrouille:
                return CalculerCoutVadrouille(gameManager, equipe);

            default:
                return 0;
        }
    }

    private static int CalculerCoutExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null || gameManager.ExplorationConfig == null)
            return 0;

        ExplorationConfig config = gameManager.ExplorationConfig;
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);

        DATA_EXPLORATION_Resultat resultat = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            config.toursBase,
            config.coutParTourBase,
            config.prestigeBase,
            config.chanceArtefactBase,
            config.chanceArtefactRareBase,
            enclavement
        );

        return resultat != null ? resultat.coutTotal : 0;
    }

    private static int CalculerCoutVadrouille(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null || gameManager.VadrouilleConfig == null)
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
}