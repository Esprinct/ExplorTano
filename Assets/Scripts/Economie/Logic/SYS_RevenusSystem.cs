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

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain == null)
            return;

        EtriniumBreakdownData breakdown = CalculerBreakdownEtriniumPourJoueur(gameManager, humain);

        humain.etriniumBreakdown = breakdown;
        humain.etriniumParTour = breakdown.totalNet;

        gameManager.JoueurData.etriniumBreakdown = breakdown;
        gameManager.JoueurData.etriniumParTour = Mathf.RoundToInt(breakdown.totalNet);

        Debug.Log(
            $"Revenus recalculés | humain={humain.compagnie} | " +
            $"netRuntime={humain.etriniumParTour} | netHud={gameManager.JoueurData.etriniumParTour}"
        );
    }

    private void SoustraireCoutsPersonnages(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        Dictionary<ENUM_Compagnie, float> revenus)
    {
        if (gameManager == null || joueur == null || !revenus.ContainsKey(joueur.compagnie))
            return;

        int coutTotal = 0;

        if (joueur.personnagesRecrutes != null)
        {
            foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
            {
                if (personnage == null)
                    continue;

                bool enExploration = EstPersonnageEnExploration(gameManager, joueur, personnage);
                int cout = enExploration
                    ? SVC_PERSONNAGE_CostService.GetCoutExploration(personnage)
                    : SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);

                coutTotal += cout;
            }
        }

        revenus[joueur.compagnie] -= coutTotal;

        Debug.Log(
            $"Compagnie {joueur.compagnie} | " +
            $"cout entretien personnages = {coutTotal} | " +
            $"revenu net provisoire = {revenus[joueur.compagnie]}"
        );
    }

    private void SoustraireCoutsEquipes(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        Dictionary<ENUM_Compagnie, float> revenus)
    {
        if (gameManager == null || joueur == null || !revenus.ContainsKey(joueur.compagnie))
            return;

        if (joueur.equipes == null || joueur.equipes.Count == 0)
            return;

        int coutTotalEquipes = 0;

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

            if (equipe.explorationEnCours)
            {
                coutEquipe += gameManager.SurcoutEquipeEnExplorationParTour;
            }

            coutTotalEquipes += coutEquipe;
        }

        revenus[joueur.compagnie] -= coutTotalEquipes;

        Debug.Log(
            $"Compagnie {joueur.compagnie} | " +
            $"cout entretien équipes = {coutTotalEquipes} | " +
            $"revenu net provisoire = {revenus[joueur.compagnie]}"
        );
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
            if (equipe == null || !equipe.explorationEnCours || equipe.compagnie != joueur.compagnie)
                continue;

            if (equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
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

            float total =
                province.influenceMaizin +
                province.influenceKinia +
                province.influenceJoho +
                province.influenceAutre;

            if (total <= 0f)
                continue;

            float etrinium = province.data.etrinium;

            revenus[ENUM_Compagnie.Maizin] += etrinium * (province.influenceMaizin / total);
            revenus[ENUM_Compagnie.Kinia] += etrinium * (province.influenceKinia / total);
            revenus[ENUM_Compagnie.Joho] += etrinium * (province.influenceJoho / total);
        }

        Debug.Log(
            $"Revenus bruts | " +
            $"Maizin={revenus[ENUM_Compagnie.Maizin]} | " +
            $"Kinia={revenus[ENUM_Compagnie.Kinia]} | " +
            $"Joho={revenus[ENUM_Compagnie.Joho]}"
        );

        return revenus;
    }

    private EtriniumBreakdownData CalculerBreakdownEtriniumPourJoueur(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur)
    {
        EtriniumBreakdownData breakdown = new();

        if (gameManager == null || joueur == null)
            return breakdown;

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

            int revenuProvince = Mathf.RoundToInt(province.data.etrinium * part);

            if (revenuProvince > 0)
            {
                breakdown.revenusProvinces.Add(new EtriniumLineData
                {
                    label = province.data.nom,
                    valeurBase = revenuProvince,
                    valeurFinale = revenuProvince
                });

                breakdown.totalRevenus += revenuProvince;
            }
        }

        int depenseBase = 0;
        int depenseFinale = 0;

        if (joueur.personnagesRecrutes != null)
        {
            foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
            {
                if (personnage == null)
                    continue;

                int coutBase = Mathf.Max(0, personnage.coutParTour);
                bool enExploration = EstPersonnageEnExploration(gameManager, joueur, personnage);
                int coutFinal = enExploration
                    ? SVC_PERSONNAGE_CostService.GetCoutExploration(personnage)
                    : SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);

                depenseBase += coutBase;
                depenseFinale += coutFinal;
            }
        }

        int depenseEquipesFixes = 0;
        int depenseEquipesExploration = 0;

        if (joueur.equipes != null)
        {
            foreach (STATE_EQUIPE equipe in joueur.equipes)
            {
                if (equipe == null)
                    continue;

                bool aDesMembres =
                    equipe.membresActuels != null &&
                    equipe.membresActuels.Exists(p => p != null);

                depenseEquipesFixes += aDesMembres
                    ? gameManager.CoutFixeEquipeAvecMembresParTour
                    : gameManager.CoutFixeEquipeParTour;

                if (equipe.explorationEnCours)
                {
                    depenseEquipesExploration += gameManager.SurcoutEquipeEnExplorationParTour;
                }
            }
        }

        breakdown.depensesPersonnagesBase = depenseBase;
        breakdown.depensesPersonnagesFinales = depenseFinale;
        breakdown.depensesEquipesFixes = depenseEquipesFixes;
        breakdown.depensesEquipesExploration = depenseEquipesExploration;

        breakdown.totalDepenses =
            depenseFinale +
            depenseEquipesFixes +
            depenseEquipesExploration;

        breakdown.totalNet = breakdown.totalRevenus - breakdown.totalDepenses;

        Debug.Log(
            $"Breakdown calculé | revenus={breakdown.totalRevenus} | " +
            $"depenses={breakdown.totalDepenses} | net={breakdown.totalNet}"
        );

        return breakdown;
    }
}