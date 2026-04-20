using System;
using UnityEngine;
public class EFFET_Contexte
{
     public SCOBJ_Personnage personnage;
    public STATE_EQUIPE equipe;
    public DATA_JOUEUR joueur;
    public STATE_PROVINCE province;
    public ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune;
    public STATE_PERSONNAGE STATE_PERSONNAGE;

    public SCOBJ_DIRIGEANT dirigeant;

    public int nbShikisEquipe;
    public int nbFrisiensEquipe;
    public int nbAutresEquipe;

    public float ratioPopulationShiki;
    public float ratioPopulationFrisien;
    public float ratioPopulationAutre;
    public static EFFET_Contexte ForPersonnage(
        SCOBJ_Personnage personnage,
        ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune,
        STATE_PERSONNAGE STATE_PERSONNAGE = null,
        STATE_EQUIPE equipe = null,
        DATA_JOUEUR joueur = null,
        STATE_PROVINCE province = null)
    {
        return new EFFET_Contexte
        {
            personnage = personnage,
            compagnie = compagnie,
            STATE_PERSONNAGE = STATE_PERSONNAGE,
            equipe = equipe,
            joueur = joueur,
            province = province
        };
    }
}