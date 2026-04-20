using System.Collections.Generic;
using UnityEngine;

public static class UTIL_JOUEUR_EQUIPPEMENT
{
    public static bool EquiperObjetAuPersonnage(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (joueur == null || personnage == null || objet == null)
            return false;

        if (joueur.objetsPossedes == null)
            joueur.objetsPossedes = new List<SCOBJ_OBJET>();

        if (!joueur.objetsPossedes.Contains(objet))
        {
            Debug.LogWarning("Objet non présent dans inventaire");
            return false;
        }

        if (SVC_OBJET_EquipementRequeteService.EstEquipeParUnDesPersonnagesDuJoueur(joueur, objet))
        {
            Debug.LogWarning("Objet déjà équipé");
            return false;
        }

        SCOBJ_OBJET_EQUIPPABLE ancien = UTIL_PERSONNAGE_EQUIPEMENT.Equiper(personnage, objet);

        if (ancien == null && !UTIL_PERSONNAGE_EQUIPEMENT.PeutEquiper(personnage, objet))
            return false;

        Debug.Log(
            $"Équipement | joueur={joueur.nomJoueur} | " +
            $"personnage={personnage.nom} {personnage.prenom} | " +
            $"objet={objet.nom}"
        );

        return true;
    }

    public static bool DesequiperObjetDuPersonnage(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        ENUM_OBJET_EQUIPPABLE type)
    {
        if (joueur == null || personnage == null)
            return false;

        SCOBJ_OBJET_EQUIPPABLE objetEquipe =
            UTIL_PERSONNAGE_EQUIPEMENT.GetObjetEquipe(personnage, type);

        if (objetEquipe == null)
        {
            Debug.LogWarning("Aucun objet équipé dans ce slot");
            return false;
        }

        bool success = UTIL_PERSONNAGE_EQUIPEMENT.Desequiper(personnage, type);
        if (!success)
            return false;

        Debug.Log(
            $"Déséquipement | joueur={joueur.nomJoueur} | " +
            $"personnage={personnage.nom} {personnage.prenom} | " +
            $"objet={objetEquipe.nom}"
        );

        return true;
    }
}