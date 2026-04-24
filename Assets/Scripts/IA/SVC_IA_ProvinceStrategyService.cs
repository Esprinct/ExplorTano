using UnityEngine;

public static class SVC_IA_ProvinceStrategyService
{
    public static void AffecterEquipesAuxProvinces(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = SVC_IA_EquipeRosterService.GetTailleMinEquipePourAction(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.AUneActionEnCours)
                continue;

            if (equipe.membresActuels == null || equipe.membresActuels.Count < tailleMinEquipe)
                continue;

            if (PeutGarderProvinceActuelle(gameManager, joueur, equipe))
                continue;

            STATE_PROVINCE cible = ChoisirProvincePourEquipe(gameManager, joueur, equipe);

            if (cible == null)
            {
                equipe.provinceAffectee = null;
                continue;
            }

            equipe.provinceAffectee = cible;
            equipe.actionTerminee = false;
        }
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
                float exploration = province.GetExploration(equipe.compagnie);
                return exploration < 100f;
            }

            case ENUM_EQUIPE_ACTION.Vadrouille:
            {
                float exploration = province.GetExploration(equipe.compagnie);
                if (exploration < 100f)
                    return false;

                bool claimParIA =
                    province.estClaim &&
                    province.proprietaireActuel.HasValue &&
                    province.proprietaireActuel.Value == joueur.compagnie;

                if (claimParIA)
                    return false;

                float influenceIA = GetInfluenceCompagnie(province, joueur.compagnie);
                float influenceAdverse = GetInfluenceAdverseDominante(province, joueur.compagnie);

                return influenceAdverse > 0.01f || (influenceIA > 0f && influenceIA < 60f) || !province.estClaim;
            }

            case ENUM_EQUIPE_ACTION.Construction:
                return province.estClaim &&
                       province.proprietaireActuel.HasValue &&
                       province.proprietaireActuel.Value == joueur.compagnie;

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
        float politique = province.data.poidsPolitique;
        float accessibilite = province.data.accesibilite;

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
                if (exploration >= 100f)
                    return float.MinValue;

                // L’exploration ne sert plus à générer directement de l’étrinium par tour.
                // On priorise donc la complétion + prestige + positionnement.
                score += (100f - exploration) * 5f;
                score += prestige * 1.5f;
                score += politique * 1.2f;
                score += accessibilite * 0.5f;
                score -= nbEquipesAllieesDejaSurPlace * 120f;
                break;
            }

            case ENUM_EQUIPE_ACTION.Vadrouille:
            {
                if (exploration < 100f)
                    return float.MinValue;

                if (claimParIA)
                    return float.MinValue;

                score += influenceAdverse * 4.5f;
                score += influenceIA * 1.2f;
                score += prestige * 1.0f;
                score += politique * 1.6f;

                if (!estClaim)
                    score += 120f;

                if (claimParEnnemi)
                    score += 80f;

                score -= nbEquipesAllieesDejaSurPlace * 150f;
                break;
            }

            case ENUM_EQUIPE_ACTION.Construction:
            {
                if (!claimParIA)
                    return float.MinValue;

                score += prestige * 1.0f;
                score += politique * 1.0f;
                score -= nbEquipesAllieesDejaSurPlace * 100f;
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

    private static float GetInfluenceAdverseDominante(STATE_PROVINCE province, ENUM_Compagnie compagnie)
    {
        if (province == null)
            return 0f;

        float maizin = compagnie == ENUM_Compagnie.Maizin ? 0f : province.influenceMaizin;
        float kinia = compagnie == ENUM_Compagnie.Kinia ? 0f : province.influenceKinia;
        float joho = compagnie == ENUM_Compagnie.Joho ? 0f : province.influenceJoho;

        return Mathf.Max(maizin, Mathf.Max(kinia, joho));
    }

    private static float GetInfluenceCompagnie(STATE_PROVINCE province, ENUM_Compagnie compagnie)
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