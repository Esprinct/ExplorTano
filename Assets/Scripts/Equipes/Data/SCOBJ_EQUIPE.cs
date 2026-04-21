using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCOBJ_EQUIPE", menuName = "Game/Equipe")]
public class SCOBJ_EQUIPE : ScriptableObject
{
    [Header("Identité")]
    public string nomEquipe;
    public Sprite portraitChef;
[Header("Progression")]
public CFG_LevelProgression progressionConfig;
    [Header("Progression initiale")]
    public int niveauDeBase = 1;
    public ENUM_EQUIPE_SPECIALISATION specialisationInitiale = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
    public SCOBJ_EQUIPE_SPECIALISATION dataSpecialisationInitiale;

    [Header("Composition")]
    public List<SCOBJ_Personnage> membres = new();

    [Header("Options IA / confort")]
    public bool affectationAutomatiqueParDefaut = false;
    public bool lancementExplorationAutomatiqueParDefaut = false;
}