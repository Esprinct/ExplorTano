using UnityEngine;

public class SYS_VadrouilleAction : SYS_EquipeActionBase
{
    public SYS_VadrouilleAction(SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Vadrouille;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);

        int toursBase = 1;
        if (gameManager.VadrouilleConfig != null)
            toursBase = gameManager.VadrouilleConfig.toursBase;

        int toursFinaux = SVC_EQUIPE_VadrouilleEffects.GetToursVadrouilleFinals(
            equipe,
            joueur,
            toursBase
        );

        InitialiserAction(equipe, TypeAction, toursFinaux);

        Debug.Log(
            $"[DEMARRAGE_VADROUILLE] equipe={equipe.data?.nomEquipe} | " +
            $"tours={equipe.actionToursRestants}/{equipe.actionToursTotaux}"
        );

        uiSystem?.RefreshToutLeHUD(gameManager);
    }

    public override void MettreAJour(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.EquipesRuntime == null)
            return;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (!PeutTraiter(equipe))
                continue;

            equipe.actionToursRestants--;

            if (equipe.actionToursRestants <= 0)
                Terminer(gameManager, equipe);
        }

        uiSystem?.RefreshToutLeHUD(gameManager);
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        // TODO: appliquer ici les gains / effets de fin de vadrouille
        CloturerAction(equipe);
    }
}