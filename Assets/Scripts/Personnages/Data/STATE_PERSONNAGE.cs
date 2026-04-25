using System;
using UnityEngine;

[Serializable]
public class STATE_PERSONNAGE
{
    public string idInstance;
    public SCOBJ_Personnage data;

    public int niveau;
    public int xpActuel;
    public int xpNiveauSuivant;

    public int bonusCuriosite;
    public int bonusIntelligence;
    public int bonusDexterite;
    public int bonusEndurance;
}