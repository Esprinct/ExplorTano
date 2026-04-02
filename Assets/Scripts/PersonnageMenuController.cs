using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PersonnageMenuController : MonoBehaviour
{
    private GameObject panelRoot;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text prenomText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private RareteStarsView rareteStarsView;

    private void Awake()
    {
        AutoBind();
        CloseMenu();
    }

    private void Update()
    {
        if (panelRoot != null && panelRoot.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseMenu();
            }
        }
    }

    private void AutoBind()
    {
        PanelRootTag tag = GetComponentInChildren<PanelRootTag>(true);
        if (tag != null)
        {
            panelRoot = tag.gameObject;
        }
        else
        {
            Debug.LogWarning($"PanelRootTag introuvable dans {name}");
        }
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
public void OpenPersonnageMenu(PersonnageHudData data)
{
    if (data == null)
    {
        Debug.LogWarning("OpenPersonnageMenu : data est null");
        return;
    }

    Debug.Log($"OpenPersonnageMenu : {data.nom} {data.prenom}");

    if (panelRoot == null)
    {
        Debug.LogError("panelRoot est null");
        return;
    }

    panelRoot.SetActive(true);

    if (portraitImage != null)
    {
        portraitImage.sprite = data.sprite;
        portraitImage.enabled = data.sprite != null;
    }

    if (nomText != null)
    {
        nomText.text = PersonnageUiFormatter.FormatNom(data.nom);
    }

    if (prenomText != null)
    {
        prenomText.text = PersonnageUiFormatter.FormatPrenom(data.prenom);
    }

    if (niveauText != null)
    {
        niveauText.text = PersonnageUiFormatter.FormatNiveau(data.niveau);
    }

    if (roleText != null)
    {
        roleText.text = PersonnageUiFormatter.FormatRole(data.role);
    }

    if (rareteStarsView != null)
{
    rareteStarsView.Refresh(data.rareteEtoiles);
}

    if (descriptionText != null)
    {
        descriptionText.text = data.description;
    }
}
public bool IsOpen()
{
    return panelRoot != null && panelRoot.activeInHierarchy;
}
    public void CloseMenu()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }
}
