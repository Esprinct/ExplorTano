using System;

[Serializable]
public class DATA_STATS_AllocationDraft
{
    public int force;
    public int intelligence;
    public int dexterite;
    public int endurance;

    public bool autoForce;
    public bool autoIntelligence;
    public bool autoDexterite;
    public bool autoEndurance;

    public int pointsRestants;

    public static DATA_STATS_AllocationDraft FromPersonnage(SCOBJ_Personnage personnage)
    {
        DATA_STATS_AllocationDraft draft = new();

        if (personnage == null)
            return draft;

        if (personnage.allocation != null)
        {
            draft.force = personnage.allocation.force;
            draft.intelligence = personnage.allocation.intelligence;
            draft.dexterite = personnage.allocation.dexterite;
            draft.endurance = personnage.allocation.endurance;

            draft.autoForce = personnage.allocation.autoForce;
            draft.autoIntelligence = personnage.allocation.autoIntelligence;
            draft.autoDexterite = personnage.allocation.autoDexterite;
            draft.autoEndurance = personnage.allocation.autoEndurance;
        }

        if (personnage.progression != null)
        {
            draft.pointsRestants = personnage.progression.pointsDisponibles;
        }

        return draft;
    }
}