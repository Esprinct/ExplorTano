using UnityEngine;

public class ExplorationSystem
{
    private readonly SYS_ExplorationAction explorationAction;

    public ExplorationSystem(SYS_InfluenceSystem influenceSystem, SYS_GameUiRefreshService uiSystem)
    {
        explorationAction = new SYS_ExplorationAction(influenceSystem, uiSystem);
    }

    public void DemarrerExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe, ENUM_Compagnie compagnie, int dureeTours)
    {
        if (equipe == null || gameManager == null)
            return;

        equipe.compagnie = compagnie;
        explorationAction.Demarrer(gameManager, equipe);
    }

    public void MettreAJourExplorations(SYS_GameManager gameManager)
    {
        explorationAction.MettreAJour(gameManager);
    }
}