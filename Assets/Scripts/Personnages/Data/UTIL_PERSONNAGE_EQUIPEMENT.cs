using System.Collections.Generic;
using UnityEngine;

public static class UTIL_PERSONNAGE_EQUIPEMENT
{
    public static bool PeutEquiper(SCOBJ_Personnage personnage, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (personnage == null || objet == null)
            return false;

        return ConditionsRespectees(personnage, objet);
    }

    public static SCOBJ_OBJET_EQUIPPABLE GetObjetEquipe(
        SCOBJ_Personnage personnage,
        ENUM_OBJET_EQUIPPABLE type)
    {
        if (personnage == null || personnage.objetsEquipes == null)
            return null;

        foreach (var obj in personnage.objetsEquipes)
        {
            if (obj != null && obj.typeEquipable == type)
                return obj;
        }

        return null;
    }

    public static SCOBJ_OBJET_EQUIPPABLE Equiper(SCOBJ_Personnage personnage, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (!PeutEquiper(personnage, objet))
        {
            Debug.LogWarning("Conditions non respectées");
            return null;
        }

        if (personnage.objetsEquipes == null)
            personnage.objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>();

        // 🔥 Trouver ancien objet du même type
        SCOBJ_OBJET_EQUIPPABLE ancien = GetObjetEquipe(personnage, objet.typeEquipable);

        if (ancien != null)
        {
            personnage.objetsEquipes.Remove(ancien);
        }

        personnage.objetsEquipes.Add(objet);

        Debug.Log(
            $"Equipement | {personnage.nom} {personnage.prenom} | " +
            $"type={objet.typeEquipable} | " +
            $"nouveau={objet.nom} | " +
            $"ancien={(ancien != null ? ancien.nom : "Aucun")}"
        );

        return ancien;
    }

    public static bool Desequiper(SCOBJ_Personnage personnage, ENUM_OBJET_EQUIPPABLE type)
    {
        if (personnage == null || personnage.objetsEquipes == null)
            return false;

        SCOBJ_OBJET_EQUIPPABLE objet = GetObjetEquipe(personnage, type);
        if (objet == null)
            return false;

        personnage.objetsEquipes.Remove(objet);

        Debug.Log(
            $"Desequipement | {personnage.nom} {personnage.prenom} | {objet.nom}"
        );

        return true;
    }

    // 🔥 CONDITIONS
    public static bool ConditionsRespectees(SCOBJ_Personnage personnage, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (objet.conditionsEquipement == null || objet.conditionsEquipement.Count == 0)
            return true;

        foreach (var condition in objet.conditionsEquipement)
        {
            if (!ConditionRespectee(personnage, condition))
                return false;
        }

        return true;
    }

    private static bool ConditionRespectee(SCOBJ_Personnage p, DATA_OBJET_EQUIPPABLE_ConditionEquipement c)
    {
        switch (c.type)
        {
            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.NiveauMinimum:
                return p.progression != null && p.progression.niveau >= c.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CuriositeMinimum:
                return p.curiosite >= c.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.IngeniositeMinimum:
                return p.ingeniosite >= c.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.CombativiteMinimum:
                return p.combativite >= c.valeur;

            case ENUM_OBJET_EQUIPPABLE_ConditionEquipement.EnduranceMinimum:
                return p.endurance >= c.valeur;

            default:
                return true;
        }
    }

    public static List<SCOBJ_EFFET> GetEffetsEquipements(SCOBJ_Personnage personnage)
    {
        List<SCOBJ_EFFET> effets = new();

        if (personnage == null || personnage.objetsEquipes == null)
            return effets;

        foreach (SCOBJ_OBJET_EQUIPPABLE objet in personnage.objetsEquipes)
        {
            if (objet == null || objet.effets == null)
                continue;

            foreach (SCOBJ_EFFET effet in objet.effets)
            {
                if (effet != null)
                    effets.Add(effet);
            }
        }

        return effets;
    }
}