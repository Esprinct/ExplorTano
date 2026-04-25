public static class SVC_RECRUTEMENT_IA_ScoringService
{
    public static int EvaluerInteret(DATA_OffreRecrutement offre, ENUM_Compagnie compagnie)
    {
        if (offre == null || offre.personnage == null)
            return int.MinValue;

        SCOBJ_Personnage personnage = offre.personnage;

        int score = 0;

        // 1. Priorité principale : rareté
        switch (personnage.rareteEtoiles)
        {
            case 5:
                score += 5000;
                break;
            case 4:
                score += 3200;
                break;
            case 3:
                score += 1800;
                break;
            case 2:
                score += 700;
                break;
            default:
                score += 150;
                break;
        }

        // 2. Bonus secondaire : compagnie préférée correspondante
        if (personnage.aPreferenceCompagnie &&
            personnage.compagniePreferee != ENUM_Compagnie.Aucune)
        {
            if (personnage.compagniePreferee == compagnie)
            {
                score += 2200;
            }
            else
            {
                score -= 350;
            }
        }

        // 3. Stats utiles, mais moins importantes que la rareté / affinité
        score += personnage.curiosite * 2;
        score += personnage.ingeniosite * 2;
        score += personnage.combativite * 2;
        score += personnage.endurance * 2;

        // 4. Léger frein sur les personnages trop chers
        score -= offre.prixMinimum / 8;

        return score;
    }
}