using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PERSONNAGE_Slot : BaseSlotUI<DATA_PERSONNAGE_Detail>
{
    [Header("Références UI Personnage")]
    [SerializeField] private Image spriteImage;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text prenomText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private UI_RareteStarsView UI_RareteStarsView;

    protected override void AutoBind()
    {
        base.AutoBind();

        if (spriteImage == null)
        {
            spriteImage = GetComponentInChildren<Image>(true);
        }
    }

    protected override void ValidateReferences()
    {
        base.ValidateReferences();

        UTIL_UiReferenceValidator.Require(spriteImage, nameof(spriteImage), this);
        UTIL_UiReferenceValidator.Require(niveauText, nameof(niveauText), this);
        UTIL_UiReferenceValidator.Require(xpSlider, nameof(xpSlider), this);
        UTIL_UiReferenceValidator.Require(nomText, nameof(nomText), this);
        UTIL_UiReferenceValidator.Require(prenomText, nameof(prenomText), this);
        UTIL_UiReferenceValidator.Require(roleText, nameof(roleText), this);
        UTIL_UiReferenceValidator.Require(UI_RareteStarsView, nameof(UI_RareteStarsView), this);
    }

    protected override void RefreshVisuals(DATA_PERSONNAGE_Detail data)
    {
        RefreshPortrait(data);
        RefreshIdentity(data);
        RefreshProgression(data);
    }

    private void RefreshPortrait(DATA_PERSONNAGE_Detail data)
    {
        if (spriteImage != null)
        {
            spriteImage.sprite = data.sprite;
            spriteImage.enabled = data.sprite != null;
        }
    }

    private void RefreshIdentity(DATA_PERSONNAGE_Detail data)
    {
        if (nomText != null)
            nomText.text = FMT_PERSONNAGE_UiFormatter.FormatNom(data.nom);

        if (prenomText != null)
            prenomText.text = FMT_PERSONNAGE_UiFormatter.FormatPrenom(data.prenom);

        if (roleText != null)
            roleText.text = FMT_PERSONNAGE_UiFormatter.FormatRole(data.role);

        if (UI_RareteStarsView != null)
            UI_RareteStarsView.Refresh(data.rareteEtoiles);
    }

    private void RefreshProgression(DATA_PERSONNAGE_Detail data)
    {
        DATA_LevelProgressionView progression = data.progression;

        if (progression == null)
        {
            if (niveauText != null)
                niveauText.text = FMT_PERSONNAGE_UiFormatter.FormatNiveau(1);

            if (xpSlider != null)
            {
                xpSlider.minValue = 0f;
                xpSlider.maxValue = 1f;
                xpSlider.value = 0f;
            }

            return;
        }

        if (niveauText != null)
            niveauText.text = FMT_PERSONNAGE_UiFormatter.FormatNiveau(progression.niveau);

        if (xpSlider == null)
            return;

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