using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoutiqueCatalogue", menuName = "Game/Boutique/Catalogue")]
public class SCOBJ_BOUTIQUE_Catalogue : ScriptableObject
{
    public List<DATA_BOUTIQUE_Offre> offres = new();
}

[System.Serializable]
public class DATA_BOUTIQUE_Offre
{
    public SCOBJ_OBJET objet;

    [Tooltip("Si <= 0, utilise objet.valeur")]
    public int prixOverride = 0;

    [Min(1)]
    public int quantite = 1;

    public int GetPrix()
    {
        if (prixOverride > 0)
            return prixOverride;

        return objet != null ? Mathf.Max(0, objet.valeur) : 0;
    }
}