using System.Collections.Generic;
using UnityEngine;

public class GameInitializationService
{
    public void InitialiserPartie(GameManager gameManager, List<EquipeData> equipesData)
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager null !");
            return;
        }

        InitialiserEquipes(gameManager, equipesData);
        InitialiserProvinces(gameManager);
    }

    // 🔥 NOUVEAU : gestion PlayerData
    private void InitialiserEquipes(GameManager gameManager, List<EquipeData> equipesData)
    {
        if (equipesData == null || equipesData.Count == 0)
        {
            Debug.LogWarning("Aucune équipe fournie.");
            return;
        }

        PlayerData joueurHumain = gameManager.GetHumanPlayer();

        foreach (EquipeData equipeData in equipesData)
        {
            if (equipeData == null)
                continue;

            EquipeState equipeState = new EquipeState
            {
                data = equipeData,
                compagnie = joueurHumain != null ? joueurHumain.compagnie : Compagnie.Maizin,
                niveauActuel = equipeData.niveauDeBase,
                provinceAffectee = null,
                explorationEnCours = false,
                toursRestants = 0,
                toursTotaux = 0
            };

            // 🔥 Ajout dans le runtime global
            gameManager.EquipesRuntime.Add(equipeState);

            // 🔥 Ajout dans le joueur humain
            if (joueurHumain != null)
            {
                joueurHumain.equipes.Add(equipeState);
            }
        }

        Debug.Log($"{gameManager.EquipesRuntime.Count} équipes initialisées.");
    }

    private void InitialiserProvinces(GameManager gameManager)
    {
        // Rien à faire ici si tes provinces sont déjà placées en scène
        // Elles seront enregistrées via RegisterProvince()

       // Debug.Log("Provinces prêtes.");
    }
}