using System.Collections.Generic;
using UnityEngine;

public static class EFFET_ApplicationService
{
    public static int ComputeDelta(
        List<SCOBJ_EFFET> effets,
        EFFET_Contexte contexte,
        EffetENUM_Stats stat,
        int baseValue)
    {
        if (effets == null || contexte == null)
            return 0;

        int total = 0;

        foreach (SCOBJ_EFFET effet in effets)
        {
            if (effet == null || effet.modificateurs == null)
                continue;

            if (!EstActifCompatible(effet, contexte))
                continue;

            foreach (DATA_StatModifier modificateur in effet.modificateurs)
            {
                if (modificateur == null || modificateur.valeur == 0)
                    continue;

                if (modificateur.stat != stat)
                    continue;

                int valeur = CalculerValeurSignee(baseValue, modificateur);
                if (valeur == 0)
                    continue;

                total += valeur;
            }
        }

        return total;
    }

    public static List<string> BuildDetailLines(
        List<SCOBJ_EFFET> effets,
        EFFET_Contexte contexte,
        EffetENUM_Stats stat,
        int baseValue)
    {
        List<string> lignes = new();

        if (effets == null || contexte == null)
            return lignes;

        foreach (SCOBJ_EFFET effet in effets)
        {
            if (effet == null || effet.modificateurs == null)
                continue;

            if (!EstActifCompatible(effet, contexte))
                continue;

            int deltaEffet = 0;

            foreach (DATA_StatModifier modificateur in effet.modificateurs)
            {
                if (modificateur == null || modificateur.valeur == 0)
                    continue;

                if (modificateur.stat != stat)
                    continue;

                int valeur = CalculerValeurSignee(baseValue, modificateur);
                if (valeur == 0)
                    continue;

                deltaEffet += valeur;
            }

            if (deltaEffet == 0)
                continue;

            string signe = deltaEffet > 0 ? "+" : "";
            string nomEffet = GetNomEffet(effet, contexte);

            lignes.Add($"{nomEffet} : {signe}{deltaEffet}");
        }

        return lignes;
    }

    private static bool EstActifCompatible(SCOBJ_EFFET effet, EFFET_Contexte contexte)
    {
        if (effet == null || contexte == null)
            return false;

        if (effet is SCOBJ_PERSONNAGE_EFFET personnageEffet)
            return EFFET_Resolver.EstActif(personnageEffet, contexte);

        return true;
    }

    private static string GetNomEffet(SCOBJ_EFFET effet, EFFET_Contexte contexte)
    {
        if (effet == null)
            return "Effet";

        if (effet is SCOBJ_PERSONNAGE_EFFET personnageEffet && contexte.personnage != null)
        {
            return personnageEffet.GetTitre(contexte.personnage.genre);
        }

        return string.IsNullOrWhiteSpace(effet.titre)
            ? "Effet"
            : effet.titre;
    }

    private static int CalculerValeurSignee(int baseValue, DATA_StatModifier modificateur)
    {
        if (modificateur == null)
            return 0;

        switch (modificateur.valeurType)
        {
            case EffetValeurType.Pourcentage:
                return Mathf.RoundToInt(baseValue * (modificateur.valeur / 100f));

            case EffetValeurType.Fixe:
            default:
                return modificateur.valeur;
        }
    }
}