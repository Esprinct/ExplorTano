using System.Collections.Generic;
using UnityEngine;

public class SYS_EquipeManagementService
{
    public STATE_EQUIPE CreerEquipeRuntime(
        SYS_GameManager gameManager,
        SCOBJ_EQUIPE dataEquipe,
        DATA_JOUEUR proprietaire)
    {
        if (dataEquipe == null || proprietaire == null)
            return null;

        SCOBJ_EQUIPE dataRuntime = ScriptableObject.Instantiate(dataEquipe);
        dataRuntime.name = $"{dataEquipe.name}_Runtime";

        List<SCOBJ_Personnage> membres = new();
        if (dataEquipe.membres != null)
        {
            foreach (SCOBJ_Personnage membre in dataEquipe.membres)
            {
                if (membre != null)
                    membres.Add(membre);
            }
        }

        dataRuntime.membres = new List<SCOBJ_Personnage>(membres);

        STATE_EQUIPE equipe = new STATE_EQUIPE
        {
            data = dataRuntime,
            compagnie = proprietaire.compagnie,
            niveauActuel = Mathf.Max(1, dataEquipe.niveauDeBase),
            provinceAffectee = null,

            specialisation = dataEquipe.specialisationInitiale,
            dataSpecialisation = dataEquipe.dataSpecialisationInitiale,

            actionEnCours = ENUM_EQUIPE_ACTION.Aucune,
            actionToursRestants = 0,
            actionToursTotaux = 0,
            actionTerminee = false,

            resultatExploration = null,
            resultatVadrouille = null,

            affectationAutomatique = !proprietaire.estHumain,
            lancementActionAutomatique = !proprietaire.estHumain,

            membresActuels = new List<SCOBJ_Personnage>(membres),
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),

            progression = new STATE_LevelProgression(),
            progressionConfig = gameManager != null ? gameManager.ProgressionConfigEquipe : null
        };

        if (equipe.progression != null)
            equipe.progression.niveau = Mathf.Max(1, dataEquipe.niveauDeBase);

        equipe.SynchroniserNiveauLegacyDepuisProgression();

        if (equipe.niveauActuel <= 0)
            equipe.niveauActuel = Mathf.Max(1, dataEquipe.niveauDeBase);

        return equipe;
    }

   public STATE_EQUIPE CreerEquipePourJoueur(
    SYS_GameManager gameManager,
    DATA_JOUEUR joueur,
    string nomEquipe = null)
{
    if (gameManager == null || joueur == null)
        return null;

    SCOBJ_EQUIPE dataEquipe = ScriptableObject.CreateInstance<SCOBJ_EQUIPE>();
    if (dataEquipe == null)
        return null;

    int index = joueur.equipes != null ? joueur.equipes.Count + 1 : 1;

    dataEquipe.name = $"Equipe_{joueur.compagnie}_{index}";
    dataEquipe.nomEquipe = string.IsNullOrWhiteSpace(nomEquipe)
        ? $"Équipe {joueur.compagnie} {index}"
        : nomEquipe;

    dataEquipe.niveauDeBase = 1;
    dataEquipe.membres = new List<SCOBJ_Personnage>();
    dataEquipe.specialisationInitiale = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;
    dataEquipe.dataSpecialisationInitiale = null;

    STATE_EQUIPE equipe = new STATE_EQUIPE
    {
        data = dataEquipe,
        compagnie = joueur.compagnie,
        niveauActuel = 1,
        provinceAffectee = null,

        specialisation = ENUM_EQUIPE_SPECIALISATION.Reconnaissance,
        dataSpecialisation = null,

        actionEnCours = ENUM_EQUIPE_ACTION.Aucune,
        actionToursRestants = 0,
        actionToursTotaux = 0,
        actionTerminee = false,

        resultatExploration = null,
        resultatVadrouille = null,

        affectationAutomatique = !joueur.estHumain,
        lancementActionAutomatique = !joueur.estHumain,

        membresActuels = new List<SCOBJ_Personnage>(),
        objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
        consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),

        progression = new STATE_LevelProgression(),
        progressionConfig = gameManager.ProgressionConfigEquipe
    };

    if (equipe.progression != null)
        equipe.progression.niveau = 1;

    equipe.SynchroniserNiveauLegacyDepuisProgression();

    if (equipe.niveauActuel <= 0)
        equipe.niveauActuel = 1;

    joueur.equipes ??= new List<STATE_EQUIPE>();
    joueur.equipes.Add(equipe);

    if (gameManager.EquipesRuntime != null)
    {
        gameManager.EquipesRuntime.Add(equipe);
    }

    return equipe;
}

    // Compatibilité avec l'ancien GameManager
    public bool CreerEquipePourJoueur(
        DATA_JOUEUR joueur,
        List<STATE_EQUIPE> equipesRuntime,
        SCOBJ_EQUIPE modeleEquipeVide,
        int coutCreationEquipe,
        int maxEquipesParJoueur,
        SYS_GameRulesService rulesService,
        CFG_LevelProgression progressionConfigEquipe,
        out STATE_EQUIPE nouvelleEquipe)
    {
        nouvelleEquipe = null;

        if (joueur == null)
        {
            Debug.LogWarning("Création équipe impossible : joueur null.");
            return false;
        }

        if (equipesRuntime == null)
        {
            Debug.LogWarning("Création équipe impossible : equipesRuntime null.");
            return false;
        }

        if (rulesService == null)
        {
            Debug.LogWarning("Création équipe impossible : rulesService null.");
            return false;
        }

        if (!rulesService.PeutCreerEquipe(joueur, maxEquipesParJoueur, coutCreationEquipe))
        {
            Debug.LogWarning(
                $"Création équipe impossible | " +
                $"équipes={rulesService.GetNombreEquipesJoueur(joueur)}/{maxEquipesParJoueur} | " +
                $"etrinium={joueur.etrinium} | coût={coutCreationEquipe}"
            );
            return false;
        }

        nouvelleEquipe = ConstruireNouvelleEquipeRuntime(
            joueur,
            modeleEquipeVide,
            rulesService.GetNombreEquipesJoueur(joueur) + 1,
            progressionConfigEquipe
        );

        if (nouvelleEquipe == null)
        {
            Debug.LogWarning("Impossible de construire la nouvelle équipe runtime.");
            return false;
        }

        joueur.etrinium -= coutCreationEquipe;

        equipesRuntime.Add(nouvelleEquipe);

        joueur.equipes ??= new List<STATE_EQUIPE>();
        joueur.equipes.Add(nouvelleEquipe);

        Debug.Log(
            $"Nouvelle équipe créée : {nouvelleEquipe.data.nomEquipe} | " +
            $"coût={coutCreationEquipe} | " +
            $"équipes joueur={rulesService.GetNombreEquipesJoueur(joueur)}/{maxEquipesParJoueur}"
        );

        return true;
    }

    public STATE_EQUIPE ConstruireNouvelleEquipeRuntime(
        DATA_JOUEUR joueur,
        SCOBJ_EQUIPE modeleEquipeVide,
        int indexEquipe,
        CFG_LevelProgression progressionConfigEquipe)
    {
        if (joueur == null)
            return null;

        SCOBJ_EQUIPE source = modeleEquipeVide != null
            ? ScriptableObject.Instantiate(modeleEquipeVide)
            : ScriptableObject.CreateInstance<SCOBJ_EQUIPE>();

        if (source == null)
            return null;

        source.name = $"Equipe_Runtime_{indexEquipe}";
        source.nomEquipe = string.IsNullOrWhiteSpace(source.nomEquipe)
            ? $"Équipe {indexEquipe}"
            : $"{source.nomEquipe} {indexEquipe}";

        source.niveauDeBase = Mathf.Max(1, source.niveauDeBase);
        source.membres ??= new List<SCOBJ_Personnage>();
        source.membres.Clear();

        if (source.specialisationInitiale == 0)
            source.specialisationInitiale = ENUM_EQUIPE_SPECIALISATION.Reconnaissance;

        STATE_EQUIPE equipeRuntime = new STATE_EQUIPE
        {
            data = source,
            compagnie = joueur.compagnie,
            niveauActuel = source.niveauDeBase,
            provinceAffectee = null,

            specialisation = source.specialisationInitiale,
            dataSpecialisation = source.dataSpecialisationInitiale,

            actionEnCours = ENUM_EQUIPE_ACTION.Aucune,
            actionToursRestants = 0,
            actionToursTotaux = 0,
            actionTerminee = false,

            resultatExploration = null,
            resultatVadrouille = null,

            affectationAutomatique = !joueur.estHumain,
            lancementActionAutomatique = !joueur.estHumain,

            membresActuels = new List<SCOBJ_Personnage>(),
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),

            progression = new STATE_LevelProgression(),
            progressionConfig = progressionConfigEquipe
        };

        if (equipeRuntime.progression != null)
            equipeRuntime.progression.niveau = source.niveauDeBase;

        equipeRuntime.SynchroniserNiveauLegacyDepuisProgression();

        if (equipeRuntime.niveauActuel <= 0)
            equipeRuntime.niveauActuel = source.niveauDeBase;

        return equipeRuntime;
    }

    public void InterrompreAction(STATE_EQUIPE equipe, bool nettoyerProvince = false)
    {
        if (equipe == null)
            return;

        equipe.actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
        equipe.actionToursRestants = 0;
        equipe.actionToursTotaux = 0;
        equipe.actionTerminee = false;

        equipe.resultatExploration = null;
        equipe.resultatVadrouille = null;

        if (nettoyerProvince)
            equipe.provinceAffectee = null;
    }

    public bool PeutModifierEquipe(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        return !equipe.AUneActionEnCours;
    }

    public bool AjouterPersonnage(STATE_EQUIPE equipe, SCOBJ_Personnage personnage, int tailleMax = 12)
    {
        if (equipe == null || personnage == null)
            return false;

        if (!PeutModifierEquipe(equipe))
            return false;

        equipe.membresActuels ??= new List<SCOBJ_Personnage>();
        equipe.membresActuels.RemoveAll(p => p == null);

        if (equipe.membresActuels.Contains(personnage))
            return false;

        if (equipe.membresActuels.Count >= tailleMax)
            return false;

        equipe.membresActuels.Add(personnage);
        return true;
    }

    public bool RetirerPersonnage(STATE_EQUIPE equipe, SCOBJ_Personnage personnage)
    {
        if (equipe == null || personnage == null)
            return false;

        if (!PeutModifierEquipe(equipe))
            return false;

        if (equipe.membresActuels == null)
            return false;

        return equipe.membresActuels.Remove(personnage);
    }

    public bool AffecterProvince(STATE_EQUIPE equipe, STATE_PROVINCE province)
    {
        if (equipe == null)
            return false;

        if (!PeutModifierEquipe(equipe))
            return false;

        equipe.provinceAffectee = province;
        equipe.actionTerminee = false;
        return true;
    }

    public int CompterMembresValides(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.membresActuels == null)
            return 0;

        int total = 0;

        foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
        {
            if (personnage != null)
                total++;
        }

        return total;
    }

    public bool ADesMembresValides(STATE_EQUIPE equipe)
    {
        return CompterMembresValides(equipe) > 0;
    }

    public void NettoyerEquipesVides(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            equipe.membresActuels ??= new List<SCOBJ_Personnage>();
            equipe.membresActuels.RemoveAll(p => p == null);

            if (equipe.membresActuels.Count == 0)
            {
                InterrompreAction(equipe, nettoyerProvince: true);
            }
        }
    }

    public void SynchroniserNiveauEquipeDepuisProgression(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return;

        equipe.SynchroniserNiveauLegacyDepuisProgression();
    }
}