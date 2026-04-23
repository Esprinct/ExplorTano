public class SYS_ConstructionAction : SYS_EquipeActionBase
{
    public SYS_ConstructionAction(SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Construction;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        InitialiserAction(equipe, TypeAction, 1);
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
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        CloturerAction(equipe);
        uiSystem?.RefreshToutLeHUD(gameManager);
    }
}