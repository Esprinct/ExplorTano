using UnityEngine;

public static class SVC_IA_ProvinceStrategyService
{
    private const float ExplorationCompleteThreshold = 99.99f;

    public static void AffecterEquipesAuxProvinces(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = SVC_IA_EquipeRosterService.GetTailleMinEquipePourAction(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.AUneActionEnCours)
                continue;

            ConvertirExplorationEnVadrouilleSiNecessaire(gameManager, joueur, equipe);

            if (equipe.membresActuels == null || equipe.membresActuels.Count < tailleMinEquipe)
                continue;

            if (PeutGarderProvinceActuelle(gameManager, joueur, equipe))
                continue;

            STATE_PROVINCE cible = ChoisirProvincePourEquipe(gameManager, joueur, equipe);

            if (cible == null)
            {
                equipe.provinceAffectee = null;
                equipe.actionTerminee = false;

                Debug.Log(
                    $"[IA_PROVINCE_CLEAR] joueur={joueur.nomJoueur} | " +
                    $"équipe={equipe.data?.nomEquipe} | " +
                    $"raison=aucune province valide trouvée"
                );

                continue;
            }

            equipe.provinceAffectee = cible;
            equipe.actionTerminee = false;

            Debug.Log(
                $"[IA_PROVINCE_ASSIGN] joueur={joueur.nomJoueur} | " +
                $"équipe={equipe.data?.nomEquipe} | " +
                $"province={cible.data?.nom} | " +
                $"action={SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe)} | " +
                $"exploration={cible.GetExploration(joueur.compagnie):0.##}%"
            );
        }
    }

    private static void ConvertirExplorationEnVadrouilleSiNecessaire(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (gameManager == null || joueur == null || equipe == null)
            return;

        if (equipe.AUneActionEnCours)
            return;

        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

        if (action != ENUM_EQUIPE_ACTION.Exploration)
            return;

        bool provinceActuelleTerminee = false;

        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            float explorationActuelle = equipe.provinceAffectee.GetExploration(joueur.compagnie);
            provinceActuelleTerminee = explorationActuelle >= ExplorationCompleteThreshold;

            if (provinceActuelleTerminee)
            {
                Debug.Log(
                    $"[IA_EXPLORATION_COMPLETE] joueur={joueur.nomJoueur} | " +
                    $"équipe={equipe.data?.nomEquipe} | " +
                    $"province={equipe.provinceAffectee.data?.nom} | " +
                    $"exploration={explorationActuelle:0.##}%"
                );

                equipe.provinceAffectee = null;
                equipe.actionTerminee = false;
            }
        }

        bool existeProvincePourVadrouille = ExisteProvinceInteressantePourVadrouille(gameManager, joueur);
        bool existeProvincePourExploration = ExisteProvinceInteressantePourExploration(gameManager, joueur);

        if (!existeProvincePourVadrouille)
            return;

        bool doitBasculerEnVadrouille =
            provinceActuelleTerminee ||
            !existeProvincePourExploration ||
            equipe.provinceAffectee == null;

        if (!doitBasculerEnVadrouille)
            return;

        equipe.specialisation = ENUM_EQUIPE_SPECIALISATION.Miliciens;
        equipe.dataSpecialisation = null;
        equipe.provinceAffectee = null;
        equipe.actionTerminee = false;

        Debug.Log(
            $"[IA_SPECIALISATION_SWITCH] joueur={joueur.nomJoueur} | " +
            $"équipe={equipe.data?.nomEquipe} | " +
            $"nouvelle spécialisation=Miliciens | " +
            $"raison=vadrouille prioritaire après exploration"
        );
    }

    private static bool PeutGarderProvinceActuelle(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (gameManager == null || joueur == null || equipe == null || equipe.provinceAffectee == null)
            return false;

        STATE_PROVINCE province = equipe.provinceAffectee;

        if (province.data == null)
            return false;

        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Exploration:
            {
                float exploration = province.GetExploration(joueur.compagnie);
                return exploration < ExplorationCompleteThreshold;
            }

            case ENUM_EQUIPE_ACTION.Vadrouille:
            {
                float exploration = province.GetExploration(joueur.compagnie);

                if (exploration < ExplorationCompleteThreshold)
                    return false;

                bool claimParIA =
                    province.estClaim &&
                    province.proprietaireActuel.HasValue &&
                    province.proprietaireActuel.Value == joueur.compagnie;

                if (claimParIA)
                    return false;

                float influenceIA = GetInfluenceCompagnie(province, joueur.compagnie);
                float influenceAdverse = GetInfluenceAdverseDominante(province, joueur.compagnie);

                if (!province.estClaim)
                    return true;

                if (influenceAdverse > 0.01f)
                    return true;

                if (influenceIA > 0f && influenceIA < 60f)
                    return true;

                return false;
            }

            case ENUM_EQUIPE_ACTION.Construction:
            {
                return province.estClaim &&
                       province.proprietaireActuel.HasValue &&
                       province.proprietaireActuel.Value == joueur.compagnie;
            }

            default:
                return false;
        }
    }

    private static STATE_PROVINCE ChoisirProvincePourEquipe(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (gameManager == null || joueur == null || equipe == null)
            return null;

        STATE_PROVINCE meilleureProvince = null;
        float meilleurScore = float.MinValue;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float score = EvaluerProvincePourCompagnie(gameManager, province, joueur, equipe);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleureProvince = province;
            }
        }

        return meilleureProvince;
    }

    private static bool ExisteProvinceInteressantePourExploration(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return false;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float exploration = province.GetExploration(joueur.compagnie);

            if (exploration < ExplorationCompleteThreshold)
                return true;
        }

        return false;
    }

    private static bool ExisteProvinceInteressantePourVadrouille(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return false;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float exploration = province.GetExploration(joueur.compagnie);

            if (exploration < ExplorationCompleteThreshold)
                continue;

            bool claimParIA =
                province.estClaim &&
                province.proprietaireActuel.HasValue &&
                province.proprietaireActuel.Value == joueur.compagnie;

            if (claimParIA)
                continue;

            return true;
        }

        return false;
    }

    private static float EvaluerProvincePourCompagnie(
        SYS_GameManager gameManager,
        STATE_PROVINCE province,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (province == null || province.data == null || joueur == null || equipe == null)
            return float.MinValue;

        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);

        float prestige = province.data.prestige;
        float accessibilite = province.data.accesibilite;
float etrinium = province.data.etrinium;
int provincesControlees = Mathf.Max(0, joueur.provincesControlees);

// Plus l'IA a déjà de provinces, plus elle cherche à snowball.
// Exemple : 0 province = x1, 5 provinces = x2.5, 10 provinces = x4.
float multiplicateurExpansion = 1f + (provincesControlees * 0.30f);
        float influenceIA = GetInfluenceCompagnie(province, joueur.compagnie);
        float influenceAdverse = GetInfluenceAdverseDominante(province, joueur.compagnie);
        float exploration = province.GetExploration(joueur.compagnie);

        bool estClaim = province.estClaim;

        bool claimParEnnemi =
            estClaim &&
            province.proprietaireActuel.HasValue &&
            province.proprietaireActuel.Value != joueur.compagnie;

        bool claimParIA =
            estClaim &&
            province.proprietaireActuel.HasValue &&
            province.proprietaireActuel.Value == joueur.compagnie;

        int nbEquipesAllieesDejaSurPlace =
            CompterEquipesAllieesSurProvince(gameManager, joueur.compagnie, province, equipe);

        float score = 0f;

        switch (action)
        {
case ENUM_EQUIPE_ACTION.Exploration:
{
    if (exploration >= ExplorationCompleteThreshold)
        return float.MinValue;

    score += (100f - exploration) * 1.2f;
    score += etrinium * 3.0f;
    score += prestige * 0.6f;
    score += accessibilite * 0.3f;

    if (!estClaim)
    {
        score += provincesControlees * 120f;
    }

    if (nbEquipesAllieesDejaSurPlace > 0)
    {
        score -= 3000f * nbEquipesAllieesDejaSurPlace;
    }

    break;
}
       case ENUM_EQUIPE_ACTION.Vadrouille:
{
    if (exploration < ExplorationCompleteThreshold)
        return float.MinValue;

    if (claimParIA)
        return float.MinValue;

    if (nbEquipesAllieesDejaSurPlace > 0)
    {
        score -= 5000f * nbEquipesAllieesDejaSurPlace;
    }

    // Base : la vadrouille est l'action de conquête.
    score += 600f;

    // Une province neutre explorée est une opportunité directe d'expansion.
    if (!estClaim)
        score += 500f * multiplicateurExpansion;

    // Une province ennemie peut valoir cher si l'IA est déjà dominante.
    if (claimParEnnemi)
        score += 250f * multiplicateurExpansion;

    // Énorme bonus de snowball :
    // plus l'IA possède de provinces, plus chaque nouvelle province devient prioritaire.
    score += provincesControlees * 700f;

    // Bonus supplémentaire si la province peut devenir un claim rapidement.
    float influenceManquantePourClaim = Mathf.Max(0f, 50f - influenceIA);
    score += (50f - influenceManquantePourClaim) * 12f * multiplicateurExpansion;

    // Rentabilité économique.
    score += etrinium * 4.0f;

    // Combat d'influence.
    score += influenceAdverse * 6.0f;
    score += influenceIA * 2.0f;

    score += prestige * 1.8f;
    score += accessibilite * 0.6f;

    break;
}

            case ENUM_EQUIPE_ACTION.Construction:
            {
                if (!claimParIA)
                    return float.MinValue;

                score += prestige * 1.0f;
                score += accessibilite * 0.8f;

                score -= nbEquipesAllieesDejaSurPlace * 5000f;

                break;
            }

            default:
                return float.MinValue;
        }

        score += Random.Range(0f, 8f);
        return score;
    }

    private static int CompterEquipesAllieesSurProvince(
        SYS_GameManager gameManager,
        ENUM_Compagnie compagnie,
        STATE_PROVINCE province,
        STATE_EQUIPE equipeIgnoree)
    {
        if (gameManager == null || province == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || equipe == equipeIgnoree)
                continue;

            if (equipe.compagnie != compagnie)
                continue;

            if (equipe.provinceAffectee == province)
                total++;
        }

        return total;
    }

    private static float GetInfluenceAdverseDominante(
        STATE_PROVINCE province,
        ENUM_Compagnie compagnie)
    {
        if (province == null)
            return 0f;

        float maizin = compagnie == ENUM_Compagnie.Maizin ? 0f : province.influenceMaizin;
        float kinia = compagnie == ENUM_Compagnie.Kinia ? 0f : province.influenceKinia;
        float joho = compagnie == ENUM_Compagnie.Joho ? 0f : province.influenceJoho;

        return Mathf.Max(maizin, Mathf.Max(kinia, joho));
    }

    private static float GetInfluenceCompagnie(
        STATE_PROVINCE province,
        ENUM_Compagnie compagnie)
    {
        if (province == null)
            return 0f;

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                return province.influenceMaizin;

            case ENUM_Compagnie.Kinia:
                return province.influenceKinia;

            case ENUM_Compagnie.Joho:
                return province.influenceJoho;

            default:
                return 0f;
        }
    }
}