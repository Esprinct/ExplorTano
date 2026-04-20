using UnityEngine;

public static class DATA_STATS_AllocationDraftService
{
    public static int GetValue(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat)
    {
        if (draft == null)
            return 0;

        return stat switch
        {
            EffetENUM_Stats.Force => draft.force,
            EffetENUM_Stats.Intelligence => draft.intelligence,
            EffetENUM_Stats.Dexterite => draft.dexterite,
            EffetENUM_Stats.Endurance => draft.endurance,
            _ => 0
        };
    }

    public static bool TryAddPoint(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat)
    {
        if (draft == null || draft.pointsRestants <= 0)
            return false;

        switch (stat)
        {
            case EffetENUM_Stats.Force:
                draft.force++;
                break;
            case EffetENUM_Stats.Intelligence:
                draft.intelligence++;
                break;
            case EffetENUM_Stats.Dexterite:
                draft.dexterite++;
                break;
            case EffetENUM_Stats.Endurance:
                draft.endurance++;
                break;
            default:
                return false;
        }

        draft.pointsRestants--;
        return true;
    }

    public static bool TryRemovePoint(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat, SCOBJ_Personnage personnage)
    {
        if (draft == null || personnage == null || personnage.allocation == null)
            return false;

        int minimumConfirme = stat switch
        {
            EffetENUM_Stats.Force => personnage.allocation.force,
            EffetENUM_Stats.Intelligence => personnage.allocation.intelligence,
            EffetENUM_Stats.Dexterite => personnage.allocation.dexterite,
            EffetENUM_Stats.Endurance => personnage.allocation.endurance,
            _ => 0
        };

        int current = GetValue(draft, stat);

        if (current <= minimumConfirme)
            return false;

        switch (stat)
        {
            case EffetENUM_Stats.Force:
                draft.force--;
                break;
            case EffetENUM_Stats.Intelligence:
                draft.intelligence--;
                break;
            case EffetENUM_Stats.Dexterite:
                draft.dexterite--;
                break;
            case EffetENUM_Stats.Endurance:
                draft.endurance--;
                break;
            default:
                return false;
        }

        draft.pointsRestants++;
        return true;
    }

    public static bool TrySetValue(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat, int targetValue, SCOBJ_Personnage personnage)
    {
        if (draft == null || personnage == null || personnage.allocation == null)
            return false;

        targetValue = Mathf.Max(0, targetValue);

        int minimumConfirme = stat switch
        {
            EffetENUM_Stats.Force => personnage.allocation.force,
            EffetENUM_Stats.Intelligence => personnage.allocation.intelligence,
            EffetENUM_Stats.Dexterite => personnage.allocation.dexterite,
            EffetENUM_Stats.Endurance => personnage.allocation.endurance,
            _ => 0
        };

        if (targetValue < minimumConfirme)
            targetValue = minimumConfirme;

        int current = GetValue(draft, stat);
        int diff = targetValue - current;

        if (diff == 0)
            return true;

        if (diff > 0)
        {
            if (draft.pointsRestants < diff)
                return false;

            SetValue(draft, stat, targetValue);
            draft.pointsRestants -= diff;
            return true;
        }

        draft.pointsRestants += -diff;
        SetValue(draft, stat, targetValue);
        return true;
    }

    public static void SetAuto(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat, bool enabled)
    {
        if (draft == null)
            return;

        switch (stat)
        {
            case EffetENUM_Stats.Force:
                draft.autoForce = enabled;
                break;
            case EffetENUM_Stats.Intelligence:
                draft.autoIntelligence = enabled;
                break;
            case EffetENUM_Stats.Dexterite:
                draft.autoDexterite = enabled;
                break;
            case EffetENUM_Stats.Endurance:
                draft.autoEndurance = enabled;
                break;
        }
    }

    public static bool IsAutoEnabled(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat)
    {
        if (draft == null)
            return false;

        return stat switch
        {
            EffetENUM_Stats.Force => draft.autoForce,
            EffetENUM_Stats.Intelligence => draft.autoIntelligence,
            EffetENUM_Stats.Dexterite => draft.autoDexterite,
            EffetENUM_Stats.Endurance => draft.autoEndurance,
            _ => false
        };
    }

    public static void ApplyAutoAllocation(DATA_STATS_AllocationDraft draft)
    {
        if (draft == null)
            return;

        while (draft.pointsRestants > 0)
        {
            bool allocated = false;

            if (draft.autoForce)
            {
                draft.force++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (draft.autoIntelligence)
            {
                draft.intelligence++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (draft.autoDexterite)
            {
                draft.dexterite++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (draft.autoEndurance)
            {
                draft.endurance++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (!allocated)
                break;
        }
    }

    public static void Commit(SCOBJ_Personnage personnage, DATA_STATS_AllocationDraft draft)
    {
        if (personnage == null || personnage.allocation == null || personnage.progression == null || draft == null)
            return;

        personnage.allocation.force = draft.force;
        personnage.allocation.intelligence = draft.intelligence;
        personnage.allocation.dexterite = draft.dexterite;
        personnage.allocation.endurance = draft.endurance;

        personnage.allocation.autoForce = draft.autoForce;
        personnage.allocation.autoIntelligence = draft.autoIntelligence;
        personnage.allocation.autoDexterite = draft.autoDexterite;
        personnage.allocation.autoEndurance = draft.autoEndurance;

        personnage.progression.pointsDisponibles = draft.pointsRestants;
    }

    private static void SetValue(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat, int value)
    {
        switch (stat)
        {
            case EffetENUM_Stats.Force:
                draft.force = value;
                break;
            case EffetENUM_Stats.Intelligence:
                draft.intelligence = value;
                break;
            case EffetENUM_Stats.Dexterite:
                draft.dexterite = value;
                break;
            case EffetENUM_Stats.Endurance:
                draft.endurance = value;
                break;
        }
    }
}