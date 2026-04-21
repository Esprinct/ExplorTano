using System.Collections.Generic;

public static class EFFET_EQUIPE_Aggregator
{
    public static List<SCOBJ_EFFET> GetEffetsPourEquipe(STATE_EQUIPE equipe)
    {
        List<SCOBJ_EFFET> result = new();

        if (equipe == null)
            return result;

        AjouterEffetsSpecialisation(result, equipe);
        AjouterEffetsMembres(result, equipe);

        return result;
    }

    private static void AjouterEffetsSpecialisation(List<SCOBJ_EFFET> result, STATE_EQUIPE equipe)
    {
        if (result == null || equipe == null || equipe.dataSpecialisation == null || equipe.dataSpecialisation.effets == null)
            return;

        foreach (SCOBJ_EQUIPE_EFFET effet in equipe.dataSpecialisation.effets)
        {
            if (effet == null)
                continue;

            result.Add(effet);
        }
    }

    private static void AjouterEffetsMembres(List<SCOBJ_EFFET> result, STATE_EQUIPE equipe)
    {
        if (result == null || equipe == null || equipe.membresActuels == null)
            return;

        foreach (SCOBJ_Personnage membre in equipe.membresActuels)
        {
            if (membre == null || membre.effets == null)
                continue;

            foreach (SCOBJ_PERSONNAGE_EFFET effet in membre.effets)
            {
                if (effet == null)
                    continue;

                result.Add(effet);
            }
        }
    }
}