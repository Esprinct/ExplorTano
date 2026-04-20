using System.Collections.Generic;
using UnityEngine;

public static class PERSONNAGE_EFFET_AutoAssigner
{
    public static void AssignerEffetsAffinite(
        SCOBJ_Personnage personnage,
        ENUM_Compagnie compagnieActuelle,
        SCOBJ_PERSONNAGE_EFFET effetAffiniteRespectee,
        SCOBJ_PERSONNAGE_EFFET effetAffiniteNonRespectee)
    {
        if (personnage == null)
            return;

        personnage.effets ??= new List<SCOBJ_PERSONNAGE_EFFET>();

        // Nettoyage des anciens effets d'affinité
        if (effetAffiniteRespectee != null)
            personnage.effets.Remove(effetAffiniteRespectee);

        if (effetAffiniteNonRespectee != null)
            personnage.effets.Remove(effetAffiniteNonRespectee);

        // Pas de préférence = aucun effet à appliquer
        if (!personnage.aPreferenceCompagnie || personnage.compagniePreferee ==  ENUM_Compagnie.Aucune)
            return;

        bool affiniteRespectee = personnage.compagniePreferee == compagnieActuelle;
        SCOBJ_PERSONNAGE_EFFET effetAAjouter = affiniteRespectee
            ? effetAffiniteRespectee
            : effetAffiniteNonRespectee;

#if UNITY_EDITOR
        Debug.Log(
            $"Affinité | Perso: {personnage.nom} {personnage.prenom} | " +
            $"Préférence: {personnage.compagniePreferee} | " +
            $"Compagnie actuelle: {compagnieActuelle} | " +
            $"Respectée: {affiniteRespectee} | " +
            $"Effet ajouté: {(effetAAjouter != null ? effetAAjouter.name : "null")}"
        );
#endif

        if (effetAAjouter != null)
            personnage.effets.Add(effetAAjouter);
    }
}