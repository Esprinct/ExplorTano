using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PERSONNAGE_Detail_Header : MonoBehaviour
{
    [Header("UI - En-tête / infos")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text prenomText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text coutParTourText;
    [SerializeField] private UI_RareteStarsView UI_RareteStarsView;

    public void Refresh(DATA_PERSONNAGE_Detail data)
    {
        if (data == null)
            return;

        RefreshPortrait(data);
        RefreshIdentity(data);
        RefreshXp(data);
    }

    private void RefreshPortrait(DATA_PERSONNAGE_Detail data)
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = data.sprite;
            portraitImage.enabled = data.sprite != null;
        }
    }

    private void RefreshIdentity(DATA_PERSONNAGE_Detail data)
    {
        nomText?.SetText(FMT_PERSONNAGE_UiFormatter.FormatNom(data.nom));
        prenomText?.SetText(FMT_PERSONNAGE_UiFormatter.FormatPrenom(data.prenom));
        descriptionText?.SetText(data.description);
        coutParTourText?.SetText(data.coutParTour.ToString());

        if (data.progression != null)
            niveauText?.SetText(FMT_PERSONNAGE_UiFormatter.FormatNiveau(data.progression.niveau));
        else
            niveauText?.SetText(FMT_PERSONNAGE_UiFormatter.FormatNiveau(1));

        UI_RareteStarsView?.Refresh(data.rareteEtoiles);
    }

    private void RefreshXp(DATA_PERSONNAGE_Detail data)
    {
        if (xpSlider == null)
            return;

        DATA_LevelProgressionView progression = data.progression;

        if (progression == null)
        {
            xpSlider.minValue = 0f;
            xpSlider.maxValue = 1f;
            xpSlider.value = 0f;
            return;
        }

        if (progression.niveauMaxAtteint)
        {
            xpSlider.minValue = 0f;
            xpSlider.maxValue = 1f;
            xpSlider.value = 1f;
            return;
        }

        xpSlider.minValue = 0f;
        xpSlider.maxValue = Mathf.Max(1, progression.xpRequise);
        xpSlider.value = Mathf.Clamp(progression.xpActuelle, 0, progression.xpRequise);
    }
}