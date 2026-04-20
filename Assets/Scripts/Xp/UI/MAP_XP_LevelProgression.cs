using UnityEngine;

public static class MAP_XP_LevelProgression
{
    public static DATA_LevelProgressionView ToViewData(STATE_LevelProgression state, CFG_LevelProgression config)
    {
        DATA_LevelProgressionView data = new();

        if (state == null || config == null)
            return data;

        int xpRequise = SVC_LevelProgression.GetXpRequiredForNextLevel(state, config);
        bool niveauMax = state.niveau >= config.niveauMax;

        data.niveau = state.niveau;
        data.xpActuelle = state.xpActuelle;
        data.xpRequise = niveauMax ? 0 : xpRequise;
        data.niveauMaxAtteint = niveauMax;
        data.progressionNormalisee = niveauMax || xpRequise <= 0
            ? 1f
            : Mathf.Clamp01(state.xpActuelle / (float)xpRequise);

        return data;
    }
}