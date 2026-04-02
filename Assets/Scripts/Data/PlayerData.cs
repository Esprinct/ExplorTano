using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string nomJoueur;
    public bool estHumain;
    public Compagnie compagnie;

    public float etrinium;
    public float prestige;

    public List<EquipeState> equipes = new();

    public int provincesControlees;
}