using UnityEngine;

public class SYS_VadrouilleSystem
{
    private readonly SYS_VadrouilleAction vadrouilleAction;

    public SYS_VadrouilleSystem(SYS_InfluenceSystem influenceSystem, SYS_GameUiRefreshService uiSystem)
    {
        vadrouilleAction = new SYS_VadrouilleAction(influenceSystem, uiSystem);
    }

    public void DemarrerVadrouille(SYS_GameManager gameManager, STATE_EQUIPE equipe, ENUM_Compagnie compagnie)
    {
        if (gameManager == null || equipe == null)
            return;

        equipe.compagnie = compagnie;
        vadrouilleAction.Demarrer(gameManager, equipe);
    }

    public void MettreAJourVadrouilles(SYS_GameManager gameManager)
    {
        vadrouilleAction.MettreAJour(gameManager);
    }
}