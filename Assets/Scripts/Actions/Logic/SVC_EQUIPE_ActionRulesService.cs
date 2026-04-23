public static class SVC_EQUIPE_ActionRulesService
{
    public static bool PeutExplorer(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        switch (equipe.specialisation)
        {
            case ENUM_EQUIPE_SPECIALISATION.Reconnaissance:
            case ENUM_EQUIPE_SPECIALISATION.Exploration:
            case ENUM_EQUIPE_SPECIALISATION.Archeologues:
            case ENUM_EQUIPE_SPECIALISATION.Arpenteurs:
                return true;

            default:
                return false;
        }
    }

    public static bool PeutConstruire(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        switch (equipe.specialisation)
        {
            case ENUM_EQUIPE_SPECIALISATION.Construction:
            case ENUM_EQUIPE_SPECIALISATION.Colons:
            case ENUM_EQUIPE_SPECIALISATION.GenieCivil:
                return true;

            default:
                return false;
        }
    }

    public static bool PeutVadrouiller(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        switch (equipe.specialisation)
        {
            case ENUM_EQUIPE_SPECIALISATION.Miliciens:
            case ENUM_EQUIPE_SPECIALISATION.GardienDeLaPaix:
            case ENUM_EQUIPE_SPECIALISATION.Intervention:
                return true;

            default:
                return false;
        }
    }

    public static string GetNomActionPrincipale(STATE_EQUIPE equipe)
    {
        if (PeutVadrouiller(equipe))
            return "Démarrer la vadrouille";

        if (PeutConstruire(equipe))
            return "Démarrer la construction";

        if (PeutExplorer(equipe))
            return "Démarrer l'exploration";

        return "Aucune action";
    }

    public static string GetNomAffectation(STATE_EQUIPE equipe)
    {
        if (PeutVadrouiller(equipe))
            return "Affecter à une province à sécuriser";

        if (PeutConstruire(equipe))
            return "Affecter à une province à construire";

        if (PeutExplorer(equipe))
            return "Affecter à une province à explorer";

        return "Affecter à une province";
    }
}