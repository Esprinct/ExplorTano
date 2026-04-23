using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplorationConfig", menuName = "Game/Exploration Config")]
public class ExplorationConfig : ScriptableObject
{
    [Header("Base Exploration")]
    public int toursBase = 3;
    public int coutParTourBase = 5;
    public int prestigeBase = 1;
    public float chanceArtefactBase = 10f;
    public float chanceArtefactRareBase = 2f;
[Header("Progression province")]
public float gainExplorationBase = 20f;
    [Header("Influence")]
    public float gainInfluence = 10f;
    public float influenceIATourKinia = 10f;
    public float influenceIATourJoho = 10f;

    [Header("XP")]
    public int xpPersonnageParExploration = 25;

    [Header("Loot Artefacts")]
    public List<SCOBJ_OBJET_EQUIPPABLE> artefactsCommuns = new();
    public List<SCOBJ_OBJET_EQUIPPABLE> artefactsRares = new();
}