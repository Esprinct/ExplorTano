using System.Collections.Generic;
using UnityEngine;

public static class FMT_StatTooltip_Value
{
    private const string BonusColor = "#2E7D32";
    private const string MalusColor = "#B71C1C";

    public static string BuildStatDetail(
        int baseValue,
        int runtimeBonus,
        SCOBJ_Personnage personnage,
        ENUM_Compagnie compagnie,
        EffetENUM_Stats stat)
    {
        List<string> lignes = new();

        lignes.Add($"Base : {baseValue}");

        if (runtimeBonus != 0)
        {
            string signe = runtimeBonus > 0 ? "+" : "";
            string color = runtimeBonus > 0 ? BonusColor : MalusColor;
            lignes.Add($"Bonus runtime : <b><color={color}>{signe}{runtimeBonus}</color></b>");
        }

        int totalEffets = 0;

        if (personnage != null && personnage.effets != null)
        {
            foreach (SCOBJ_PERSONNAGE_EFFET effet in personnage.effets)
            {
                if (effet == null)
                    continue;

                if (!CALS_PERSONNAGE_STATS_Calculator.EstEffetActif(effet, personnage, compagnie))
                    continue;

                if (effet.modificateurs == null)
                    continue;

                int deltaEffet = 0;

                foreach (DATA_StatModifier modificateur in effet.modificateurs)
                {
                    if (modificateur == null)
                        continue;

                    if (modificateur.stat != stat)
                        continue;

                    int valeur = Mathf.Abs(modificateur.valeur);

                    if (effet.type == EffetType.Malus)
                        valeur = -valeur;

                    deltaEffet += valeur;
                }

                if (deltaEffet == 0)
                    continue;

                totalEffets += deltaEffet;

                string signe = deltaEffet > 0 ? "+" : "";
                string color = deltaEffet > 0 ? BonusColor : MalusColor;
                string titre = string.IsNullOrWhiteSpace(effet.titre) ? "Effet" : effet.titre;

                lignes.Add($"{titre} : <b><color={color}>{signe}{deltaEffet}</color></b>");
            }
        }

        int total = Mathf.Max(0, baseValue + runtimeBonus + totalEffets);
        lignes.Add($"Total : <b>{total}</b>");

        return string.Join("\n", lignes);
    }
}