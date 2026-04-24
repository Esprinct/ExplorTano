using UnityEngine;

public abstract class SYS_EquipeActionBase
{
    protected readonly SYS_GameUiRefreshService uiSystem;

    protected SYS_EquipeActionBase(SYS_GameUiRefreshService uiSystem)
    {
        this.uiSystem = uiSystem;
    }

    public abstract ENUM_EQUIPE_ACTION TypeAction { get; }

    public abstract void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe);

    public abstract void MettreAJour(SYS_GameManager gameManager);

    protected abstract void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe);

    protected void InitialiserAction(STATE_EQUIPE equipe, ENUM_EQUIPE_ACTION typeAction, int toursTotaux)
    {
        if (equipe == null)
            return;

        toursTotaux = Mathf.Max(1, toursTotaux);

        equipe.actionEnCours = typeAction;
        equipe.actionToursTotaux = toursTotaux;
        equipe.actionToursRestants = toursTotaux;
        equipe.actionTerminee = false;
    }

    protected void CloturerAction(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return;

        equipe.actionEnCours = ENUM_EQUIPE_ACTION.Aucune;
        equipe.actionToursTotaux = 0;
        equipe.actionToursRestants = 0;
        equipe.actionTerminee = true;
    }

    protected bool PeutTraiter(STATE_EQUIPE equipe)
    {
        return equipe != null
            && equipe.actionEnCours == TypeAction
            && equipe.actionToursRestants > 0;
    }
}