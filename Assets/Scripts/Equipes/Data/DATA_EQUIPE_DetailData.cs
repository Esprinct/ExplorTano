using UnityEngine;

[System.Serializable]
public class DATA_EQUIPE_DetailData
{
    public STATE_EQUIPE source;

    public string nomEquipe;
    public string nomProvince;

    public Sprite portraitChef;

    public int niveau;

    public ENUM_EQUIPE_ACTION actionEnCours;
    public bool aUneActionEnCours;
    public string nomActionEnCours;

    public bool lancementActionAutomatique;

    public int toursRestants;
    public int toursTotaux;

    public string statutAction;
    public int coutParTour;
    public string forceTooltipDetail;
    public string intelligenceTooltipDetail;
    public string dexteriteTooltipDetail;
    public string enduranceTooltipDetail;
}