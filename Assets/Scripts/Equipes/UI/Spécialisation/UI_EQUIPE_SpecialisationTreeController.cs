using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EQUIPE_SpecialisationTreeController : UTIL_UiPanelControllerBase
{
    [Header("Références")]
    [SerializeField] private SYS_GameManager gameManager;
[Header("Couleurs conditions")]
[SerializeField] private Color couleurConditionValide = new Color(0.2f, 0.75f, 0.2f);
[SerializeField] private Color couleurConditionInvalide = new Color(0.85f, 0.2f, 0.2f);
    [Header("Slots Tier 1")]
    [SerializeField] private List<UI_EQUIPE_SpecialisationNode> tier1Slots = new();

    [Header("Slots Tier 2")]
    [SerializeField] private List<UI_EQUIPE_SpecialisationNode> tier2Slots = new();

    [Header("Slots Tier 3")]
    [SerializeField] private List<UI_EQUIPE_SpecialisationNode> tier3Slots = new();

    [Header("Détails sélection")]
    [SerializeField] private TMP_Text titreSelectionText;
    [SerializeField] private TMP_Text descriptionSelectionText;
    [SerializeField] private TMP_Text conditionSelectionText;
    [SerializeField] private TMP_Text bonusSelectionText;

    [Header("Boutons")]
    [SerializeField] private Button boutonChoisir;
    [SerializeField] private Button boutonFermer;

    [Header("Sources")]
    [SerializeField] private List<SCOBJ_EQUIPE_SPECIALISATION> specialisations = new();

    private STATE_EQUIPE equipeActuelle;
    private SCOBJ_EQUIPE_SPECIALISATION specialisationSelectionnee;

    private void Awake()
    {
        AutoBind();

        if (gameManager == null)
            gameManager = FindAnyObjectByType<SYS_GameManager>();

        if (boutonChoisir != null)
            boutonChoisir.onClick.AddListener(ConfirmerChoix);

        if (boutonFermer != null)
            boutonFermer.onClick.AddListener(Close);

        ClosePanel();
    }

    private void OnDestroy()
    {
        if (boutonChoisir != null)
            boutonChoisir.onClick.RemoveListener(ConfirmerChoix);

        if (boutonFermer != null)
            boutonFermer.onClick.RemoveListener(Close);
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

    public void Open(STATE_EQUIPE equipe)
    {
        equipeActuelle = equipe;
        specialisationSelectionnee = GetCurrentSpecialisationAsset(equipe);

        
        OpenPanel();
        RefreshSelectionPanel();
        RefreshTree();
    }

    public void Close()
    {
        ClosePanel();
    }

    public bool HasAnyAvailableChoice(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return false;

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in specialisations)
        {
            if (specialisation == null)
                continue;

            if (SVC_EQUIPE_SpecialisationService.PeutChoisirSpecialisation(equipe, specialisation))
                return true;
        }

        return false;
    }
private SCOBJ_EQUIPE_SPECIALISATION TrouverSpecialisationActuelle()
{
    if (equipeActuelle == null || specialisations == null)
        return null;

    foreach (SCOBJ_EQUIPE_SPECIALISATION spec in specialisations)
    {
        if (spec == null)
            continue;

        if (spec.type == equipeActuelle.specialisation)
            return spec;
    }

    return null;
}
    public void RefreshTree()
    {
        if (equipeActuelle == null)
        {
            HideAllSlots(tier1Slots);
            HideAllSlots(tier2Slots);
            HideAllSlots(tier3Slots);
            return;
        }

        PopulateTier(tier1Slots, GetByTier(ENUM_EQUIPE_TIER.Tier1));
        PopulateTier(tier2Slots, GetByTier(ENUM_EQUIPE_TIER.Tier2));
        PopulateTier(tier3Slots, GetByTier(ENUM_EQUIPE_TIER.Tier3));
    }

    private List<SCOBJ_EQUIPE_SPECIALISATION> GetByTier(ENUM_EQUIPE_TIER tier)
    {
        List<SCOBJ_EQUIPE_SPECIALISATION> result = new();

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in specialisations)
        {
            if (specialisation == null)
                continue;

            if (specialisation.tier == tier)
                result.Add(specialisation);
        }

        return result;
    }

    private void PopulateTier(
        List<UI_EQUIPE_SpecialisationNode> slots,
        List<SCOBJ_EQUIPE_SPECIALISATION> dataList)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Count; i++)
        {
            UI_EQUIPE_SpecialisationNode slot = slots[i];

            if (slot == null)
                continue;

            if (dataList == null || i >= dataList.Count)
            {
                slot.gameObject.SetActive(false);
                continue;
            }

            SCOBJ_EQUIPE_SPECIALISATION data = dataList[i];

            bool estActuelle =
                equipeActuelle != null &&
                equipeActuelle.specialisation == data.type;

            bool estDebloquee =
                equipeActuelle != null &&
                SVC_EQUIPE_SpecialisationService.EstSpecialisationDejaDebloquee(
                    equipeActuelle,
                    data,
                    specialisations
                );

            bool estDisponibleMaintenant =
                equipeActuelle != null &&
                SVC_EQUIPE_SpecialisationService.PeutChoisirSpecialisation(
                    equipeActuelle,
                    data
                );

            bool estSelectionnee = specialisationSelectionnee == data;

            slot.gameObject.SetActive(true);
            slot.Setup(
                data,
                BuildBonusText(data),
                BuildConditionText(equipeActuelle, data, estDebloquee, estDisponibleMaintenant),
                estActuelle,
                estDebloquee,
                estDisponibleMaintenant,
                estSelectionnee,
                OnNodeClicked
            );
        }
    }

    private void HideAllSlots(List<UI_EQUIPE_SpecialisationNode> slots)
    {
        if (slots == null)
            return;

        foreach (UI_EQUIPE_SpecialisationNode slot in slots)
        {
            if (slot != null)
                slot.gameObject.SetActive(false);
        }
    }

    private void OnNodeClicked(SCOBJ_EQUIPE_SPECIALISATION specialisation)
    {
        specialisationSelectionnee = specialisation;

        RefreshSelectionOnAllSlots(tier1Slots);
        RefreshSelectionOnAllSlots(tier2Slots);
        RefreshSelectionOnAllSlots(tier3Slots);

        RefreshSelectionPanel();
    }

    private void RefreshSelectionOnAllSlots(List<UI_EQUIPE_SpecialisationNode> slots)
    {
        if (slots == null)
            return;

        foreach (UI_EQUIPE_SpecialisationNode slot in slots)
        {
            if (slot == null || !slot.gameObject.activeSelf)
                continue;

            slot.SetSelected(slot.GetSpecialisation() == specialisationSelectionnee);
        }
    }

    private void RefreshSelectionPanel()
    {
        if (titreSelectionText != null)
        {
            titreSelectionText.text = specialisationSelectionnee != null
                ? specialisationSelectionnee.nomAffiche
                : "Aucune sélection";
        }

        if (descriptionSelectionText != null)
        {
            descriptionSelectionText.text = specialisationSelectionnee != null
                ? specialisationSelectionnee.description
                : "";
        }

        bool estDebloquee = equipeActuelle != null &&
                            specialisationSelectionnee != null &&
                            SVC_EQUIPE_SpecialisationService.EstSpecialisationDejaDebloquee(
                                equipeActuelle,
                                specialisationSelectionnee,
                                specialisations
                            );

        bool estDisponibleMaintenant = equipeActuelle != null &&
                                       specialisationSelectionnee != null &&
                                       SVC_EQUIPE_SpecialisationService.PeutChoisirSpecialisation(
                                           equipeActuelle,
                                           specialisationSelectionnee
                                       );

 if (conditionSelectionText != null)
{
    bool estActuelle = equipeActuelle != null &&
                       specialisationSelectionnee != null &&
                       equipeActuelle.specialisation == specialisationSelectionnee.type;

    bool conditionRespectee = estActuelle || estDebloquee || estDisponibleMaintenant;

    conditionSelectionText.text = specialisationSelectionnee != null
        ? BuildConditionText(equipeActuelle, specialisationSelectionnee, estDebloquee, estDisponibleMaintenant)
        : "";

    conditionSelectionText.color = conditionRespectee
        ? couleurConditionValide
        : couleurConditionInvalide;
}

        if (bonusSelectionText != null)
        {
            bonusSelectionText.text = specialisationSelectionnee != null
                ? BuildBonusText(specialisationSelectionnee)
                : "";
        }

        if (boutonChoisir != null)
        {
            bool peutChoisir =
                equipeActuelle != null &&
                specialisationSelectionnee != null &&
                SVC_EQUIPE_SpecialisationService.PeutChoisirSpecialisation(
                    equipeActuelle,
                    specialisationSelectionnee
                );

            boutonChoisir.interactable = peutChoisir;
        }
    }

   public void ConfirmerChoix()
{
    if (equipeActuelle == null)
        return;

    if (specialisationSelectionnee == null)
    {
        Debug.LogWarning("Aucune spécialisation sélectionnée.");
        return;
    }

    SCOBJ_EQUIPE_SPECIALISATION cible = specialisationSelectionnee;

    bool success = SVC_EQUIPE_SpecialisationService.AppliquerSpecialisation(equipeActuelle, cible);

    if (!success)
    {
        Debug.LogWarning($"Choix refusé : {cible.nomAffiche}");
        return;
    }

    Debug.Log(
        $"[SPECIALISATION_APPLIQUEE] equipe={equipeActuelle.data?.nomEquipe} | " +
        $"cible={cible.nomAffiche} | type={cible.type}"
    );

    Close();

    if (gameManager != null)
        gameManager.RefreshToutLeHUD();
}

    private string BuildConditionText(
        STATE_EQUIPE equipe,
        SCOBJ_EQUIPE_SPECIALISATION specialisation,
        bool estDebloquee,
        bool estDisponibleMaintenant)
    {
        if (equipe == null || specialisation == null)
            return "";

        if (equipe.specialisation == specialisation.type)
            return "Spécialisation actuelle";

        if (estDisponibleMaintenant)
            return "Disponible";

        if (estDebloquee)
            return "Déjà débloquée";

        if (equipe.NiveauActuel < specialisation.niveauMinimum)
            return $"Niveau {specialisation.niveauMinimum} requis";

        if (equipe.specialisation == ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
        {
            if (specialisation.specialisationParent != ENUM_EQUIPE_SPECIALISATION.Reconnaissance)
                return $"Requiert {specialisation.specialisationParent}";
        }
        else
        {
            if (specialisation.specialisationParent != equipe.specialisation)
                return $"Requiert {specialisation.specialisationParent}";
        }

        return "Verrouillée";
    }

    private string BuildBonusText(SCOBJ_EQUIPE_SPECIALISATION specialisation)
    {
        if (specialisation == null || specialisation.effets == null)
            return "";

        List<string> lignes = new();

        foreach (SCOBJ_EQUIPE_EFFET effet in specialisation.effets)
        {
            if (effet == null)
                continue;

            string ligne = FMT_EFFET.BuildValeurAfficheeLongue(effet);
            if (!string.IsNullOrWhiteSpace(ligne))
                lignes.Add(ligne);
        }

        return string.Join("\n", lignes);
    }

    private SCOBJ_EQUIPE_SPECIALISATION GetCurrentSpecialisationAsset(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return null;

        if (equipe.dataSpecialisation != null)
            return equipe.dataSpecialisation;

        foreach (SCOBJ_EQUIPE_SPECIALISATION specialisation in specialisations)
        {
            if (specialisation == null)
                continue;

            if (specialisation.type == equipe.specialisation)
                return specialisation;
        }

        return null;
    }
}