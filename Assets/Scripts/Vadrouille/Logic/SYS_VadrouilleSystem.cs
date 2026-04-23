using UnityEngine;

public class SYS_VadrouilleSystem
{
    private readonly SYS_InfluenceSystem influenceSystem;
    private readonly SYS_GameUiRefreshService uiSystem;

    public SYS_VadrouilleSystem(SYS_InfluenceSystem influenceSystem, SYS_GameUiRefreshService uiSystem)
    {
        this.influenceSystem = influenceSystem;
        this.uiSystem = uiSystem;
    }

    public void DemarrerVadrouille(SYS_GameManager gameManager, STATE_EQUIPE equipe, ENUM_Compagnie compagnie)
    {
        if (gameManager == null || equipe == null || gameManager.VadrouilleConfig == null)
            return;

        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);
        if (joueur == null)
            return;

        VadrouilleConfig config = gameManager.VadrouilleConfig;

        equipe.compagnie = compagnie;
        equipe.vadrouilleTerminee = false;

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

        if (joueur.etrinium < resultat.coutTotal)
            return;

        joueur.etrinium -= resultat.coutTotal;

        equipe.vadrouilleEnCours = true;
        equipe.toursVadrouilleRestants = resultat.toursFinaux;
        equipe.toursVadrouilleTotaux = resultat.toursFinaux;
        equipe.resultatVadrouille = resultat;

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);
    }

    public void MettreAJourVadrouilles(SYS_GameManager gameManager)
    {
        if (gameManager == null || gameManager.EquipesRuntime == null)
            return;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || !equipe.vadrouilleEnCours)
                continue;

            equipe.toursVadrouilleRestants--;

            if (equipe.toursVadrouilleRestants <= 0)
            {
                TerminerVadrouille(gameManager, equipe);
            }
        }
    }

    private void TerminerVadrouille(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        STATE_PROVINCE province = equipe.provinceAffectee;
        DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(equipe.compagnie);

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

        equipe.vadrouilleEnCours = false;
        equipe.vadrouilleTerminee = true;
        equipe.toursVadrouilleRestants = 0;
        equipe.toursVadrouilleTotaux = 0;

        if (!equipe.affectationAutomatique)
        {
            equipe.provinceAffectee = null;
        }

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem.RefreshToutLeHUD(gameManager);

        Debug.Log($"Vadrouille terminée pour {equipe.data?.nomEquipe}");
    }
}