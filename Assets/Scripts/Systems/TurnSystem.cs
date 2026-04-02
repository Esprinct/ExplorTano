using System.Collections.Generic;
using UnityEngine;
public class TurnSystem
{
    private ExplorationSystem explorationSystem;
    private InfluenceSystem influenceSystem;
    private GameUiSystem uiSystem;

    public TurnSystem(ExplorationSystem explorationSystem, InfluenceSystem influenceSystem, GameUiSystem uiSystem)
    {
        this.explorationSystem = explorationSystem;
        this.influenceSystem = influenceSystem;
        this.uiSystem = uiSystem;
    }

    public void TourSuivant(GameManager gameManager)
    {
        if (gameManager.PartieTerminee)
            return;

        gameManager.PartieData.tourActuel++;

        // 1. Revenus
        AjouterRevenusDuTour(gameManager);

        // 2. Explorations
        explorationSystem.MettreAJourExplorations(gameManager);

        // 3. IA influence
        influenceSystem.JouerTourIA(gameManager);

        // 4. Recalcul HUD
        RecalculerRevenusSeulement(gameManager);

        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);

        Debug.Log($"Tour {gameManager.PartieData.tourActuel}");
        
    }

    // 🔥 Revenus avec PlayerData
    private void AjouterRevenusDuTour(GameManager gameManager)
    {
        Dictionary<Compagnie, float> revenus = CalculerRevenus(gameManager);

        foreach (var kvp in revenus)
        {
            PlayerData joueur = gameManager.GetPlayerDataByCompagnie(kvp.Key);

            if (joueur != null)
            {
                joueur.etrinium += kvp.Value;
            }
        }
        
    }

    public void RecalculerRevenusSeulement(GameManager gameManager)
    {
        Dictionary<Compagnie, float> revenus = CalculerRevenus(gameManager);

        PlayerData humain = gameManager.GetHumanPlayer();

        if (humain != null && revenus.ContainsKey(humain.compagnie))
        {
            gameManager.JoueurData.etriniumParTour =
                Mathf.RoundToInt(revenus[humain.compagnie]);
        }

        Debug.Log($"Revenus recalculés");
    }

    private Dictionary<Compagnie, float> CalculerRevenus(GameManager gameManager)
    {
        Dictionary<Compagnie, float> revenus = new()
        {
            { Compagnie.Maizin, 0f },
            { Compagnie.Kinia, 0f },
            { Compagnie.Joho, 0f }
        };

        foreach (ProvinceState province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float etrinium = province.data.etrinium;

            float total =
                province.influenceMaizin +
                province.influenceKinia +
                province.influenceJoho +
                province.influenceAutre;

            if (total <= 0f)
                continue;

            revenus[Compagnie.Maizin] += etrinium * (province.influenceMaizin / total);
            revenus[Compagnie.Kinia] += etrinium * (province.influenceKinia / total);
            revenus[Compagnie.Joho] += etrinium * (province.influenceJoho / total);
        }

        return revenus;
    }
}