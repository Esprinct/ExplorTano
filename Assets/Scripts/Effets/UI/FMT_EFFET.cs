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

            yield return FormatterLigne(modificateur, richText);
        }
    }

    private static bool EstModificateurValide(DATA_StatModifier modificateur)
    {
        return modificateur != null
            && modificateur.stat != EffetENUM_Stats.Aucune
            && modificateur.valeur != 0;
    }

    private static string FormatterLigne(DATA_StatModifier modificateur, bool richText)
    {
        string nomStat = GetNomCourtStat(modificateur.stat);

        int valeurSignee = modificateur.valeur;
        string signe = valeurSignee >= 0 ? "+" : "";
        string suffixe = modificateur.valeurType == EffetValeurType.Pourcentage ? "%" : "";
        string contenu = $"{nomStat} {signe}{valeurSignee}{suffixe}";

        if (!richText)
            return contenu;

        string color = modificateur.estMalus ? MalusColor : BonusColor;
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
            case EffetENUM_Stats.Curiosite: return "CUR";
            case EffetENUM_Stats.Ingeniosite: return "ING";
            case EffetENUM_Stats.Combativite: return "COM";
            case EffetENUM_Stats.ToursExploration: return "T.EXP";
            case EffetENUM_Stats.ToursConstruction: return "T.CST";
            case EffetENUM_Stats.ToursVadrouille: return "T.VAD";
            case EffetENUM_Stats.ChanceRelique: return "ART";
            case EffetENUM_Stats.ChanceReliqueRare: return "ART.R";
            case EffetENUM_Stats.BeneficesParTour: return "BEN";
            case EffetENUM_Stats.GainEsterlinFinConstruction: return "EST";
            case EffetENUM_Stats.GainPrestigeFinConstruction: return "PRE";
            case EffetENUM_Stats.OccupationGainVadrouille: return "OCC+";
            case EffetENUM_Stats.OccupationReductionAdverseVadrouille: return "OCC-";
            case EffetENUM_Stats.Prestige: return "PRE";
            case EffetENUM_Stats.Influence: return "INF";
            case EffetENUM_Stats.Revenus: return "REV";
            case EffetENUM_Stats.CoutExploration: return "C.EXP";
            default: return "?";
        }
    }
}