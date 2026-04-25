using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Recrutement_Slot : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text prenomText;
    [SerializeField] private TMP_Text rareteText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private UI_RareteStarsView UI_RareteStarsView;

    [Header("Bouton principal")]
    [SerializeField] private Button boutonRecruter;

    [Header("Stats")]
    [SerializeField] private TMP_Text curiositeText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text dexteriteText;
    [SerializeField] private TMP_Text enduranceText;

    [Header("Recrutement")]
    [SerializeField] private TMP_Text prixText;
    [SerializeField] private TMP_Text coutParTourText;
    [SerializeField] private TMP_Text compagnieText;

    [Header("Marqueurs d'enchère")]
    [SerializeField] private Image badgeMaizin;
    [SerializeField] private Image badgeKinia;
    [SerializeField] private Image badgeJoho;

    private SCOBJ_Personnage personnageData;
    private SYS_GameManager gameManager;
    private UI_Recrutement_MenuController menuController;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<SYS_GameManager>();

        if (boutonRecruter == null)
            boutonRecruter = GetComponentInChildren<Button>(true);

        if (boutonRecruter != null)
        {
            boutonRecruter.onClick.RemoveListener(OnClickRecruter);
            boutonRecruter.onClick.AddListener(OnClickRecruter);
        }
    }

    private void OnDestroy()
    {
        if (boutonRecruter != null)
            boutonRecruter.onClick.RemoveListener(OnClickRecruter);
    }

    public void SetMenuController(UI_Recrutement_MenuController menu)
    {
        menuController = menu;
    }

    public void Refresh(SCOBJ_Personnage data)
    {
        personnageData = data;

        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        gameObject.SetActive(true);

        if (portraitImage != null)
        {
            portraitImage.sprite = data.sprite;
            portraitImage.enabled = data.sprite != null;
        }

        if (nomText != null)
            nomText.text = string.IsNullOrWhiteSpace(data.nom) ? "Aucun nom" : data.nom;

        if (prenomText != null)
            prenomText.text = string.IsNullOrWhiteSpace(data.prenom) ? "" : data.prenom;

        if (rareteText != null)
            rareteText.text = $"{data.rareteEtoiles}★";

        if (roleText != null)
            roleText.text = data.roleActuel.ToString();

        UI_RareteStarsView?.Refresh(data.rareteEtoiles);

        if (curiositeText != null)
            curiositeText.text = $"FOR {data.curiosite}";

        if (intelligenceText != null)
            intelligenceText.text = $"INT {data.intelligence}";

        if (dexteriteText != null)
            dexteriteText.text = $"DEX {data.dexterite}";

        if (enduranceText != null)
            enduranceText.text = $"END {data.endurance}";

        DATA_OffreRecrutement offre = GetOffreCourante();
        int prixMinimum = offre != null ? offre.prixMinimum : Mathf.Max(0, data.coutRecrutementBase);

        if (prixText != null)
            prixText.text = prixMinimum.ToString();

        if (coutParTourText != null)
            coutParTourText.text = data.coutParTour.ToString();

        RefreshCompagnieText(data);
        RefreshBadgesEncheres(offre);
        RefreshBoutonRecruter(offre, prixMinimum);
    }

    private void RefreshCompagnieText(SCOBJ_Personnage data)
    {
        if (compagnieText == null || data == null)
            return;

        if (!data.aPreferenceCompagnie || data.compagniePreferee == ENUM_Compagnie.Aucune)
        {
            compagnieText.text = "Neutre";
            compagnieText.color = Color.gray;
            return;
        }

        compagnieText.text = data.compagniePreferee.ToString();

        switch (data.compagniePreferee)
        {
            case ENUM_Compagnie.Maizin:
                compagnieText.color = Color.green;
                break;

            case ENUM_Compagnie.Kinia:
                compagnieText.color = Color.red;
                break;

            case ENUM_Compagnie.Joho:
                compagnieText.color = Color.yellow;
                break;

            default:
                compagnieText.color = Color.gray;
                break;
        }
    }

    private void RefreshBadgesEncheres(DATA_OffreRecrutement offre)
    {
        SetBadgeState(badgeMaizin, false);
        SetBadgeState(badgeKinia, false);
        SetBadgeState(badgeJoho, false);

        if (offre == null)
            return;

        SetBadgeState(badgeMaizin, offre.AUneEnchere(ENUM_Compagnie.Maizin));
        SetBadgeState(badgeKinia, offre.AUneEnchere(ENUM_Compagnie.Kinia));
        SetBadgeState(badgeJoho, offre.AUneEnchere(ENUM_Compagnie.Joho));
    }

    private void SetBadgeState(Image badge, bool actif)
    {
        if (badge == null)
            return;

        badge.gameObject.SetActive(actif);
        badge.enabled = actif;
    }

    private void RefreshBoutonRecruter(DATA_OffreRecrutement offre, int prixMinimum)
    {
        if (boutonRecruter == null)
            return;

        DATA_JOUEUR humain = gameManager != null ? gameManager.GetHumanPlayer() : null;

        bool peutOffrir =
            humain != null &&
            gameManager != null &&
            gameManager.PeutRecruterCeTour() &&
            humain.etrinium >= prixMinimum &&
            (offre == null || !offre.estResolue);

        boutonRecruter.interactable = peutOffrir;
    }

    private void OnClickRecruter()
    {
        if (personnageData == null || menuController == null)
            return;

        menuController.OuvrirConfirmationPour(personnageData);
    }

    private DATA_OffreRecrutement GetOffreCourante()
    {
        if (gameManager == null || gameManager.SYS_RecrutementSystem == null || personnageData == null)
            return null;

        return gameManager.SYS_RecrutementSystem.GetOffre(personnageData);
    }
}