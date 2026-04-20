using UnityEngine;

public static class SVC_OBJET_EquipementRequeteService
{
    public static bool EstEquipeParPersonnage(SCOBJ_Personnage personnage, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (personnage == null || objet == null)
            return false;

        SCOBJ_OBJET_EQUIPPABLE equipe =
            UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnage, objet.typeEquipable);

        if (equipe == null)
            return false;

        if (ReferenceEquals(equipe, objet))
            return true;

        return !string.IsNullOrWhiteSpace(equipe.idUnique)
            && equipe.idUnique == objet.idUnique;
    }

    public static bool EstEquipeParUnDesPersonnagesDuJoueur(DATA_JOUEUR joueur, SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (joueur == null || joueur.personnagesRecrutes == null || objet == null)
            return false;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            if (EstEquipeParPersonnage(personnage, objet))
                return true;
        }

        return false;
    }
}