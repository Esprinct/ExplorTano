using TMPro;
using UnityEngine;

public class UI_EQUIPE_ActionView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject rowActionView;

    [Header("UI Action")]
    [SerializeField] private TMP_Text titreActionText;
    [SerializeField] private TMP_Text toursEnCoursText;
    [SerializeField] private TMP_Text prestigeGagneText;
    [SerializeField] private TMP_Text etriniumParTourText;
    [SerializeField] private TMP_Text dureeText;
    [SerializeField] private TMP_Text infoPrincipaleText;
    [SerializeField] private TMP_Text infoSecondaireText;
    [SerializeField] private TMP_Text impactText;

    public void Refresh(STATE_EQUIPE equipe, SYS_GameManager gameManager)
    {
        DATA_EQUIPE_ActionPreview preview =
            SVC_EQUIPE_ActionPreviewService.BuildPreview(equipe, gameManager);

        if (rowActionView != null)
            rowActionView.SetActive(preview != null && preview.afficher);

        if (preview == null || !preview.afficher)
        {
            Clear();
            return;
        }

        if (titreActionText != null) titreActionText.text = preview.titreAction;
        if (toursEnCoursText != null) toursEnCoursText.text = preview.toursEnCoursText;
        if (prestigeGagneText != null) prestigeGagneText.text = preview.prestigeText;
        if (etriniumParTourText != null) etriniumParTourText.text = preview.etriniumText;
        if (dureeText != null) dureeText.text = preview.dureeText;
        if (infoPrincipaleText != null) infoPrincipaleText.text = preview.chancePrincipaleText;
        if (infoSecondaireText != null) infoSecondaireText.text = preview.chanceSecondaireText;
        if (impactText != null) impactText.text = preview.impactText;
    }

    private void Clear()
    {
        if (titreActionText != null) titreActionText.text = "-";
        if (toursEnCoursText != null) toursEnCoursText.text = "-";
        if (prestigeGagneText != null) prestigeGagneText.text = "-";
        if (etriniumParTourText != null) etriniumParTourText.text = "-";
        if (dureeText != null) dureeText.text = "-";
        if (infoPrincipaleText != null) infoPrincipaleText.text = "-";
        if (infoSecondaireText != null) infoSecondaireText.text = "-";
        if (impactText != null) impactText.text = "-";
    }
}