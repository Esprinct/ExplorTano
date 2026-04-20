using System.Collections.Generic;

public static class UTIL_PERSONNNAGE_EQUIPEMENT_EFFET
{
    public static List<SCOBJ_EFFET> GetEffetsEquipements(SCOBJ_Personnage personnage)
    {
        List<SCOBJ_EFFET> effets = new();

        if (personnage == null || personnage.objetsEquipes == null)
            return effets;

        foreach (var objet in personnage.objetsEquipes)
        {
            if (objet == null || objet.effets == null)
                continue;

            foreach (var effet in objet.effets)
            {
                if (effet != null)
                    effets.Add(effet);
            }
        }

        return effets;
    }
}