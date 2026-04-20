using System;
using UnityEngine;

[Serializable]
public class STATE_PERSONNAGE_EQUIPEMENT
{
    public SCOBJ_OBJET_EQUIPPABLE outil;
    public SCOBJ_OBJET_EQUIPPABLE tenue;
    public SCOBJ_OBJET_EQUIPPABLE accessoire;

    public SCOBJ_OBJET_EQUIPPABLE GetObjetEquipe(ENUM_OBJET_EQUIPPABLE type)
    {
        switch (type)
        {
            case ENUM_OBJET_EQUIPPABLE.Outil:
                return outil;

            case ENUM_OBJET_EQUIPPABLE.Tenue:
                return tenue;

            case ENUM_OBJET_EQUIPPABLE.Accessoire:
                return accessoire;

            default:
                return null;
        }
    }

    public void SetObjetEquipe(ENUM_OBJET_EQUIPPABLE type, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        switch (type)
        {
            case ENUM_OBJET_EQUIPPABLE.Outil:
                outil = objet;
                break;

            case ENUM_OBJET_EQUIPPABLE.Tenue:
                tenue = objet;
                break;

            case ENUM_OBJET_EQUIPPABLE.Accessoire:
                accessoire = objet;
                break;
        }
    }
}