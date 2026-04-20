using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DIRIGEANT_DetailController : UTIL_UiPanelControllerBase
{
    [Header("Header")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Image logoCompagnieImage;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private TMP_Text descriptionText;
[SerializeField] private Slider xpSlider;
[SerializeField] private TMP_Text xpText;
    [Header("Effets")]
    [SerializeField] private Transform effetsContainer;
    [SerializeField] private UI_EFFET_Slot effetSlotPrefab;

    [Header("Boutons")]
    [SerializeField] private Button boutonFermer;

    private readonly List<UI_EFFET_Slot> slotsEffets = new();
    private SCOBJ_DIRIGEANT dirigeantActuel;

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
        }
    }

    public void OpenDirigeantMenu(SCOBJ_DIRIGEANT dirigeant)
    {
        Debug.Log($"[OPEN DIRIGEANT MENU] {dirigeant?.nomDirigeant}");

        if (dirigeant == null)
            return;

        dirigeantActuel = dirigeant;
        RefreshView();
        OpenPanel();
    }

    public void RefreshCurrentDirigeant()
    {
        if (dirigeantActuel == null)
            return;

        RefreshView();
    }

    public void CloseMenu()
    {
        ClosePanel();
    }

    private void RefreshView()
    {
        if (dirigeantActuel == null)
            return;

        if (portraitImage != null)
        {
            portraitImage.sprite = dirigeantActuel.portraitDirigeant;
            portraitImage.enabled = dirigeantActuel.portraitDirigeant != null;
        }

        if (logoCompagnieImage != null)
        {
            logoCompagnieImage.sprite = dirigeantActuel.logoCompagnie;
            logoCompagnieImage.enabled = dirigeantActuel.logoCompagnie != null;
        }

        if (nomText != null)
            nomText.text = dirigeantActuel.nomDirigeant;

        if (niveauText != null)
            niveauText.text = $"Niveau {dirigeantActuel.niveauDirigeant}";

        if (descriptionText != null)
            descriptionText.text = dirigeantActuel.description;
if (xpSlider != null)
{
    xpSlider.minValue = 0f;
    xpSlider.maxValue = dirigeantActuel != null ? dirigeantActuel.xpPourNiveauSuivant : 1f;
    xpSlider.value = dirigeantActuel != null ? dirigeantActuel.xpDirigeant : 0f;
}

if (xpText != null)
{
    xpText.text = dirigeantActuel != null
        ? $"{dirigeantActuel.xpDirigeant} / {dirigeantActuel.xpPourNiveauSuivant}"
        : "0 / 0";
}
        RefreshEffets();
    }

    private void RefreshEffets()
    {
        ClearEffets();

        if (effetsContainer == null || effetSlotPrefab == null)
            return;

        if (dirigeantActuel == null || dirigeantActuel.effets == null)
            return;

        foreach (SCOBJ_DIRIGEANT_EFFET effet in dirigeantActuel.effets)
        {
            if (effet == null)
                continue;

            bool actif = dirigeantActuel.niveauDirigeant >= effet.niveauRequis;

            UI_EFFET_Slot slot = Instantiate(effetSlotPrefab, effetsContainer);
            slot.gameObject.SetActive(true);
            slot.Setup(effet);

            if (!actif)
            {
                CanvasGroup cg = slot.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = slot.gameObject.AddComponent<CanvasGroup>();

                cg.alpha = 0.45f;
            }

            slotsEffets.Add(slot);
        }
    }

    private void ClearEffets()
    {
        foreach (UI_EFFET_Slot slot in slotsEffets)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slotsEffets.Clear();
    }
}