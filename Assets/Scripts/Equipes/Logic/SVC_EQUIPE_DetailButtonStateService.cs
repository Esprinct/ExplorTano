using UnityEngine;

public static class SVC_EQUIPE_DetailButtonStateService
{
    public static DATA_EQUIPE_DetailButtonState BuildState(
        STATE_EQUIPE equipe,
        bool enAttenteSelectionProvince,
        SYS_GameManager gameManager,
        int coutLancement)
    {
        DATA_EQUIPE_DetailButtonState state = new DATA_EQUIPE_DetailButtonState();

        state.equipeValide = equipe != null && equipe.data != null;
        state.enAttenteSelectionProvince = enAttenteSelectionProvince;

        state.aDesMembres =
            state.equipeValide &&
            equipe.membresActuels != null &&
            equipe.membresActuels.Exists(p => p != null);

        state.actionEnCours =
            state.equipeValide &&
            equipe.AUneActionEnCours;

        state.provinceAffectee =
            state.equipeValide &&
            equipe.provinceAffectee != null &&
            equipe.provinceAffectee.data != null;

        state.actionCourante = SVC_EQUIPE_ActionRulesService.GetActionPrincipale(equipe);
        state.peutFaireAction = state.actionCourante != ENUM_EQUIPE_ACTION.Aucune;
        state.coutLancement = coutLancement;

        DATA_JOUEUR humain = gameManager != null ? gameManager.GetHumanPlayer() : null;
        state.aLesFonds = humain != null && humain.etrinium >= coutLancement;

        state.boutonAffecterInteractable =
            state.equipeValide &&
            state.aDesMembres &&
            !state.enAttenteSelectionProvince &&
            !state.actionEnCours;

        state.boutonDemarrerInteractable =
            state.equipeValide &&
            state.aDesMembres &&
            state.provinceAffectee &&
            !state.enAttenteSelectionProvince &&
            !state.actionEnCours &&
            state.peutFaireAction &&
            state.aLesFonds;

        state.boutonAjouterInteractable =
            state.equipeValide &&
            !state.actionEnCours;

        state.toggleAffectationInteractable = state.equipeValide;
        state.toggleLancementAutoInteractable = state.equipeValide;

        state.texteBoutonAffecter = SVC_EQUIPE_ActionRulesService.GetNomAffectation(equipe);

        if (state.actionEnCours)
        {
            switch (equipe.actionEnCours)
            {
                case ENUM_EQUIPE_ACTION.Vadrouille:
                    state.texteBoutonDemarrer = "Vadrouille en cours";
                    break;
                case ENUM_EQUIPE_ACTION.Construction:
                    state.texteBoutonDemarrer = "Construction en cours";
                    break;
                case ENUM_EQUIPE_ACTION.Exploration:
                    state.texteBoutonDemarrer = "Exploration en cours";
                    break;
                default:
                    state.texteBoutonDemarrer = "Action en cours";
                    break;
            }
        }
        else
        {
            string labelAction = SVC_EQUIPE_ActionRulesService.GetNomActionPrincipale(equipe);
            state.texteBoutonDemarrer = $"{labelAction} ({state.coutLancement})";
        }

        state.afficherTexteVerrouillage =
            state.actionEnCours ||
            state.enAttenteSelectionProvince ||
            (state.aDesMembres && !state.provinceAffectee) ||
            (state.provinceAffectee && !state.aLesFonds);

        if (state.actionEnCours)
        {
            state.texteVerrouillage = "Action en cours : modifications verrouillées";
        }
        else if (state.enAttenteSelectionProvince)
        {
            state.texteVerrouillage = "Sélectionnez une province à affecter";
        }
        else if (state.aDesMembres && !state.provinceAffectee)
        {
            state.texteVerrouillage = "Affectez une province pour lancer l'action";
        }
        else if (state.provinceAffectee && !state.aLesFonds)
        {
            state.texteVerrouillage = $"Fonds insuffisants : {state.coutLancement} Etrinium requis";
        }

        return state;
    }
}