using System.Collections.Generic;
using UnityEngine;  
public class SYS_TurnSystem
{
    private readonly ExplorationSystem explorationSystem;
    private readonly SYS_VadrouilleSystem vadrouilleSystem;
    private readonly SYS_InfluenceSystem influenceSystem;
    private readonly SYS_GameUiRefreshService uiSystem;
    private readonly SYS_IA_PlayerSystem iaPlayerSystem;

    public SYS_TurnSystem(
        ExplorationSystem explorationSystem,
        SYS_VadrouilleSystem vadrouilleSystem,
        SYS_InfluenceSystem influenceSystem,
        SYS_GameUiRefreshService uiSystem)
    {
        this.explorationSystem = explorationSystem;
        this.vadrouilleSystem = vadrouilleSystem;
        this.influenceSystem = influenceSystem;
        this.uiSystem = uiSystem;

        iaPlayerSystem = new SYS_IA_PlayerSystem();
    }

 public void TourSuivant(SYS_GameManager gameManager)
{
    if (gameManager == null || gameManager.PartieTerminee)
        return;
    gameManager.ExplorationRecompensePopup?.FermerSiDemandeeAuTourSuivant();
    const int securiteMax = 100;
    int securite = 0;

    while (!gameManager.PartieTerminee && securite < securiteMax)
    {
        securite++;

        bool resteDesJoueurs = gameManager.PasserAuJoueurSuivantDansLeTour();

        if (resteDesJoueurs)
        {
            DATA_JOUEUR joueurActif = gameManager.GetJoueurActifTour();

            if (joueurActif == null)
            {
                RafraichirHUD(gameManager);
                return;
            }

            if (joueurActif.estHumain)
            {
                RafraichirHUD(gameManager);

                Debug.Log($"Tour en cours | Joueur actif : {joueurActif.nomJoueur}");
                return;
            }

            iaPlayerSystem.JouerTourIA(gameManager, joueurActif);
            continue;
        }

        ResoudreFinDeTour(gameManager);
        return;
    }

    if (securite >= securiteMax)
    {
        Debug.LogWarning(
            "SYS_TurnSystem : arrêt de sécurité déclenché. " +
            "Boucle de tour trop longue."
        );

        RafraichirHUD(gameManager);
    }
}

   private void ResoudreFinDeTour(SYS_GameManager gameManager)
{
    if (gameManager == null)
        return;

    DATA_RecrutementResolutionResult resolutionRecrutement = null;

    if (gameManager.SYS_RecrutementSystem != null)
    {
        resolutionRecrutement = gameManager.SYS_RecrutementSystem.ResoudreOffres(gameManager);
    }

    gameManager.ResetRecrutementTour();
    gameManager.PartieData.tourActuel++;

    gameManager.RevenusSystem?.AjouterRevenusDuTour(gameManager);
    explorationSystem?.MettreAJourExplorations(gameManager);
    vadrouilleSystem?.MettreAJourVadrouilles(gameManager);
    gameManager.FailliteSystem?.ResoudreFaillites(gameManager);
    gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);

    if (gameManager.VictoireSystem?.VerifierFinDePartie(gameManager) == true)
    {
        AfficherResolutionEtRefresh(gameManager, resolutionRecrutement);
        return;
    }

    if (gameManager.SYS_RecrutementSystem != null)
    {
        gameManager.SYS_RecrutementSystem.GenererMarche(
            5,
            gameManager.PartieData.tourActuel
        );
    }

    gameManager.ConstruireOrdreTourSelonPrestige();
    AfficherResolutionEtRefresh(gameManager, resolutionRecrutement);

    DATA_JOUEUR premierJoueur = gameManager.GetJoueurActifTour();

    Debug.Log(
        $"Début du tour {gameManager.PartieData.tourActuel} | " +
        $"Premier joueur : {premierJoueur?.nomJoueur}"
    );
}

    private void AfficherResolutionEtRefresh(
        SYS_GameManager gameManager,
        DATA_RecrutementResolutionResult resolutionRecrutement)
    {
        if (gameManager == null)
            return;

        if (gameManager.HudController != null && resolutionRecrutement != null)
        {
            gameManager.HudController.ShowNotificationRecrutement(resolutionRecrutement);
               Debug.Log("[RECRUTEMENT RESOLUTION] résultat non null, notification envoyée au HUD");
        }

        RafraichirHUD(gameManager);
    }

    private void RafraichirHUD(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);
    }
}