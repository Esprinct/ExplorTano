using System.Collections.Generic;

public class SYS_PlayerInitializationService
{
    public void InitialiserJoueurs(
        DATA_JOUEUR joueur1,
        DATA_JOUEUR joueur2,
        DATA_JOUEUR joueur3)
    {
        InitialiserJoueur(joueur1, "Joueur 1");
        InitialiserJoueur(joueur2, "Joueur 2");
        InitialiserJoueur(joueur3, "Joueur 3");
    }

    private void InitialiserJoueur(DATA_JOUEUR joueur, string nomParDefaut)
    {
        if (joueur == null)
            return;

        if (string.IsNullOrWhiteSpace(joueur.nomJoueur))
            joueur.nomJoueur = nomParDefaut;

        joueur.SynchroniserCompagnieDepuisDirigeant();

        joueur.equipes ??= new List<STATE_EQUIPE>();
        joueur.personnagesRecrutes ??= new List<SCOBJ_Personnage>();
        joueur.objetsPossedes ??= new List<SCOBJ_OBJET>();
        joueur.consommablesPossedes ??= new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();
        joueur.etriniumBreakdown ??= new EtriniumBreakdownData();
    }
}