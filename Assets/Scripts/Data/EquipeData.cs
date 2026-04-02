using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EquipeData", menuName = "Game/Equipe")]
public class EquipeData : ScriptableObject
{
    public string nomEquipe;
    public Sprite portraitChef;
    public int niveauDeBase;
    [Header("Composition")]
    public List<PersonnageData> membres = new List<PersonnageData>();
}