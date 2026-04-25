using UnityEngine;

public static class CALC_VADROUILLE_Resolver
{
    public static DATA_VADROUILLE_Resultat CalculerResultat(
        EQUIPE_StatsSnapshot stats,
        int toursBase,
        int coutParTourBase,
        int prestigeBase,
        float gainOccupationBase,
        float reductionOccupationAdverseBase)
    {
        DATA_VADROUILLE_Resultat result = new DATA_VADROUILLE_Resultat();

        int bonusToursCombativite = Mathf.Max(0, Mathf.FloorToInt(stats.EcartCuriosite / 160f));
        int bonusCoutEndurance = Mathf.Max(0, Mathf.FloorToInt(stats.EcartEndurance / 40f));
        int bonusPrestige = Mathf.Max(0, Mathf.FloorToInt(stats.EcartCuriosite / 100f));

        float bonusOccupation = Mathf.Max(0f, stats.EcartCuriosite * 0.02f);
        float bonusReductionAdverse = Mathf.Max(0f, stats.EcartCombativite * 0.02f);

        result.toursFinaux = Mathf.Max(1, toursBase - bonusToursCombativite);

        int reductionCout = bonusCoutEndurance * 40;
        result.coutParTourFinal = Mathf.Max(350, coutParTourBase - reductionCout);
        result.coutTotal = result.toursFinaux * result.coutParTourFinal;

        result.prestigeFinal = Mathf.Max(0, prestigeBase + bonusPrestige);
        result.gainOccupationFinal = Mathf.Max(0f, gainOccupationBase + bonusOccupation);
        result.reductionOccupationAdverseFinal = Mathf.Max(0f, reductionOccupationAdverseBase + bonusReductionAdverse);

        return result;
    }
}