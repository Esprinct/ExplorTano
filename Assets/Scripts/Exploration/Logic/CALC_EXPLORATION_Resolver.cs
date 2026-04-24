using UnityEngine;

public static class CALC_EXPLORATION_Resolver
{
    public static DATA_EXPLORATION_Resultat CalculerResultat(
        EQUIPE_StatsSnapshot stats,
        int toursBase,
        int coutParTourBase,
        int prestigeBase,
        float chanceReliqueBase,
        float chanceReliqueRareBase,
        int enclavementComte)
    {
        DATA_EXPLORATION_Resultat result = new DATA_EXPLORATION_Resultat();

        int nombreMembres = Mathf.Max(0, stats.nombreMembres);

        float multiplicateurSousEffectif = GetMultiplicateurSousEffectif(nombreMembres);
        int malusToursSousEffectif = GetMalusToursSousEffectif(nombreMembres);
        int surcoutSousEffectif = GetSurcoutSousEffectif(nombreMembres);
        int malusPrestigeSousEffectif = GetMalusPrestigeSousEffectif(nombreMembres);

        int bonusPrestigeForce = Mathf.Max(0, Mathf.FloorToInt(stats.EcartForce / 80f));
        float bonusRelique = Mathf.Max(0f, stats.EcartIntelligence * 0.015f);
        float bonusReliqueRare = Mathf.Max(0f, stats.EcartIntelligence * 0.0035f);
        int bonusToursDex = Mathf.Max(0, Mathf.FloorToInt(stats.EcartDexterite / 160f));
        int bonusCoutEndurance = Mathf.Max(0, Mathf.FloorToInt(stats.EcartEndurance / 40f));
        int bonusToursEnclavement = Mathf.Max(0, Mathf.FloorToInt(enclavementComte / 100f));

        result.prestigeFinal = Mathf.Max(
            0,
            prestigeBase + bonusPrestigeForce - malusPrestigeSousEffectif
        );

        result.chanceRelique = Mathf.Clamp(
            (chanceReliqueBase + bonusRelique) * multiplicateurSousEffectif,
            0f,
            100f
        );

        result.chanceReliqueRare = Mathf.Clamp(
            (chanceReliqueRareBase + bonusReliqueRare) * multiplicateurSousEffectif,
            0f,
            100f
        );

        result.toursFinaux = Mathf.Max(
            2,
            toursBase + bonusToursEnclavement - bonusToursDex + malusToursSousEffectif
        );

        int reductionCout = bonusCoutEndurance * 40;
        result.coutParTourFinal = Mathf.Max(
            350,
            coutParTourBase - reductionCout + surcoutSousEffectif
        );

        result.coutTotal = result.toursFinaux * result.coutParTourFinal;

        return result;
    }

    private static float GetMultiplicateurSousEffectif(int nombreMembres)
    {
        if (nombreMembres <= 1) return 0.55f;
        if (nombreMembres == 2) return 0.8f;
        if (nombreMembres == 3) return 0.95f;
        return 1f;
    }

    private static int GetMalusToursSousEffectif(int nombreMembres)
    {
        if (nombreMembres <= 1) return 2;
        if (nombreMembres == 2) return 1;
        return 0;
    }

    private static int GetSurcoutSousEffectif(int nombreMembres)
    {
        if (nombreMembres <= 1) return 250;
        if (nombreMembres == 2) return 120;
        if (nombreMembres == 3) return 40;
        return 0;
    }

    private static int GetMalusPrestigeSousEffectif(int nombreMembres)
    {
        if (nombreMembres <= 1) return 2;
        if (nombreMembres == 2) return 1;
        return 0;
    }
}