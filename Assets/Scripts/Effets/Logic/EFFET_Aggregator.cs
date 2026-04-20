using System.Collections.Generic;

public static class EFFET_Aggregator
{
    public static EFFET_AggregationResult GetEffetsPourPersonnage(EFFET_Contexte contexte)
    {
        EFFET_AggregationResult result = new();

        if (contexte == null)
            return result;

        AjouterEffetsPersonnage(result, contexte.personnage);

        // Extensions futures :
        // AjouterEffetsEquipe(result, contexte.equipe);
        // AjouterEffetsCompagnie(result, contexte.joueur);
        // AjouterEffetsProvince(result, contexte.province);
        // AjouterEffetsDirigeant(result, contexte.dirigeant);
        // AjouterEffetsObjets(result, contexte.personnage);

        return result;
    }

    private static void AjouterEffetsPersonnage(EFFET_AggregationResult result, SCOBJ_Personnage personnage)
    {
        if (result == null || personnage == null || personnage.effets == null)
            return;

        foreach (SCOBJ_PERSONNAGE_EFFET effet in personnage.effets)
        {
            if (effet == null)
                continue;

            result.effets.Add(effet);
        }
    }
}