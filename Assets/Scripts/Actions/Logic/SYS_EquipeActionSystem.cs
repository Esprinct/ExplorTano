using System.Collections.Generic;

public class SYS_EquipeActionSystem
{
    private readonly Dictionary<ENUM_EQUIPE_ACTION, SYS_EquipeActionBase> actions = new();

    public SYS_EquipeActionSystem(
        SYS_ExplorationAction explorationAction,
        SYS_ConstructionAction constructionAction,
        SYS_VadrouilleAction vadrouilleAction)
    {
        actions[ENUM_EQUIPE_ACTION.Exploration] = explorationAction;
        actions[ENUM_EQUIPE_ACTION.Construction] = constructionAction;
        actions[ENUM_EQUIPE_ACTION.Vadrouille] = vadrouilleAction;
    }

    public void DemarrerAction(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        if (equipe.AUneActionEnCours)
            return;

        ENUM_EQUIPE_ACTION action = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);
        if (action == ENUM_EQUIPE_ACTION.Aucune)
            return;

        if (!actions.TryGetValue(action, out SYS_EquipeActionBase systeme))
            return;

        systeme.Demarrer(gameManager, equipe);
    }

    public void MettreAJourActions(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        foreach (SYS_EquipeActionBase systeme in actions.Values)
        {
            systeme.MettreAJour(gameManager);
        }
    }
}