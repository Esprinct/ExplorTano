using System;
using UnityEngine;

[Serializable]
public class STATE_PROVINCE
{
    public SCOBJ_PROVINCE data;

    public ENUM_Compagnie? proprietaireActuel;
    public bool estClaim;

    public bool explorationEnCours;
    public int toursRestants;

    public float influenceMaizin;
    public float influenceKinia;
    public float influenceJoho;
    public float influenceAutre;

    // Exploration V2 par compagnie
    public float explorationMaizin = 0f;
    public float explorationKinia = 0f;
    public float explorationJoho = 0f;

    public float GetExploration(ENUM_Compagnie compagnie)
    {
        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                return explorationMaizin;

            case ENUM_Compagnie.Kinia:
                return explorationKinia;

            case ENUM_Compagnie.Joho:
                return explorationJoho;

            default:
                return 0f;
        }
    }

    public void SetExploration(ENUM_Compagnie compagnie, float valeur)
    {
        float clamped = Mathf.Clamp(valeur, 0f, 100f);

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                explorationMaizin = clamped;
                break;

            case ENUM_Compagnie.Kinia:
                explorationKinia = clamped;
                break;

            case ENUM_Compagnie.Joho:
                explorationJoho = clamped;
                break;
        }
    }

    public void AjouterExploration(ENUM_Compagnie compagnie, float montant)
    {
        float actuelle = GetExploration(compagnie);
        SetExploration(compagnie, actuelle + montant);
    }

    public bool EstEntierementExploreePar(ENUM_Compagnie compagnie)
    {
        return GetExploration(compagnie) >= 100f;
    }
}