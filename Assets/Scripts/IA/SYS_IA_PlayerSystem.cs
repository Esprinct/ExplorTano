public class SYS_IA_PlayerSystem
{
    private readonly SYS_IA_ProgressionEquipementSystem progressionEquipementSystem = new();

    public void JouerTourIA(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.estHumain)
            return;

        progressionEquipementSystem.OptimiserRosterIA(gameManager, joueur);

        SVC_IA_EquipeSpecialisationService.TenterSpecialisationEquipes(gameManager, joueur);
        SVC_IA_RecruitmentService.TenterRecrutement(gameManager, joueur);
        SVC_IA_EquipeRosterService.TenterCreationEquipe(gameManager, joueur);
        SVC_IA_EquipeRosterService.CompleterEquipes(gameManager, joueur);
        SVC_IA_ProvinceStrategyService.AffecterEquipesAuxProvinces(gameManager, joueur);
        SVC_IA_ActionExecutionService.LancerActions(gameManager, joueur);
    }
}