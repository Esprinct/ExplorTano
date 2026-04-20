using UnityEngine;
public static class SVC_PERSONNAGE_CostService
{
    private const int CoutMinimumParTour = 300;
    private const float MultiplicateurExploration = 1.5f;

    public static int GetCoutNormal(SCOBJ_Personnage personnage)
    {
        if (personnage == null)
            return 0;

        int coutBase = Mathf.Max(0, personnage.coutParTour);
        int reductionEndurance = Mathf.Max(0, personnage.endurance * 2);

        return Mathf.Max(CoutMinimumParTour, coutBase - reductionEndurance);
    }

    public static int GetCoutExploration(SCOBJ_Personnage personnage)
    {
        int coutNormal = GetCoutNormal(personnage);
        if (coutNormal <= 0)
            return 0;

        return Mathf.CeilToInt(coutNormal * MultiplicateurExploration);
    }

    public static int GetSurcoutExploration(SCOBJ_Personnage personnage)
    {
        int coutNormal = GetCoutNormal(personnage);
        int coutExploration = GetCoutExploration(personnage);

        return Mathf.Max(0, coutExploration - coutNormal);
    }
    
}