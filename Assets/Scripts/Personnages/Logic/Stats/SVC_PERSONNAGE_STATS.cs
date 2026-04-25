using UnityEngine;

public static class SVC_PERSONNAGE_STATS
{
    public static CALC_PERSONNAGE_STATS_ComputedStats Compute(
        SCOBJ_Personnage personnage,
        DATA_PERSONNAGE_DisplayContext contexte)
    {
        if (contexte == null)
            contexte = DATA_PERSONNAGE_DisplayContext.Default;

        return Compute(personnage, contexte.compagnie, contexte.state);
    }

    public static CALC_PERSONNAGE_STATS_ComputedStats Compute(
        SCOBJ_Personnage personnage,
        ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune,
        STATE_PERSONNAGE state = null)
    {
        CALC_PERSONNAGE_STATS_ComputedStats result = new();

        EFFET_Contexte EFFET_Contexte = EFFET_Contexte.ForPersonnage(
            personnage,
            compagnie,
            state
        );

        result.curiosite = ComputeOne(personnage, EffetENUM_Stats.Curiosite, EFFET_Contexte);
        result.ingeniosite = ComputeOne(personnage, EffetENUM_Stats.Ingeniosite, EFFET_Contexte);
        result.combativite = ComputeOne(personnage, EffetENUM_Stats.Combativite, EFFET_Contexte);
        result.endurance = ComputeOne(personnage, EffetENUM_Stats.Endurance, EFFET_Contexte);

        return result;
    }

    private static DATA_STATS_ComputationResult ComputeOne(
        SCOBJ_Personnage personnage,
        EffetENUM_Stats stat,
        EFFET_Contexte contexte)
    {
        DATA_STATS_ComputationResult result = new();

        if (personnage == null)
            return result;

        DATA_STATS_Result DATA_STATS_Result = CALS_PERSONNAGE_STATS_Calculator.ComputeStat(
            personnage,
            stat,
            contexte.STATE_PERSONNAGE,
            contexte.compagnie
        );

        result.baseValue = DATA_STATS_Result.baseValue;
        result.delta = DATA_STATS_Result.delta;
        result.finalValue = DATA_STATS_Result.finalValue;

        int baseBrute = CALS_PERSONNAGE_STATS_Calculator.GetBaseStat(personnage, stat);
        int bonusAllocation = SVC_STATS_Allocation.GetAllocationBonus(personnage, stat);

        if (bonusAllocation > 0)
        {
            result.detailLines.Add($"Base : {baseBrute}");
            result.detailLines.Add($"Investi : +{bonusAllocation}");
        }
        else
        {
            result.detailLines.Add($"Base : {baseBrute}");
        }

        // Effets du personnage
        if (personnage.effets != null)
        {
            foreach (SCOBJ_PERSONNAGE_EFFET effet in personnage.effets)
            {
                if (effet == null || effet.modificateurs == null)
                    continue;

                if (!EFFET_Resolver.EstActif(effet, contexte))
                    continue;

                int deltaEffet = CalculerDeltaEffetPourAffichage(DATA_STATS_Result.baseValue, stat, effet);
                if (deltaEffet == 0)
                    continue;

                string signe = deltaEffet > 0 ? "+" : "";
                string nomEffet = effet.GetTitre(personnage.genre);

                if (string.IsNullOrWhiteSpace(nomEffet))
                    nomEffet = "Effet";

                result.detailLines.Add($"{nomEffet} : {signe}{deltaEffet}");
            }
        }

        // Effets des objets équipés
        if (personnage.objetsEquipes != null)
        {
            foreach (SCOBJ_OBJET_EQUIPPABLE objetEquipe in personnage.objetsEquipes)
            {
                if (objetEquipe == null || objetEquipe.effets == null)
                    continue;

                foreach (SCOBJ_EFFET effetObjet in objetEquipe.effets)
                {
                    if (effetObjet == null || effetObjet.modificateurs == null)
                        continue;

                    int deltaEffet = CalculerDeltaEffetPourAffichage(DATA_STATS_Result.baseValue, stat, effetObjet);
                    if (deltaEffet == 0)
                        continue;

                    string signe = deltaEffet > 0 ? "+" : "";
                    string nomEffet = !string.IsNullOrWhiteSpace(effetObjet.GetTitreAffiche())
                        ? effetObjet.GetTitreAffiche()
                        : objetEquipe.nom;

                    if (string.IsNullOrWhiteSpace(nomEffet))
                        nomEffet = "Équipement";

                    result.detailLines.Add($"{nomEffet} : {signe}{deltaEffet}");
                }
            }
        }

        int bonusRuntime = GetRuntimeBonus(contexte.STATE_PERSONNAGE, stat);
        if (bonusRuntime != 0)
        {
            string signe = bonusRuntime > 0 ? "+" : "";
            result.detailLines.Add($"Bonus runtime : {signe}{bonusRuntime}");
        }

        string signeTotal = result.delta > 0 ? "+" : "";
        result.detailLines.Add($"Total : {result.finalValue} ({signeTotal}{result.delta})");

        return result;
    }

    private static int CalculerDeltaEffetPourAffichage(
        int baseStat,
        EffetENUM_Stats stat,
        SCOBJ_EFFET effet)
    {
        if (effet == null || effet.modificateurs == null)
            return 0;

        int deltaEffet = 0;

        foreach (DATA_StatModifier modificateur in effet.modificateurs)
        {
            if (modificateur == null || modificateur.valeur == 0)
                continue;

            if (modificateur.stat != stat)
                continue;

            int valeurAbsolue = Mathf.Abs(modificateur.valeur);
            int valeur = modificateur.valeurType == EffetValeurType.Pourcentage
                ? Mathf.RoundToInt(baseStat * (valeurAbsolue / 100f))
                : valeurAbsolue;

            if (valeur == 0)
                continue;

            valeur = effet.type == EffetType.Malus
                ? -Mathf.Abs(valeur)
                : Mathf.Abs(valeur);

            deltaEffet += valeur;
        }

        return deltaEffet;
    }

    private static int GetRuntimeBonus(STATE_PERSONNAGE state, EffetENUM_Stats stat)
    {
        if (state == null)
            return 0;

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                return state.bonusCuriosite;
            case EffetENUM_Stats.Ingeniosite:
                return state.bonusIngeniosite;
            case EffetENUM_Stats.Combativite:
                return state.bonusCombativite;
            case EffetENUM_Stats.Endurance:
                return state.bonusEndurance;
            default:
                return 0;
        }
    }
}