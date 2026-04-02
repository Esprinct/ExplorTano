using System.Collections.Generic;
using UnityEngine;
public class ExplorationSystem
{
    private InfluenceSystem influenceSystem;
    private GameUiSystem uiSystem;

    public ExplorationSystem(InfluenceSystem influenceSystem, GameUiSystem uiSystem)
    {
        this.influenceSystem = influenceSystem;
        this.uiSystem = uiSystem;
    }

    public void DemarrerExploration(GameManager gameManager, EquipeState equipe, Compagnie compagnie, int dureeTours)
    {
        if (equipe == null)
            return;

        equipe.compagnie = compagnie;
        equipe.explorationEnCours = true;
        equipe.toursRestants = dureeTours;
        equipe.toursTotaux = dureeTours;
    }

    public void MettreAJourExplorations(GameManager gameManager)
    {
        foreach (var equipe in gameManager.EquipesRuntime)
        {
            if (!equipe.explorationEnCours)
                continue;

            equipe.toursRestants--;

            if (equipe.toursRestants <= 0)
            {
                TerminerExploration(gameManager, equipe);
            }
        }
    }

    private void TerminerExploration(GameManager gameManager, EquipeState equipe)
    {
        if (equipe == null)
            return;

        ProvinceState province = equipe.provinceAffectee;

        if (province != null)
        {
            influenceSystem.AppliquerInfluence(province, equipe.compagnie, 1f);
            influenceSystem.MettreAJourClaimProvince(gameManager, province);
        }

        equipe.explorationEnCours = false;
        equipe.toursRestants = 0;
        equipe.toursTotaux = 0;
        equipe.provinceAffectee = null;

        // 🔥 NOUVEAU : PlayerData
        PlayerData joueur = gameManager.GetPlayerDataByCompagnie(equipe.compagnie);

        if (joueur != null)
        {
            joueur.etrinium += 10f;
            joueur.prestige += 1f;
        }

        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);

        Debug.Log($"Exploration terminée pour {equipe.data.nomEquipe}");
    }
}