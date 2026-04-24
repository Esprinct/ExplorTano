using System.Collections.Generic;
using UnityEngine;

public static class SVC_EQUIPE_RecruitmentUiService
{
    public static bool PeutOuvrirInventairePourAjout(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.data == null)
        {
            Debug.LogWarning("Aucune équipe ouverte.");
            return false;
        }

        if (equipe.AUneActionEnCours)
        {
            Debug.LogWarning("Impossible d'ajouter un personnage pendant une action en cours.");
            return false;
        }

        return true;
    }

    public static bool PeutAjouterPersonnageAEquipe(
        STATE_EQUIPE equipe,
        SCOBJ_Personnage personnage,
        SYS_GameManager gameManager,
        int tailleMax = 12)
    {
        if (equipe == null || equipe.data == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return false;
        }

        if (equipe.AUneActionEnCours)
        {
            Debug.LogWarning("Impossible d'ajouter un personnage pendant une action en cours.");
            return false;
        }

        if (personnage == null)
        {
            Debug.LogWarning("Personnage null.");
            return false;
        }

        if (gameManager != null && gameManager.EstPersonnageDansUneEquipe(personnage))
        {
            Debug.LogWarning("Le personnage est déjà dans une autre équipe.");
            return false;
        }

        equipe.membresActuels ??= new List<SCOBJ_Personnage>();
        equipe.membresActuels.RemoveAll(p => p == null);

        if (equipe.membresActuels.Contains(personnage))
        {
            Debug.LogWarning("Le personnage est déjà dans cette équipe.");
            return false;
        }

        if (equipe.membresActuels.Count >= tailleMax)
        {
            Debug.LogWarning("L'équipe est complète.");
            return false;
        }

        return true;
    }

    public static bool AjouterPersonnageAEquipe(
        STATE_EQUIPE equipe,
        SCOBJ_Personnage personnage)
    {
        if (equipe == null || personnage == null)
            return false;

        equipe.membresActuels ??= new List<SCOBJ_Personnage>();
        equipe.membresActuels.RemoveAll(p => p == null);

        if (equipe.membresActuels.Contains(personnage))
            return false;

        equipe.membresActuels.Add(personnage);
        return true;
    }
}