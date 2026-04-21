using System.Collections.Generic;

public static class SVC_EQUIPE_SpecialisationService
{
    public static bool PeutChoisirSpecialisation(
        STATE_EQUIPE equipe,
        SCOBJ_EQUIPE_SPECIALISATION cible)
    {
        if (equipe == null || cible == null)
            return false;

        if (equipe.NiveauActuel < cible.niveauMinimum)
            return false;

        // Déjà cette spécialisation
        if (equipe.specialisation == cible.type)
            return false;

        // Depuis reconnaissance : on ne peut choisir qu'une spécialisation
        // dont le parent est reconnaissance
        if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
        {
            return cible.specialisationParent == ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
        }

        // Sinon il faut que la spécialisation ciblée ait comme parent
        // la spécialisation actuelle de l'équipe
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

    // La spécialisation actuelle est évidemment débloquée
    if (equipe.specialisation == cible.type)
        return true;

    // On remonte uniquement la chaîne des parents de la spécialisation actuelle.
    // Donc seuls les ancêtres sont "déjà débloqués".
    ENUM_EQUIPE_SPECIALISATION courant = equipe.specialisation;

    while (true)
    {
        SCOBJ_EQUIPE_SPECIALISATION assetCourant =
            GetSpecialisationByType(courant, toutesLesSpecialisations);

        if (assetCourant == null)
            return false;

        // Si le parent du courant est la cible, alors la cible est un ancêtre
        if (assetCourant.specialisationParent == cible.type)
            return true;

        // Si on est arrivé à Reconnaissance, on s'arrête
        if (assetCourant.type == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
            break;

        // Sécurité : éviter une boucle infinie si un asset est mal configuré
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