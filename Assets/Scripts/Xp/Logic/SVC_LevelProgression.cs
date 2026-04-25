public static class SVC_LevelProgression
{
    public static int GetXpRequiredForNextLevel(STATE_LevelProgression state, CFG_LevelProgression config)
    {
        if (state == null || config == null)
            return 0;

        return config.GetXpRequiredForLevel(state.niveau);
    }

    public static bool CanLevelUp(STATE_LevelProgression state, CFG_LevelProgression config)
    {
        if (state == null || config == null)
            return false;

        if (state.niveau >= config.niveauMax)
            return false;

        return state.xpActuelle >= GetXpRequiredForNextLevel(state, config);
    }

    public static int AddXp(STATE_LevelProgression state, CFG_LevelProgression config, int amount)
    {
        if (state == null || config == null || amount <= 0)
            return 0;

        if (state.niveau >= config.niveauMax)
            return 0;

        state.xpActuelle += amount;

        int niveauxGagnes = 0;

        while (CanLevelUp(state, config))
        {
            state.xpActuelle -= GetXpRequiredForNextLevel(state, config);
            state.niveau++;
            state.pointsDisponibles += config.pointsParNiveau;
            niveauxGagnes++;

            if (state.niveau >= config.niveauMax)
            {
                state.xpActuelle = 0;
                break;
            }
        }

        return niveauxGagnes;
    }
    private static int GetAllocationBonus(SCOBJ_Personnage personnage, EffetENUM_Stats cible)
{
    if (personnage == null || personnage.allocation == null)
        return 0;

    return cible switch
    {
        EffetENUM_Stats.Curiosite => personnage.allocation.curiosite,
        EffetENUM_Stats.Intelligence => personnage.allocation.intelligence,
        EffetENUM_Stats.Dexterite => personnage.allocation.dexterite,
        EffetENUM_Stats.Endurance => personnage.allocation.endurance,
        _ => 0
    };
}
}