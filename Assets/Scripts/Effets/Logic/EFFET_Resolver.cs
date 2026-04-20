public static class EFFET_Resolver
{
    public static bool EstActif(SCOBJ_PERSONNAGE_EFFET effet, EFFET_Contexte contexte)
    {
        if (effet == null || contexte == null)
            return false;

        switch (effet.conditionType)
        {
            case EffetConditionType.Aucune:
                return true;

            case EffetConditionType.CompagnieRespectee:
                return contexte.personnage != null
                    && contexte.personnage.aPreferenceCompagnie
                    && contexte.personnage.compagniePreferee !=  ENUM_Compagnie.Aucune
                    && contexte.personnage.compagniePreferee == contexte.compagnie;

            case EffetConditionType.CompagnieNonRespectee:
                return contexte.personnage != null
                    && contexte.personnage.aPreferenceCompagnie
                    && contexte.personnage.compagniePreferee !=  ENUM_Compagnie.Aucune
                    && contexte.personnage.compagniePreferee != contexte.compagnie;

            default:
                return true;
        }
    }
}