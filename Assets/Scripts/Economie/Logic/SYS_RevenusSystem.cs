using System.Collections.Generic;
using UnityEngine;

public class SYS_RevenusSystem
{
    public void AjouterRevenusDuTour(SYS_GameManager gameManager)
    {
        Dictionary<ENUM_Compagnie, float> revenusBruts = CalculerRevenusBruts(gameManager);

        Dictionary<ENUM_Compagnie, float> revenusNets = new()
        {
            { ENUM_Compagnie.Maizin, revenusBruts[ENUM_Compagnie.Maizin] },
            { ENUM_Compagnie.Kinia, revenusBruts[ENUM_Compagnie.Kinia] },
            { ENUM_Compagnie.Joho, revenusBruts[ENUM_Compagnie.Joho] }
        };

        SoustraireCoutsPersonnages(gameManager, gameManager.Joueur1, revenusNets);
        SoustraireCoutsPersonnages(gameManager, gameManager.Joueur2, revenusNets);
        SoustraireCoutsPersonnages(gameManager, gameManager.Joueur3, revenusNets);

        SoustraireCoutsEquipes(gameManager, gameManager.Joueur1, revenusNets);
        SoustraireCoutsEquipes(gameManager, gameManager.Joueur2, revenusNets);
        SoustraireCoutsEquipes(gameManager, gameManager.Joueur3, revenusNets);

        foreach (KeyValuePair<ENUM_Compagnie, float> kvp in revenusNets)
        {
            DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(kvp.Key);
            if (joueur != null)
            {
                joueur.etrinium += kvp.Value;
            }
        }
    }

    public void RecalculerRevenusSeulement(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        RecalculerRevenuParTourPourJoueur(gameManager, gameManager.Joueur1);
        RecalculerRevenuParTourPourJoueur(gameManager, gameManager.Joueur2);
        RecalculerRevenuParTourPourJoueur(gameManager, gameManager.Joueur3);

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain != null)
        {
            gameManager.JoueurData.etriniumParTour = Mathf.RoundToInt(humain.etriniumParTour);
        }
    }

    private Dictionary<ENUM_Compagnie, float> CalculerRevenusBruts(SYS_GameManager gameManager)
    {
        Dictionary<ENUM_Compagnie, float> revenus = new()
        {
            { ENUM_Compagnie.Maizin, 0f },
            { ENUM_Compagnie.Kinia, 0f },
            { ENUM_Compagnie.Joho, 0f }
        };

        if (gameManager == null)
            return revenus;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float totalInfluence =
                province.influenceMaizin +
                province.influenceKinia +
                province.influenceJoho +
                province.influenceAutre;

            if (totalInfluence <= 0f)
                continue;

            revenus[ENUM_Compagnie.Maizin] += province.data.etrinium * (province.influenceMaizin / totalInfluence);
            revenus[ENUM_Compagnie.Kinia] += province.data.etrinium * (province.influenceKinia / totalInfluence);
            revenus[ENUM_Compagnie.Joho] += province.data.etrinium * (province.influenceJoho / totalInfluence);
        }

        return revenus;
    }

    private void SoustraireCoutsPersonnages(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        Dictionary<ENUM_Compagnie, float> revenusNets)
    {
        if (gameManager == null || joueur == null || joueur.personnagesRecrutes == null)
            return;

        if (!revenusNets.ContainsKey(joueur.compagnie))
            return;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            bool enExploration = EstPersonnageEnExploration(gameManager, joueur, personnage);

            revenusNets[joueur.compagnie] -= enExploration
                ? SVC_PERSONNAGE_CostService.GetCoutExploration(personnage)
                : SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
        }
    }

    private void SoustraireCoutsEquipes(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        Dictionary<ENUM_Compagnie, float> revenusNets)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        if (!revenusNets.ContainsKey(joueur.compagnie))
            return;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            bool aDesMembres =
                equipe.membresActuels != null &&
                equipe.membresActuels.Exists(p => p != null);

            int coutEquipe = aDesMembres
                ? gameManager.CoutFixeEquipeAvecMembresParTour
                : gameManager.CoutFixeEquipeParTour;

            if (equipe.actionEnCours == ENUM_EQUIPE_ACTION.Exploration)
            {
                coutEquipe += gameManager.SurcoutEquipeEnExplorationParTour;
            }

            revenusNets[joueur.compagnie] -= coutEquipe;
        }
    }

    private void RecalculerRevenuParTourPourJoueur(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        float revenus = 0f;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            float totalInfluence =
                province.influenceMaizin +
                province.influenceKinia +
                province.influenceJoho +
                province.influenceAutre;

            if (totalInfluence <= 0f)
                continue;

            float part = 0f;

            switch (joueur.compagnie)
            {
                case ENUM_Compagnie.Maizin:
                    part = province.influenceMaizin / totalInfluence;
                    break;
                case ENUM_Compagnie.Kinia:
                    part = province.influenceKinia / totalInfluence;
                    break;
                case ENUM_Compagnie.Joho:
                    part = province.influenceJoho / totalInfluence;
                    break;
            }

            revenus += province.data.etrinium * part;
        }

        int depenses = 0;

        if (joueur.personnagesRecrutes != null)
        {
            foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
            {
                if (personnage == null)
                    continue;

                bool enExploration = EstPersonnageEnExploration(gameManager, joueur, personnage);
                depenses += enExploration
                    ? SVC_PERSONNAGE_CostService.GetCoutExploration(personnage)
                    : SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);
            }
        }

        if (joueur.equipes != null)
        {
            foreach (STATE_EQUIPE equipe in joueur.equipes)
            {
                if (equipe == null)
                    continue;

                bool aDesMembres =
                    equipe.membresActuels != null &&
                    equipe.membresActuels.Exists(p => p != null);

                int coutEquipe = aDesMembres
                    ? gameManager.CoutFixeEquipeAvecMembresParTour
                    : gameManager.CoutFixeEquipeParTour;

                if (equipe.actionEnCours == ENUM_EQUIPE_ACTION.Exploration)
                {
                    coutEquipe += gameManager.SurcoutEquipeEnExplorationParTour;
                }

                depenses += coutEquipe;
            }
        }

        joueur.etriniumParTour = revenus - depenses;
    }

    private bool EstPersonnageEnExploration(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage)
    {
        if (gameManager == null || joueur == null || personnage == null)
            return false;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || equipe.actionEnCours != ENUM_EQUIPE_ACTION.Exploration || equipe.compagnie != joueur.compagnie)
                continue;

            if (equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
    }
}