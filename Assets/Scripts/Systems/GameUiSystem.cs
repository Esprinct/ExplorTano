using System.Collections.Generic;
using UnityEngine;

public class GameUiSystem
{
    private EquipeHudData BuildEquipeHudData(EquipeState equipeState)
    {
        return new EquipeHudData
        {
            source = equipeState,
            nomEquipe = equipeState.data != null ? equipeState.data.nomEquipe : "Equipe",
            nomProvince = equipeState.provinceAffectee != null && equipeState.provinceAffectee.data != null
                ? equipeState.provinceAffectee.data.nom
                : "Aucune province",
            portraitChef = equipeState.data != null ? equipeState.data.portraitChef : null,
            niveau = equipeState.niveauActuel,
            explorationEnCours = equipeState.explorationEnCours,
            toursRestants = equipeState.toursRestants,
            toursTotaux = equipeState.toursTotaux,
            statutExploration = equipeState.explorationEnCours
                ? "Exploration en cours"
                : (equipeState.provinceAffectee == null ? "Non affectée" : "Affectée")
        };
    }

    public void RefreshToutLeHUD(GameManager gameManager)
    {
        if (gameManager.HudController == null || !gameManager.HudController)
        {
            gameManager.HudController = Object.FindAnyObjectByType<HudController>();
        }

        if (gameManager.HudController == null || !gameManager.HudController)
        {
            Debug.LogWarning("HudController introuvable.");
            return;
        }

        List<EquipeHudData> equipesHud = new();

        foreach (EquipeState equipeState in gameManager.EquipesRuntime)
        {
            if (equipeState != null)
            {
                equipesHud.Add(BuildEquipeHudData(equipeState));
            }
        }

        gameManager.HudController.RefreshAll(gameManager.JoueurData, gameManager.PartieData, equipesHud);

        if (gameManager.EquipeMenuController == null || !gameManager.EquipeMenuController)
        {
            gameManager.EquipeMenuController = Object.FindAnyObjectByType<EquipeMenuController>(FindObjectsInactive.Include);
        }

        if (gameManager.EquipeMenuController != null && gameManager.EquipeMenuController.IsOpen())
        {
            gameManager.EquipeMenuController.RefreshCurrentEquipe();
        }
    }

    public void RefreshProvinceView(ProvinceState province)
    {
        ProvinceView[] provinceViews = Object.FindObjectsByType<ProvinceView>();

        foreach (ProvinceView view in provinceViews)
        {
            if (view != null && view.ProvinceState == province)
            {
                view.RefreshVisual();
                return;
            }
        }
    }

    public void RefreshProvinceMenu()
    {
        ProvinceMenuController provinceMenuController =
            Object.FindAnyObjectByType<ProvinceMenuController>(FindObjectsInactive.Include);

        if (provinceMenuController != null && provinceMenuController.IsOpen())
        {
            provinceMenuController.RefreshCurrentProvince();
        }
    }
}