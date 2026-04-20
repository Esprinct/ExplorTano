using System.Collections.Generic;

public class SYS_PlayerAccessService
{
    public DATA_JOUEUR GetByCompagnie(
        ENUM_Compagnie compagnie,
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        if (joueur1 != null && joueur1.compagnie == compagnie)
            return joueur1;

        if (joueur2 != null && joueur2.compagnie == compagnie)
            return joueur2;

        if (joueur3 != null && joueur3.compagnie == compagnie)
            return joueur3;

        return null;
    }

    public List<DATA_JOUEUR> GetAllPlayers(
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        List<DATA_JOUEUR> joueurs = new();

        if (joueur1 != null) joueurs.Add(joueur1);
        if (joueur2 != null) joueurs.Add(joueur2);
        if (joueur3 != null) joueurs.Add(joueur3);

        return joueurs;
    }

    public DATA_JOUEUR GetHumanPlayer(
        
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        
        if (joueur1 != null && joueur1.estHumain) return joueur1;
        if (joueur2 != null && joueur2.estHumain) return joueur2;
        if (joueur3 != null && joueur3.estHumain) return joueur3;

        return null;
    }

    public DATA_JOUEUR GetJoueurActifTour(
        List<ENUM_Compagnie> ordreTourCourant,
        int indexJoueurActifTour,
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        if (ordreTourCourant == null || ordreTourCourant.Count == 0)
            return null;

        if (indexJoueurActifTour < 0 || indexJoueurActifTour >= ordreTourCourant.Count)
            return null;

        return GetByCompagnie(
            ordreTourCourant[indexJoueurActifTour],
            joueur1,
            joueur2,
            joueur3
        );
    }

    public SCOBJ_DIRIGEANT GetDirigeantHumain(
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        DATA_JOUEUR humain = GetHumanPlayer(joueur1, joueur2, joueur3);
        return humain != null ? humain.Dirigeant : null;
    }

    public void SynchroniserCompagniesDepuisDirigeants(
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        joueur1?.SynchroniserCompagnieDepuisDirigeant();
        joueur2?.SynchroniserCompagnieDepuisDirigeant();
        joueur3?.SynchroniserCompagnieDepuisDirigeant();
    }

    public void ReinitialiserDirigeants(
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        ReinitialiserDirigeant(joueur1);
        ReinitialiserDirigeant(joueur2);
        ReinitialiserDirigeant(joueur3);
    }

    private void ReinitialiserDirigeant(DATA_JOUEUR joueur)
    {
        if (joueur == null)
            return;

        SCOBJ_DIRIGEANT dirigeant = joueur.Dirigeant;
        if (dirigeant == null)
            return;

        dirigeant.ResetProgression();
    }
}