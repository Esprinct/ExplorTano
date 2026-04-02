using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudPersonnageSlot : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Image spriteImage;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text prenomText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private Button button;
    [SerializeField] private RareteStarsView rareteStarsView;

    private PersonnageHudData personnageData;
    private PersonnageMenuController personnageMenuController;

    private void Awake()
    {
        AutoBind();
    }

    private void AutoBind()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
        }
        

        if (spriteImage == null)
        {
            spriteImage = GetComponentInChildren<Image>(true);
        }

        if (button != null)
        {
            button.onClick.RemoveListener(OnClickSlot);
            button.onClick.AddListener(OnClickSlot);
        }
        else
        {
            Debug.LogWarning($"Aucun Button trouvé dans {name}");
        }
    }

    public void SetMenuController(PersonnageMenuController controller)
    {
        personnageMenuController = controller;
    }
private string FormaterEtoiles(int nbEtoiles)
{
    nbEtoiles = Mathf.Clamp(nbEtoiles, 1, 5);
    return new string('★', nbEtoiles) + new string('☆', 5 - nbEtoiles);
}
private string FormaterEtoilesRichText(int nbEtoiles)
{
    nbEtoiles = Mathf.Clamp(nbEtoiles, 1, 5);

    string pleines = "<color=#FFD700>" + new string('★', nbEtoiles) + "</color>";
    string vides = "<color=#666666>" + new string('☆', 5 - nbEtoiles) + "</color>";

    return pleines + vides;
}
    public void Refresh(PersonnageHudData data)
{
    personnageData = data;

    if (data == null)
    {
        gameObject.SetActive(false);
        return;
    }

    gameObject.SetActive(true);

    if (spriteImage != null)
    {
        spriteImage.sprite = data.sprite;
        spriteImage.enabled = data.sprite != null;
    }

    if (niveauText != null)
    {
        niveauText.text = PersonnageUiFormatter.FormatNiveau(data.niveau);
    }

    if (nomText != null)
    {
        nomText.text = PersonnageUiFormatter.FormatNom(data.nom);
    }

    if (prenomText != null)
    {
        prenomText.text = PersonnageUiFormatter.FormatPrenom(data.prenom);
    }

    if (roleText != null)
    {
        roleText.text = PersonnageUiFormatter.FormatRole(data.role);
    }

    if (rareteStarsView != null)
{
    rareteStarsView.Refresh(data.rareteEtoiles);
}

    if (xpSlider != null)
    {
        xpSlider.minValue = 0f;
        xpSlider.maxValue = Mathf.Max(1, data.xpNiveauSuivant);
        xpSlider.value = Mathf.Clamp(data.xpActuel, 0, data.xpNiveauSuivant);
    }

    if (button != null)
    {
        button.interactable = true;
    }
}

    private void OnClickSlot()
    {
        if (personnageData == null)
        {
            Debug.LogWarning("personnageData est null");
            return;
        }

        if (personnageMenuController == null)
        {
            personnageMenuController = FindAnyObjectByType<PersonnageMenuController>(FindObjectsInactive.Include);
        }

        if (personnageMenuController == null)
        {
            Debug.LogWarning("personnageMenuController est null");
            return;
        }
        personnageMenuController.OpenPersonnageMenu(personnageData);
    }
}