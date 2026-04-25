using UnityEngine;

public static class SVC_STATS_Allocation
{
    public static int GetAllocatedValue(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null || personnage.allocation == null)
            return 0;

        return stat switch
        {
            EffetENUM_Stats.Curiosite => personnage.allocation.curiosite,
            EffetENUM_Stats.Intelligence => personnage.allocation.intelligence,
            EffetENUM_Stats.Dexterite => personnage.allocation.dexterite,
            EffetENUM_Stats.Endurance => personnage.allocation.endurance,
            _ => 0
        };
    }

    public static bool IsAutoEnabled(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null || personnage.allocation == null)
            return false;

        return stat switch
        {
            EffetENUM_Stats.Curiosite => personnage.allocation.autoCuriosite,
            EffetENUM_Stats.Intelligence => personnage.allocation.autoIntelligence,
            EffetENUM_Stats.Dexterite => personnage.allocation.autoDexterite,
            EffetENUM_Stats.Endurance => personnage.allocation.autoEndurance,
            _ => false
        };
    }

    public static void SetAuto(SCOBJ_Personnage personnage, EffetENUM_Stats stat, bool enabled)
    {
        if (personnage == null || personnage.allocation == null)
            return;

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                personnage.allocation.autoCuriosite = enabled;
                break;
            case EffetENUM_Stats.Intelligence:
                personnage.allocation.autoIntelligence = enabled;
                break;
            case EffetENUM_Stats.Dexterite:
                personnage.allocation.autoDexterite = enabled;
                break;
            case EffetENUM_Stats.Endurance:
                personnage.allocation.autoEndurance = enabled;
                break;
        }
    }

    public static bool TryAddPoint(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null || personnage.progression == null || personnage.allocation == null)
            return false;

        if (personnage.progression.pointsDisponibles <= 0)
            return false;

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                personnage.allocation.curiosite++;
                break;
            case EffetENUM_Stats.Intelligence:
                personnage.allocation.intelligence++;
                break;
            case EffetENUM_Stats.Dexterite:
                personnage.allocation.dexterite++;
                break;
            case EffetENUM_Stats.Endurance:
                personnage.allocation.endurance++;
                break;
            default:
                return false;
        }

        personnage.progression.pointsDisponibles--;
        return true;
    }

    public static bool TryRemovePoint(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
    {
        if (personnage == null || personnage.progression == null || personnage.allocation == null)
            return false;

        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                if (personnage.allocation.curiosite <= 0) return false;
                personnage.allocation.curiosite--;
                break;
            case EffetENUM_Stats.Intelligence:
                if (personnage.allocation.intelligence <= 0) return false;
                personnage.allocation.intelligence--;
                break;
            case EffetENUM_Stats.Dexterite:
                if (personnage.allocation.dexterite <= 0) return false;
                personnage.allocation.dexterite--;
                break;
            case EffetENUM_Stats.Endurance:
                if (personnage.allocation.endurance <= 0) return false;
                personnage.allocation.endurance--;
                break;
            default:
                return false;
        }

        personnage.progression.pointsDisponibles++;
        return true;
    }

    public static bool TrySetValue(SCOBJ_Personnage personnage, EffetENUM_Stats stat, int targetValue)
    {
        if (personnage == null || personnage.progression == null || personnage.allocation == null)
            return false;

        targetValue = Mathf.Max(0, targetValue);

        int currentValue = GetAllocatedValue(personnage, stat);
        int diff = targetValue - currentValue;

        if (diff == 0)
            return true;

        if (diff > 0)
        {
            if (personnage.progression.pointsDisponibles < diff)
                return false;

            SetAllocatedValue(personnage, stat, targetValue);
            personnage.progression.pointsDisponibles -= diff;
            return true;
        }

        int remboursement = -diff;
        SetAllocatedValue(personnage, stat, targetValue);
        personnage.progression.pointsDisponibles += remboursement;
        return true;
    }

    public static void ApplyAutoAllocation(SCOBJ_Personnage personnage)
    {
        if (personnage == null || personnage.progression == null || personnage.allocation == null)
            return;

        while (personnage.progression.pointsDisponibles > 0)
        {
            bool allocated = false;

            if (personnage.allocation.autoCuriosite)
            {
                personnage.allocation.curiosite++;
                personnage.progression.pointsDisponibles--;
                allocated = true;
                if (personnage.progression.pointsDisponibles <= 0) break;
            }

            if (personnage.allocation.autoIntelligence)
            {
                personnage.allocation.intelligence++;
                personnage.progression.pointsDisponibles--;
                allocated = true;
                if (personnage.progression.pointsDisponibles <= 0) break;
            }

            if (personnage.allocation.autoDexterite)
            {
                personnage.allocation.dexterite++;
                personnage.progression.pointsDisponibles--;
                allocated = true;
                if (personnage.progression.pointsDisponibles <= 0) break;
            }

            if (personnage.allocation.autoEndurance)
            {
                personnage.allocation.endurance++;
                personnage.progression.pointsDisponibles--;
                allocated = true;
                if (personnage.progression.pointsDisponibles <= 0) break;
            }

            if (!allocated)
                break;
        }
    }

    public static void ResetAllocation(SCOBJ_Personnage personnage)
    {
        if (personnage == null || personnage.progression == null || personnage.allocation == null)
            return;

        personnage.progression.pointsDisponibles += personnage.allocation.curiosite;
        personnage.progression.pointsDisponibles += personnage.allocation.intelligence;
        personnage.progression.pointsDisponibles += personnage.allocation.dexterite;
        personnage.progression.pointsDisponibles += personnage.allocation.endurance;

        personnage.allocation.curiosite = 0;
        personnage.allocation.intelligence = 0;
        personnage.allocation.dexterite = 0;
        personnage.allocation.endurance = 0;
    }
public static int GetAllocationBonus(SCOBJ_Personnage personnage, EffetENUM_Stats stat)
{
    if (personnage == null || personnage.allocation == null)
        return 0;

    return stat switch
    {
        EffetENUM_Stats.Curiosite => personnage.allocation.curiosite,
        EffetENUM_Stats.Intelligence => personnage.allocation.intelligence,
        EffetENUM_Stats.Dexterite => personnage.allocation.dexterite,
        EffetENUM_Stats.Endurance => personnage.allocation.endurance,
        _ => 0
    };
}
    private static void SetAllocatedValue(SCOBJ_Personnage personnage, EffetENUM_Stats stat, int value)
    {
        switch (stat)
        {
            case EffetENUM_Stats.Curiosite:
                personnage.allocation.curiosite = value;
                break;
            case EffetENUM_Stats.Intelligence:
                personnage.allocation.intelligence = value;
                break;
            case EffetENUM_Stats.Dexterite:
                personnage.allocation.dexterite = value;
                break;
            case EffetENUM_Stats.Endurance:
                personnage.allocation.endurance = value;
                break;
        }
    }
}