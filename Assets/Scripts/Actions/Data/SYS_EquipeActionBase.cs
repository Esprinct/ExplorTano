public abstract class SYS_EquipeActionBase
{
    protected readonly SYS_GameUiRefreshService uiSystem;

    protected SYS_EquipeActionBase(SYS_GameUiRefreshService uiSystem)
    {
        this.uiSystem = uiSystem;
    }

    public abstract void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe);
    public abstract void MettreAJour(SYS_GameManager gameManager);
    protected abstract void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe);
}