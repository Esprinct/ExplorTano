using System.Collections.Generic;
using UnityEngine;

public static class SVC_PERSONNAGE_DismissService
{
    public static bool CongedierPersonnage(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage)
    {
        if (gameManager == null || joueur == null || personnage == null)
            return false;

        bool retireDuRoster = false;

        if (joueur.personnagesRecrutes != null)
        {
            retireDuRoster = joueur.personnagesRecrutes.Remove(personnage);
        }

        bool retireDesEquipes = RetirerDesEquipes(gameManager, joueur, personnage);

        if (!retireDuRoster && !retireDesEquipes)
        {
            Debug.LogWarning(
                $"Congédiement impossible : {personnage.nom} {personnage.prenom} " +
                $"n'a été trouvé ni dans le roster ni dans les équipes."
            );
            return false;
        }

        gameManager.RevenusSystem?.RecalculerRevenusSeulement(gameManager);
        gameManager.SynchroniserHudAvecJoueurHumain();
        gameManager.RefreshToutLeHUD();

        Debug.Log(
            $"Personnage congédié | joueur={joueur.nomJoueur} | " +
            $"personnage={personnage.nom} {personnage.prenom}"
        );

        return true;
    }

    private static bool RetirerDesEquipes(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage)
    {
        if (gameManager == null || joueur == null || personnage == null)
            return false;

        bool retire = false;

        if (gameManager.EquipesRuntime == null)
            return false;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || equipe.compagnie != joueur.compagnie || equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Remove(personnage))
            {
                retire = true;
            }
        }

        return retire;
    }

    public static SCOBJ_Personnage ChoisirPersonnageACongedierAutomatiquement(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.personnagesRecrutes == null || joueur.personnagesRecrutes.Count == 0)
            return null;

        SCOBJ_Personnage meilleurChoix = null;
        int coutLePlusEleve = int.MinValue;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            int cout = SVC_PERSONNAGE_CostService.GetCoutNormal(personnage);

            if (cout > coutLePlusEleve)
            {
                coutLePlusEleve = cout;
                meilleurChoix = personnage;
            }
        }

        return meilleurChoix;
    }
}