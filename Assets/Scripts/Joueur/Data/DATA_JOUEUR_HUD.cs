using UnityEngine;

[System.Serializable]
public class DATA_JOUEUR_HUD
{
    [Header("Dirigeant")]
    public Sprite portraitDirigeant;
    public Sprite logoCompagnie;
    public string nomDirigeant = "Dirigeant";
    public int niveauDirigeant = 1;
    public SCOBJ_DIRIGEANT dirigeant;
    public int xpDirigeant;
public int xpDirigeantPourNiveauSuivant;

    [Header("Ressources")]
    public int etriniumTotal;
    public int etriniumParTour;
    public int prestige;
    public int provincesControlees;
    public string positionTexte = "";
    public EtriniumBreakdownData etriniumBreakdown = new();
}