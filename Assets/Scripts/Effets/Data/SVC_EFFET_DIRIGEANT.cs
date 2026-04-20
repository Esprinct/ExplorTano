 using System.Collections.Generic;
public static class SVC_EFFET_Dirigeant
{
    public static List<SCOBJ_DIRIGEANT_EFFET> GetEffetsActifs(
        SCOBJ_DIRIGEANT dirigeant,
        int niveauDirigeant)
    {
        List<SCOBJ_DIRIGEANT_EFFET> actifs = new();

        if (dirigeant == null || dirigeant.effets == null)
            return actifs;

        foreach (var effet in dirigeant.effets)
        {
            if (effet == null)
                continue;

            if (niveauDirigeant < effet.niveauRequis)
                continue;

            actifs.Add(effet);
        }

        return actifs;
    }
}