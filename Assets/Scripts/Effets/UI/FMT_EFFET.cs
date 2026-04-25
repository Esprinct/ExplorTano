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
public static string BuildValeurAfficheeLongueRich(SCOBJ_EFFET effet)
{
    if (effet == null || effet.modificateurs == null || effet.modificateurs.Count == 0)
        return "";

    List<string> lignes = new();

    foreach (DATA_StatModifier modificateur in effet.modificateurs)
    {
        if (modificateur == null || modificateur.stat == EffetENUM_Stats.Aucune || modificateur.valeur == 0)
            continue;

        string nomStat = GetNomLongStat(modificateur.stat);
        string signe = modificateur.valeur >= 0 ? "+" : "";
        string suffixe = modificateur.valeurType == EffetValeurType.Pourcentage ? "%" : "";
        string contenu = $"{nomStat} {signe}{modificateur.valeur}{suffixe}";

        string color = modificateur.estMalus ? MalusColor : BonusColor;
        lignes.Add($"<b><color={color}>{contenu}</color></b>");
    }

    return string.Join("\n", lignes);
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
            case EffetENUM_Stats.Curiosite: return "CUR";
            case EffetENUM_Stats.Ingeniosite: return "ING";
            case EffetENUM_Stats.Combativite: return "COM";
            case EffetENUM_Stats.Endurance: return "END";
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
            case EffetENUM_Stats.GainExplorationPct:
    return "EXPLO%";
        }
    }
    
    public static string BuildValeurAfficheeLongue(SCOBJ_EFFET effet)
{
    if (effet == null || effet.modificateurs == null || effet.modificateurs.Count == 0)
        return "";

    List<string> lignes = new();

    foreach (DATA_StatModifier modificateur in effet.modificateurs)
    {
        if (modificateur == null || modificateur.stat == EffetENUM_Stats.Aucune || modificateur.valeur == 0)
            continue;

        string nomStat = GetNomLongStat(modificateur.stat);
        string signe = modificateur.valeur >= 0 ? "+" : "";
        string suffixe = modificateur.valeurType == EffetValeurType.Pourcentage ? "%" : "";

        lignes.Add($"{nomStat} {signe}{modificateur.valeur}{suffixe}");
    }

    return string.Join("\n", lignes);
}

public static string GetNomLongStat(EffetENUM_Stats stat)
{
    switch (stat)
    {
  
       case EffetENUM_Stats.Curiosite: return "Curiosité";
        case EffetENUM_Stats.Ingeniosite: return "Ingéniosité";
        case EffetENUM_Stats.Combativite: return "Combativité";
       
         case EffetENUM_Stats.Endurance: return "Endurance";

        case EffetENUM_Stats.ToursExploration: return "Tours d'exploration";
        case EffetENUM_Stats.ToursConstruction: return "Tours de construction";
        case EffetENUM_Stats.ToursVadrouille: return "Tours de vadrouille";

        case EffetENUM_Stats.ChanceRelique: return "Chance d'artefact";
        case EffetENUM_Stats.ChanceReliqueRare: return "Chance d'artefact rare";

        case EffetENUM_Stats.BeneficesParTour: return "Bénéfices par tour";
        case EffetENUM_Stats.GainEsterlinFinConstruction: return "Esterlin en fin de construction";
        case EffetENUM_Stats.GainPrestigeFinConstruction: return "Prestige en fin de construction";

        case EffetENUM_Stats.OccupationGainVadrouille: return "Occupation gagnée en vadrouille";
        case EffetENUM_Stats.OccupationReductionAdverseVadrouille: return "Réduction d'occupation adverse";

        case EffetENUM_Stats.Prestige: return "Prestige";
        case EffetENUM_Stats.Influence: return "Influence";
        case EffetENUM_Stats.Revenus: return "Revenus";
        case EffetENUM_Stats.CoutExploration: return "Coût d'exploration";
case EffetENUM_Stats.GainExplorationPct:
    return "Exploration gagnée";
        default: return stat.ToString();
    }
}
}