using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DATA_PERSONNAGE_Detail
{
    public string nom;
    public string prenom;
    public Sprite sprite;
    public int rareteEtoiles;
 
public DATA_LevelProgressionView progression;
    public string role;
    public string description;
    public ENUM_PERSONNAGE_Genre genre;

    public int force;
    public int intelligence;
    public int dexterite;
    public int endurance;

    public int forceBase;
    public int intelligenceBase;
    public int dexteriteBase;
    public int enduranceBase;

    public int forceDelta;
    public int intelligenceDelta;
    public int dexteriteDelta;
    public int enduranceDelta;

    public string forceTooltipDetail;
    public string intelligenceTooltipDetail;
    public string dexteriteTooltipDetail;
    public string enduranceTooltipDetail;

    public int coutParTour;
    public EtriniumBreakdownData etriniumBreakdown = new();
    public string idUnique;
    public List<SCOBJ_EFFET> effets = new();
    
}