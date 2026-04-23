using UnityEngine;

public class SYS_VadrouilleAction : SYS_EquipeActionBase
{
    private readonly SYS_InfluenceSystem influenceSystem;

    public SYS_VadrouilleAction(
        SYS_InfluenceSystem influenceSystem,
        SYS_GameUiRefreshService uiSystem) : base(uiSystem)
    {
        this.influenceSystem = influenceSystem;
    }

    public override ENUM_EQUIPE_ACTION TypeAction => ENUM_EQUIPE_ACTION.Vadrouille;

    public override void Demarrer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null || gameManager.VadrouilleConfig == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        if (joueur == null)
            return;

        VadrouilleConfig config = gameManager.VadrouilleConfig;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int toursModifies = SVC_EQUIPE_VadrouilleEffects.GetToursVadrouilleFinals(
            equipe,
            joueur,
            config.toursBase
        );

        float gainOccupation = SVC_EQUIPE_VadrouilleEffects.GetGainOccupationFinal(
            equipe,
            joueur,
            config.gainOccupationBase
        );

        float reductionAdverse = SVC_EQUIPE_VadrouilleEffects.GetReductionOccupationAdverseFinal(
            equipe,
            joueur,
            config.reductionOccupationAdverseBase
        );

        DATA_VADROUILLE_Resultat resultat = CALC_VADROUILLE_Resolver.CalculerResultat(
            stats,
            toursModifies,
            config.coutParTourBase,
            config.prestigeBase,
            gainOccupation,
            reductionAdverse
        );

        if (resultat == null)
            return;

        if (joueur.etrinium < resultat.coutTotal)
            return;

        joueur.etrinium -= resultat.coutTotal;

        equipe.compagnie = joueur.compagnie;
        equipe.vadrouilleTerminee = false;
        equipe.resultatVadrouille = resultat;

        InitialiserAction(equipe, TypeAction, resultat.toursFinaux);

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);

        Debug.Log(
            $"[VADROUILLE_ACTION_START] equipe={equipe.data?.nomEquipe} | " +
            $"tours={equipe.actionToursRestants}/{equipe.actionToursTotaux}"
        );
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
            {
                Terminer(gameManager, equipe);
            }
        }
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        STATE_PROVINCE province = equipe.provinceAffectee;
        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);

        if (province != null && equipe.resultatVadrouille != null)
        {
            influenceSystem.ReduireOccupationAdverse(
                province,
                equipe.compagnie,
                equipe.resultatVadrouille.reductionOccupationAdverseFinal
            );

            influenceSystem.AppliquerOccupation(
                province,
                equipe.compagnie,
                equipe.resultatVadrouille.gainOccupationFinal
            );

            influenceSystem.MettreAJourClaimProvince(gameManager, province);
        }

        if (joueur != null && equipe.resultatVadrouille != null)
        {
            joueur.prestige += equipe.resultatVadrouille.prestigeFinal;
        }

        CloturerAction(equipe);

        equipe.vadrouilleTerminee = true;
        equipe.resultatVadrouille = null;

        if (!equipe.affectationAutomatique)
        {
            equipe.provinceAffectee = null;
        }

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);

        Debug.Log($"[VADROUILLE_ACTION_END] equipe={equipe.data?.nomEquipe}");
    }
}