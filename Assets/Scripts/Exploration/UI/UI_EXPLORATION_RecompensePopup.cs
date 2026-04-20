using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EXPLORATION_RecompensePopup : UTIL_UiPanelControllerBase
{
    [Header("Navigation")]
    [SerializeField] private Button boutonFermer;

    [Header("Texte")]
    [SerializeField] private TMP_Text titreText;
    [SerializeField] private TMP_Text equipeText;
    [SerializeField] private TMP_Text provinceText;
    [SerializeField] private TMP_Text prestigeText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text artefactNomText;
    [SerializeField] private TMP_Text artefactDescriptionText;

    [Header("Blocs UI")]
    [SerializeField] private GameObject blocArtefact;
    [SerializeField] private GameObject blocAucunArtefact;

    [Header("Artefact")]
    [SerializeField] private Image artefactIconeImage;
    [SerializeField] private UI_RareteStarsView rareteView;

    private DATA_EXPLORATION_RecompensePopup currentData;
    private bool fermetureAuProchainTour;

    private void Awake()
    {
        AutoBind();

        if (boutonFermer != null)
            boutonFermer.onClick.AddListener(CloseMenu);

        ClosePanel();
    }

    private void OnDestroy()
    {
        if (boutonFermer != null)
            boutonFermer.onClick.RemoveListener(CloseMenu);
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
            {
                panelRoot = panelTag.gameObject;
            }
            else
            {
                Debug.LogWarning($"PanelRootTag introuvable dans {name}");
            }
        }
    }

    public void OpenMenu(DATA_EXPLORATION_RecompensePopup data, bool fermerAuTourSuivant = true)
    {
        if (data == null)
        {
            Debug.LogWarning("UI_EXPLORATION_RecompensePopup : data null");
            return;
        }

        currentData = data;
        fermetureAuProchainTour = fermerAuTourSuivant;

        OpenPanel();
        RefreshUI();
    }

    public void CloseMenu()
    {
        currentData = null;
        fermetureAuProchainTour = false;
        ClosePanel();
    }

    public void FermerSiDemandeeAuTourSuivant()
    {
        if (!IsOpen())
            return;

        if (!fermetureAuProchainTour)
            return;

        CloseMenu();
    }

    private void RefreshUI()
    {
        if (currentData == null)
            return;

        if (titreText != null)
            titreText.text = "Récompenses d'exploration";

        if (equipeText != null)
        {
            equipeText.text = "Équipe : " + (
                string.IsNullOrWhiteSpace(currentData.nomEquipe)
                    ? "-"
                    : currentData.nomEquipe
            );
        }

        if (provinceText != null)
        {
            provinceText.text = "Province : " + (
                string.IsNullOrWhiteSpace(currentData.nomProvince)
                    ? "-"
                    : currentData.nomProvince
            );
        }

        if (prestigeText != null)
            prestigeText.text = $"Prestige gagné : +{Mathf.Max(0, currentData.prestigeGagne)}";

        if (xpText != null)
            xpText.text = $"XP gagnée par personnage : +{Mathf.Max(0, currentData.xpGagneParPersonnage)} XP";

        bool afficherArtefact = currentData.artefactTrouve;

        if (blocArtefact != null)
            blocArtefact.SetActive(afficherArtefact);

        if (blocAucunArtefact != null)
            blocAucunArtefact.SetActive(!afficherArtefact);

        if (!afficherArtefact)
        {
            if (artefactNomText != null)
                artefactNomText.text = "";

            if (artefactDescriptionText != null)
                artefactDescriptionText.text = "";

            if (artefactIconeImage != null)
            {
                artefactIconeImage.sprite = null;
                artefactIconeImage.enabled = false;
            }

            return;
        }

        if (artefactNomText != null)
        {
            artefactNomText.text = "Artefact trouvé : " + (
                string.IsNullOrWhiteSpace(currentData.nomArtefact)
                    ? "Artefact inconnu"
                    : currentData.nomArtefact
            );
        }

        if (artefactDescriptionText != null)
        {
            artefactDescriptionText.text = string.IsNullOrWhiteSpace(currentData.descriptionArtefact)
                ? ""
                : currentData.descriptionArtefact;
        }

        if (artefactIconeImage != null)
        {
            artefactIconeImage.sprite = currentData.iconeArtefact;
            artefactIconeImage.enabled = currentData.iconeArtefact != null;
        }

        if (rareteView != null)
            rareteView.Refresh(Mathf.Max(1, currentData.rareteArtefact));
    }
}