using UnityEngine;

public static class DATA_STATS_AllocationDraftService
{
    public static int GetValue(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat)
    {
        if (draft == null)
            return 0;

        return stat switch
        {
            EffetENUM_Stats.Curiosite => draft.curiosite,
            EffetENUM_Stats.Ingeniosite => draft.ingeniosite,
            EffetENUM_Stats.Combativite => draft.combativite,
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
            case EffetENUM_Stats.Curiosite:
                draft.curiosite++;
                break;
            case EffetENUM_Stats.Ingeniosite:
                draft.ingeniosite++;
                break;
            case EffetENUM_Stats.Combativite:
                draft.combativite++;
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
            EffetENUM_Stats.Curiosite => personnage.allocation.curiosite,
            EffetENUM_Stats.Ingeniosite => personnage.allocation.ingeniosite,
            EffetENUM_Stats.Combativite => personnage.allocation.combativite,
            EffetENUM_Stats.Endurance => personnage.allocation.endurance,
            _ => 0
        };

        int current = GetValue(draft, stat);

        if (current <= minimumConfirme)
            return false;

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                draft.curiosite--;
                break;
            case EffetENUM_Stats.Ingeniosite:
                draft.ingeniosite--;
                break;
            case EffetENUM_Stats.Combativite:
                draft.combativite--;
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
            EffetENUM_Stats.Curiosite => personnage.allocation.curiosite,
            EffetENUM_Stats.Ingeniosite => personnage.allocation.ingeniosite,
            EffetENUM_Stats.Combativite => personnage.allocation.combativite,
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
            case EffetENUM_Stats.Curiosite:
                draft.autoCuriosite = enabled;
                break;
            case EffetENUM_Stats.Ingeniosite:
                draft.autoIngeniosite = enabled;
                break;
            case EffetENUM_Stats.Combativite:
                draft.autoCombativite = enabled;
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
            EffetENUM_Stats.Curiosite => draft.autoCuriosite,
            EffetENUM_Stats.Ingeniosite => draft.autoIngeniosite,
            EffetENUM_Stats.Combativite => draft.autoCombativite,
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

            if (draft.autoCuriosite)
            {
                draft.curiosite++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (draft.autoIngeniosite)
            {
                draft.ingeniosite++;
                draft.pointsRestants--;
                allocated = true;
                if (draft.pointsRestants <= 0) break;
            }

            if (draft.autoCombativite)
            {
                draft.combativite++;
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

        personnage.allocation.curiosite = draft.curiosite;
        personnage.allocation.ingeniosite = draft.ingeniosite;
        personnage.allocation.combativite = draft.combativite;
        personnage.allocation.endurance = draft.endurance;

        personnage.allocation.autoCuriosite = draft.autoCuriosite;
        personnage.allocation.autoIngeniosite = draft.autoIngeniosite;
        personnage.allocation.autoCombativite = draft.autoCombativite;
        personnage.allocation.autoEndurance = draft.autoEndurance;

        personnage.progression.pointsDisponibles = draft.pointsRestants;
    }

    private static void SetValue(DATA_STATS_AllocationDraft draft, EffetENUM_Stats stat, int value)
    {
        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                draft.curiosite = value;
                break;
            case EffetENUM_Stats.Ingeniosite:
                draft.ingeniosite = value;
                break;
            case EffetENUM_Stats.Combativite:
                draft.combativite = value;
                break;
            case EffetENUM_Stats.Endurance:
                draft.endurance = value;
                break;
        }
    }
}