using UnityEngine;

public static class SVC_IA_PersonnaliteResolver
{
    public static SCOBJ_IA_Personnalite GetProfil(DATA_JOUEUR joueur)
    {
        if (joueur == null)
            return null;

        if (joueur.profilIA != null)
            return joueur.profilIA;

        Debug.LogWarning($"[IA_PERSONNALITE] Aucun profilIA assigné pour {joueur.nomJoueur}");
        return null;
    }

    public static int GetTailleCibleEquipe(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = GetProfil(joueur);
        return profil != null ? profil.tailleCibleEquipe : 4;
    }

    public static int GetTailleMinEquipePourAction(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = GetProfil(joueur);
        return profil != null ? profil.tailleMinEquipePourAction : 1;
    }

    public static float GetRatioMinimalBudgetPourAction(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = GetProfil(joueur);
        return profil != null ? profil.ratioMinimalBudgetPourAction : 1f;
    }

    public static float EvaluerPersonnage(SCOBJ_Personnage personnage, DATA_JOUEUR joueur)
    {
        if (personnage == null || joueur == null)
            return float.MinValue;

        SCOBJ_IA_Personnalite profil = GetProfil(joueur);
        if (profil == null)
            return float.MinValue;

        int force = CALS_PERSONNAGE_STATS_Calculator.GetForceEffective(personnage, joueur.compagnie);
        int intelligence = CALS_PERSONNAGE_STATS_Calculator.GetIntelligenceEffective(personnage, joueur.compagnie);
        int dexterite = CALS_PERSONNAGE_STATS_Calculator.GetDexteriteEffective(personnage, joueur.compagnie);
        int endurance = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        float score = 0f;
        score += personnage.rareteEtoiles * profil.poidsRarete;
        score += force * profil.poidsForce;
        score += intelligence * profil.poidsIntelligence;
        score += dexterite * profil.poidsDexterite;
        score += endurance * profil.poidsEndurance;
        score -= personnage.coutParTour * profil.poidsCoutParTour;

        if (personnage.aPreferenceCompagnie && personnage.compagniePreferee == joueur.compagnie)
            score += profil.bonusPreferenceCompagnie;

        return score;
    }

    public static float GetBonusSpecialisation(
        DATA_JOUEUR joueur,
        ENUM_EQUIPE_SPECIALISATION specialisation)
    {
        SCOBJ_IA_Personnalite profil = GetProfil(joueur);
        if (profil == null)
            return 0f;

        switch (specialisation)
        {
            case ENUM_EQUIPE_SPECIALISATION.Exploration: return profil.bonusExploration;
            case ENUM_EQUIPE_SPECIALISATION.Archeologues: return profil.bonusArcheologues;
            case ENUM_EQUIPE_SPECIALISATION.Arpenteurs: return profil.bonusArpenteurs;
            case ENUM_EQUIPE_SPECIALISATION.Miliciens: return profil.bonusMiliciens;
            case ENUM_EQUIPE_SPECIALISATION.GardienDeLaPaix: return profil.bonusGardienPaix;
            case ENUM_EQUIPE_SPECIALISATION.Intervention: return profil.bonusIntervention;
            case ENUM_EQUIPE_SPECIALISATION.Construction: return profil.bonusConstruction;
            case ENUM_EQUIPE_SPECIALISATION.Colons: return profil.bonusColons;
            case ENUM_EQUIPE_SPECIALISATION.GenieCivil: return profil.bonusGenieCivil;
            default: return 0f;
        }
    }
}