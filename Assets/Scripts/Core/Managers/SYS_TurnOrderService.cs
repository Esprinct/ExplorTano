using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SYS_TurnOrderService
{
    public void ConstruireOrdreTourSelonPrestige(
        List<DATA_JOUEUR> joueurs,
        List<ENUM_Compagnie> ordreTourCourant,
        ref int indexJoueurActifTour)
    {
        if (ordreTourCourant == null)
            return;

        joueurs ??= new List<DATA_JOUEUR>();

        for (int i = 0; i < joueurs.Count; i++)
        {
            int j = Random.Range(i, joueurs.Count);
            (joueurs[i], joueurs[j]) = (joueurs[j], joueurs[i]);
        }

        joueurs = joueurs
            .Where(j => j != null)
            .OrderBy(j => j.prestige)
            .ToList();

        ordreTourCourant.Clear();

        foreach (DATA_JOUEUR joueur in joueurs)
        {
            ordreTourCourant.Add(joueur.compagnie);
        }

        indexJoueurActifTour = 0;

        Debug.Log("Ordre du tour : " + string.Join(" > ", ordreTourCourant));
    }

    public bool PasserAuJoueurSuivantDansLeTour(
    List<ENUM_Compagnie> ordreTourCourant,
    ref int indexJoueurActifTour)
{
    if (ordreTourCourant == null || ordreTourCourant.Count == 0)
        return false;

    if (indexJoueurActifTour >= ordreTourCourant.Count - 1)
        return false;

    indexJoueurActifTour++;
    return true;
}
}