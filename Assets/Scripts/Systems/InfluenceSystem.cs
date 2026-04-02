using System.Collections.Generic;
using UnityEngine;

public class InfluenceSystem
{
    public void JouerTourIA(GameManager gameManager)
    {
        JouerInfluenceIA(gameManager, Compagnie.Kinia, 12.9f);
        JouerInfluenceIA(gameManager, Compagnie.Joho, 6.9f);
    }

    private void JouerInfluenceIA(GameManager gameManager, Compagnie compagnie, float montantInfluence)
    {
        ProvinceState cible = ChoisirProvincePourCompagnie(gameManager, compagnie);

        if (cible == null || cible.data == null)
        {
            return;
        }

        if (!ProvincePeutEncoreRecevoirInfluenceCompagnies(cible))
        {
            return;
        }

        AppliquerInfluence(cible, compagnie, montantInfluence);
        MettreAJourClaimProvince(gameManager, cible);

        Debug.Log($"{compagnie} influence {cible.data.nom} (+{montantInfluence})");
    }

    private ProvinceState ChoisirProvincePourCompagnie(GameManager gameManager, Compagnie compagnie)
    {
        ProvinceState meilleureProvince = null;
        float meilleurScore = float.MinValue;

        foreach (ProvinceState province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
            {
                continue;
            }

            if (province.estClaim && province.proprietaireActuel == compagnie.ToString())
            {
                continue;
            }

            if (!ProvincePeutEncoreRecevoirInfluenceCompagnies(province))
            {
                continue;
            }

            float score = EvaluerProvincePourCompagnie(province, compagnie);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleureProvince = province;
            }
        }

        return meilleureProvince;
    }

    private float EvaluerProvincePourCompagnie(ProvinceState province, Compagnie compagnie)
    {
        if (province == null || province.data == null)
        {
            return float.MinValue;
        }

        float score = 0f;

        score += province.data.etrinium * 3f;
        score += province.data.prestige * 2f;
        score += province.data.poidsPolitique * 2.5f;
        score += province.data.accesibilite * 1.5f;

        if (!province.estClaim)
        {
            score += 5f;
        }

        switch (compagnie)
        {
            case Compagnie.Kinia:
                score += province.data.poidsPolitique * 1.5f;
                score += province.data.populationFrisien * 1.2f;
                score += province.influenceKinia * 1.5f;
                break;

            case Compagnie.Joho:
                score += province.data.prestige * 1.8f;
                score += province.data.populationShiki * 1.2f;
                score += province.influenceJoho * 1.5f;
                break;

            case Compagnie.Maizin:
                score += province.data.etrinium * 1.5f;
                score += province.data.accesibilite * 1.2f;
                score += province.influenceMaizin * 1.5f;
                break;
        }

        score += Random.Range(0f, 2f);

        return score;
    }

    private bool ProvincePeutEncoreRecevoirInfluenceCompagnies(ProvinceState province)
    {
        if (province == null)
        {
            return false;
        }

        return province.influenceAutre > 0f;
    }

    public void AppliquerInfluence(ProvinceState province, Compagnie compagnie, float montant)
    {
        if (province == null || montant <= 0f)
        {
            return;
        }

        float reductionAutre = Mathf.Min(province.influenceAutre, montant);

        if (reductionAutre <= 0f)
        {
            return;
        }

        switch (compagnie)
        {
            case Compagnie.Kinia:
                province.influenceKinia += reductionAutre;
                break;

            case Compagnie.Joho:
                province.influenceJoho += reductionAutre;
                break;

            case Compagnie.Maizin:
                province.influenceMaizin += reductionAutre;
                break;

            default:
                return;
        }

        province.influenceAutre -= reductionAutre;
        province.influenceAutre = Mathf.Max(0f, province.influenceAutre);
    }

    public void MettreAJourClaimProvince(GameManager gameManager, ProvinceState province)
    {
        if (province == null)
        {
            return;
        }

        string ancienProprietaire = province.proprietaireActuel;

        float totalInfluence =
            province.influenceMaizin +
            province.influenceKinia +
            province.influenceJoho +
            province.influenceAutre;

        if (totalInfluence <= 0f)
        {
            province.estClaim = false;
            province.proprietaireActuel = "";
            gameManager.UiSystem.RefreshProvinceView(province);
            gameManager.UiSystem.RefreshProvinceMenu();
            return;
        }

        float ratioMaizin = province.influenceMaizin / totalInfluence;
        float ratioKinia = province.influenceKinia / totalInfluence;
        float ratioJoho = province.influenceJoho / totalInfluence;

        province.estClaim = false;
        province.proprietaireActuel = "";

        if (ratioMaizin > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = Compagnie.Maizin.ToString();
        }
        else if (ratioKinia > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = Compagnie.Kinia.ToString();
        }
        else if (ratioJoho > 0.5f)
        {
            province.estClaim = true;
            province.proprietaireActuel = Compagnie.Joho.ToString();
        }

        if (province.estClaim && province.proprietaireActuel != ancienProprietaire)
        {
            if (System.Enum.TryParse(province.proprietaireActuel, out Compagnie compagnieGagnante))
            {
                PlayerData joueur = gameManager.GetPlayerDataByCompagnie(compagnieGagnante);

                if (joueur != null)
                {
                    joueur.prestige += 1f;
                }

                PlayerData humain = gameManager.GetHumanPlayer();
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
        }

        MettreAJourProvincesControlees(gameManager);

        gameManager.SynchroniserHudAvecJoueurHumain();
        gameManager.UiSystem.RefreshProvinceView(province);
        gameManager.UiSystem.RefreshProvinceMenu();
    }

    private void MettreAJourProvincesControlees(GameManager gameManager)
    {
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur1);
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur2);
        RecalculerProvincesControleesPourJoueur(gameManager, gameManager.Joueur3);
    }

    private void RecalculerProvincesControleesPourJoueur(GameManager gameManager, PlayerData joueur)
    {
        if (joueur == null)
        {
            return;
        }

        int total = 0;

        foreach (ProvinceState province in gameManager.ProvincesRuntime)
        {
            if (province == null || !province.estClaim)
            {
                continue;
            }

            if (province.proprietaireActuel == joueur.compagnie.ToString())
            {
                total++;
            }
        }

        joueur.provincesControlees = total;
    }
}