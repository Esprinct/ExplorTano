using System.Linq;
using UnityEngine;

public static class CALC_EQUIPE_StatsCalculator
{
    public static EQUIPE_StatsSnapshot Calculer(STATE_EQUIPE equipe)
    {
        EQUIPE_StatsSnapshot snapshot = new();

        if (equipe == null || equipe.membresActuels == null)
            return snapshot;

        snapshot.nombreMembres = equipe.membresActuels.Count(p => p != null);

        int baseCuriosite = 0;
        int baseIngeniosite = 0;
        int baseCombativite = 0;
        int baseEndurance = 0;
        int coutTotal = 0;

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage == null)
                continue;

            baseCuriosite += CALS_PERSONNAGE_STATS_Calculator.GetCuriositeEffective(
                personnage,
                equipe.compagnie
            );

            baseIngeniosite += CALS_PERSONNAGE_STATS_Calculator.GetIngeniositeEffective(
                personnage,
                equipe.compagnie
            );

            baseCombativite += CALS_PERSONNAGE_STATS_Calculator.GetCombativiteEffective(
                personnage,
                equipe.compagnie
            );

            baseEndurance += CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(
                personnage,
                equipe.compagnie
            );

            coutTotal += SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
        }

        DATA_JOUEUR joueur = ResolveJoueurProprietaire(equipe);

        snapshot.curiositeTotale = SVC_EQUIPE_EffetService.GetValeurFinale(
            equipe,
            joueur,
            EffetENUM_Stats.Curiosite,
            baseCuriosite
        );

        snapshot.ingeniositeTotale = SVC_EQUIPE_EffetService.GetValeurFinale(
            equipe,
            joueur,
            EffetENUM_Stats.Ingeniosite,
            baseIngeniosite
        );

        snapshot.combativiteTotale = SVC_EQUIPE_EffetService.GetValeurFinale(
            equipe,
            joueur,
            EffetENUM_Stats.Combativite,
            baseCombativite
        );

        snapshot.enduranceTotale = SVC_EQUIPE_EffetService.GetValeurFinale(
            equipe,
            joueur,
            EffetENUM_Stats.Endurance,
            baseEndurance
        );

        snapshot.coutTotal = coutTotal;

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

    private static DATA_JOUEUR ResolveJoueurProprietaire(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return null;

        SYS_GameManager gameManager = Object.FindAnyObjectByType<SYS_GameManager>();
        if (gameManager == null)
            return null;

        return gameManager.GetJoueurProprietaireEquipe(equipe);
    }
}