using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SCOBJ_Personnage", menuName = "Game/Personnage")]
public class SCOBJ_Personnage : ScriptableObject
{
    [Header("Identité")]
    public string idUnique;
    public string nom;
    public string prenom;
    public Sprite sprite;
    [TextArea] public string description;
    public ENUM_PERSONNAGE_Genre genre = ENUM_PERSONNAGE_Genre.Neutre;
[Header("Équipement")]
public List<SCOBJ_OBJET_EQUIPPABLE> objetsEquipes = new();
    [Header("Gameplay")]
    public ENUM_RolePersonnage roleActuel;
    [Range(1, 5)] public int rareteEtoiles = 1;
    public bool estUnique = false;
    public bool estGenere = false;

    [Header("Préférence de compagnie")]
    public bool aPreferenceCompagnie = false;
    public ENUM_Compagnie compagniePreferee = ENUM_Compagnie.Aucune;

    [Header("Progression")]
public STATE_LevelProgression progression = new();
public CFG_LevelProgression progressionConfig;

    [Header("Stats")]
    public int curiosite = 100;
    public int intelligence = 100;
    public int dexterite = 100;
    public int endurance = 100;
    public int pointsCuriositeInvestis;
public int pointsIntelligenceInvestis;
public int pointsDexteriteInvestis;
public int pointsEnduranceInvestis;
public STATE_STATS_Allocation allocation = new();

    [Header("Effets affichés dans l'UI")]
    public List<SCOBJ_PERSONNAGE_EFFET> effets = new();

    [Header("Économie")]
    public int coutRecrutementBase = 100;
    public int coutParTour = 10;
    
}