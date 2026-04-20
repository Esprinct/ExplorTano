using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DATA_OBJET_Detail : IDetailData
{
    public string idUnique;
    public string nom;
    public string description;
    public Sprite icone;
    public int valeur;
    public int rareteEtoiles;
    public string categorie;
    public int quantite;

    public SCOBJ_OBJET sourceObjet;
    public List<SCOBJ_EFFET> effets = new();

    public string IdUnique => idUnique;
    public string NomAffiche => nom;
    public string DescriptionAffichee => description;
    public Sprite IconeAffichee => icone;
    public IReadOnlyList<SCOBJ_EFFET> Effets => effets;
}