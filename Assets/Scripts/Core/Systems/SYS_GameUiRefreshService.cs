using System.Collections.Generic;
using UnityEngine;

public class SYS_GameUiRefreshService
{
    private HudController hudControllerCache;
    private UI_EQUIPE_DetailController equipeDetailControllerCache;
    private UI_PROVINCE_MenuController UI_PROVINCE_MenuControllerCache;

    private DATA_EQUIPE_DetailData BuildDATA_EQUIPE_DetailData(STATE_EQUIPE STATE_EQUIPE)
    {
        return new DATA_EQUIPE_DetailData
        {
            source = STATE_EQUIPE,
            nomEquipe = STATE_EQUIPE.data != null ? STATE_EQUIPE.data.nomEquipe : "Equipe",
            nomProvince = STATE_EQUIPE.provinceAffectee != null && STATE_EQUIPE.provinceAffectee.data != null
                ? STATE_EQUIPE.provinceAffectee.data.nom
                : "Aucune province",
            portraitChef = STATE_EQUIPE.data != null ? STATE_EQUIPE.data.portraitChef : null,
            niveau = STATE_EQUIPE.niveauActuel,
            explorationEnCours = STATE_EQUIPE.explorationEnCours,
            toursRestants = STATE_EQUIPE.toursRestants,
            toursTotaux = STATE_EQUIPE.toursTotaux,
            statutExploration = STATE_EQUIPE.explorationEnCours
                ? "Exploration en cours"
                : (STATE_EQUIPE.provinceAffectee == null ? "Non affectée" : "Affectée")
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

    // Important : on resynchronise toujours les données HUD depuis le runtime
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

        UI_PROVINCE_View[] UI_PROVINCE_Views = Object.FindObjectsByType<UI_PROVINCE_View>();

        foreach (UI_PROVINCE_View view in UI_PROVINCE_Views)
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
        if (UI_PROVINCE_MenuControllerCache == null || !UI_PROVINCE_MenuControllerCache)
        {
            UI_PROVINCE_MenuControllerCache =
                Object.FindAnyObjectByType<UI_PROVINCE_MenuController>(FindObjectsInactive.Include);
        }

        if (UI_PROVINCE_MenuControllerCache != null && UI_PROVINCE_MenuControllerCache.IsOpen())
        {
            UI_PROVINCE_MenuControllerCache.RefreshCurrentProvince();
        }
    }
}