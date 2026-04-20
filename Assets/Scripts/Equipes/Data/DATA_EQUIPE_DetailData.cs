using UnityEngine;

[System.Serializable]
public class DATA_EQUIPE_DetailData
{
    public STATE_EQUIPE source;

    public string nomEquipe;
    public string nomProvince;

    public Sprite portraitChef;

    public int niveau;

    public bool explorationEnCours;
    public int toursRestants;
    public int toursTotaux;

    public string statutExploration;
    public int coutParTour;
    public string forceTooltipDetail;
public string intelligenceTooltipDetail;
public string dexteriteTooltipDetail;
public string enduranceTooltipDetail;
}