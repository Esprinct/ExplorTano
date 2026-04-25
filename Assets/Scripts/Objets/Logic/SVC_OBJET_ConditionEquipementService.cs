using System;
public static class SVC_OBJET_ConditionEquipementService
{
    public static bool PeutEquiper(SCOBJ_Personnage personnage, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (personnage == null || objet == null)
            return false;

        if (objet.conditionsEquipement == null || objet.conditionsEquipement.Count == 0)
            return true;

        foreach (DATA_OBJET_EQUIPPABLE_ConditionEquipement condition in objet.conditionsEquipement)
        {
            if (condition == null)
                continue;

            if (!RespecteCondition(personnage, condition))
                return false;
        }

        return true;
    }

    private static bool RespecteCondition(SCOBJ_Personnage personnage, DATA_OBJET_EQUIPPABLE_ConditionEquipement condition)
    {
        if (personnage == null || condition == null)
            return false;

        switch (condition.type)
        {
            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.NiveauMinimum:
                return personnage.progression != null && personnage.progression.niveau >= condition.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CuriositeMinimum:
                return personnage.curiosite >= condition.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.IngeniositeMinimum:
                return personnage.ingeniosite >= condition.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CombativiteMinimum:
                return personnage.combativite >= condition.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.EnduranceMinimum:
                return personnage.endurance >= condition.valeur;

            default:
                return true;
        }
    }
}