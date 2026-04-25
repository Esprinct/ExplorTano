using System.Linq;

public static class CALC_EQUIPE_StatsCalculator
{
    public static EQUIPE_StatsSnapshot Calculer(STATE_EQUIPE equipe)
    {
        EQUIPE_StatsSnapshot snapshot = new();

        if (equipe == null || equipe.membresActuels == null)
            return snapshot;

        snapshot.nombreMembres = equipe.membresActuels.Count(p => p != null);

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage == null)
                continue;

            snapshot.curiositeTotale += CALS_PERSONNAGE_STATS_Calculator.GetCuriositeEffective(personnage, equipe.compagnie);
            snapshot.ingeniositeTotale += CALS_PERSONNAGE_STATS_Calculator.GetIngeniositeEffective(personnage, equipe.compagnie);
            snapshot.combativiteTotale += CALS_PERSONNAGE_STATS_Calculator.GetCombativiteEffective(personnage, equipe.compagnie);
            snapshot.enduranceTotale += CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, equipe.compagnie);
            snapshot.coutTotal += SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
        }

        return snapshot;
    }

    public static int CalculerSurcoutExploration(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.membresActuels == null)
            return 0;

        int surcoutTotal = 0;

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage == null)
                continue;

            surcoutTotal += SVC_PERSONNAGE_CostService.GetSurcoutExploration(personnage);
        }

        return surcoutTotal;
    }
}