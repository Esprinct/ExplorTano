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

    public static ENUM_EQUIPE_ACTION GetActionPrincipale(STATE_EQUIPE equipe)
    {
        if (PeutVadrouiller(equipe))
            return ENUM_EQUIPE_ACTION.Vadrouille;

        if (PeutConstruire(equipe))
            return ENUM_EQUIPE_ACTION.Construction;

        if (PeutExplorer(equipe))
            return ENUM_EQUIPE_ACTION.Exploration;

        return ENUM_EQUIPE_ACTION.Aucune;
    }

    public static string GetNomActionPrincipale(STATE_EQUIPE equipe)
    {
        switch (GetActionPrincipale(equipe))
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                return "Démarrer la vadrouille";

            case ENUM_EQUIPE_ACTION.Construction:
                return "Démarrer la construction";

            case ENUM_EQUIPE_ACTION.Exploration:
                return "Démarrer l'exploration";

            default:
                return "Aucune action";
        }
    }

    public static string GetNomAffectation(STATE_EQUIPE equipe)
    {
        switch (GetActionPrincipale(equipe))
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                return "Affecter à une province à sécuriser";

            case ENUM_EQUIPE_ACTION.Construction:
                return "Affecter à une province à construire";

            case ENUM_EQUIPE_ACTION.Exploration:
                return "Affecter à une province à explorer";

            default:
                return "Affecter à une province";
        }
    }

    public static bool PeutDemarrerAction(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        if (equipe.AUneActionEnCours)
            return false;

        return GetActionPrincipale(equipe) != ENUM_EQUIPE_ACTION.Aucune;
    }
}