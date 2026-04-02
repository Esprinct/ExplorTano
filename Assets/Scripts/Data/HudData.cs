using UnityEngine;

[System.Serializable]
public class JoueurHudData
{
    [Header("Dirigeant")]
    public Sprite portraitDirigeant;
    public Sprite logoCompagnie;
    public string nomDirigeant;
    public int niveauDirigeant;

    [Header("Ressources")]
    public int etriniumTotal;
    public int etriniumParTour;
    public int prestige;
    public int provincesControlees;
    public string positionTexte;
}



