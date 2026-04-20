using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DATA_JOUEUR
{
    public string nomJoueur;
    public bool estHumain;
    public ENUM_Compagnie compagnie;

    [SerializeField] private SCOBJ_DIRIGEANT dirigeant;
public ENUM_IA_Personnalite personnaliteIA = ENUM_IA_Personnalite.Equilibree;
    public SCOBJ_DIRIGEANT Dirigeant
    {
        get => dirigeant;
        set
        {
            dirigeant = value;
            SynchroniserCompagnieDepuisDirigeant();
        }
    }

    public float etrinium;
    public float prestige;
    public float etriniumParTour;
    public EtriniumBreakdownData etriniumBreakdown;

    public List<STATE_EQUIPE> equipes = new();

    public int provincesControlees;
    public List<SCOBJ_Personnage> personnagesRecrutes = new();
    public List<SCOBJ_OBJET> objetsPossedes = new();
    public List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommablesPossedes = new();

    public Sprite GetLogoCompagnie()
    {
        return dirigeant != null ? dirigeant.logoCompagnie : null;
    }

    public Sprite GetPortraitDirigeant()
    {
        return dirigeant != null ? dirigeant.portraitDirigeant : null;
    }

    public string GetNomDirigeant()
    {
        return dirigeant != null && !string.IsNullOrWhiteSpace(dirigeant.nomDirigeant)
            ? dirigeant.nomDirigeant
            : "Dirigeant";
    }

    public int GetNiveauDirigeant()
    {
        return dirigeant != null ? dirigeant.niveauDirigeant : 1;
    }

    public ENUM_Compagnie GetCompagnieDepuisDirigeant()
    {
        return dirigeant != null && dirigeant.compagnie != ENUM_Compagnie.Aucune
            ? dirigeant.compagnie
            : compagnie;
    }

    public void SynchroniserCompagnieDepuisDirigeant()
    {
        if (dirigeant != null && dirigeant.compagnie != ENUM_Compagnie.Aucune)
        {
            compagnie = dirigeant.compagnie;
        }
    }
}