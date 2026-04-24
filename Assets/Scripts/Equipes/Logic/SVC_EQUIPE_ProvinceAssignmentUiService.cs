using UnityEngine;

public static class SVC_EQUIPE_ProvinceAssignmentUiService
{
    public static DATA_EQUIPE_ProvinceAssignmentResult DemarrerSelectionProvince(STATE_EQUIPE equipe)
    {
        DATA_EQUIPE_ProvinceAssignmentResult result = new DATA_EQUIPE_ProvinceAssignmentResult();

        if (equipe == null || equipe.data == null)
        {
            result.succes = false;
            result.messageErreur = "Aucune équipe actuellement ouverte.";
            result.refreshBoutons = false;
            result.resterFerme = false;
            result.reouvrirPanel = false;
            return result;
        }

        if (equipe.AUneActionEnCours)
        {
            result.succes = false;
            result.messageErreur = "Action en cours : modifications verrouillées";
            result.refreshBoutons = false;
            result.resterFerme = false;
            result.reouvrirPanel = false;
            return result;
        }

        result.succes = true;
        result.resterFerme = true;
        result.refreshBoutons = true;
        result.reouvrirPanel = false;
        return result;
    }

    public static DATA_EQUIPE_ProvinceAssignmentResult AffecterProvince(
        STATE_EQUIPE equipe,
        STATE_PROVINCE province)
    {
        DATA_EQUIPE_ProvinceAssignmentResult result = new DATA_EQUIPE_ProvinceAssignmentResult();

        if (equipe == null || equipe.data == null)
        {
            result.succes = false;
            result.messageErreur = "Aucune équipe actuellement ouverte.";
            result.refreshBoutons = true;
            return result;
        }

        if (province == null || province.data == null)
        {
            result.succes = false;
            result.messageErreur = "Province invalide.";
            result.refreshBoutons = false;
            return result;
        }

        equipe.provinceAffectee = province;
        equipe.actionTerminee = false;

        result.succes = true;
        result.reouvrirPanel = true;
        result.refreshBoutons = true;
        return result;
    }
}