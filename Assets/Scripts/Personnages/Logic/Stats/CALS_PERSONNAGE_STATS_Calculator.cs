using System.Collections.Generic;
using UnityEngine;

public static class CALS_PERSONNAGE_STATS_Calculator
{
    public static DATA_STATS_Result ComputeStat(
        SCOBJ_Personnage personnage,
        EffetENUM_Stats stat,
        STATE_PERSONNAGE state = null,
        ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune)
    {
        DATA_STATS_Result result = new();

        if (personnage == null)
            return result;

        int baseBrute = GetBaseStat(personnage, stat);
        int bonusAllocation = SVC_STATS_Allocation.GetAllocationBonus(personnage, stat);
        int baseValue = baseBrute + bonusAllocation;

        int delta = 0;

        EFFET_Contexte contexte = EFFET_Contexte.ForPersonnage(
            personnage,
            compagnie,
            state
        );

        // Effets du personnage
        if (personnage.effets != null)
        {
            foreach (SCOBJ_PERSONNAGE_EFFET effet in personnage.effets)
            {
                if (effet == null || effet.modificateurs == null)
                    continue;

                if (!EFFET_Resolver.EstActif(effet, contexte))
                    continue;

                delta += CalculerDeltaEffet(baseValue, stat, effet);
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

                    delta += CalculerDeltaEffet(baseValue, stat, effetObjet);
                }
            }
        }

        delta += GetRuntimeBonus(state, stat);

        result.baseValue = baseValue;
        result.delta = delta;
        result.finalValue = Mathf.Max(0, baseValue + delta);

        return result;
    }

    public static int GetForceEffective(SCOBJ_Personnage personnage)
        => GetStatEffective(personnage, EffetENUM_Stats.Force);

    public static int GetIntelligenceEffective(SCOBJ_Personnage personnage)
        => GetStatEffective(personnage, EffetENUM_Stats.Intelligence);

    public static int GetDexteriteEffective(SCOBJ_Personnage personnage)
        => GetStatEffective(personnage, EffetENUM_Stats.Dexterite);

    public static int GetEnduranceEffective(SCOBJ_Personnage personnage)
        => GetStatEffective(personnage, EffetENUM_Stats.Endurance);

    public static int GetForceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatEffective(personnage, EffetENUM_Stats.Force, null, compagnie);

    public static int GetIntelligenceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatEffective(personnage, EffetENUM_Stats.Intelligence, null, compagnie);

    public static int GetDexteriteEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatEffective(personnage, EffetENUM_Stats.Dexterite, null, compagnie);

    public static int GetEnduranceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatEffective(personnage, EffetENUM_Stats.Endurance, null, compagnie);

    public static int GetForceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatEffective(personnage, EffetENUM_Stats.Force, state, compagnie);

    public static int GetIntelligenceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatEffective(personnage, EffetENUM_Stats.Intelligence, state, compagnie);

    public static int GetDexteriteEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatEffective(personnage, EffetENUM_Stats.Dexterite, state, compagnie);

    public static int GetEnduranceEffective(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatEffective(personnage, EffetENUM_Stats.Endurance, state, compagnie);

    public static int GetForceDelta(SCOBJ_Personnage personnage)
        => GetStatDelta(personnage, EffetENUM_Stats.Force);

    public static int GetIntelligenceDelta(SCOBJ_Personnage personnage)
        => GetStatDelta(personnage, EffetENUM_Stats.Intelligence);

    public static int GetDexteriteDelta(SCOBJ_Personnage personnage)
        => GetStatDelta(personnage, EffetENUM_Stats.Dexterite);

    public static int GetEnduranceDelta(SCOBJ_Personnage personnage)
        => GetStatDelta(personnage, EffetENUM_Stats.Endurance);

    public static int GetForceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatDelta(personnage, EffetENUM_Stats.Force, null, compagnie);

    public static int GetIntelligenceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatDelta(personnage, EffetENUM_Stats.Intelligence, null, compagnie);

    public static int GetDexteriteDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatDelta(personnage, EffetENUM_Stats.Dexterite, null, compagnie);

    public static int GetEnduranceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie)
        => GetStatDelta(personnage, EffetENUM_Stats.Endurance, null, compagnie);

    public static int GetForceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatDelta(personnage, EffetENUM_Stats.Force, state, compagnie);

    public static int GetIntelligenceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatDelta(personnage, EffetENUM_Stats.Intelligence, state, compagnie);

    public static int GetDexteriteDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatDelta(personnage, EffetENUM_Stats.Dexterite, state, compagnie);

    public static int GetEnduranceDelta(SCOBJ_Personnage personnage, ENUM_Compagnie compagnie, STATE_PERSONNAGE state)
        => GetStatDelta(personnage, EffetENUM_Stats.Endurance, state, compagnie);

    public static int GetStatEffective(
        SCOBJ_Personnage personnage,
        EffetENUM_Stats stat,
        STATE_PERSONNAGE state = null,
        ENUM_Compagnie compagnie =  ENUM_Compagnie.Aucune)
    {
        return ComputeStat(personnage, stat, state, compagnie).finalValue;
    }

    public static int GetStatDelta(
        SCOBJ_Personnage personnage,
        EffetENUM_Stats stat,
        STATE_PERSONNAGE state = null,
        ENUM_Compagnie compagnie =  ENUM_Compagnie.Aucune)
    {
        return ComputeStat(personnage, stat, state, compagnie).delta;
    }

    public static int GetBaseStat(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null)
            return 0;

        switch (stat)
        {
            case EffetENUM_Stats.Force:
                return personnage.force;
            case EffetENUM_Stats.Intelligence:
                return personnage.intelligence;
            case EffetENUM_Stats.Dexterite:
                return personnage.dexterite;
            case EffetENUM_Stats.Endurance:
                return personnage.endurance;
            default:
                return 0;
        }
    }

    public static int GetBaseStatAvecAllocation(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null)
            return 0;

        return GetBaseStat(personnage, stat) + SVC_STATS_Allocation.GetAllocationBonus(personnage, stat);
    }

    private static int CalculerDeltaEffet(int baseStat, EffetENUM_Stats stat, SCOBJ_EFFET effet)
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

            int valeur = CalculerValeurModificateur(baseStat, modificateur);

            if (valeur == 0)
                continue;

            valeur = effet.type == EffetType.Malus
                ? -Mathf.Abs(valeur)
                : Mathf.Abs(valeur);

            deltaEffet += valeur;
        }

        return deltaEffet;
    }

    private static int CalculerValeurModificateur(int baseStat, DATA_StatModifier modificateur)
    {
        if (modificateur == null)
            return 0;

        int valeurAbsolue = Mathf.Abs(modificateur.valeur);

        switch (modificateur.valeurType)
        {
            case EffetValeurType.Pourcentage:
                return Mathf.RoundToInt(baseStat * (valeurAbsolue / 100f));

            case EffetValeurType.Fixe:
            default:
                return valeurAbsolue;
        }
    }

    private static int GetRuntimeBonus(STATE_PERSONNAGE state, EffetENUM_Stats stat)
    {
        if (state == null)
            return 0;

        switch (stat)
        {
            case EffetENUM_Stats.Force:
                return state.bonusForce;
            case EffetENUM_Stats.Intelligence:
                return state.bonusIntelligence;
            case EffetENUM_Stats.Dexterite:
                return state.bonusDexterite;
            case EffetENUM_Stats.Endurance:
                return state.bonusEndurance;
            default:
                return 0;
        }
    }
    public static bool EstEffetActif(
    SCOBJ_PERSONNAGE_EFFET effet,
    SCOBJ_Personnage personnage,
    ENUM_Compagnie compagnie)
{
    if (effet == null || personnage == null)
        return false;

    EFFET_Contexte contexte = EFFET_Contexte.ForPersonnage(
        personnage,
        compagnie,
        null
    );

    return EFFET_Resolver.EstActif(effet, contexte);
}
}