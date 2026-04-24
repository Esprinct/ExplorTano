public static class SVC_IA_RecruitmentService
{
    public static void TenterRecrutement(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || gameManager.SYS_RecrutementSystem == null)
            return;

        if (!gameManager.PeutRecruterCeTour(joueur))
            return;

        gameManager.SYS_RecrutementSystem.FaireJouerEnchereIAPourJoueur(gameManager, joueur);
        gameManager.MarquerRecrutementEffectue(joueur);
    }
}