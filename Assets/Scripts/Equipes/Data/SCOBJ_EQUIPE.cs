using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SCOBJ_EQUIPE", menuName = "Game/Equipe")]
public class SCOBJ_EQUIPE : ScriptableObject
{
    public string nomEquipe;
    public Sprite portraitChef;
    public int niveauDeBase;
    [Header("Composition")]
    public List<SCOBJ_Personnage> membres = new List<SCOBJ_Personnage>();

    
}