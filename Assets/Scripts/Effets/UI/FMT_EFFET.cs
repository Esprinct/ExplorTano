using System.Collections.Generic;
using UnityEngine;

public static class FMT_EFFET
{
    private const string BonusColor = "#2E7D32";
    private const string MalusColor = "#B71C1C";

    public static string BuildValeurAffichee(SCOBJ_EFFET effet)
    {
        return string.Join("\n", BuildLignes(effet, richText: false));
    }

    public static string BuildValeurAfficheeRich(SCOBJ_EFFET effet)
    {
        return string.Join("\n", BuildLignes(effet, richText: true));
    }

    private static IEnumerable<string> BuildLignes(SCOBJ_EFFET effet, bool richText)
    {
        if (effet == null || effet.modificateurs == null || effet.modificateurs.Count == 0)
            yield break;

        foreach (DATA_StatModifier modificateur in effet.modificateurs)
        {
            if (!EstModificateurValide(modificateur))
                continue;

            yield return FormatterLigne(modificateur, effet.type, richText);
        }
    }

    private static bool EstModificateurValide(DATA_StatModifier modificateur)
    {
        return modificateur != null
            && modificateur.stat != EffetENUM_Stats.Aucune
            && modificateur.valeur != 0;
    }

    private static string FormatterLigne(
        DATA_StatModifier modificateur,
        EffetType typeEffet,
        bool richText)
    {
        string nomStat = GetNomCourtStat(modificateur.stat);

        int valeurSignee = typeEffet == EffetType.Malus
            ? -Mathf.Abs(modificateur.valeur)
            : Mathf.Abs(modificateur.valeur);

        string signe = valeurSignee >= 0 ? "+" : "";
        string suffixe = modificateur.valeurType == EffetValeurType.Pourcentage ? "%" : "";
        string contenu = $"{nomStat} {signe}{valeurSignee}{suffixe}";

        if (!richText)
            return contenu;

        string color = valeurSignee >= 0 ? BonusColor : MalusColor;
        return $"<b><color={color}>{contenu}</color></b>";
    }

   public static string GetNomCourtStat(EffetENUM_Stats stat)
{
    switch (stat)
    {
        case EffetENUM_Stats.Force: return "FOR";
        case EffetENUM_Stats.Intelligence: return "INT";
        case EffetENUM_Stats.Dexterite: return "DEX";
        case EffetENUM_Stats.Endurance: return "END";
        case EffetENUM_Stats.Prestige: return "Prestige";
        case EffetENUM_Stats.Influence: return "Influence";
        case EffetENUM_Stats.Revenus: return "Revenus";
        case EffetENUM_Stats.CoutExploration: return "Coût d'exploration";
        case EffetENUM_Stats.ChanceRelique: return "Artefact";
        case EffetENUM_Stats.ChanceReliqueRare: return "Artefact Rare";
        default: return "?";
    }
}
}