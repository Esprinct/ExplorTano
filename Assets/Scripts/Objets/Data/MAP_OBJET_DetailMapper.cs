using System.Collections.Generic;

public static class MAP_OBJET_DetailMapper
{
    public static DATA_OBJET_Detail ToDetailData(SCOBJ_OBJET data)
    {
        if (data == null)
            return null;

        return new DATA_OBJET_Detail
        {
            idUnique = data.idUnique,
            nom = data.nom,
            description = data.description,
            icone = data.icone,
            valeur = data.valeur,
            rareteEtoiles = data.rareteEtoiles,
            categorie = data.categorie.ToString(),
            quantite = 1,
            sourceObjet = data,
            effets = data.effets != null
                ? new List<SCOBJ_EFFET>(data.effets)
                : new List<SCOBJ_EFFET>()
        };
    }

    public static DATA_OBJET_Detail ToDetailData(DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack)
    {
        if (stack == null || stack.objet == null)
            return null;

        SCOBJ_OBJET_CONSOMMABLE data = stack.objet;

        return new DATA_OBJET_Detail
        {
            idUnique = data.idUnique,
            nom = data.nom,
            description = data.description,
            icone = data.icone,
            valeur = data.valeur,
            rareteEtoiles = data.rareteEtoiles,
            categorie = data.categorie.ToString(),
            quantite = stack.quantite,
            sourceObjet = data,
            effets = data.effets != null
                ? new List<SCOBJ_EFFET>(data.effets)
                : new List<SCOBJ_EFFET>()
        };
    }
}