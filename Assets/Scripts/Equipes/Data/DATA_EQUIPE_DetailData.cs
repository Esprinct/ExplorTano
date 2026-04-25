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
    public bool vadrouilleEnCours;

    public bool actionEnCours;
    public string nomActionEnCours;

    public bool lancementActionAutomatique;

    public int toursRestants;
    public int toursTotaux;

    public string statutExploration;
    public int coutParTour;
    public string curiositeTooltipDetail;
    public string ingeniositeTooltipDetail;
    public string combativiteTooltipDetail;
    public string enduranceTooltipDetail;
}