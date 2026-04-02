using UnityEngine;

public static class PersonnageHudMapper
{
    public static PersonnageHudData ToHudData(PersonnageData data)
    {
        if (data == null)
        {
            return null;
        }

        return new PersonnageHudData
        {
            nom = data.nom,
            prenom = data.prenom,
            sprite = data.sprite,
            niveau = data.niveau,
            xpActuel = data.xpActuel,
            xpNiveauSuivant = data.xpNiveauSuivant,
            role = data.roleActuel.ToString(),
            description = data.description,
            rareteEtoiles = data.rareteEtoiles
        };
    }
}