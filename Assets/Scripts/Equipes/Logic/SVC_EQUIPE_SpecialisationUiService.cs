using TMPro;
using UnityEngine;
using UnityEngine.UI;
public static class SVC_EQUIPE_SpecialisationUiService
{
    public static void RefreshBoutonSpecialisation(
        STATE_EQUIPE equipe,
        UI_EQUIPE_SpecialisationTreeController specialisationTreeController,
        Button boutonSpecialisation,
        TMP_Text boutonSpecialisationText)
    {
        if (boutonSpecialisation == null)
            return;

        if (equipe == null)
        {
            boutonSpecialisation.interactable = false;

            if (boutonSpecialisationText != null)
                boutonSpecialisationText.text = "Spécialisation";

            return;
        }

        bool actionEnCours = equipe.AUneActionEnCours;

        bool aChoixDisponible =
            specialisationTreeController != null &&
            specialisationTreeController.HasAnyAvailableChoice(equipe);

        bool aDejaSpec =
            equipe.specialisation != ENUM_EQUIPE_SPECIALISATION.Reconnaissance;

        boutonSpecialisation.interactable = !actionEnCours;

        if (boutonSpecialisationText == null)
            return;

        if (actionEnCours)
        {
            boutonSpecialisationText.text = "Spécialisation verrouillée";
        }
        else if (aChoixDisponible)
        {
            boutonSpecialisationText.text = "Choisir spécialisation";
        }
        else if (aDejaSpec && equipe.dataSpecialisation != null)
        {
            boutonSpecialisationText.text = $"Voir : {equipe.dataSpecialisation.nomAffiche}";
        }
        else
        {
            boutonSpecialisationText.text = "Voir arbre";
        }
    }

    public static void OuvrirArbreSpecialisation(
        STATE_EQUIPE equipe,
        UI_EQUIPE_SpecialisationTreeController specialisationTreeController)
    {
        if (equipe == null)
            return;

        if (specialisationTreeController == null)
        {
            Debug.LogWarning("specialisationTreeController non assigné.");
            return;
        }

        specialisationTreeController.Open(equipe);
    }
}