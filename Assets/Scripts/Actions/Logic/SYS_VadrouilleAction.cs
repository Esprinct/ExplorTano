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

        if (resultat == null || joueur.etrinium < resultat.coutTotal)
            return;

        joueur.etrinium -= resultat.coutTotal;

        equipe.compagnie = joueur.compagnie;
        equipe.actionTerminee = false;
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
                Terminer(gameManager, equipe);
        }
    }

    protected override void Terminer(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null)
            return;

        STATE_PROVINCE province = equipe.provinceAffectee;
        DATA_JOUEUR joueur = gameManager.GetJoueurProprietaireEquipe(equipe);

        float gainOccupation = 0f;
        float reductionAdverse = 0f;
        int prestigeGagne = 0;

        if (province != null && equipe.resultatVadrouille != null)
        {
            gainOccupation = equipe.resultatVadrouille.gainOccupationFinal;
            reductionAdverse = equipe.resultatVadrouille.reductionOccupationAdverseFinal;
            prestigeGagne = equipe.resultatVadrouille.prestigeFinal;

            influenceSystem.ReduireOccupationAdverse(
                province,
                equipe.compagnie,
                reductionAdverse
            );

            influenceSystem.AppliquerOccupation(
                province,
                equipe.compagnie,
                gainOccupation
            );

            influenceSystem.MettreAJourClaimProvince(gameManager, province);
        }

        if (joueur != null)
        {
            joueur.prestige += prestigeGagne;
        }

        AfficherPopupRecompenseAction(
            gameManager,
            equipe,
            province,
            prestigeGagne,
            gainOccupation,
            reductionAdverse
        );

        CloturerAction(equipe);
        equipe.actionTerminee = true;
        equipe.resultatVadrouille = null;

        if (!equipe.affectationAutomatique)
            equipe.provinceAffectee = null;

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        uiSystem?.RefreshToutLeHUD(gameManager);

        if (equipe.lancementActionAutomatique &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null)
        {
            Demarrer(gameManager, equipe);
            return;
        }

        Debug.Log($"[VADROUILLE_ACTION_END] equipe={equipe.data?.nomEquipe}");
    }

    private void AfficherPopupRecompenseAction(
        SYS_GameManager gameManager,
        STATE_EQUIPE equipe,
        STATE_PROVINCE province,
        int gainPrestige,
        float gainOccupation,
        float reductionOccupationAdverse)
    {
        if (gameManager == null || equipe == null)
            return;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return;

        if (humain.equipes == null || !humain.equipes.Contains(equipe))
            return;

        UI_EQUIPE_ACTION_RecompensePopup popup = gameManager.ActionRecompensePopup;

        if (popup == null)
        {
            popup = Object.FindAnyObjectByType<UI_EQUIPE_ACTION_RecompensePopup>(FindObjectsInactive.Include);
            gameManager.ActionRecompensePopup = popup;
        }

        if (popup == null)
            return;

        DATA_EQUIPE_ACTION_RecompensePopup data = new DATA_EQUIPE_ACTION_RecompensePopup
        {
            action = ENUM_EQUIPE_ACTION.Vadrouille,
            titre = "Résultat de la vadrouille",
            nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Équipe",
            nomProvince = province != null && province.data != null ? province.data.nom : "Province inconnue",
            prestigeGagne = gainPrestige,
            xpGagneParPersonnage = 0,
            lignePrincipale = $"+Occupation : {gainOccupation:0.#}%",
            ligneSecondaire = $"-Occupation adverse : {reductionOccupationAdverse:0.#}%",
            objetTrouve = false,
            nomObjet = "",
            descriptionObjet = "",
            iconeObjet = null,
            rareteObjet = 0
        };

        popup.OpenMenu(data, true);
    }
}