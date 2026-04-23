using System.Collections.Generic;
using UnityEngine;

public class SYS_GameInitializationService
{
    public void InitialiserPartie(SYS_GameManager gameManager, List<SCOBJ_EQUIPE> equipesData)
    {
        if (gameManager == null)
        {
            Debug.LogError("SYS_GameManager null !");
            return;
        }

        InitialiserEquipes(gameManager, equipesData);
        InitialiserProvinces(gameManager);
    }

    private void InitialiserEquipes(SYS_GameManager gameManager, List<SCOBJ_EQUIPE> equipesData)
    {
        if (gameManager == null)
            return;

        gameManager.EquipesRuntime.Clear();
        NettoyerJoueurs(gameManager);

        if (equipesData == null || equipesData.Count == 0)
        {
            Debug.LogWarning("Aucune équipe fournie.");
            return;
        }

        List<DATA_JOUEUR> joueurs = RecupererJoueursValides(gameManager);

        if (joueurs.Count == 0)
        {
            Debug.LogWarning("Aucun joueur valide trouvé.");
            return;
        }

        int indexAttribution = 0;

        foreach (SCOBJ_EQUIPE equipeSource in equipesData)
        {
            if (equipeSource == null)
                continue;

            DATA_JOUEUR proprietaire = joueurs[indexAttribution % joueurs.Count];
            indexAttribution++;

            STATE_EQUIPE equipeRuntime = CreerEquipeRuntime(equipeSource, proprietaire);
            if (equipeRuntime == null)
                continue;

            gameManager.EquipesRuntime.Add(equipeRuntime);

            proprietaire.equipes ??= new List<STATE_EQUIPE>();
            proprietaire.equipes.Add(equipeRuntime);

            AjouterMembresAuRosterDuJoueur(proprietaire, equipeRuntime.membresActuels);

            Debug.Log(
                $"Equipe initialisée | " +
                $"équipe={equipeRuntime.data?.nomEquipe} | " +
                $"joueur={proprietaire.nomJoueur} | " +
                $"compagnie={proprietaire.compagnie}"
            );
        }

        Debug.Log($"{gameManager.EquipesRuntime.Count} équipes initialisées.");
    }

    private void NettoyerJoueurs(SYS_GameManager gameManager)
    {
        foreach (DATA_JOUEUR joueur in RecupererJoueursValides(gameManager))
        {
            if (joueur == null)
                continue;

            joueur.equipes ??= new List<STATE_EQUIPE>();
            joueur.equipes.Clear();

            joueur.personnagesRecrutes ??= new List<SCOBJ_Personnage>();
            joueur.personnagesRecrutes.Clear();
        }
    }

    private List<DATA_JOUEUR> RecupererJoueursValides(SYS_GameManager gameManager)
    {
        List<DATA_JOUEUR> joueurs = new();

        if (gameManager == null)
            return joueurs;

        if (gameManager.Joueur1 != null) joueurs.Add(gameManager.Joueur1);
        if (gameManager.Joueur2 != null) joueurs.Add(gameManager.Joueur2);
        if (gameManager.Joueur3 != null) joueurs.Add(gameManager.Joueur3);

        return joueurs;
    }

    private STATE_EQUIPE CreerEquipeRuntime(SCOBJ_EQUIPE equipeSource, DATA_JOUEUR proprietaire)
    {
        if (equipeSource == null || proprietaire == null)
            return null;

        SCOBJ_EQUIPE equipeRuntimeData = ScriptableObject.Instantiate(equipeSource);
        equipeRuntimeData.name = $"{equipeSource.name}_Runtime";

        List<SCOBJ_Personnage> membresRuntime = new();

        if (equipeSource.membres != null)
        {
            foreach (SCOBJ_Personnage membre in equipeSource.membres)
            {
                if (membre != null)
                    membresRuntime.Add(membre);
            }
        }

        equipeRuntimeData.membres = new List<SCOBJ_Personnage>(membresRuntime);

        STATE_EQUIPE equipeRuntime = new STATE_EQUIPE
        {
            data = equipeRuntimeData,
            compagnie = proprietaire.compagnie,
            niveauActuel = Mathf.Max(1, equipeSource.niveauDeBase),
            provinceAffectee = null,

            specialisation = ENUM_EQUIPE_SPECIALISATION.Reconnaissance,
            dataSpecialisation = null,

            actionEnCours = ENUM_EQUIPE_ACTION.Aucune,
            actionToursRestants = 0,
            actionToursTotaux = 0,
            actionTerminee = false,

            resultatExploration = null,
            resultatVadrouille = null,

            affectationAutomatique = !proprietaire.estHumain,
            lancementActionAutomatique = !proprietaire.estHumain,

            membresActuels = new List<SCOBJ_Personnage>(membresRuntime),
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>()
        };

        return equipeRuntime;
    }

    private void AjouterMembresAuRosterDuJoueur(DATA_JOUEUR joueur, List<SCOBJ_Personnage> membres)
    {
        if (joueur == null || membres == null)
            return;

        joueur.personnagesRecrutes ??= new List<SCOBJ_Personnage>();

        foreach (SCOBJ_Personnage membre in membres)
        {
            if (membre == null)
                continue;

            if (!joueur.personnagesRecrutes.Contains(membre))
            {
                joueur.personnagesRecrutes.Add(membre);
            }
        }
    }

    private void InitialiserProvinces(SYS_GameManager gameManager)
    {
        // Les provinces sont déjà placées en scène et s'enregistrent via RegisterProvince().
    }
}