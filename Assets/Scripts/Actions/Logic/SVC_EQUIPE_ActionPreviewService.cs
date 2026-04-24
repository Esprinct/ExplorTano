using UnityEngine;

public static class SVC_EQUIPE_ActionPreviewService
{
    public static DATA_EQUIPE_ActionPreview BuildPreview(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager)
    {
        DATA_EQUIPE_ActionPreview preview = new DATA_EQUIPE_ActionPreview();

        bool provinceAffectee =
            equipe != null &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null;

        if (!provinceAffectee || gameManager == null)
        {
            preview.afficher = false;
            return preview;
        }

        preview.afficher = true;

        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Exploration:
                return BuildExplorationPreview(equipe, gameManager);

            case ENUM_EQUIPE_ACTION.Vadrouille:
                return BuildVadrouillePreview(equipe, gameManager);

            case ENUM_EQUIPE_ACTION.Construction:
                return BuildConstructionPreview(equipe, gameManager);

            default:
                preview.afficher = false;
                return preview;
        }
    }

    private static DATA_EQUIPE_ActionPreview BuildExplorationPreview(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager)
    {
        DATA_EQUIPE_ActionPreview preview = new DATA_EQUIPE_ActionPreview
        {
            afficher = true,
            titreAction = "Exploration"
        };

        if (gameManager.ExplorationConfig == null)
            return preview;

        ExplorationConfig config = gameManager.ExplorationConfig;
        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        float explorationActuelle = 0f;

        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
            explorationActuelle = equipe.provinceAffectee.GetExploration(equipe.compagnie);
        }

        int toursModifies = SVC_EQUIPE_ExplorationEffects.GetToursBaseModifies(
            equipe,
            joueur,
            config.toursBase
        );

        float chanceArtefactModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactModifiee(
            equipe,
            joueur,
            config.chanceArtefactBase
        );

        float chanceArtefactRareModifiee = SVC_EQUIPE_ExplorationEffects.GetChanceArtefactRareModifiee(
            equipe,
            joueur,
            config.chanceArtefactRareBase
        );

        float gainExploration = SVC_EQUIPE_ExplorationEffects.GetGainExplorationFinal(
            equipe,
            joueur,
            config.gainExplorationBase
        );

        float explorationProjetee = Mathf.Clamp(explorationActuelle + gainExploration, 0f, 100f);

        DATA_EXPLORATION_Resultat result = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            chanceArtefactModifiee,
            chanceArtefactRareModifiee,
            enclavement
        );

        if (result == null)
            return preview;

        bool actionEnCours =
            equipe.AUneActionEnCours &&
            equipe.actionEnCours == ENUM_EQUIPE_ACTION.Exploration;

        preview.toursEnCoursText = actionEnCours
            ? $"Exploration : {equipe.actionToursRestants}"
            : "-";

        preview.prestigeText = $"+{result.prestigeFinal} Prestige";
        preview.etriniumText = $"-{result.coutTotal} coût total";
        preview.dureeText = $"Durée : {result.toursFinaux} tours";
        preview.chancePrincipaleText = $"Artefact : {result.chanceRelique:0.#}%";
        preview.chanceSecondaireText = $"Artefact rare : {result.chanceReliqueRare:0.#}%";
        preview.impactText =
            $"Exploration : {explorationActuelle:0.#}% → {explorationProjetee:0.#}%";

        return preview;
    }

    private static DATA_EQUIPE_ActionPreview BuildVadrouillePreview(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager)
    {
        DATA_EQUIPE_ActionPreview preview = new DATA_EQUIPE_ActionPreview
        {
            afficher = true,
            titreAction = "Vadrouille"
        };

        if (gameManager.VadrouilleConfig == null)
            return preview;

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

        DATA_VADROUILLE_Resultat result = CALC_VADROUILLE_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            gainOccupation,
            reductionAdverse
        );

        if (result == null)
            return preview;

        bool actionEnCours =
            equipe.AUneActionEnCours &&
            equipe.actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille;

        float influenceActuelle = CalculerPourcentageInfluenceJoueurDansProvince(equipe);
        float influenceProjetee = Mathf.Clamp(influenceActuelle + result.gainOccupationFinal, 0f, 100f);

        preview.toursEnCoursText = actionEnCours
            ? $"Vadrouille : {equipe.actionToursRestants}"
            : "-";

        preview.prestigeText = $"+{result.prestigeFinal} Prestige";
        preview.etriniumText = $"-{result.coutTotal} coût total";
        preview.dureeText = $"Durée : {result.toursFinaux} tours";
        preview.chancePrincipaleText = $"+Occupation : {result.gainOccupationFinal:0.#}%";
        preview.chanceSecondaireText = $"-Adverse : {result.reductionOccupationAdverseFinal:0.#}%";
        preview.impactText =
            $"Occupation : {influenceActuelle:0.#}% → {influenceProjetee:0.#}%";

        return preview;
    }

    private static DATA_EQUIPE_ActionPreview BuildConstructionPreview(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager)
    {
        return new DATA_EQUIPE_ActionPreview
        {
            afficher = true,
            titreAction = "Construction",
            toursEnCoursText = equipe.AUneActionEnCours && equipe.actionEnCours == ENUM_EQUIPE_ACTION.Construction
                ? $"Construction : {equipe.actionToursRestants}"
                : "-",
            prestigeText = "-",
            etriniumText = "-",
            dureeText = "Construction non branchée",
            chancePrincipaleText = "-",
            chanceSecondaireText = "-",
            impactText = "-"
        };
    }

    private static float CalculerPourcentageInfluenceJoueurDansProvince(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.provinceAffectee == null)
            return 0f;

        STATE_PROVINCE province = equipe.provinceAffectee;

        float totalInfluence =
            province.influenceMaizin +
            province.influenceKinia +
            province.influenceJoho +
            province.influenceAutre;

        if (totalInfluence <= 0f)
            return 0f;

        float influenceJoueur = 0f;

        switch (equipe.compagnie)
        {
            case ENUM_Compagnie.Maizin:
                influenceJoueur = province.influenceMaizin;
                break;
            case ENUM_Compagnie.Kinia:
                influenceJoueur = province.influenceKinia;
                break;
            case ENUM_Compagnie.Joho:
                influenceJoueur = province.influenceJoho;
                break;
        }

        return (influenceJoueur / totalInfluence) * 100f;
    }
}