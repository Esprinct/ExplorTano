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

    public int curiosite;
    public int ingeniosite;
    public int combativite;
    public int endurance;

    public int curiositeBase;
    public int ingeniositeBase;
    public int combativiteBase;
    public int enduranceBase;

    public int curiositeDelta;
    public int ingeniositeDelta;
    public int combativiteDelta;
    public int enduranceDelta;

    public string curiositeTooltipDetail;
    public string ingeniositeTooltipDetail;
    public string combativiteTooltipDetail;
    public string enduranceTooltipDetail;

    public int coutParTour;
    public EtriniumBreakdownData etriniumBreakdown = new();
    public string idUnique;
    public List<SCOBJ_EFFET> effets = new();
    
}