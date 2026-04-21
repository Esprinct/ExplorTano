using System.Collections.Generic;

public static class SVC_EQUIPE_SpecialisationService
{
    public static bool PeutChoisirSpecialisation(STATE_EQUIPE equipe, SCOBJ_EQUIPE_SPECIALISATION cible)
    {
        if (equipe == null || cible == null)
            return false;

        if (equipe.niveauActuel < cible.niveauMinimum)
            return false;

        ENUM_EQUIPE_SPECIALISATION specialisationActuelle = equipe.specialisation;

        if (cible.type == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
            return false;

        if (specialisationActuelle == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
        {
            return cible.specialisationParent == ENUM_EQUIPE_SPECIALISATION.Reconnaissance
                && cible.niveauMinimum <= 3;
        }

        return cible.specialisationParent == specialisationActuelle
            && cible.niveauMinimum <= equipe.niveauActuel;
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
}