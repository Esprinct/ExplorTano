using System;

[Serializable]
public class DATA_STATS_AllocationDraft
{
    public int curiosite;
    public int ingeniosite;
    public int combativite;
    public int endurance;

    public bool autoCuriosite;
    public bool autoIngeniosite;
    public bool autoCombativite;
    public bool autoEndurance;

    public int pointsRestants;

    public static DATA_STATS_AllocationDraft FromPersonnage(SCOBJ_Personnage personnage)
    {
        DATA_STATS_AllocationDraft draft = new();

        if (personnage == null)
            return draft;

        if (personnage.allocation != null)
        {
            draft.curiosite = personnage.allocation.curiosite;
            draft.ingeniosite = personnage.allocation.ingeniosite;
            draft.combativite = personnage.allocation.combativite;
            draft.endurance = personnage.allocation.endurance;

            draft.autoCuriosite = personnage.allocation.autoCuriosite;
            draft.autoIngeniosite = personnage.allocation.autoIngeniosite;
            draft.autoCombativite = personnage.allocation.autoCombativite;
            draft.autoEndurance = personnage.allocation.autoEndurance;
        }

        if (personnage.progression != null)
        {
            draft.pointsRestants = personnage.progression.pointsDisponibles;
        }

        return draft;
    }
}