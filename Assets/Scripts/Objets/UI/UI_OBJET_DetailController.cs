using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_OBJET_DetailController : BaseDetailController<DATA_OBJET_Detail>
{
    [Header("UI - Header")]
    [SerializeField] private Image iconeImage;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private UI_RareteStarsView UI_RareteStarsView;

    [Header("UI - Infos")]
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text valeurText;
    [SerializeField] private TMP_Text categorieText;
    [SerializeField] private TMP_Text quantiteText;

    [Header("UI - Roots optionnels")]
    [SerializeField] private GameObject descriptionRoot;
    [SerializeField] private GameObject valeurRoot;
    [SerializeField] private GameObject categorieRoot;
    [SerializeField] private GameObject quantiteRoot;

    [Header("Actions")]
    [SerializeField] private UI_INVENTAIRE_Controller personnageUI_INVENTAIRE_Controller;

    private SYS_GameManager gameManager;
    private bool modeCompact;

    protected override void Awake()
    {
        base.Awake();
        ResolveDependencies();
    }

    protected override void ValidateReferences()
    {
        base.ValidateReferences();

        UTIL_UiReferenceValidator.Require(iconeImage, nameof(iconeImage), this);
        UTIL_UiReferenceValidator.Require(nomText, nameof(nomText), this);
        UTIL_UiReferenceValidator.Require(descriptionText, nameof(descriptionText), this);
        UTIL_UiReferenceValidator.Require(valeurText, nameof(valeurText), this);
        UTIL_UiReferenceValidator.Require(categorieText, nameof(categorieText), this);
        UTIL_UiReferenceValidator.Require(quantiteText, nameof(quantiteText), this);
        UTIL_UiReferenceValidator.Require(UI_RareteStarsView, nameof(UI_RareteStarsView), this);
    }

    public void OpenCompactMenu(DATA_OBJET_Detail data)
    {
        modeCompact = true;
        base.OpenMenu(data);
    }

    public new void OpenMenu(DATA_OBJET_Detail data)
    {
        modeCompact = false;
        base.OpenMenu(data);
    }

    protected override void RefreshUI(DATA_OBJET_Detail data)
    {
        if (data == null)
            return;

        RefreshHeader(data);
        RefreshInfos(data);
        RefreshCompactMode();
    }

    private void ResolveDependencies()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<SYS_GameManager>();
        }

        if (personnageUI_INVENTAIRE_Controller == null)
        {
            personnageUI_INVENTAIRE_Controller =
                FindAnyObjectByType<UI_INVENTAIRE_Controller>(FindObjectsInactive.Include);
        }
    }

    private void RefreshHeader(DATA_OBJET_Detail data)
    {
        if (iconeImage != null)
        {
            iconeImage.sprite = data.icone;
            iconeImage.enabled = data.icone != null;
        }

        if (nomText != null)
        {
            nomText.text = data.nom;
        }

        UI_RareteStarsView?.Refresh(data.rareteEtoiles);
    }

    private void RefreshInfos(DATA_OBJET_Detail data)
    {
        if (descriptionText != null)
            descriptionText.text = data.description;

        if (valeurText != null)
            valeurText.text = data.valeur.ToString();

        if (categorieText != null)
            categorieText.text = data.categorie;

        if (quantiteText != null)
            quantiteText.text = data.quantite.ToString();
    }

    private void RefreshCompactMode()
    {
        if (descriptionRoot != null)
            descriptionRoot.SetActive(!modeCompact);

        if (valeurRoot != null)
            valeurRoot.SetActive(!modeCompact);

        if (categorieRoot != null)
            categorieRoot.SetActive(!modeCompact);

        if (quantiteRoot != null)
            quantiteRoot.SetActive(!modeCompact);
    }

    protected override void RefreshPrimaryAction(DATA_OBJET_Detail data)
    {
        if (primaryActionButton == null)
            return;

        bool equipable = data != null && data.sourceObjet is SCOBJ_OBJET_EQUIPPABLE;
        bool afficherBouton = equipable && !modeCompact;

        primaryActionButton.gameObject.SetActive(afficherBouton);
        primaryActionButton.interactable = afficherBouton;

        if (primaryActionText != null)
        {
            primaryActionText.text = "Équiper";
        }
    }

    protected override void OnClickPrimaryAction()
    {
        if (currentData == null)
            return;

        SCOBJ_OBJET_EQUIPPABLE objetEquipable = currentData.sourceObjet as SCOBJ_OBJET_EQUIPPABLE;
        if (objetEquipable == null)
        {
            Debug.LogWarning("OnClickPrimaryAction : objet non équipable");
            return;
        }

        ResolveDependencies();

        if (personnageUI_INVENTAIRE_Controller == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_Controller introuvable");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogWarning("SYS_GameManager introuvable");
            return;
        }

        DATA_JOUEUR joueur = gameManager.GetHumanPlayer();
        if (joueur == null)
        {
            Debug.LogWarning("Joueur humain introuvable");
            return;
        }

        DATA_PERSONNAGE_DisplayContext contexte = new(joueur.compagnie);

        personnageUI_INVENTAIRE_Controller.OpenSelectionPersonnage(
            joueur.personnagesRecrutes,
            personnage => HandlePersonnageChoisiPourEquipement(joueur, personnage, objetEquipable),
            contexte
        );
    }

    private void HandlePersonnageChoisiPourEquipement(
        DATA_JOUEUR joueur,
        SCOBJ_Personnage personnage,
        SCOBJ_OBJET_EQUIPPABLE objet)
    {
        if (joueur == null || personnage == null || objet == null)
            return;

        bool success = UTIL_JOUEUR_EQUIPPEMENT.EquiperObjetAuPersonnage(joueur, personnage, objet);
        if (!success)
        {
            Debug.LogWarning("Équipement échoué");
            return;
        }

        Debug.Log($"Objet équipé : {objet.nom} sur {personnage.nom} {personnage.prenom}");

        if (gameManager != null)
        {
            gameManager.SynchroniserHudAvecJoueurHumain();
            gameManager.RefreshToutLeHUD();
        }

        CloseMenu();
    }

    protected override IReadOnlyList<SCOBJ_EFFET> GetEffets(DATA_OBJET_Detail data)
    {
        return data != null ? data.effets : null;
    }

    protected override ENUM_PERSONNAGE_Genre? GetGenreForEffets(DATA_OBJET_Detail data)
    {
        return null;
    }
}