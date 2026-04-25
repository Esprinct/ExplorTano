using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BOUTIQUE_Controller : UTIL_UiPanelControllerBase
{
    [System.Serializable]
    public class DATA_BOUTIQUE_Categorie
    {
        public string nomCategorie;
        public Button bouton;
        public SCOBJ_BOUTIQUE_Catalogue catalogue;
    }

    [Header("Catalogues")]
    [SerializeField] private List<DATA_BOUTIQUE_Categorie> categories = new();

    [Header("Références")]
    [SerializeField] private SYS_GameManager gameManager;
    [SerializeField] private UI_OBJET_DetailController objetDetailController;
    [SerializeField] private UI_ConfirmationDialog confirmationDialog;

    [Header("Liste objets")]
    [SerializeField] private Transform contentObjets;
    [SerializeField] private UI_OBJET_Slot slotObjetTemplate;

    [Header("Achat")]
    [SerializeField] private TMP_Text nomObjetSelectionneText;
    [SerializeField] private TMP_Text prixObjetSelectionneText;
    [SerializeField] private TMP_Text prixTotalText;
    [SerializeField] private TMP_Text possedeText;
    [SerializeField] private TMP_Text previewQuantiteText;

    [Header("Quantité")]
    [SerializeField] private Button boutonQuantiteMoins;
    [SerializeField] private Button boutonQuantitePlus;
    [SerializeField] private TMP_Text quantiteText;

    [Header("Boutons")]
    [SerializeField] private Button boutonAcheter;
    [SerializeField] private TMP_Text boutonAcheterText;
    [SerializeField] private Button boutonFermer;

    [Header("Options")]
    [SerializeField] private bool autoriserAchatMultipleNonStackable = true;
    [SerializeField] private bool selectionnerPremierObjetALOuverture = true;

    private readonly List<UI_OBJET_Slot> slots = new();

    private SCOBJ_BOUTIQUE_Catalogue catalogueActuel;
    private DATA_BOUTIQUE_Categorie categorieActuelle;
    private DATA_BOUTIQUE_Offre offreSelectionnee;

    private int quantiteAchat = 1;
    private bool listenersInitialises;

    private void Awake()
    {
        AutoBind();
        ResolveDependencies();
        InitialiserListeners();

        if (slotObjetTemplate != null)
            slotObjetTemplate.gameObject.SetActive(false);

        InitialiserPoolSlotsExistants();

        ClosePanel();
    }

    private void OnDestroy()
    {
        if (boutonAcheter != null)
            boutonAcheter.onClick.RemoveListener(DemanderConfirmationAchat);

        if (boutonFermer != null)
            boutonFermer.onClick.RemoveListener(CloseMenu);

        if (boutonQuantiteMoins != null)
            boutonQuantiteMoins.onClick.RemoveListener(DiminuerQuantite);

        if (boutonQuantitePlus != null)
            boutonQuantitePlus.onClick.RemoveListener(AugmenterQuantite);
    }

    private void AutoBind()
    {
        if (panelRoot == null)
        {
            PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
            if (panelTag != null)
                panelRoot = panelTag.gameObject;
        }
    }

    private void ResolveDependencies()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        if (objetDetailController == null)
            objetDetailController = FindAnyObjectByType<UI_OBJET_DetailController>(FindObjectsInactive.Include);

        if (confirmationDialog == null)
            confirmationDialog = FindAnyObjectByType<UI_ConfirmationDialog>(FindObjectsInactive.Include);
    }

    private void InitialiserListeners()
    {
        if (listenersInitialises)
            return;

        listenersInitialises = true;

        if (boutonAcheter != null)
            boutonAcheter.onClick.AddListener(DemanderConfirmationAchat);

        if (boutonFermer != null)
            boutonFermer.onClick.AddListener(CloseMenu);

        if (boutonQuantiteMoins != null)
            boutonQuantiteMoins.onClick.AddListener(DiminuerQuantite);

        if (boutonQuantitePlus != null)
            boutonQuantitePlus.onClick.AddListener(AugmenterQuantite);

        if (categories == null)
            return;

        foreach (DATA_BOUTIQUE_Categorie categorie in categories)
        {
            if (categorie == null || categorie.bouton == null)
                continue;

            DATA_BOUTIQUE_Categorie categorieCapture = categorie;
            categorie.bouton.onClick.AddListener(() => SelectionnerCategorie(categorieCapture));
        }
    }

    public void OpenMenu()
    {
        ResolveDependencies();
        InitialiserListeners();

        OpenPanel();

        if (catalogueActuel == null)
            SelectionnerPremiereCategorieValide();
        else
            RefreshView();

        if (selectionnerPremierObjetALOuverture && offreSelectionnee == null)
            SelectionnerPremiereOffreValide();

        RefreshAchatPanel();
        ForcerRebuildLayout();
    }

    public void CloseMenu()
    {
        ClosePanel();
    }

    public void RefreshCurrentBoutique()
    {
        if (!IsOpen())
            return;

        RefreshView();
        RefreshAchatPanel();
        ForcerRebuildLayout();
    }

    private void SelectionnerPremiereCategorieValide()
    {
        if (categories == null || categories.Count == 0)
        {
            catalogueActuel = null;
            categorieActuelle = null;
            offreSelectionnee = null;

            RefreshBoutonsCategories();
            RefreshView();
            RefreshAchatPanel();
            return;
        }

        foreach (DATA_BOUTIQUE_Categorie categorie in categories)
        {
            if (categorie == null || categorie.catalogue == null)
                continue;

            categorieActuelle = categorie;
            catalogueActuel = categorie.catalogue;
            offreSelectionnee = null;
            quantiteAchat = 1;

            RefreshBoutonsCategories();
            RefreshView();
            RefreshAchatPanel();
            ForcerRebuildLayout();
            return;
        }

        catalogueActuel = null;
        categorieActuelle = null;
        offreSelectionnee = null;

        RefreshBoutonsCategories();
        RefreshView();
        RefreshAchatPanel();
    }

    private void SelectionnerCategorie(DATA_BOUTIQUE_Categorie categorie)
    {
        if (categorie == null)
            return;

        categorieActuelle = categorie;
        catalogueActuel = categorie.catalogue;
        offreSelectionnee = null;
        quantiteAchat = 1;

        RefreshBoutonsCategories();
        RefreshView();

        if (selectionnerPremierObjetALOuverture)
            SelectionnerPremiereOffreValide();

        RefreshAchatPanel();
        ForcerRebuildLayout();
    }

    private void SelectionnerPremiereOffreValide()
    {
        if (catalogueActuel == null || catalogueActuel.offres == null)
            return;

        foreach (DATA_BOUTIQUE_Offre offre in catalogueActuel.offres)
        {
            if (offre == null || offre.objet == null)
                continue;

            SelectionnerOffre(offre);
            return;
        }
    }

    private void RefreshBoutonsCategories()
    {
        if (categories == null)
            return;

        foreach (DATA_BOUTIQUE_Categorie categorie in categories)
        {
            if (categorie == null || categorie.bouton == null)
                continue;

            categorie.bouton.interactable = categorie != categorieActuelle;
        }
    }

    private void InitialiserPoolSlotsExistants()
    {
        slots.Clear();

        if (contentObjets == null)
            return;

        UI_OBJET_Slot[] slotsExistants = contentObjets.GetComponentsInChildren<UI_OBJET_Slot>(true);

        foreach (UI_OBJET_Slot slot in slotsExistants)
        {
            if (slot == null)
                continue;

            if (slotObjetTemplate != null && slot == slotObjetTemplate)
                continue;

            if (!slots.Contains(slot))
                slots.Add(slot);

            slot.gameObject.SetActive(false);
        }
    }

    private UI_OBJET_Slot GetOrCreateSlot(int index)
    {
        if (index < slots.Count && slots[index] != null)
            return slots[index];

        if (slotObjetTemplate == null || contentObjets == null)
            return null;

        UI_OBJET_Slot nouveauSlot = Instantiate(slotObjetTemplate, contentObjets);
        nouveauSlot.gameObject.name = $"ObjetSlot_Boutique_{index}";
        nouveauSlot.gameObject.SetActive(false);

        slots.Add(nouveauSlot);
        return nouveauSlot;
    }

    private void HideSlotsFromIndex(int startIndex)
    {
        for (int i = startIndex; i < slots.Count; i++)
        {
            if (slots[i] != null)
                slots[i].gameObject.SetActive(false);
        }
    }

    private void RefreshView()
    {
        ResolveDependencies();

        if (catalogueActuel == null || catalogueActuel.offres == null)
        {
            HideSlotsFromIndex(0);
            Debug.LogWarning("Boutique : aucun catalogue actif.");
            return;
        }

        if (contentObjets == null || slotObjetTemplate == null)
        {
            Debug.LogWarning("Boutique : contentObjets ou slotObjetTemplate manquant.");
            return;
        }

        DATA_JOUEUR joueur = gameManager != null ? gameManager.GetHumanPlayer() : null;

        int slotIndex = 0;

        foreach (DATA_BOUTIQUE_Offre offre in catalogueActuel.offres)
        {
            if (offre == null || offre.objet == null)
                continue;

            UI_OBJET_Slot slot = GetOrCreateSlot(slotIndex);
            if (slot == null)
                continue;

            DATA_BOUTIQUE_Offre offreCapture = offre;

            int prix = SVC_BOUTIQUE_Service.GetPrixUnitaire(offre);
            int possede = SVC_BOUTIQUE_Service.GetQuantitePossedee(joueur, offre.objet);

            slot.gameObject.SetActive(true);
            slot.RefreshBoutique(offre.objet, prix, possede);
            slot.SetOnClick(_ => SelectionnerOffre(offreCapture));

            slotIndex++;
        }

        HideSlotsFromIndex(slotIndex);
        ForcerRebuildLayout();
    }

    private void SelectionnerOffre(DATA_BOUTIQUE_Offre offre)
    {
        offreSelectionnee = offre;
        quantiteAchat = 1;

        if (offreSelectionnee != null)
            OuvrirDetailObjet(offreSelectionnee.objet);

        RefreshAchatPanel();
        ForcerRebuildLayout();
    }

    private void OuvrirDetailObjet(SCOBJ_OBJET objet)
    {
        if (objet == null)
            return;

        ResolveDependencies();

        if (objetDetailController == null)
        {
            Debug.LogWarning("Boutique : UI_OBJET_DetailController introuvable.");
            return;
        }

        DATA_OBJET_Detail detailData = MAP_OBJET_DetailMapper.ToDetailData(objet);
        if (detailData == null)
            return;

        objetDetailController.OpenMenu(detailData);
    }

    private void RefreshAchatPanel()
    {
        ResolveDependencies();

        DATA_JOUEUR joueur = gameManager != null ? gameManager.GetHumanPlayer() : null;

        bool aSelection = offreSelectionnee != null && offreSelectionnee.objet != null;

        int possede = aSelection
            ? SVC_BOUTIQUE_Service.GetQuantitePossedee(joueur, offreSelectionnee.objet)
            : 0;

        int maxAchetable = aSelection
            ? SVC_BOUTIQUE_Service.GetQuantiteMaxAchetable(
                joueur,
                offreSelectionnee,
                autoriserAchatMultipleNonStackable
            )
            : 0;

        if (!aSelection)
        {
            quantiteAchat = 1;
        }
        else
        {
            int maxClamp = Mathf.Max(1, maxAchetable);
            quantiteAchat = Mathf.Clamp(quantiteAchat, 1, maxClamp);
        }

        bool peutAcheter =
            aSelection &&
            maxAchetable > 0 &&
            SVC_BOUTIQUE_Service.PeutAcheter(joueur, offreSelectionnee, quantiteAchat);

        int prixUnitaire = aSelection
            ? SVC_BOUTIQUE_Service.GetPrixUnitaire(offreSelectionnee)
            : 0;

        int prixTotal = aSelection
            ? SVC_BOUTIQUE_Service.GetPrixTotal(offreSelectionnee, quantiteAchat)
            : 0;

        bool stackable = aSelection && SVC_BOUTIQUE_Service.EstStackable(offreSelectionnee.objet);

        if (nomObjetSelectionneText != null)
        {
            nomObjetSelectionneText.text = aSelection
                ? offreSelectionnee.objet.nom
                : "Aucun objet sélectionné";
        }

        if (prixObjetSelectionneText != null)
        {
            prixObjetSelectionneText.text = aSelection
                ? $"{prixUnitaire} étrinium / unité"
                : "-";
        }

        if (prixTotalText != null)
        {
            prixTotalText.text = aSelection
                ? $"Total : {prixTotal} étrinium"
                : "Total : -";
        }

        if (quantiteText != null)
        {
            quantiteText.text = aSelection
                ? quantiteAchat.ToString()
                : "-";
        }

        if (possedeText != null)
        {
            possedeText.text = aSelection
                ? $"Possédé : {possede}"
                : "Possédé : -";
        }

        if (previewQuantiteText != null)
        {
            if (!aSelection)
            {
                previewQuantiteText.text = "Après achat : -";
            }
            else if (stackable)
            {
                previewQuantiteText.text = $"Après achat : {possede} → {possede + quantiteAchat}";
            }
            else
            {
                previewQuantiteText.text = $"Après achat : {possede} → {possede + quantiteAchat} exemplaire(s)";
            }
        }

        if (boutonQuantiteMoins != null)
            boutonQuantiteMoins.interactable = aSelection && quantiteAchat > 1;

        if (boutonQuantitePlus != null)
            boutonQuantitePlus.interactable = aSelection && quantiteAchat < maxAchetable;

        if (boutonAcheter != null)
            boutonAcheter.interactable = peutAcheter;

        if (boutonAcheterText != null)
        {
            if (!aSelection)
                boutonAcheterText.text = "Sélectionnez un objet";
            else if (maxAchetable <= 0)
                boutonAcheterText.text = "Fonds insuffisants";
            else
                boutonAcheterText.text = "Acheter";
        }
    }

    private void DiminuerQuantite()
    {
        quantiteAchat = Mathf.Max(1, quantiteAchat - 1);
        RefreshAchatPanel();
    }

    private void AugmenterQuantite()
    {
        ResolveDependencies();

        DATA_JOUEUR joueur = gameManager != null ? gameManager.GetHumanPlayer() : null;

        int maxAchetable = SVC_BOUTIQUE_Service.GetQuantiteMaxAchetable(
            joueur,
            offreSelectionnee,
            autoriserAchatMultipleNonStackable
        );

        quantiteAchat = Mathf.Clamp(
            quantiteAchat + 1,
            1,
            Mathf.Max(1, maxAchetable)
        );

        RefreshAchatPanel();
    }

    private void DemanderConfirmationAchat()
    {
        ResolveDependencies();

        DATA_JOUEUR joueur = gameManager != null ? gameManager.GetHumanPlayer() : null;

        if (joueur == null || offreSelectionnee == null || offreSelectionnee.objet == null)
            return;

        int quantite = Mathf.Max(1, quantiteAchat);
        int prixTotal = SVC_BOUTIQUE_Service.GetPrixTotal(offreSelectionnee, quantite);

        if (!SVC_BOUTIQUE_Service.PeutAcheter(joueur, offreSelectionnee, quantite))
        {
            RefreshAchatPanel();
            return;
        }

        string nomObjet = offreSelectionnee.objet.nom;
        int possedeAvant = SVC_BOUTIQUE_Service.GetQuantitePossedee(joueur, offreSelectionnee.objet);
        int possedeApres = possedeAvant + quantite;

        string message =
            $"Acheter {nomObjet} x{quantite} pour {prixTotal} étrinium ?\n\n" +
            $"Possédé : {possedeAvant} → {possedeApres}";

        if (confirmationDialog != null)
        {
            confirmationDialog.Open(
                message,
                ConfirmerAchatSelection,
                "Acheter",
                "Annuler"
            );
        }
        else
        {
            ConfirmerAchatSelection();
        }
    }

    private void ConfirmerAchatSelection()
    {
        ResolveDependencies();

        DATA_JOUEUR joueur = gameManager != null ? gameManager.GetHumanPlayer() : null;

        if (joueur == null || offreSelectionnee == null || offreSelectionnee.objet == null)
            return;

        SCOBJ_OBJET objetAchete = offreSelectionnee.objet;
        int quantite = Mathf.Max(1, quantiteAchat);

        bool success = SVC_BOUTIQUE_Service.Acheter(joueur, offreSelectionnee, quantite);
        if (!success)
        {
            RefreshAchatPanel();
            RefreshView();
            return;
        }

        gameManager.SynchroniserHudAvecJoueurHumain();
        gameManager.RefreshToutLeHUD();

        RefreshBoutiqueApresAchat(objetAchete);
    }

    private void RefreshBoutiqueApresAchat(SCOBJ_OBJET objetAReselectionner)
    {
        RefreshView();

        offreSelectionnee = TrouverOffrePourObjet(objetAReselectionner);

        if (offreSelectionnee != null)
            OuvrirDetailObjet(offreSelectionnee.objet);

        quantiteAchat = 1;

        RefreshAchatPanel();
        ForcerRebuildLayout();
    }

    private DATA_BOUTIQUE_Offre TrouverOffrePourObjet(SCOBJ_OBJET objet)
    {
        if (objet == null || catalogueActuel == null || catalogueActuel.offres == null)
            return null;

        foreach (DATA_BOUTIQUE_Offre offre in catalogueActuel.offres)
        {
            if (offre == null || offre.objet == null)
                continue;

            if (offre.objet == objet)
                return offre;

            if (!string.IsNullOrWhiteSpace(offre.objet.idUnique) &&
                !string.IsNullOrWhiteSpace(objet.idUnique) &&
                offre.objet.idUnique == objet.idUnique)
            {
                return offre;
            }

            if (offre.objet.name == objet.name)
                return offre;
        }

        return null;
    }

    private void ForcerRebuildLayout()
    {
        if (contentObjets == null)
            return;

        Canvas.ForceUpdateCanvases();

        RectTransform rect = contentObjets as RectTransform;
        if (rect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        Canvas.ForceUpdateCanvases();
    }
}