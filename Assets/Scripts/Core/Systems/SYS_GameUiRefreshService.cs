using System.Collections.Generic;
using UnityEngine;

public class SYS_GameUiRefreshService
{
    private HudController hudControllerCache;
    private UI_EQUIPE_DetailController equipeDetailControllerCache;
    private UI_PROVINCE_MenuController provinceMenuControllerCache;

    private DATA_EQUIPE_DetailData BuildDATA_EQUIPE_DetailData(STATE_EQUIPE equipe)
{
    return new DATA_EQUIPE_DetailData
    {
        source = equipe,
        nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Equipe",
        nomProvince = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
            ? equipe.provinceAffectee.data.nom
            : "Aucune province",
        portraitChef = equipe.data != null ? equipe.data.portraitChef : null,
        niveau = equipe.niveauActuel,

        explorationEnCours = equipe.explorationEnCours,
        vadrouilleEnCours = equipe.vadrouilleEnCours,

        toursRestants = equipe.toursRestants,
        toursTotaux = equipe.toursTotaux,

        statutExploration =
            equipe.vadrouilleEnCours
                ? "Vadrouille en cours"
                : equipe.explorationEnCours
                    ? "Exploration en cours"
                    : (equipe.provinceAffectee == null ? "Non affectée" : "Affectée")
    };
}

    public void RefreshToutLeHUD(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        if (hudControllerCache == null || !hudControllerCache)
        {
            hudControllerCache = gameManager.HudController != null
                ? gameManager.HudController
                : Object.FindAnyObjectByType<HudController>();
        }

        if (hudControllerCache == null || !hudControllerCache)
        {
            Debug.LogWarning("HudController introuvable.");
            return;
        }

        if (gameManager.HudController == null || !gameManager.HudController)
        {
            gameManager.HudController = hudControllerCache;
        }

        gameManager.SynchroniserHudAvecJoueurHumain();

        List<DATA_EQUIPE_DetailData> equipesHud = new();

        DATA_JOUEUR joueurHumain = gameManager.GetHumanPlayer();

        if (joueurHumain != null && joueurHumain.equipes != null)
        {
            foreach (STATE_EQUIPE equipe in joueurHumain.equipes)
            {
                if (equipe == null)
                    continue;

                DATA_EQUIPE_DetailData equipeData = BuildDATA_EQUIPE_DetailData(equipe);
                if (equipeData != null)
                {
                    equipesHud.Add(equipeData);
                }
            }
        }

        gameManager.RefreshDebugEquipesRuntimeView();

        hudControllerCache.RefreshAll(
            gameManager.JoueurData,
            gameManager.PartieData,
            equipesHud
        );

        RefreshProvinceMenu();

        if (equipeDetailControllerCache == null || !equipeDetailControllerCache)
        {
            equipeDetailControllerCache = gameManager.EquipeDetailController != null
                ? gameManager.EquipeDetailController
                : Object.FindAnyObjectByType<UI_EQUIPE_DetailController>(FindObjectsInactive.Include);
        }

        if (gameManager.EquipeDetailController == null || !gameManager.EquipeDetailController)
        {
            gameManager.EquipeDetailController = equipeDetailControllerCache;
        }

        if (equipeDetailControllerCache != null && equipeDetailControllerCache.IsOpen())
        {
            equipeDetailControllerCache.RefreshCurrentEquipe();
        }
    }

    public void RefreshUI_PROVINCE_View(STATE_PROVINCE province)
    {
        if (province == null)
            return;

        UI_PROVINCE_View[] provinceViews = Object.FindObjectsByType<UI_PROVINCE_View>(FindObjectsSortMode.None);

        foreach (UI_PROVINCE_View view in provinceViews)
        {
            if (view != null && view.STATE_PROVINCE == province)
            {
                view.RefreshVisual();
                return;
            }
        }
    }

    public void RefreshProvinceMenu()
    {
        if (provinceMenuControllerCache == null || !provinceMenuControllerCache)
        {
            provinceMenuControllerCache =
                Object.FindAnyObjectByType<UI_PROVINCE_MenuController>(FindObjectsInactive.Include);
        }

        if (provinceMenuControllerCache != null && provinceMenuControllerCache.IsOpen())
        {
            provinceMenuControllerCache.RefreshCurrentProvince();
        }
    }
}