using UnityEngine;

public static class SVC_EQUIPE_VadrouilleEffects
{
    public static int GetToursVadrouilleFinals(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        int toursBase)
    {
        if (equipe == null)
            return Mathf.Max(1, toursBase);

        int delta = SVC_EQUIPE_EffetService.GetDelta(
            equipe,
            joueur,
            EffetENUM_Stats.ToursVadrouille,
            toursBase
        );

        return Mathf.Max(1, toursBase + delta);
    }

    public static float GetGainOccupationFinal(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        float gainBase)
    {
        if (equipe == null)
            return Mathf.Max(0f, gainBase);

        int delta = SVC_EQUIPE_EffetService.GetDelta(
            equipe,
            joueur,
            EffetENUM_Stats.OccupationGainVadrouille,
            Mathf.RoundToInt(gainBase)
        );

        return Mathf.Max(0f, gainBase + delta);
    }

    public static float GetReductionOccupationAdverseFinal(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        float gainBase)
    {
        if (equipe == null)
            return Mathf.Max(0f, gainBase);

        int delta = SVC_EQUIPE_EffetService.GetDelta(
            equipe,
            joueur,
            EffetENUM_Stats.OccupationReductionAdverseVadrouille,
            Mathf.RoundToInt(gainBase)
        );

        return Mathf.Max(0f, gainBase + delta);
    }
}