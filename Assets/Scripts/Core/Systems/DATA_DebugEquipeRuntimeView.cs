using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DATA_DebugPersonnageRuntimeView
{
    public string nomComplet;
    public string role;
    public int rarete;
    public int niveau;
    public int force;
    public int intelligence;
    public int dexterite;
    public int endurance;
    public int coutParTour;
}

[System.Serializable]
public class DATA_DebugEquipeRuntimeView
{
    [Header("Identité")]
    public string nomEquipe;
    public ENUM_Compagnie compagnie;

    [Header("État")]
    public string province;
    public bool explorationEnCours;
    public bool explorationTerminee;
    public int toursRestants;
    public int toursTotaux;
    public bool affectationAutomatique;
    public bool lancementExplorationAutomatique;

    [Header("Composition")]
    public int nombreMembres;
    public int niveauEquipe;

    [Header("Stats équipe")]
    public int forceTotale;
    public int intelligenceTotale;
    public int dexteriteTotale;
    public int enduranceTotale;

    [Header("Économie")]
    public int coutPersonnagesParTour;
    public int coutFixeEquipeParTourEstime;
    public int coutTotalEquipeParTourEstime;

    [Header("Membres")]
    public List<DATA_DebugPersonnageRuntimeView> membres = new();
}