using UnityEngine;

public static class SVC_IA_ActionExecutionService
{
    private const bool DebugIAActions = true;

    public static void LancerActions(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
         Debug.Log(
        $"[IA_ACTION_ENTER] joueur={(joueur != null ? joueur.nomJoueur : "null")} | " +
        $"humain={(joueur != null && joueur.estHumain)} | " +
        $"equipes={(joueur != null && joueur.equipes != null ? joueur.equipes.Count : -1)}"
    );
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = SVC_IA_EquipeRosterService.GetTailleMinEquipePourAction(joueur);
        float ratioMinimalBudget = SVC_IA_PersonnaliteResolver.GetRatioMinimalBudgetPourAction(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            string nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Équipe IA";
            string nomProvince = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
                ? equipe.provinceAffectee.data.nom
                : "Aucune";
    Debug.Log(
        $"[IA_ACTION_CHECK] joueur={joueur.nomJoueur} | equipe={nomEquipe} | " +
        $"membres={(equipe.membresActuels != null ? equipe.membresActuels.Count : -1)} | " +
        $"province={nomProvince} | actionEnCours={equipe.AUneActionEnCours} | " +
        $"actionPrincipale={SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe)} | " +
        $"etrinium={joueur.etrinium}"
    );
            if (equipe.AUneActionEnCours)
            {
                LogSkip(joueur, nomEquipe, nomProvince, $"action déjà en cours : {equipe.actionEnCours}");
                continue;
            }

            if (equipe.provinceAffectee == null || equipe.provinceAffectee.data == null)
            {
                LogSkip(joueur, nomEquipe, nomProvince, "aucune province affectée");
                continue;
            }

            int nbMembres = equipe.membresActuels != null ? equipe.membresActuels.Count : 0;
            if (nbMembres < tailleMinEquipe)
            {
                LogSkip(joueur, nomEquipe, nomProvince, $"pas assez de membres : {nbMembres}/{tailleMinEquipe}");
                continue;
            }

            ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

            if (action == ENUM_EQUIPE_ACTION.Aucune)
            {
                LogSkip(joueur, nomEquipe, nomProvince, "action principale = Aucune");
                continue;
            }

            bool peutLancer = SVC_EQUIPE_ActionLaunchService.PeutLancerAction(equipe, action);
            if (!peutLancer)
            {
                float exploration = equipe.provinceAffectee.GetExploration(equipe.compagnie);

                LogSkip(
                    joueur,
                    nomEquipe,
                    nomProvince,
                    $"PeutLancerAction=false | action={action} | exploration={exploration:0.#}%"
                );

                continue;
            }

            int coutAction = CalculerCoutAction(gameManager, equipe, action);

            if (coutAction <= 0)
            {
                LogSkip(joueur, nomEquipe, nomProvince, $"coût action invalide : {coutAction} | action={action}");
                continue;
            }

            if (joueur.etrinium < coutAction)
            {
                LogSkip(
                    joueur,
                    nomEquipe,
                    nomProvince,
                    $"pas assez d'étrinium : {joueur.etrinium:0} / {coutAction} | action={action}"
                );

                continue;
            }

            if (joueur.etriniumParTour < 0f && joueur.etrinium < coutAction * ratioMinimalBudget)
            {
                LogSkip(
                    joueur,
                    nomEquipe,
                    nomProvince,
                    $"budget de sécurité insuffisant : etrinium={joueur.etrinium:0}, coût={coutAction}, ratio={ratioMinimalBudget}"
                );

                continue;
            }

            LogLaunch(joueur, nomEquipe, nomProvince, action, coutAction);

            switch (action)
            {
                case ENUM_EQUIPE_ACTION.Vadrouille:
                    gameManager.DemarrerVadrouille(equipe);
                    break;

                case ENUM_EQUIPE_ACTION.Exploration:
                    gameManager.DemarrerExploration(equipe, 0);
                    break;

                case ENUM_EQUIPE_ACTION.Construction:
                    Debug.LogWarning($"[IA_ACTION] Construction choisie mais non implémentée | joueur={joueur.nomJoueur} | équipe={nomEquipe}");
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
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.enclavement);

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

    private static void LogSkip(DATA_JOUEUR joueur, string equipe, string province, string raison)
    {
        if (!DebugIAActions)
            return;

        Debug.Log(
            $"[IA_ACTION_SKIP] joueur={joueur?.nomJoueur} | équipe={equipe} | province={province} | raison={raison}"
        );
    }

    private static void LogLaunch(DATA_JOUEUR joueur, string equipe, string province, ENUM_EQUIPE_ACTION action, int cout)
    {
        if (!DebugIAActions)
            return;

        Debug.Log(
            $"[IA_ACTION_LAUNCH] joueur={joueur?.nomJoueur} | équipe={equipe} | province={province} | action={action} | coût={cout}"
        );
    }
}