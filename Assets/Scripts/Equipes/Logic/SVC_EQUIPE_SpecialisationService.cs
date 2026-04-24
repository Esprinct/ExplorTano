using System.Collections.Generic;
using UnityEngine;

public static class SVC_EQUIPE_SpecialisationService
{
    public static bool PeutChoisirSpecialisation(
        STATE_EQUIPE equipe,
        SCOBJ_EQUIPE_SPECIALISATION cible)
    {
        if (equipe == null || cible == null)
            return false;

        if (equipe.AUneActionEnCours)
            return false;

        if (equipe.NiveauActuel < cible.niveauMinimum)
            return false;

        if (equipe.specialisation == cible.type)
            return false;

        if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
            return cible.specialisationParent == ENUM_EQUIPE_SPECIALISATION.Reconnaissance;

        return cible.specialisationParent == equipe.specialisation;
    }

    public static bool AppliquerSpecialisation(
        STATE_EQUIPE equipe,
        SCOBJ_EQUIPE_SPECIALISATION cible)
    {
        if (!PeutChoisirSpecialisation(equipe, cible))
            return false;

        equipe.specialisation = cible.type;
        equipe.dataSpecialisation = cible;
        return true;
    }

    public static List<SCOBJ_EQUIPE_SPECIALISATION> GetChoixDisponibles(
        STATE_EQUIPE equipe,
        List<SCOBJ_EQUIPE_SPECIALISATION> toutesLesSpecialisations)
    {
        List<SCOBJ_EQUIPE_SPECIALISATION> result = new();

        if (equipe == null || toutesLesSpecialisations == null)
            return result;

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in toutesLesSpecialisations)
        {
            if (specialisation == null)
                continue;

            if (PeutChoisirSpecialisation(equipe, specialisation))
                result.Add(specialisation);
        }

        Debug.Log(
            $"[SPEC_CHOIX] equipe={equipe.data?.nomEquipe} | " +
            $"niveau={equipe.NiveauActuel} | " +
            $"specialisationActuelle={equipe.specialisation} | " +
            $"choix={string.Join(", ", result.ConvertAll(c => c != null ? c.type.ToString() : "null"))}"
        );

        return result;
    }

    public static bool AAuMoinsUnChoixDisponible(
        STATE_EQUIPE equipe,
        List<SCOBJ_EQUIPE_SPECIALISATION> toutesLesSpecialisations)
    {
        if (equipe == null || toutesLesSpecialisations == null)
            return false;

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in toutesLesSpecialisations)
        {
            if (specialisation == null)
                continue;

            if (PeutChoisirSpecialisation(equipe, specialisation))
                return true;
        }

        return false;
    }

    public static bool EstSpecialisationDejaDebloquee(
        STATE_EQUIPE equipe,
        SCOBJ_EQUIPE_SPECIALISATION cible,
        List<SCOBJ_EQUIPE_SPECIALISATION> toutesLesSpecialisations)
    {
        if (equipe == null || cible == null || toutesLesSpecialisations == null)
            return false;

        if (equipe.specialisation == cible.type)
            return true;

        ENUM_EQUIPE_SPECIALISATION courant = equipe.specialisation;

        while (true)
        {
            SCOBJ_EQUIPE_SPECIALISATION assetCourant =
                GetSpecialisationByType(courant, toutesLesSpecialisations);

            if (assetCourant == null)
                return false;

            if (assetCourant.specialisationParent == cible.type)
                return true;

            if (assetCourant.type == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
                break;

            if (assetCourant.specialisationParent == assetCourant.type)
                break;

            courant = assetCourant.specialisationParent;
        }

        return false;
    }

    public static SCOBJ_EQUIPE_SPECIALISATION GetSpecialisationByType(
        ENUM_EQUIPE_SPECIALISATION type,
        List<SCOBJ_EQUIPE_SPECIALISATION> toutesLesSpecialisations)
    {
        if (toutesLesSpecialisations == null)
            return null;

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in toutesLesSpecialisations)
        {
            if (specialisation == null)
                continue;

            if (specialisation.type == type)
                return specialisation;
        }

        return null;
    }
}