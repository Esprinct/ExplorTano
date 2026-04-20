using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCOBJ_Dirigeant", menuName = "Game/Dirigeant")]
public class SCOBJ_DIRIGEANT : ScriptableObject
{
    [Header("Identité")]
    public ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune;
    public Sprite portraitDirigeant;
    public Sprite logoCompagnie;
    public string nomDirigeant = "Dirigeant";

    [Header("Valeurs initiales")]
    public int niveauInitial = 1;
    public int xpInitiale = 0;
    public int xpPourNiveauSuivantInitial = 100;

    [Header("Runtime")]
    public int niveauDirigeant = 1;
    public int xpDirigeant = 0;
    public int xpPourNiveauSuivant = 100;

    [TextArea]
    public string description;

    [Header("Effets affichés dans l'UI")]
    public List<SCOBJ_DIRIGEANT_EFFET> effets = new();

    public void ResetProgression()
    {
        niveauDirigeant = niveauInitial;
        xpDirigeant = xpInitiale;
        xpPourNiveauSuivant = xpPourNiveauSuivantInitial;
    }

    public int AjouterXp(int montant)
    {
        if (montant <= 0)
            return 0;

        xpDirigeant += montant;

        int niveauxGagnes = 0;

        while (xpDirigeant >= xpPourNiveauSuivant)
        {
            xpDirigeant -= xpPourNiveauSuivant;
            niveauDirigeant++;
            niveauxGagnes++;

            xpPourNiveauSuivant = CalculerXpPourNiveauSuivant(niveauDirigeant);
        }

        return niveauxGagnes;
    }

    private int CalculerXpPourNiveauSuivant(int niveauActuel)
    {
        return 100 + ((niveauActuel - 1) * 25);
    }
}