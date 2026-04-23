using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class STATE_EQUIPE
{
    public SCOBJ_EQUIPE data;

    // Legacy conservé pour compatibilité immédiate
    public int niveauActuel;
public ENUM_EQUIPE_ACTION actionCourante = ENUM_EQUIPE_ACTION.Aucune;

public bool actionEnCours;
public bool actionTerminee;
public int toursActionRestants;
public int toursActionTotaux;
public bool vadrouilleEnCours;
public bool vadrouilleTerminee;
public int toursVadrouilleRestants;
public int toursVadrouilleTotaux;
public DATA_VADROUILLE_Resultat resultatVadrouille;
    public STATE_PROVINCE provinceAffectee;
    public ENUM_Compagnie compagnie;

    public bool explorationEnCours;
    public bool explorationTerminee;
    public int toursRestants;
    public int toursTotaux;

    public List<SCOBJ_Personnage> membresActuels = new List<SCOBJ_Personnage>();

    public ENUM_EXPLORATION_Resultat resultatExploration;
    public bool affectationAutomatique;
    public bool lancementExplorationAutomatique;

    public List<SCOBJ_OBJET_EQUIPPABLE> objetsEquipes = new();
    public List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = new();

    // Progression équipe
    public STATE_LevelProgression progression;
    public CFG_LevelProgression progressionConfig;

    // V2
    public ENUM_EQUIPE_SPECIALISATION specialisation = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
    public SCOBJ_EQUIPE_SPECIALISATION dataSpecialisation;

    public int NiveauActuel
    {
        get
        {
            if (progression != null)
                return progression.niveau;

            return Mathf.Max(1, niveauActuel);
        }
    }

    public void SynchroniserNiveauLegacyDepuisProgression()
    {
        niveauActuel = NiveauActuel;
    }
}