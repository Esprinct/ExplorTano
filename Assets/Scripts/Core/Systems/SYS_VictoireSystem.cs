using System.Collections.Generic;
using UnityEngine;

public class SYS_VictoireSystem
{
    public bool VerifierFinDePartie(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.PartieData == null)
            return false;

        if (gameManager.PartieData.tourActuel < gameManager.PartieData.tourMax)
            return false;

        gameManager.PartieTerminee = true;

        List<DATA_JOUEUR> joueurs = RecupererJoueursValides(gameManager);
        if (joueurs.Count == 0)
        {
            Debug.LogWarning("Fin de partie atteinte, mais aucun joueur valide trouvé.");
            return true;
        }

        DATA_JOUEUR gagnant = TrouverGagnant(joueurs, out bool egaliteParfaite);

        if (egaliteParfaite)
        {
            Debug.Log("Partie terminée ! Égalité parfaite au classement final.");
        }
        else
        {
            Debug.Log(
                $"Partie terminée ! Vainqueur : {gagnant.nomJoueur} | " +
                $"Compagnie={gagnant.compagnie} | " +
                $"Prestige={gagnant.prestige} | " +
                $"Provinces={gagnant.provincesControlees} | " +
                $"Etrinium={gagnant.etrinium}"
            );
        }

        return true;
    }

    public DATA_JOUEUR GetGagnant(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return null;

        List<DATA_JOUEUR> joueurs = RecupererJoueursValides(gameManager);
        if (joueurs.Count == 0)
            return null;

        return TrouverGagnant(joueurs, out _);
    }

    public int ComparerJoueursPourVictoire(DATA_JOUEUR a, DATA_JOUEUR b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return -1;
        if (b == null) return 1;

        int comparaisonPrestige = a.prestige.CompareTo(b.prestige);
        if (comparaisonPrestige != 0)
            return comparaisonPrestige;

        int comparaisonProvinces = a.provincesControlees.CompareTo(b.provincesControlees);
        if (comparaisonProvinces != 0)
            return comparaisonProvinces;

        return a.etrinium.CompareTo(b.etrinium);
    }

    private DATA_JOUEUR TrouverGagnant(List<DATA_JOUEUR> joueurs, out bool egaliteParfaite)
    {
        egaliteParfaite = false;

        if (joueurs == null || joueurs.Count == 0)
            return null;

        DATA_JOUEUR gagnant = joueurs[0];

        for (int i = 1; i < joueurs.Count; i++)
        {
            DATA_JOUEUR challenger = joueurs[i];
            int comparaison = ComparerJoueursPourVictoire(challenger, gagnant);

            if (comparaison > 0)
            {
                gagnant = challenger;
                egaliteParfaite = false;
            }
            else if (comparaison == 0)
            {
                egaliteParfaite = true;
            }
        }

        return gagnant;
    }

    private List<DATA_JOUEUR> RecupererJoueursValides(SYS_GameManager gameManager)
    {
        List<DATA_JOUEUR> joueurs = new();

        if (gameManager == null)
            return joueurs;

        if (gameManager.Joueur1 != null) joueurs.Add(gameManager.Joueur1);
        if (gameManager.Joueur2 != null) joueurs.Add(gameManager.Joueur2);
        if (gameManager.Joueur3 != null) joueurs.Add(gameManager.Joueur3);

        return joueurs;
    }
}