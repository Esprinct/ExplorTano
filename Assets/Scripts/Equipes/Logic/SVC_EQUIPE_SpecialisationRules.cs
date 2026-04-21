public static class SVC_EQUIPE_SpecialisationRules
{
    public static bool PeutPasserTier2(STATE_EQUIPE equipe)
    {
        return equipe != null
            && equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Reconnaissance
            && equipe.niveauActuel >= 3;
    }

    public static bool PeutPasserTier3(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.niveauActuel < 6)
            return false;

        return equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Exploration
            || equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Construction
            || equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Miliciens;
    }
}