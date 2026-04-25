using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OBJET_Slot : BaseSlotUI<SCOBJ_OBJET>
{
    [Header("Références UI Objet")]
    [SerializeField] private Image iconeImage;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private UI_RareteStarsView UI_RareteStarsView;
    [SerializeField] private TMP_Text descriptionText;
[Header("Boutique")]
[SerializeField] private TMP_Text prixBoutiqueText;
[SerializeField] private TMP_Text quantitePossedeeText;
    private string overrideDescription;
    private bool descriptionEstIndisponible;
    private SCOBJ_OBJET currentObjet;
public void RefreshBoutique(SCOBJ_OBJET objet, int prix, int quantitePossedee = 0)
{
    Refresh(objet);

    if (prixBoutiqueText != null)
    {
        prixBoutiqueText.gameObject.SetActive(true);
        prixBoutiqueText.text = $"{prix} étrinium";
    }

    if (quantitePossedeeText != null)
    {
        quantitePossedeeText.gameObject.SetActive(quantitePossedee > 0);
        quantitePossedeeText.text = $"x{quantitePossedee}";
    }
}    protected override void AutoBind()
    {
        base.AutoBind();

        if (iconeImage == null)
        {
            iconeImage = GetComponentInChildren<Image>(true);
        }
    }

    protected override void ValidateReferences()
    {
        base.ValidateReferences();

        UTIL_UiReferenceValidator.Require(iconeImage, nameof(iconeImage), this);
        UTIL_UiReferenceValidator.Require(nomText, nameof(nomText), this);
        UTIL_UiReferenceValidator.Require(UI_RareteStarsView, nameof(UI_RareteStarsView), this);
    }

    public void SetOverrideDescription(string texte, bool estIndisponible = false)
    {
        overrideDescription = texte;
        descriptionEstIndisponible = estIndisponible;

        if (currentObjet != null)
        {
            Refresh(currentObjet);
        }
    }

    public new void Refresh(SCOBJ_OBJET data)
    {
        currentObjet = data;
        base.Refresh(data);
    }

    protected override void RefreshVisuals(SCOBJ_OBJET data)
    {
        currentObjet = data;

        if (iconeImage != null)
        {
            iconeImage.sprite = data.icone;
            iconeImage.enabled = data.icone != null;
        }

        if (nomText != null)
            nomText.text = data.nom;

        if (UI_RareteStarsView != null)
            UI_RareteStarsView.Refresh(data.rareteEtoiles);

        if (descriptionText != null)
        {
            if (!string.IsNullOrWhiteSpace(overrideDescription))
            {
                descriptionText.text = descriptionEstIndisponible
                    ? $"<color=#B71C1C>{overrideDescription}</color>"
                    : overrideDescription;
            }
            else
            {
                descriptionText.text = data.description;
            }
        }
    }
}