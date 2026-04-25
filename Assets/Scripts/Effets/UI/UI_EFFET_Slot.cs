using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EFFET_Slot : MonoBehaviour
{
    [SerializeField] private TMP_Text titreText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valeurText;
    [SerializeField] private Image iconeImage;

    private void Awake()
    {
        ValidateReferences();
    }

    private void ValidateReferences()
    {
        UTIL_UiReferenceValidator.Require(titreText, nameof(titreText), this);
        UTIL_UiReferenceValidator.Require(descriptionText, nameof(descriptionText), this);
        UTIL_UiReferenceValidator.Require(valeurText, nameof(valeurText), this);
        UTIL_UiReferenceValidator.Require(iconeImage, nameof(iconeImage), this);
    }

    public void Setup(SCOBJ_EFFET effet, ENUM_PERSONNAGE_Genre? genre = null)
    {
        if (effet == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (titreText != null)
            titreText.text = GetTitreAffiche(effet, genre);

        if (descriptionText != null)
            descriptionText.text = GetDescriptionAffichee(effet, genre);

        if (valeurText != null)
         valeurText.text = FMT_EFFET.BuildValeurAfficheeLongueRich(effet);

        if (iconeImage != null)
        {
            iconeImage.sprite = effet.icone;
            iconeImage.enabled = effet.icone != null;
        }
    }

    private string GetTitreAffiche(SCOBJ_EFFET effet, ENUM_PERSONNAGE_Genre? genre)
    {
        if (effet is SCOBJ_PERSONNAGE_EFFET personnageEffet && genre.HasValue)
            return personnageEffet.GetTitre(genre.Value);

        if (!string.IsNullOrWhiteSpace(effet.GetTitreAffiche()))
            return effet.GetTitreAffiche();

        return effet.titre;
    }

    private string GetDescriptionAffichee(SCOBJ_EFFET effet, ENUM_PERSONNAGE_Genre? genre)
    {
        if (effet is SCOBJ_PERSONNAGE_EFFET personnageEffet && genre.HasValue)
            return personnageEffet.GetDescription(genre.Value);

        if (!string.IsNullOrWhiteSpace(effet.GetDescriptionAffiche()))
            return effet.GetDescriptionAffiche();

        return effet.description;
    }
}