using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipeMenuController : MonoBehaviour
{
    private GameObject panelRoot;
    private Transform personnagesContent;
    private HudPersonnageSlot personnageSlotTemplate;
    private PersonnageMenuController personnageMenuController;
    private EquipeState equipeActuelle;
    private MapController mapController;
    private GameManager gameManager;

    [Header("UI Equipe")]
    [SerializeField] private Image portraitChef;
    [SerializeField] private TMP_Text nomEquipe;
    [SerializeField] private TMP_Text nomProvince;
    [SerializeField] private TMP_Text niveau;
    [SerializeField] private TMP_Text statutExploration;
    [SerializeField] private TMP_Text toursRestants;
    [SerializeField] private Button boutonAffecterProvince;

    private readonly List<HudPersonnageSlot> slotsInstancies = new();

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        if (boutonAffecterProvince != null)
        {
            boutonAffecterProvince.onClick.AddListener(AffecterEquipeAProvinceSelectionnee);
        }
        else
        {
            Debug.LogWarning("boutonAffecterProvince est null");
        }

        AutoBind();
        CloseMenu();
    }

    public bool IsOpen()
    {
        return panelRoot != null && panelRoot.activeInHierarchy;
    }

    private void AutoBind()
    {
        mapController = FindAnyObjectByType<MapController>();

        PanelRootTag panelTag = GetComponentInChildren<PanelRootTag>(true);
        if (panelTag != null)
        {
            panelRoot = panelTag.gameObject;
        }
        else
        {
            Debug.LogWarning($"PanelRootTag introuvable dans {name}");
        }

        PersonnageContentTag contentTag = GetComponentInChildren<PersonnageContentTag>(true);
        if (contentTag != null)
        {
            personnagesContent = contentTag.transform;
        }
        else
        {
            Debug.LogWarning($"PersonnageContentTag introuvable dans {name}");
        }

        PersonnageSlotPrefabTag slotTag = GetComponentInChildren<PersonnageSlotPrefabTag>(true);
        if (slotTag != null)
        {
            personnageSlotTemplate = slotTag.GetComponent<HudPersonnageSlot>();
        }
        else
        {
            Debug.LogWarning($"PersonnageSlotPrefabTag introuvable dans {name}");
        }

        personnageMenuController = FindAnyObjectByType<PersonnageMenuController>(FindObjectsInactive.Include);
        if (personnageMenuController == null)
        {
            Debug.LogWarning("Aucun PersonnageMenuController trouvé dans la scène.");
        }
    }

    private void AffecterEquipeAProvinceSelectionnee()
    {
        if (equipeActuelle == null)
        {
            Debug.LogWarning("Aucune équipe actuellement ouverte.");
            return;
        }

        if (mapController == null)
        {
            Debug.LogWarning("MapController introuvable.");
            return;
        }

        ProvinceState province = mapController.GetProvinceStateSelectionnee();

        if (province == null)
        {
            Debug.LogWarning("Aucune province sélectionnée.");
            return;
        }

        if (province.data == null)
        {
            Debug.LogError("province.data est null !");
            return;
        }

        equipeActuelle.provinceAffectee = province;

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        if (gameManager != null)
        {
            gameManager.DemarrerExploration(equipeActuelle, 3);
            gameManager.RefreshToutLeHUD();
        }
        else
        {
            equipeActuelle.explorationEnCours = true;
            equipeActuelle.toursTotaux = 3;
            equipeActuelle.toursRestants = 3;
            RefreshEquipeUI(equipeActuelle);
        }

        Debug.Log($"Équipe {equipeActuelle.data.nomEquipe} affectée à {province.data.nom}");
    }

    public void RefreshCurrentEquipe()
    {
        if (equipeActuelle == null)
        {
            return;
        }

        RefreshEquipeUI(equipeActuelle);
    }

    public void OpenEquipeMenu(EquipeState equipe)
    {
        equipeActuelle = equipe;

        if (equipe == null || equipe.data == null)
        {
            Debug.LogWarning("EquipeState ou EquipeData est null");
            return;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        RefreshEquipeUI(equipe);
        RefreshPersonnages(BuildPersonnagesHudData(equipe));
    }

    private void RefreshEquipeUI(EquipeState equipe)
    {
        if (equipe == null || equipe.data == null)
        {
            return;
        }

        if (portraitChef != null)
        {
            portraitChef.sprite = equipe.data.portraitChef;
            portraitChef.enabled = equipe.data.portraitChef != null;
        }

        if (nomEquipe != null)
        {
            nomEquipe.text = equipe.data.nomEquipe;
        }

        if (nomProvince != null)
        {
            nomProvince.text = equipe.provinceAffectee != null && equipe.provinceAffectee.data != null
                ? equipe.provinceAffectee.data.nom
                : "Aucune province";
        }

        if (niveau != null)
        {
            niveau.text = $"Lv. : {equipe.niveauActuel}";
        }

        if (statutExploration != null)
        {
            statutExploration.text = equipe.explorationEnCours
                ? "Exploration en cours"
                : (equipe.provinceAffectee == null ? "Non affectée" : "Affectée");
        }

        if (toursRestants != null)
        {
            toursRestants.text = equipe.explorationEnCours
                ? $"Tours restants : {equipe.toursRestants} / {equipe.toursTotaux}"
                : "Aucune exploration en cours";
        }
    }

   private List<PersonnageHudData> BuildPersonnagesHudData(EquipeState equipe)
{
    List<PersonnageHudData> result = new();

    if (equipe == null || equipe.data == null || equipe.data.membres == null)
    {
        return result;
    }

    foreach (PersonnageData personnage in equipe.data.membres)
    {
        PersonnageHudData hudData = PersonnageHudMapper.ToHudData(personnage);
        if (hudData != null)
        {
            result.Add(hudData);
        }
    }

    return result;
}

    public void RefreshPersonnages(List<PersonnageHudData> personnages)
    {
        ClearPersonnageSlots();

        if (personnagesContent == null)
        {
            Debug.LogWarning("personnagesContent est null");
            return;
        }

        if (personnageSlotTemplate == null)
        {
            Debug.LogWarning("personnageSlotTemplate est null");
            return;
        }

        foreach (PersonnageHudData personnage in personnages)
        {
            HudPersonnageSlot slot = Instantiate(personnageSlotTemplate, personnagesContent);
            slot.gameObject.SetActive(true);
            slot.SetMenuController(personnageMenuController);
            slot.Refresh(personnage);
            slotsInstancies.Add(slot);
        }
    }

    private void ClearPersonnageSlots()
    {
        foreach (HudPersonnageSlot slot in slotsInstancies)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }

        slotsInstancies.Clear();
    }

    public void CloseMenu()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }
}