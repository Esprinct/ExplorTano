using System.Collections.Generic;
using UnityEngine;

public class SYS_EquipeManagementService
{
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

        STATE_EQUIPE equipeRuntime = new STATE_EQUIPE
        {
            data = source,
            compagnie = joueur.compagnie,
            niveauActuel = source.niveauDeBase,
            provinceAffectee = null,
            explorationEnCours = false,
            explorationTerminee = false,
            toursRestants = 0,
            toursTotaux = 0,
            membresActuels = new List<SCOBJ_Personnage>(),
            affectationAutomatique = false,
            lancementExplorationAutomatique = false,
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>(),

            // progression
            progression = new STATE_LevelProgression(),
            progressionConfig = progressionConfigEquipe,

            // spécialisation initiale
            specialisation = source.specialisationInitiale,
            dataSpecialisation = source.dataSpecialisationInitiale
        };

        if (equipeRuntime.progression != null)
        {
            equipeRuntime.progression.niveau = source.niveauDeBase;
            equipeRuntime.progression.xpActuelle = 0;
            equipeRuntime.progression.pointsDisponibles = 0;
        }

        equipeRuntime.niveauActuel = source.niveauDeBase;

        Debug.Log(
            $"[CREATE_EQUIPE_SERVICE] runtime={equipeRuntime.data?.nomEquipe} | " +
            $"progressionNull={equipeRuntime.progression == null} | " +
            $"progressionConfigNull={equipeRuntime.progressionConfig == null} | " +
            $"specialisation={equipeRuntime.specialisation}"
        );

        return equipeRuntime;
    }
}