public static class DATA_StatTooltip
{
    public static string GetTitre(ENUM_Stats ENUM_Stats)
    {
        switch (ENUM_Stats)
        {
            case ENUM_Stats.Curiosite:
                return "Curiosite";

            case ENUM_Stats.Intelligence:
                return "Intelligence";

            case ENUM_Stats.Dexterite:
                return "Dextérité";

            case ENUM_Stats.Endurance:
                return "Endurance";

            default:
                return "Statistique";
        }
    }

   public static string GetDescription(ENUM_Stats ENUM_Stats)
{
    switch (ENUM_Stats)
    {
        case ENUM_Stats.Curiosite:
            return "+1 prestige tous les 60 points de Curiosite.";

        case ENUM_Stats.Intelligence:
            return "+1% de chance de relique tous les 40 points d'Intelligence.";

        case ENUM_Stats.Dexterite:
            return "-1 tour tous les 120 points de Dextérité.";

        case ENUM_Stats.Endurance:
            return "-50 coût d'exploration par tour tous les 25 points d'Endurance.";

        default:
            return "";
    }
}
}