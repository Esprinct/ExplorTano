using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [Header("HUD Dirigeant")]
    [SerializeField] private Image portraitDirigeantImage;
    [SerializeField] private Image logoCompagnieImage;
    [SerializeField] private TMP_Text nomDirigeantText;
    [SerializeField] private TMP_Text niveauDirigeantText;
    [SerializeField] private Button boutonMenuRecrutement;
    [SerializeField] private UI_Recrutement_MenuController UI_Recrutement_MenuController;
    [SerializeField] private Button boutonInventairePersonnages;
    [SerializeField] private UI_INVENTAIRE_Controller personnageUI_INVENTAIRE_Controller;
[SerializeField] private SCOBJ_DIRIGEANT dirigeantActuelRuntime;
[SerializeField] private UI_DIRIGEANT_HudSlot dirigeantHudSlot;
private DATA_JOUEUR_HUD joueurDataCourant;
    [Header("HUD Ressources")]
    [SerializeField] private TMP_Text etriniumText;
    [SerializeField] private TMP_Text prestigeText;
    [SerializeField] private TMP_Text provincesControleesText;
    [SerializeField] private TMP_Text positionJoueurText;
    [SerializeField] private EtriniumTooltipController etriniumTooltipController;

    [Header("HUD Tour")]
    [SerializeField] private Button boutonTourSuivant;
    [SerializeField] private TMP_Text tourText;

    [Header("HUD Equipes")]
    [SerializeField] private Transform equipesContainer;
    [SerializeField] private HUD_EQUIPE_Slot hudEquipeSlotPrefab;
    [SerializeField] private Button boutonAjouterEquipe;
    [SerializeField] private TMP_Text boutonAjouterEquipeText;
    [SerializeField] private UI_EQUIPE_DetailController UI_EQUIPE_DetailController;
[Header("HUD Dirigeant")]
[SerializeField] private UI_DIRIGEANT_DetailController dirigeantDetailController;

    [Header("Confirmation création équipe")]
    [SerializeField] private UI_ConfirmationDialog confirmationDialog;

    [Header("Notification recrutement")]
    [SerializeField] private GameObject panelNotificationRecrutement;
    [SerializeField] private Transform notificationRecrutementContent;
    [SerializeField] private UI_Recrutement_NotificationItem notificationRecrutementItemPrefab;
    [SerializeField] private Button boutonFermerNotificationRecrutement;
    [Header("Boutique")]
[SerializeField] private Button boutonBoutique;
[SerializeField] private UI_BOUTIQUE_Controller boutiqueController;

    [Header("Données de test (optionnel)")]
    [SerializeField] private bool utiliserDonneesDeTestAuStart = false;
    [SerializeField] private DATA_JOUEUR_HUD joueurTest;
    [SerializeField] private DATA_Partie_Hud_Tour partieTest;
    [SerializeField] private List<DATA_EQUIPE_DetailData> equipesTest = new();

    private readonly List<HUD_EQUIPE_Slot> equipeSlots = new();
    private readonly List<UI_Recrutement_NotificationItem> notificationsInstanciees = new();

    private SYS_GameManager gameManager;

    private void Awake()
    {
        ResolveDependencies();
   AutoBindDirigeantUi();
   Debug.Log(
    $"HudController refs | " +
    $"portrait={(portraitDirigeantImage != null)} | " +
    $"logo={(logoCompagnieImage != null)} | " +
    $"nom={(nomDirigeantText != null)} | " +
    $"niveau={(niveauDirigeantText != null)}"
);
        if (boutonTourSuivant != null)
            boutonTourSuivant.onClick.AddListener(OnBoutonTourSuivantClicked);

        if (boutonMenuRecrutement != null)
            boutonMenuRecrutement.onClick.AddListener(OnBoutonMenuRecrutementClicked);

        if (boutonInventairePersonnages != null)
            boutonInventairePersonnages.onClick.AddListener(OnBoutonInventairePersonnagesClicked);

        if (boutonAjouterEquipe != null)
            boutonAjouterEquipe.onClick.AddListener(OnBoutonAjouterEquipeClicked);

        if (boutonFermerNotificationRecrutement != null)
            boutonFermerNotificationRecrutement.onClick.AddListener(HideNotificationRecrutement);
       if (boutonBoutique != null)
    boutonBoutique.onClick.AddListener(OnBoutonBoutiqueClicked);

        if (panelNotificationRecrutement != null)
            panelNotificationRecrutement.SetActive(false);

        if (confirmationDialog != null)
            confirmationDialog.Close();
    }

    private void Start()
    {
        ResolveDependencies();

        if (gameManager == null && utiliserDonneesDeTestAuStart)
        {
            RefreshAll(joueurTest, partieTest, equipesTest);
        }
    }

    private void OnDestroy()
    {
        if (boutonTourSuivant != null)
            boutonTourSuivant.onClick.RemoveListener(OnBoutonTourSuivantClicked);

        if (boutonMenuRecrutement != null)
            boutonMenuRecrutement.onClick.RemoveListener(OnBoutonMenuRecrutementClicked);

        if (boutonInventairePersonnages != null)
            boutonInventairePersonnages.onClick.RemoveListener(OnBoutonInventairePersonnagesClicked);

        if (boutonAjouterEquipe != null)
            boutonAjouterEquipe.onClick.RemoveListener(OnBoutonAjouterEquipeClicked);

        if (boutonFermerNotificationRecrutement != null)
            boutonFermerNotificationRecrutement.onClick.RemoveListener(HideNotificationRecrutement);
       if (boutonBoutique != null)
    boutonBoutique.onClick.RemoveListener(OnBoutonBoutiqueClicked);
    }
private void OnBoutonBoutiqueClicked()
{
    if (boutiqueController == null)
        boutiqueController = FindAnyObjectByType<UI_BOUTIQUE_Controller>(FindObjectsInactive.Include);

    if (boutiqueController == null)
    {
        Debug.LogWarning("Aucun UI_BOUTIQUE_Controller trouvé.");
        return;
    }

    boutiqueController.OpenMenu();
}
    private void ResolveDependencies()
    {
        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        if (UI_EQUIPE_DetailController == null)
            UI_EQUIPE_DetailController = FindAnyObjectByType<UI_EQUIPE_DetailController>(FindObjectsInactive.Include);

        if (personnageUI_INVENTAIRE_Controller == null)
            personnageUI_INVENTAIRE_Controller = FindAnyObjectByType<UI_INVENTAIRE_Controller>(FindObjectsInactive.Include);

        if (UI_Recrutement_MenuController == null)
            UI_Recrutement_MenuController = FindAnyObjectByType<UI_Recrutement_MenuController>(FindObjectsInactive.Include);
  if (dirigeantHudSlot == null)
    {
        DirigeantHudSlotTag tag =
            FindAnyObjectByType<DirigeantHudSlotTag>(FindObjectsInactive.Include);

        if (tag != null)
            dirigeantHudSlot = tag.GetComponent<UI_DIRIGEANT_HudSlot>();
    }

    if (dirigeantDetailController == null)
    {
        dirigeantDetailController =
            FindAnyObjectByType<UI_DIRIGEANT_DetailController>(FindObjectsInactive.Include);
    }
    }

   public void RefreshAll(DATA_JOUEUR_HUD joueurData, DATA_Partie_Hud_Tour partieData, List<DATA_EQUIPE_DetailData> equipesData)
{
    RefreshDirigeant(joueurData);
    RefreshRessources(joueurData);
    RefreshTour(partieData);
    RefreshEquipes(equipesData);
    RefreshBoutonAjouterEquipe();
}

public void RefreshDirigeant(DATA_JOUEUR_HUD joueurData)
{
    if (joueurData == null)
        return;

    joueurDataCourant = joueurData;
    dirigeantActuelRuntime = joueurData.dirigeant;

    Debug.Log(
        $"[HUD REFRESH DIRIGEANT] this={name} | " +
        $"joueurData null ? {joueurData == null} | " +
        $"dirigeant runtime null ? {dirigeantActuelRuntime == null} | " +
        $"nom={(dirigeantActuelRuntime != null ? dirigeantActuelRuntime.nomDirigeant : "null")}"
    );

    if (portraitDirigeantImage != null)
    {
        portraitDirigeantImage.sprite = joueurData.portraitDirigeant;
        portraitDirigeantImage.enabled = joueurData.portraitDirigeant != null;
    }

    if (logoCompagnieImage != null)
    {
        logoCompagnieImage.sprite = joueurData.logoCompagnie;
        logoCompagnieImage.enabled = joueurData.logoCompagnie != null;
    }

    if (nomDirigeantText != null)
    {
        nomDirigeantText.text = joueurData.nomDirigeant;
    }

    if (niveauDirigeantText != null)
    {
        niveauDirigeantText.text = $"{joueurData.niveauDirigeant}";
    }

    if (dirigeantHudSlot != null)
    {
        dirigeantHudSlot.Refresh(dirigeantActuelRuntime);
    }
}
private void AutoBindDirigeantUi()
{
    Transform hudDirigeant = FindChildRecursive(transform, "HUD_Dirigant");
    if (hudDirigeant == null)
    {
        Debug.LogWarning("AutoBindDirigeantUi : HUD_Dirigant introuvable.");
        return;
    }

    if (portraitDirigeantImage == null)
    {
        Transform t = FindChildRecursive(hudDirigeant, "portraitDirigeantImage");
        if (t != null)
            portraitDirigeantImage = t.GetComponent<Image>();
    }

    if (logoCompagnieImage == null)
    {
        Transform t = FindChildRecursive(hudDirigeant, "logoCompagnieImage");
        if (t != null)
            logoCompagnieImage = t.GetComponent<Image>();
    }

    if (nomDirigeantText == null)
    {
        Transform t = FindChildRecursive(hudDirigeant, "nomDirigeantText");
        if (t != null)
            nomDirigeantText = t.GetComponent<TMP_Text>();
    }

    if (niveauDirigeantText == null)
    {
        Transform t = FindChildRecursive(hudDirigeant, "niveauDirigeantText");
        if (t != null)
            niveauDirigeantText = t.GetComponent<TMP_Text>();
    }
}

private Transform FindChildRecursive(Transform parent, string childName)
{
    if (parent == null)
        return null;

    for (int i = 0; i < parent.childCount; i++)
    {
        Transform child = parent.GetChild(i);

        if (child.name == childName)
            return child;

        Transform result = FindChildRecursive(child, childName);
        if (result != null)
            return result;
    }

    return null;
}

    public void RefreshRessources(DATA_JOUEUR_HUD joueurData)
    {
        if (joueurData == null)
            return;

        if (etriniumText != null)
            etriniumText.text = $"{joueurData.etriniumTotal} (+{joueurData.etriniumParTour})";

        if (prestigeText != null)
            prestigeText.text = $"{joueurData.prestige}";

        if (provincesControleesText != null)
            provincesControleesText.text = $"{joueurData.provincesControlees}";

        if (positionJoueurText != null)
            positionJoueurText.text = joueurData.positionTexte;
    }

    public void RefreshTour(DATA_Partie_Hud_Tour partieData)
    {
        if (partieData == null)
            return;

        if (tourText != null)
            tourText.text = $"{partieData.tourActuel} / {partieData.tourMax}";
    }

    public void RefreshEquipes(List<DATA_EQUIPE_DetailData> equipesData)
    {
        EnsureEquipeSlots(equipesData != null ? equipesData.Count : 0);

        for (int i = 0; i < equipeSlots.Count; i++)
        {
            if (equipesData != null && i < equipesData.Count && equipesData[i] != null)
            {
                equipeSlots[i].Refresh(equipesData[i]);
            }
            else
            {
                equipeSlots[i].Hide();
            }
        }
    }

public SCOBJ_DIRIGEANT GetDirigeantActuel()
{
    Debug.Log(
        $"[GET DIRIGEANT ACTUEL] this={name} | " +
        $"dirigeantActuelRuntime null ? {dirigeantActuelRuntime == null} | " +
        $"nom={(dirigeantActuelRuntime != null ? dirigeantActuelRuntime.nomDirigeant : "null")}"
    );

    return dirigeantActuelRuntime;
}
    private void EnsureEquipeSlots(int count)
    {
        if (equipesContainer == null || hudEquipeSlotPrefab == null)
            return;

        while (equipeSlots.Count < count)
        {
            HUD_EQUIPE_Slot slot = Instantiate(hudEquipeSlotPrefab, equipesContainer);
            slot.gameObject.SetActive(true);
            slot.SetDetailController(UI_EQUIPE_DetailController);
            equipeSlots.Add(slot);
        }
    }

  private void RefreshBoutonAjouterEquipe()
{
    if (boutonAjouterEquipe == null)
        return;

    ResolveDependencies();

    if (gameManager == null)
    {
        boutonAjouterEquipe.gameObject.SetActive(false);
        return;
    }

    DATA_JOUEUR humain = gameManager.GetHumanPlayer();
    if (humain == null)
    {
        boutonAjouterEquipe.gameObject.SetActive(false);
        return;
    }

    int nbEquipes = gameManager.GetNombreEquipesJoueur(humain);
    bool limiteActive = gameManager.MaxEquipesParJoueur > 0;
    bool maxAtteint = limiteActive && nbEquipes >= gameManager.MaxEquipesParJoueur;

    if (maxAtteint)
    {
        boutonAjouterEquipe.gameObject.SetActive(false);
        return;
    }

    boutonAjouterEquipe.gameObject.SetActive(true);

    bool peutCreer = gameManager.PeutCreerEquipe(humain);
    boutonAjouterEquipe.interactable = peutCreer;

    int coutActuel = gameManager.GetCoutCreationEquipe(humain);

    if (boutonAjouterEquipeText != null)
    {
        boutonAjouterEquipeText.text =
            $"Créer équipe ({coutActuel})\nnombre d'équipes :{nbEquipes}";
    }
}

    public void RefreshFromGameManager()
    {
        ResolveDependencies();

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager introuvable.");
            return;
        }

        gameManager.RefreshToutLeHUD();
    }

    private void OnBoutonTourSuivantClicked()
    {
        ResolveDependencies();

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager introuvable.");
            return;
        }

        gameManager.TourSuivant();
    }

    private void OnBoutonMenuRecrutementClicked()
    {
        ResolveDependencies();

        if (UI_Recrutement_MenuController == null)
        {
            Debug.LogWarning("UI_Recrutement_MenuController introuvable.");
            return;
        }

        if (gameManager == null || gameManager.SYS_RecrutementSystem == null)
        {
            Debug.LogWarning("SYS_RecrutementSystem introuvable.");
            return;
        }

        UI_Recrutement_MenuController.OpenMenu(
            new List<SCOBJ_Personnage>(gameManager.SYS_RecrutementSystem.MarcheCourant)
        );
    }

    private void OnBoutonInventairePersonnagesClicked()
    {
        ResolveDependencies();

        if (personnageUI_INVENTAIRE_Controller == null)
        {
            Debug.LogWarning("UI_INVENTAIRE_Controller introuvable");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager introuvable.");
            return;
        }

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();

        if (humain == null)
        {
            Debug.LogWarning("Joueur humain introuvable");
            return;
        }

        List<SCOBJ_Personnage> personnages = humain.personnagesRecrutes ?? new List<SCOBJ_Personnage>();
        List<SCOBJ_OBJET> objets = humain.objetsPossedes ?? new List<SCOBJ_OBJET>();
        List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack> consommables = humain.consommablesPossedes ?? new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();

        DATA_PERSONNAGE_DisplayContext contexte = new(humain.compagnie);

        personnageUI_INVENTAIRE_Controller.OpenMenu(
            personnages,
            objets,
            contexte,
            consommables
        );
    }

  private void OnBoutonAjouterEquipeClicked()
{
    ResolveDependencies();

    if (gameManager == null)
    {
        Debug.LogWarning("GameManager introuvable.");
        return;
    }

    DATA_JOUEUR humain = gameManager.GetHumanPlayer();
    if (humain == null)
    {
        Debug.LogWarning("Aucun joueur humain trouvé.");
        return;
    }

    int nbEquipes = gameManager.GetNombreEquipesJoueur(humain);
    bool limiteActive = gameManager.MaxEquipesParJoueur > 0;
    bool maxAtteint = limiteActive && nbEquipes >= gameManager.MaxEquipesParJoueur;

    if (maxAtteint)
    {
        Debug.Log("Nombre maximum d'équipes atteint.");
        return;
    }

    bool succes = gameManager.CreerEquipePourJoueurHumain();

    if (!succes)
    {
        Debug.Log("Impossible de créer l'équipe.");
        return;
    }

    RefreshFromGameManager();
}

    private void ConfirmerCreationEquipe()
    {
        ResolveDependencies();

        if (gameManager == null)
            return;

        bool succes = gameManager.CreerEquipePourJoueurHumain();

        if (!succes)
        {
            Debug.LogWarning("Création d'équipe impossible.");
            RefreshBoutonAjouterEquipe();
            return;
        }

        RefreshFromGameManager();
    }

    public void ShowEtriniumTooltip()
    {
        ResolveDependencies();

        if (gameManager == null || etriniumTooltipController == null)
            return;

        etriniumTooltipController.Show(gameManager.JoueurData.etriniumBreakdown);
    }

    public void HideEtriniumTooltip()
    {
        etriniumTooltipController?.Hide();
    }

    public void ShowNotificationRecrutement(DATA_RecrutementResolutionResult resultat)
    {
        ClearNotificationRecrutementItems();

        if (resultat == null || !resultat.ADesNotifications())
            return;

        if (notificationRecrutementContent == null || notificationRecrutementItemPrefab == null)
        {
            Debug.LogWarning("Notification recrutement content/prefab non assigné.");
            return;
        }

        foreach (DATA_RecrutementNotificationItem notification in resultat.notifications)
        {
            if (notification == null)
                continue;

            UI_Recrutement_NotificationItem item =
                Instantiate(notificationRecrutementItemPrefab, notificationRecrutementContent);

            item.Refresh(notification);
            notificationsInstanciees.Add(item);
        }

        if (panelNotificationRecrutement != null)
            panelNotificationRecrutement.SetActive(true);
    }

    public void HideNotificationRecrutement()
    {
        if (panelNotificationRecrutement != null)
            panelNotificationRecrutement.SetActive(false);

        ClearNotificationRecrutementItems();
    }

    private void ClearNotificationRecrutementItems()
    {
        foreach (UI_Recrutement_NotificationItem item in notificationsInstanciees)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        notificationsInstanciees.Clear();
    }
}