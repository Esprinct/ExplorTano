using System;
using System.Collections.Generic;

[Serializable]
public class DATA_EnchereCompagnie
{
    public ENUM_Compagnie compagnie;
    public int montant;
}

[Serializable]
public class DATA_OffreRecrutement
{
    public SCOBJ_Personnage personnage;
    public int prixMinimum;
    public bool estResolue;
    public List<DATA_EnchereCompagnie> encheres = new();

    public DATA_EnchereCompagnie GetEnchere(ENUM_Compagnie compagnie)
    {
        if (encheres == null)
            return null;

        return encheres.Find(x => x != null && x.compagnie == compagnie);
    }

    public bool AUneEnchere(ENUM_Compagnie compagnie)
    {
        return GetEnchere(compagnie) != null;
    }

    public int GetMontant(ENUM_Compagnie compagnie)
    {
        DATA_EnchereCompagnie enchere = GetEnchere(compagnie);
        return enchere != null ? enchere.montant : 0;
    }

    public void SetEnchere(ENUM_Compagnie compagnie, int montant)
    {
        encheres ??= new List<DATA_EnchereCompagnie>();

        DATA_EnchereCompagnie enchere = GetEnchere(compagnie);
        if (enchere == null)
        {
            encheres.Add(new DATA_EnchereCompagnie
            {
                compagnie = compagnie,
                montant = montant
            });

            return;
        }

        enchere.montant = montant;
    }

    public bool AAuMoinsUneEnchere()
    {
        return encheres != null && encheres.Exists(x => x != null && x.montant > 0);
    }

    public List<ENUM_Compagnie> GetCompagniesAvecEnchere()
    {
        List<ENUM_Compagnie> result = new();

        if (encheres == null)
            return result;

        foreach (DATA_EnchereCompagnie enchere in encheres)
        {
            if (enchere == null || enchere.montant <= 0)
                continue;

            if (!result.Contains(enchere.compagnie))
                result.Add(enchere.compagnie);
        }

        return result;
    }
}

public class DATA_RareteDistribution
{
    public int tour;
    public float r1;
    public float r2;
    public float r3;
    public float r4;
    public float r5;
}