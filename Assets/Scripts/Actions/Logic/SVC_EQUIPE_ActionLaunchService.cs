using TMPro;
using UnityEngine;

public static class SVC_EQUIPE_ActionLaunchService
{
    public static string GetLibelleAction(ENUM_EQUIPE_ACTION action)
    {
        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                return "vadrouille";
            case ENUM_EQUIPE_ACTION.Construction:
                return "construction";
            case ENUM_EQUIPE_ACTION.Exploration:
                return "exploration";
            default:
                return "action";
        }
    }

    public static bool PeutLancerAction(
        STATE_EQUIPE equipe,
        ENUM_EQUIPE_ACTION actionAttendue)
    {
        if (equipe == null || equipe.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return false;
        }

        if (SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe) != actionAttendue)
            return false;

        if (equipe.actionEnCours == actionAttendue && equipe.AUneActionEnCours)
        {
            Debug.LogWarning($"Une {GetLibelleAction(actionAttendue)} est déjà en cours.");
            return false;
        }

        if (equipe.provinceAffectee == null || equipe.provinceAffectee.data == null)
        {
            Debug.LogWarning("Aucune province affectée.");
            return false;
        }

        bool aDesMembres =
            equipe.membresActuels != null &&
            equipe.membresActuels.Exists(p => p != null);

        if (!aDesMembres)
        {
            Debug.LogWarning($"Impossible de démarrer la {GetLibelleAction(actionAttendue)} sans personnages.");
            return false;
        }

        if (actionAttendue == ENUM_EQUIPE_ACTION.Vadrouille)
        {
            float exploration = equipe.provinceAffectee.GetExploration(equipe.compagnie);

            if (exploration < 100f)
            {
                Debug.LogWarning(
                    $"Impossible de démarrer la vadrouille : la province n'est pas explorée à 100%. " +
                    $"Exploration actuelle={exploration:0.#}%"
                );
                return false;
            }
        }

        return true;
    }

    public static void RemplirConfirmation(
        STATE_EQUIPE equipe,
        ENUM_EQUIPE_ACTION action,
        int coutLancement,
        TMP_Text confirmationActionText,
        GameObject panelConfirmationAction)
    {
        if (confirmationActionText == null || panelConfirmationAction == null || equipe == null)
            return;

        switch (action)
        {
            case ENUM_EQUIPE_ACTION.Vadrouille:
                confirmationActionText.text =
                    $"Démarrer la vadrouille dans {equipe.provinceAffectee.data.nom} " +
                    $"avec {equipe.data.nomEquipe} pour {coutLancement} Etrinium ?";
                break;

            case ENUM_EQUIPE_ACTION.Exploration:
                confirmationActionText.text =
                    $"Voulez-vous lancer cette exploration pour {coutLancement} Etrinium ?";
                break;

            case ENUM_EQUIPE_ACTION.Construction:
                confirmationActionText.text =
                    $"Voulez-vous lancer cette construction pour {coutLancement} Etrinium ?";
                break;

            default:
                confirmationActionText.text = "Voulez-vous lancer cette action ?";
                break;
        }

        panelConfirmationAction.SetActive(true);
    }

    public static void DemarrerAction(
        SYS_GameManager gameManager,
        STATE_EQUIPE equipe,
        ENUM_EQUIPE_ACTION action,
        int dureeExplorationParDefaut)
    {
        if (equipe == null)
            return;

        if (gameManager != null)
        {
            switch (action)
            {
                case ENUM_EQUIPE_ACTION.Vadrouille:
                    gameManager.DemarrerVadrouille(equipe);
                    gameManager.RefreshToutLeHUD();
                    return;

                case ENUM_EQUIPE_ACTION.Exploration:
                    gameManager.DemarrerExploration(equipe, dureeExplorationParDefaut);
                    gameManager.RefreshToutLeHUD();
                    return;

                case ENUM_EQUIPE_ACTION.Construction:
                    Debug.LogWarning("Construction non encore branchée.");
                    return;
            }
        }
        else
        {
            switch (action)
            {
                case ENUM_EQUIPE_ACTION.Vadrouille:
                    equipe.actionEnCours = ENUM_EQUIPE_ACTION.Vadrouille;
                    equipe.actionToursTotaux = 1;
                    equipe.actionToursRestants = 1;
                    equipe.actionTerminee = false;
                    return;

                case ENUM_EQUIPE_ACTION.Exploration:
                    equipe.actionEnCours = ENUM_EQUIPE_ACTION.Exploration;
                    equipe.actionToursTotaux = dureeExplorationParDefaut;
                    equipe.actionToursRestants = dureeExplorationParDefaut;
                    equipe.actionTerminee = false;
                    return;

                case ENUM_EQUIPE_ACTION.Construction:
                    Debug.LogWarning("Construction non encore branchée.");
                    return;
            }
        }
    }
}