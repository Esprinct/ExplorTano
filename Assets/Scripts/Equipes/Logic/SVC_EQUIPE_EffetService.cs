using System.Collections.Generic;

public static class SVC_EQUIPE_EffetService
{
    public static int GetDelta(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        EffetENUM_Stats stat,
        int baseValue)
    {
        if (equipe == null)
            return 0;

        List<SCOBJ_EFFET> effets = EFFET_EQUIPE_Aggregator.GetEffetsPourEquipe(equipe);

        EFFET_Contexte contexte = EFFET_ContexteFactory.ForEquipe(
            equipe,
            joueur,
            equipe.provinceAffectee
        );

        return EFFET_ApplicationService.ComputeDelta(
            effets,
            contexte,
            stat,
            baseValue
        );
    }

    public static int GetValeurFinale(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        EffetENUM_Stats stat,
        int baseValue)
    {
        return baseValue + GetDelta(equipe, joueur, stat, baseValue);
    }

    public static List<string> GetDetails(
        STATE_EQUIPE equipe,
        DATA_JOUEUR joueur,
        EffetENUM_Stats stat,
        int baseValue)
    {
        if (equipe == null)
            return new List<string>();

        List<SCOBJ_EFFET> effets = EFFET_EQUIPE_Aggregator.GetEffetsPourEquipe(equipe);

        EFFET_Contexte contexte = EFFET_ContexteFactory.ForEquipe(
            equipe,
            joueur,
            equipe.provinceAffectee
        );

        return EFFET_ApplicationService.BuildDetailLines(
            effets,
            contexte,
            stat,
            baseValue
        );
    }
}