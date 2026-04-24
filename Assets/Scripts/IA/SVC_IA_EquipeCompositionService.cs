using UnityEngine;

public static class SVC_IA_EquipeCompositionService
{
    public static int CompterEquipesExploration(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Exploration ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Archeologues ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Arpenteurs)
            {
                total++;
            }
        }

        return total;
    }

    public static int CompterEquipesMiliciennes(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Miliciens ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.GardienDeLaPaix ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Intervention)
            {
                total++;
            }
        }

        return total;
    }

    public static int CompterEquipesConstruction(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Construction ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Colons ||
                equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.GenieCivil)
            {
                total++;
            }
        }

        return total;
    }

    public static ENUM_EQUIPE_SPECIALISATION GetBrancheLaPlusManquante(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = SVC_IA_PersonnaliteResolver.GetProfil(joueur);
        if (joueur == null || profil == null)
            return ENUM_EQUIPE_SPECIALISATION.Exploration;

        int nbEquipes = joueur.equipes != null ? joueur.equipes.Count : 0;
        nbEquipes = Mathf.Max(1, nbEquipes);

        int nbExploration = CompterEquipesExploration(joueur);
        int nbMiliciens = CompterEquipesMiliciennes(joueur);
        int nbConstruction = CompterEquipesConstruction(joueur);

        float partExploration = (float)nbExploration / nbEquipes;
        float partMiliciens = (float)nbMiliciens / nbEquipes;
        float partConstruction = (float)nbConstruction / nbEquipes;

        float manqueExploration = profil.ratioExploration - partExploration;
        float manqueMiliciens = profil.ratioMiliciens - partMiliciens;
        float manqueConstruction = profil.ratioConstruction - partConstruction;

        if (manqueMiliciens >= manqueExploration && manqueMiliciens >= manqueConstruction)
            return ENUM_EQUIPE_SPECIALISATION.Miliciens;

        if (manqueConstruction >= manqueExploration && manqueConstruction >= manqueMiliciens)
            return ENUM_EQUIPE_SPECIALISATION.Construction;

        return ENUM_EQUIPE_SPECIALISATION.Exploration;
    }

    public static bool IADevraitCreerNouvelleEquipe(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = SVC_IA_PersonnaliteResolver.GetProfil(joueur);
        if (joueur == null || profil == null)
            return false;

        int nbEquipes = joueur.equipes != null ? joueur.equipes.Count : 0;
        return nbEquipes < profil.nombreEquipesCible;
    }

    public static int GetNombreEquipesMaximumSouhaite(DATA_JOUEUR joueur)
    {
        SCOBJ_IA_Personnalite profil = SVC_IA_PersonnaliteResolver.GetProfil(joueur);
        if (profil == null)
            return 5;

        return Mathf.Max(1, profil.nombreEquipesMaximumSouhaite);
    }
}