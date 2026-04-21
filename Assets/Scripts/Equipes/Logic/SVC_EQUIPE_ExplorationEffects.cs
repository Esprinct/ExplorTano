using UnityEngine;

public static class SVC_EQUIPE_ExplorationEffects
{
    public static int GetToursBaseModifies(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        int toursBase)
    {
        return Mathf.Max(1,
            toursBase + SVC_EQUIPE_EffetService.GetDelta(
                equipe,
                joueur,
                EffetENUM_Stats.ToursExploration,
                toursBase
            )
        );
    }

    public static float GetChanceArtefactModifiee(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        float baseValue)
    {
        return Mathf.Max(0f,
            baseValue + SVC_EQUIPE_EffetService.GetDelta(
                equipe,
                joueur,
                EffetENUM_Stats.ChanceRelique,
                Mathf.RoundToInt(baseValue)
            )
        );
    }

    public static float GetChanceArtefactRareModifiee(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        float baseValue)
    {
        return Mathf.Max(0f,
            baseValue + SVC_EQUIPE_EffetService.GetDelta(
                equipe,
                joueur,
                EffetENUM_Stats.ChanceReliqueRare,
                Mathf.RoundToInt(baseValue)
            )
        );
    }
}