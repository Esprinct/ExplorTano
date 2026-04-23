using System.Collections.Generic;
using UnityEngine;

public class SYS_GameUiRefreshService
{
    private HudController hudControllerCache;
    private UI_EQUIPE_DetailController equipeDetailControllerCache;
    private UI_PROVINCE_MenuController provinceMenuControllerCache;

    private DATA_EQUIPE_DetailData BuildDATA_EQUIPE_DetailData(STATE_EQUIPE equipe)
    {
        bool explorationEnCours = equipe != null && equipe.explorationEnCours;
        bool vadrouilleEnCours = equipe != null && equipe.vadrouilleEnCours;
        bool actionEnCours = explorationEnCours || vadrouilleEnCours;

        string nomActionEnCours = "";
        if (vadrouilleEnCours)
            nomActionEnCours = "Vadrouille";
        else if (explorationEnCours)
            nomActionEnCours = "Exploration";

        string statut = "Non affectée";
        if (vadrouilleEnCours)
            statut = "Vadrouille en cours";
        else if (explorationEnCours)
            statut = "Exploration en cours";
        else if (equipe != null && equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
            statut = "Affectée";

        return new DATA_EQUIPE_DetailData
        {
            source = equipe,
            nomEquipe = equipe != null && equipe.data != null ? equipe.data.nomEquipe : "Equipe",
            nomProvince = equipe != null && equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
                ? equipe.provinceAffectee.data.nom
                : "Aucune province",
            portraitChef = equipe != null && equipe.data != null ? equipe.data.portraitChef : null,
            niveau = equipe != null ? equipe.niveauActuel : 1,

            explorationEnCours = explorationEnCours,
            vadrouilleEnCours = vadrouilleEnCours,
            actionEnCours = actionEnCours,
            nomActionEnCours = nomActionEnCours,

            lancementActionAutomatique = equipe != null && equipe.lancementExplorationAutomatique,

            toursRestants = equipe != null ? equipe.toursRestants : 0,
            toursTotaux = equipe != null ? equipe.toursTotaux : 0,

            statutExploration = statut
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

        UI_PROVINCE_View[] provinceViews = Object.FindObjectsByType<UI_PROVINCE_View>();

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