using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EQUIPE_ACTION_RecompensePopup : UTIL_UiPanelControllerBase
{
    [Header("Navigation")]
    [SerializeField] private Button boutonFermer;

    [Header("Texte")]
    [SerializeField] private TMP_Text titreText;
    [SerializeField] private TMP_Text equipeText;
    [SerializeField] private TMP_Text provinceText;
    [SerializeField] private TMP_Text prestigeText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text lignePrincipaleText;
    [SerializeField] private TMP_Text ligneSecondaireText;
    [SerializeField] private TMP_Text objetNomText;
    [SerializeField] private TMP_Text objetDescriptionText;

    [Header("Blocs UI")]
    [SerializeField] private GameObject blocObjet;
    [SerializeField] private GameObject blocAucunObjet;

    [Header("Objet")]
    [SerializeField] private Image objetIconeImage;
    [SerializeField] private UI_RareteStarsView rareteView;

    private DATA_EQUIPE_ACTION_RecompensePopup currentData;
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
                panelRoot = panelTag.gameObject;
            else
                Debug.LogWarning($"PanelRootTag introuvable dans {name}");
        }
    }

    public void OpenMenu(DATA_EQUIPE_ACTION_RecompensePopup data, bool fermerAuTourSuivant = true)
    {
        if (data == null)
        {
            Debug.LogWarning("UI_EQUIPE_ACTION_RecompensePopup : data null");
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
        {
            titreText.text = string.IsNullOrWhiteSpace(currentData.titre)
                ? "Récompense d'action"
                : currentData.titre;
        }

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

        if (lignePrincipaleText != null)
            lignePrincipaleText.text = string.IsNullOrWhiteSpace(currentData.lignePrincipale) ? "" : currentData.lignePrincipale;

        if (ligneSecondaireText != null)
            ligneSecondaireText.text = string.IsNullOrWhiteSpace(currentData.ligneSecondaire) ? "" : currentData.ligneSecondaire;

        bool afficherObjet = currentData.AUnObjet();

        if (blocObjet != null)
            blocObjet.SetActive(afficherObjet);

        if (blocAucunObjet != null)
            blocAucunObjet.SetActive(!afficherObjet);

        if (!afficherObjet)
        {
            if (objetNomText != null)
                objetNomText.text = "";

            if (objetDescriptionText != null)
                objetDescriptionText.text = "";

            if (objetIconeImage != null)
            {
                objetIconeImage.sprite = null;
                objetIconeImage.enabled = false;
            }

            if (rareteView != null)
                rareteView.Refresh(0);

            return;
        }

        if (objetNomText != null)
        {
            objetNomText.text = "Objet trouvé : " + (
                string.IsNullOrWhiteSpace(currentData.nomObjet)
                    ? "Objet inconnu"
                    : currentData.nomObjet
            );
        }

        if (objetDescriptionText != null)
        {
            objetDescriptionText.text = string.IsNullOrWhiteSpace(currentData.descriptionObjet)
                ? ""
                : currentData.descriptionObjet;
        }

        if (objetIconeImage != null)
        {
            objetIconeImage.sprite = currentData.iconeObjet;
            objetIconeImage.enabled = currentData.iconeObjet != null;
        }

        if (rareteView != null)
            rareteView.Refresh(Mathf.Max(1, currentData.rareteObjet));
    }
    private void AfficherPopupRecompenseAction(
    SYS_GameManager gameManager,
    STATE_EQUIPE equipe,
    STATE_PROVINCE province,
    int gainPrestige,
    float gainOccupation,
    float reductionOccupationAdverse)
{
    if (gameManager == null || equipe == null)
        return;

    DATA_JOUEUR humain = gameManager.GetHumanPlayer();
    if (humain == null)
        return;

    if (humain.equipes == null || !humain.equipes.Contains(equipe))
        return;

    UI_EQUIPE_ACTION_RecompensePopup popup = gameManager.ActionRecompensePopup;

    if (popup == null)
    {
        popup = Object.FindAnyObjectByType<UI_EQUIPE_ACTION_RecompensePopup>(FindObjectsInactive.Include);
        gameManager.ActionRecompensePopup = popup;
    }

    if (popup == null)
        return;

    DATA_EQUIPE_ACTION_RecompensePopup data = new DATA_EQUIPE_ACTION_RecompensePopup
    {
        action = ENUM_EQUIPE_ACTION.Vadrouille,
        titre = "Résultat de la vadrouille",
        nomEquipe = equipe.data != null ? equipe.data.nomEquipe : "Équipe",
        nomProvince = province != null && province.data != null ? province.data.nom : "Province inconnue",
        prestigeGagne = gainPrestige,
        xpGagneParPersonnage = 0,
        lignePrincipale = $"+Occupation : {gainOccupation:0.#}%",
        ligneSecondaire = $"-Occupation adverse : {reductionOccupationAdverse:0.#}%",
        objetTrouve = false,
        nomObjet = "",
        descriptionObjet = "",
        iconeObjet = null,
        rareteObjet = 0
    };

    popup.OpenMenu(data, true);
}
}