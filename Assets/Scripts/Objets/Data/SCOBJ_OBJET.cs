using System.Collections.Generic;
using UnityEngine;

public abstract class SCOBJ_OBJET : ScriptableObject
{
    [Header("Identité")]
    public string idUnique;
    public string nom;
    [TextArea] public string description;
    public Sprite icone;

    [Header("Classification")]
    public ENUM_OBJET_Categorie categorie;
    [Range(1, 5)] public int rareteEtoiles = 1;

    [Header("Économie")]
    public int valeur = 0;

    [Header("Effets")]
    public List<SCOBJ_EFFET> effets = new();
}