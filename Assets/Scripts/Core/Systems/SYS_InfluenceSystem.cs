using UnityEngine;

public class SYS_InfluenceSystem
{
    public void AppliquerInfluence(STATE_PROVINCE province, ENUM_Compagnie compagnie, float montant)
    {
        if (province == null || montant <= 0f)
            return;

        float restant = montant;

        // 1) On absorbe d'abord l'influence neutre
        float reductionAutre = Mathf.Min(province.influenceAutre, restant);
        if (reductionAutre > 0f)
        {
            AjouterInfluence(province, compagnie, reductionAutre);
            province.influenceAutre -= reductionAutre;
            province.influenceAutre = Mathf.Max(0f, province.influenceAutre);
            restant -= reductionAutre;
        }

        if (restant <= 0f)
            return;

        // 2) Puis on prend sur les compagnies adverses
        RetirerInfluenceAuxAdversaires(province, compagnie, restant);
    }

    private void RetirerInfluenceAuxAdversaires(STATE_PROVINCE province, ENUM_Compagnie compagnieActive, float montant)
    {
        if (province == null || montant <= 0f)
            return;

        float restant = montant;

        while (restant > 0.01f)
        {
            ENUM_Compagnie cible = GetCompagnieAdverseDominante(province, compagnieActive);
            if (cible == ENUM_Compagnie.Aucune)
                break;

            float influenceCible = GetInfluence(province, cible);
            if (influenceCible <= 0f)
                break;

            float transfert = Mathf.Min(influenceCible, restant);

            RetirerInfluence(province, cible, transfert);
            AjouterInfluence(province, compagnieActive, transfert);

            restant -= transfert;
        }
    }

    private ENUM_Compagnie GetCompagnieAdverseDominante(STATE_PROVINCE province, ENUM_Compagnie compagnieActive)
    {
        if (province == null)
            return ENUM_Compagnie.Aucune;

        float scoreMaizin = compagnieActive == ENUM_Compagnie.Maizin ? -1f : province.influenceMaizin;
        float scoreKinia = compagnieActive == ENUM_Compagnie.Kinia ? -1f : province.influenceKinia;
        float scoreJoho = compagnieActive == ENUM_Compagnie.Joho ? -1f : province.influenceJoho;

        float meilleur = Mathf.Max(scoreMaizin, Mathf.Max(scoreKinia, scoreJoho));

        if (meilleur <= 0f)
            return ENUM_Compagnie.Aucune;

        if (Mathf.Approximately(meilleur, scoreMaizin))
            return ENUM_Compagnie.Maizin;

        if (Mathf.Approximately(meilleur, scoreKinia))
            return ENUM_Compagnie.Kinia;

        return ENUM_Compagnie.Joho;
    }

    private float GetInfluence(STATE_PROVINCE province, ENUM_Compagnie compagnie)
    {
        if (province == null)
            return 0f;

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                return province.influenceMaizin;
            case ENUM_Compagnie.Kinia:
                return province.influenceKinia;
            case ENUM_Compagnie.Joho:
                return province.influenceJoho;
            default:
                return 0f;
        }
    }

    private void AjouterInfluence(STATE_PROVINCE province, ENUM_Compagnie compagnie, float montant)
    {
        if (province == null || montant <= 0f)
            return;

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                province.influenceMaizin += montant;
                break;
            case ENUM_Compagnie.Kinia:
                province.influenceKinia += montant;
                break;
            case ENUM_Compagnie.Joho:
                province.influenceJoho += montant;
                break;
        }
    }

    private void RetirerInfluence(STATE_PROVINCE province, ENUM_Compagnie compagnie, float montant)
    {
        if (province == null || montant <= 0f)
            return;

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                province.influenceMaizin = Mathf.Max(0f, province.influenceMaizin - montant);
                break;
            case ENUM_Compagnie.Kinia:
                province.influenceKinia = Mathf.Max(0f, province.influenceKinia - montant);
                break;
            case ENUM_Compagnie.Joho:
                province.influenceJoho = Mathf.Max(0f, province.influenceJoho - montant);
                break;
        }
    }

    public void MettreAJourClaimProvince(SYS_GameManager gameManager, STATE_PROVINCE province)
    {
        if (gameManager == null || province == null)
            return;

        ENUM_Compagnie? ancienProprietaire = province.proprietaireActuel;

        float totalInfluence =
            province.influenceMaizin +
            province.influenceKinia +
            province.influenceJoho +
            province.influenceAutre;

        if (totalInfluence <= 0f)
        {
            province.estClaim = false;
            province.proprietaireActuel = null;

            gameManager.UiSystem?.RefreshUI_PROVINCE_View(province);
            gameManager.UiSystem?.RefreshProvinceMenu();
            return;
        }

        float ratioMaizin = province.influenceMaizin / totalInfluence;
        float ratioKinia = province.influenceKinia / totalInfluence;
        float ratioJoho = province.influenceJoho / totalInfluence;

        province.estClaim = false;
        province.proprietaireActuel = null;

        if (ratioMaizin > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = ENUM_Compagnie.Maizin;
        }
        else if (ratioKinia > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = ENUM_Compagnie.Kinia;
        }
        else if (ratioJoho > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = ENUM_Compagnie.Joho;
        }

        if (province.estClaim &&
            province.proprietaireActuel.HasValue &&
            province.proprietaireActuel != ancienProprietaire)
        {
            DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(province.proprietaireActuel.Value);

            if (joueur != null)
            {
                joueur.prestige += 1f;
            }

            DATA_JOUEUR humain = gameManager.GetHumanPlayer();
            if (humain != null)
            {
                gameManager.JoueurData.prestige = Mathf.RoundToInt(humain.prestige);
            }

            Debug.Log(
                $"Prestige claim | " +
                $"{gameManager.Joueur1.nomJoueur}: {gameManager.Joueur1.prestige} | " +
                $"{gameManager.Joueur2.nomJoueur}: {gameManager.Joueur2.prestige} | " +
                $"{gameManager.Joueur3.nomJoueur}: {gameManager.Joueur3.prestige}"
            );
        }

        MettreAJourProvincesControlees(gameManager);

        gameManager.SynchroniserHudAvecJoueurHumain();
        gameManager.UiSystem?.RefreshUI_PROVINCE_View(province);
        gameManager.UiSystem?.RefreshProvinceMenu();
    }

    private void MettreAJourProvincesControlees(SYS_GameManager gameManager)
    {
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur1);
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur2);
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur3);
    }

    private void RecalculerProvincesControleesPourJoueur(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        int total = 0;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || !province.estClaim || !province.proprietaireActuel.HasValue)
                continue;

            if (province.proprietaireActuel.Value == joueur.compagnie)
            {
                total++;
            }
        }

        joueur.provincesControlees = total;
    }
}