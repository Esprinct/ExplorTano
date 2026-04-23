using System.Collections.Generic;

public class STATE_EQUIPE
{
    public SCOBJ_EQUIPE data;
    public int niveauActuel;
    public STATE_PROVINCE provinceAffectee;
    public ENUM_Compagnie compagnie;

    public ENUM_EQUIPE_SPECIALISATION specialisation = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
    public SCOBJ_EQUIPE_SPECIALISATION dataSpecialisation;

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

    public STATE_LevelProgression progression;
    public CFG_LevelProgression progressionConfig;

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

    public void ResetAction()
    {
        actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
        actionToursRestants = 0;
        actionToursTotaux = 0;
        actionTerminee = false;
        resultatExploration = null;
        resultatVadrouille = null;
    }
}