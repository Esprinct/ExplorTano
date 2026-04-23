using System.Collections.Generic;

public class STATE_EQUIPE
{
    public SCOBJ_EQUIPE data;
    public int niveauActuel;
    public STATE_PROVINCE provinceAffectee;
    public ENUM_Compagnie compagnie;

    public ENUM_EQUIPE_SPECIALISATION specialisation = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
    public SCOBJ_EQUIPE_SPECIALISATION dataSpecialisation;

    // =========================
    // Nouveau noyau unifié
    // =========================
    public ENUM_EQUIPE_ACTION actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
    public int actionToursRestants = 0;
    public int actionToursTotaux = 0;
    public bool actionTerminee = false;

    public ENUM_EXPLORATION_Resultat resultatExploration;
    public DATA_VADROUILLE_Resultat resultatVadrouille;

    public bool affectationAutomatique;
    public bool lancementActionAutomatique;

    public List<SCOBJ_Personnage> membresActuels = new();
    public List<SCOBJ_OBJET_EQUIPPABLE> objetsEquipes = new();
    public List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = new();

    // =========================
    // Progression équipe legacy
    // =========================
    public STATE_LevelProgression progression;
    public CFG_LevelProgression progressionConfig;

    // =========================
    // Helpers nouveaux
    // =========================
    public bool AUneActionEnCours =>
        actionEnCours != ENUM_EQUIPE_ACTION.Aucune &&
        actionToursRestants > 0;

    public bool EstEnExploration =>
        actionEnCours == ENUM_EQUIPE_ACTION.Exploration &&
        AUneActionEnCours;

    public bool EstEnConstruction =>
        actionEnCours == ENUM_EQUIPE_ACTION.Construction &&
        AUneActionEnCours;

    public bool EstEnVadrouille =>
        actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille &&
        AUneActionEnCours;

    // =========================
    // Compatibilité legacy
    // =========================

    public bool explorationEnCours
    {
        get => actionEnCours == ENUM_EQUIPE_ACTION.Exploration && actionToursRestants > 0;
        set
        {
            if (value)
            {
                actionEnCours = ENUM_EQUIPE_ACTION.Exploration;

                if (actionToursTotaux <= 0)
                    actionToursTotaux = 1;

                if (actionToursRestants <= 0)
                    actionToursRestants = actionToursTotaux;

                actionTerminee = false;
            }
            else if (actionEnCours == ENUM_EQUIPE_ACTION.Exploration)
            {
                actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
                actionToursRestants = 0;
                actionToursTotaux = 0;
            }
        }
    }

    public bool vadrouilleEnCours
    {
        get => actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille && actionToursRestants > 0;
        set
        {
            if (value)
            {
                actionEnCours = ENUM_EQUIPE_ACTION.Vadrouille;

                if (actionToursTotaux <= 0)
                    actionToursTotaux = 1;

                if (actionToursRestants <= 0)
                    actionToursRestants = actionToursTotaux;

                actionTerminee = false;
            }
            else if (actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille)
            {
                actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
                actionToursRestants = 0;
                actionToursTotaux = 0;
            }
        }
    }

    public bool explorationTerminee
    {
        get => actionTerminee && actionEnCours == ENUM_EQUIPE_ACTION.Aucune;
        set
        {
            if (value)
                actionTerminee = true;
        }
    }

    public bool vadrouilleTerminee
    {
        get => actionTerminee && actionEnCours == ENUM_EQUIPE_ACTION.Aucune;
        set
        {
            if (value)
                actionTerminee = true;
        }
    }

    public int toursRestants
    {
        get => actionToursRestants;
        set => actionToursRestants = value;
    }

    public int toursTotaux
    {
        get => actionToursTotaux;
        set => actionToursTotaux = value;
    }

    public int toursVadrouilleRestants
    {
        get => actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille ? actionToursRestants : 0;
        set
        {
            actionEnCours = ENUM_EQUIPE_ACTION.Vadrouille;
            actionToursRestants = value;

            if (actionToursTotaux < value)
                actionToursTotaux = value;

            if (value > 0)
                actionTerminee = false;
        }
    }

    public int toursVadrouilleTotaux
    {
        get => actionEnCours == ENUM_EQUIPE_ACTION.Vadrouille ? actionToursTotaux : 0;
        set
        {
            actionEnCours = ENUM_EQUIPE_ACTION.Vadrouille;
            actionToursTotaux = value;

            if (actionToursRestants <= 0 && value > 0)
                actionToursRestants = value;

            if (value > 0)
                actionTerminee = false;
        }
    }

    public bool lancementExplorationAutomatique
    {
        get => lancementActionAutomatique;
        set => lancementActionAutomatique = value;
    }

    public int NiveauActuel
    {
        get => niveauActuel;
        set => niveauActuel = value;
    }

    public void SynchroniserNiveauLegacyDepuisProgression()
    {
        if (progression != null)
            niveauActuel = progression.niveau;
    }
}