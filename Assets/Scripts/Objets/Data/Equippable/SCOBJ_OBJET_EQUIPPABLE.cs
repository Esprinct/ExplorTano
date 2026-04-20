using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjetEquipable", menuName = "Game/Objets/Equipable")]
public class SCOBJ_OBJET_EQUIPPABLE : SCOBJ_OBJET
{
    [Header("Équipement")]
    public ENUM_OBJET_EQUIPPABLE typeEquipable;
    public bool uniqueParPersonnage = false;

    [Header("Conditions d'équipement")]
    public List<DATA_OBJET_EQUIPPABLE_ConditionEquipement> conditionsEquipement = new();

    private void OnValidate()
    {
        categorie = ENUM_OBJET_Categorie.Equipable;
    }
}