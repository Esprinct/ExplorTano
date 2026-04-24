using UnityEngine;

[System.Serializable]
public class DATA_EQUIPE_ACTION_RecompensePopup
{
    public ENUM_EQUIPE_ACTION action;
    public string titre;

    public string nomEquipe;
    public string nomProvince;

    public int prestigeGagne;
    public int xpGagneParPersonnage;

    public string lignePrincipale;
    public string ligneSecondaire;

    public bool objetTrouve;
    public string nomObjet;
    public string descriptionObjet;
    public Sprite iconeObjet;
    public int rareteObjet;

    public bool AUnObjet()
    {
        return objetTrouve;
    }
}