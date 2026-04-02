using UnityEngine;

[System.Serializable]
public class EquipeHudData
{
    public EquipeState source;

    public string nomEquipe;
    public string nomProvince;

    public Sprite portraitChef;

    public int niveau;

    public bool explorationEnCours;
    public int toursRestants;
    public int toursTotaux;

    public string statutExploration;
}