using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class SVC_EQUIPE_DetailRefreshService
{
    public static void RefreshVueComplete(
        STATE_EQUIPE equipe,
        SYS_GameManager gameManager,
        UI_PERSONNAGE_Detail_Controller personnageDetailController,
        UI_EQUIPE_SpecialisationTreeController specialisationTreeController,
        UI_EQUIPE_HeaderView headerView,
        UI_EQUIPE_StatsView statsView,
        UI_EQUIPE_PersonnagesView personnagesView,
        UI_EQUIPE_ActionView actionView,
        Toggle toggleAffectationAutomatique,
        Toggle toggleLancementActionAutomatique,
        Button boutonAffecterProvince,
        TMP_Text boutonAffecterProvinceText,
        Button boutonDemarrerAction,
        TMP_Text boutonDemarrerActionText,
        Button boutonAjouterPersonnage,
        Button boutonSpecialisation,
        TMP_Text boutonSpecialisationText,
        TMP_Text modificationsVerrouilleesText,
        Color couleurTexteBoutonActif,
        Color couleurTexteBoutonInactif,
        bool enAttenteSelectionProvince,
        int dureeExplorationParDefaut)
    {
        if (equipe == null || equipe.data == null)
            return;

        headerView?.Refresh(equipe);
        statsView?.Refresh(equipe);
        personnagesView?.Refresh(equipe, personnageDetailController);
        actionView?.Refresh(equipe, gameManager);
        specialisationTreeController?.RefreshTree();

        SVC_EQUIPE_ToggleUiService.RefreshToggles(
            equipe,
            toggleAffectationAutomatique,
            toggleLancementActionAutomatique
        );

        int coutLancement = SVC_EQUIPE_ActionCostService.CalculerCoutActionCourante(
            equipe,
            gameManager,
            dureeExplorationParDefaut
        );

        DATA_EQUIPE_DetailButtonState state =
            SVC_EQUIPE_DetailButtonStateService.BuildState(
                equipe,
                enAttenteSelectionProvince,
                gameManager,
                coutLancement
            );

        AppliquerEtatBoutons(
            state,
            boutonAffecterProvince,
            boutonAffecterProvinceText,
            boutonDemarrerAction,
            boutonDemarrerActionText,
            boutonAjouterPersonnage,
            toggleAffectationAutomatique,
            toggleLancementActionAutomatique,
            modificationsVerrouilleesText,
            couleurTexteBoutonActif,
            couleurTexteBoutonInactif
        );

        SVC_EQUIPE_SpecialisationUiService.RefreshBoutonSpecialisation(
            equipe,
            specialisationTreeController,
            boutonSpecialisation,
            boutonSpecialisationText
        );
    }

    public static void AppliquerEtatBoutons(
        DATA_EQUIPE_DetailButtonState state,
        Button boutonAffecterProvince,
        TMP_Text boutonAffecterProvinceText,
        Button boutonDemarrerAction,
        TMP_Text boutonDemarrerActionText,
        Button boutonAjouterPersonnage,
        Toggle toggleAffectationAutomatique,
        Toggle toggleLancementActionAutomatique,
        TMP_Text modificationsVerrouilleesText,
        Color couleurTexteBoutonActif,
        Color couleurTexteBoutonInactif)
    {
        if (state == null)
            return;

        if (boutonAffecterProvince != null)
            boutonAffecterProvince.interactable = state.boutonAffecterInteractable;

        if (boutonDemarrerAction != null)
            boutonDemarrerAction.interactable = state.boutonDemarrerInteractable;

        if (boutonAffecterProvinceText != null)
        {
            boutonAffecterProvinceText.text = state.texteBoutonAffecter;
            boutonAffecterProvinceText.color = state.boutonAffecterInteractable
                ? couleurTexteBoutonActif
                : couleurTexteBoutonInactif;
        }

        if (boutonDemarrerActionText != null)
        {
            boutonDemarrerActionText.text = state.texteBoutonDemarrer;
            boutonDemarrerActionText.color = state.boutonDemarrerInteractable
                ? couleurTexteBoutonActif
                : couleurTexteBoutonInactif;
        }

        if (boutonAjouterPersonnage != null)
            boutonAjouterPersonnage.interactable = state.boutonAjouterInteractable;

        if (toggleAffectationAutomatique != null)
            toggleAffectationAutomatique.interactable = state.toggleAffectationInteractable;

        if (toggleLancementActionAutomatique != null)
            toggleLancementActionAutomatique.interactable = state.toggleLancementAutoInteractable;

        if (modificationsVerrouilleesText != null)
        {
            modificationsVerrouilleesText.gameObject.SetActive(state.afficherTexteVerrouillage);
            modificationsVerrouilleesText.text = state.texteVerrouillage;
        }
    }
}