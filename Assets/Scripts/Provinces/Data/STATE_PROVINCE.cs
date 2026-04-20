using System;

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
}