using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PROVINCE_MenuController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Navigation")]
    [SerializeField] private Button boutonFermer;

    [Header("Infos générales")]
    [SerializeField] private TMP_Text nomProvinceText;
    [SerializeField] private TMP_Text proprietaireText;
    [SerializeField] private TMP_Text claimText;
    [SerializeField] private TMP_Text explorationText;
    [SerializeField] private TMP_Text toursRestantsText;
    [SerializeField] private Image illustrationProvinceImage;

    [Header("Influences - Légende")]
    [SerializeField] private TMP_Text influenceMaizinText;
    [SerializeField] private TMP_Text influenceKiniaText;
    [SerializeField] private TMP_Text influenceJohoText;
    [SerializeField] private TMP_Text influenceAutreText;

    [Header("Population - Légende")]
    [SerializeField] private TMP_Text populationShikiText;
    [SerializeField] private TMP_Text populationFrisienText;
    [SerializeField] private TMP_Text populationAutreText;

    [Header("Exploration par compagnie")]
    [SerializeField] private Slider explorationMaizinSlider;
    [SerializeField] private Slider explorationKiniaSlider;
    [SerializeField] private Slider explorationJohoSlider;

    [SerializeField] private TMP_Text explorationMaizinText;
    [SerializeField] private TMP_Text explorationKiniaText;
    [SerializeField] private TMP_Text explorationJohoText;

    [Header("Camemberts")]
    [SerializeField] private PieChartUI influencePieChart;
    [SerializeField] private PieChartUI populationPieChart;

    [Header("Stats fixes")]
    [SerializeField] private TMP_Text etriniumText;
    [SerializeField] private TMP_Text prestigeText;
    [SerializeField] private TMP_Text poidsPolitiqueText;
    [SerializeField] private TMP_Text accessibiliteText;

    [Header("Couleurs Influence")]
    [SerializeField] private Color couleurMaizin = Color.blue;
    [SerializeField] private Color couleurKinia = new Color(0.6f, 0f, 1f, 1f);
    [SerializeField] private Color couleurJoho = Color.green;
    [SerializeField] private Color couleurAutre = Color.gray;

    [Header("Couleurs Population")]
    [SerializeField] private Color couleurShiki = new Color(0.8f, 0.4f, 0.9f);
    [SerializeField] private Color couleurFrisien = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private Color couleurPopulationAutre = Color.gray;

    private STATE_PROVINCE provinceActuelle;

    private void Awake()
    {
        if (boutonFermer != null)
            boutonFermer.onClick.AddListener(CloseMenu);

        CloseMenu();
    }

    private void OnDestroy()
    {
        if (boutonFermer != null)
            boutonFermer.onClick.RemoveListener(CloseMenu);
    }

    public void OpenProvinceMenu(STATE_PROVINCE province)
    {
        if (province == null)
        {
            Debug.LogWarning("OpenProvinceMenu : province est null.");
            return;
        }

        provinceActuelle = province;

        if (panelRoot != null)
            panelRoot.SetActive(true);
        else
            gameObject.SetActive(true);

        RefreshCurrentProvince();
    }

    public void RefreshCurrentProvince()
    {
        if (provinceActuelle == null)
            return;

        RefreshUI();
    }

    public void CloseMenu()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        if (panelRoot != null)
            return panelRoot.activeInHierarchy;

        return gameObject.activeInHierarchy;
    }

    private void RefreshUI()
    {
        SCOBJ_PROVINCE data = provinceActuelle.data;

        RefreshInfosGenerales(data);
        RefreshInfluenceChart();
        RefreshPopulationChart();
        RefreshExplorationSliders();
        RefreshStatsFixes(data);
    }

    private void RefreshInfosGenerales(SCOBJ_PROVINCE data)
    {
        if (nomProvinceText != null)
        {
            string nom = data != null && !string.IsNullOrWhiteSpace(data.nom)
                ? data.nom
                : "Province inconnue";

            nomProvinceText.text = $"Province : {nom}";
        }

        if (proprietaireText != null)
            proprietaireText.text = $"Propriétaire : {GetProprietaireTexte()}";

        if (claimText != null)
            claimText.text = $"Claim : {(provinceActuelle.estClaim ? "Oui" : "Non")}";

        if (explorationText != null)
            explorationText.text = "Exploration par compagnie";

        if (toursRestantsText != null)
        {
            string valeurTours = provinceActuelle.explorationEnCours
                ? provinceActuelle.toursRestants.ToString()
                : "-";

            toursRestantsText.text = $"Tours restants : {valeurTours}";
        }

        if (illustrationProvinceImage != null)
        {
            illustrationProvinceImage.sprite = data != null ? data.sprite : null;
            illustrationProvinceImage.enabled = illustrationProvinceImage.sprite != null;
            illustrationProvinceImage.preserveAspect = true;
        }
    }

    private void RefreshInfluenceChart()
    {
        float maizin = provinceActuelle.influenceMaizin;
        float kinia = provinceActuelle.influenceKinia;
        float joho = provinceActuelle.influenceJoho;
        float autre = provinceActuelle.influenceAutre;

        float total = maizin + kinia + joho + autre;

        if (influencePieChart != null)
        {
            influencePieChart.SetChart(new List<PieChartEntry>
            {
                new PieChartEntry("Maizin", maizin, couleurMaizin),
                new PieChartEntry("Kinia", kinia, couleurKinia),
                new PieChartEntry("Joho", joho, couleurJoho),
                new PieChartEntry("Autre", autre, couleurAutre)
            });
        }

        if (influenceMaizinText != null)
            influenceMaizinText.text = $"Influence Maizin : {FormatPourcentage(maizin, total)}";

        if (influenceKiniaText != null)
            influenceKiniaText.text = $"Influence Kinia : {FormatPourcentage(kinia, total)}";

        if (influenceJohoText != null)
            influenceJohoText.text = $"Influence Joho : {FormatPourcentage(joho, total)}";

        if (influenceAutreText != null)
            influenceAutreText.text = $"Influence Autre : {FormatPourcentage(autre, total)}";
    }

    private void RefreshPopulationChart()
    {
        SCOBJ_PROVINCE data = provinceActuelle.data;

        if (data == null)
        {
            if (populationPieChart != null)
                populationPieChart.ClearChart();

            if (populationShikiText != null)
                populationShikiText.text = "Peuple Shiki : -";

            if (populationFrisienText != null)
                populationFrisienText.text = "Peuple Frisien : -";

            if (populationAutreText != null)
                populationAutreText.text = "Autre : -";

            return;
        }

        float shiki = data.populationShiki;
        float frisien = data.populationFrisien;
        float autre = data.populationAutre;

        float total = shiki + frisien + autre;

        if (populationPieChart != null)
        {
            populationPieChart.SetChart(new List<PieChartEntry>
            {
                new PieChartEntry("Shiki", shiki, couleurShiki),
                new PieChartEntry("Frisien", frisien, couleurFrisien),
                new PieChartEntry("Autre", autre, couleurPopulationAutre)
            });
        }

        if (populationShikiText != null)
            populationShikiText.text = $"Peuple Shiki : {FormatPourcentage(shiki, total)}";

        if (populationFrisienText != null)
            populationFrisienText.text = $"Peuple Frisien : {FormatPourcentage(frisien, total)}";

        if (populationAutreText != null)
            populationAutreText.text = $"Autre : {FormatPourcentage(autre, total)}";
    }

    private void RefreshExplorationSliders()
    {
        float maizin = provinceActuelle.GetExploration(ENUM_Compagnie.Maizin);
        float kinia = provinceActuelle.GetExploration(ENUM_Compagnie.Kinia);
        float joho = provinceActuelle.GetExploration(ENUM_Compagnie.Joho);

        RefreshExplorationSlider(explorationMaizinSlider, maizin);
        RefreshExplorationSlider(explorationKiniaSlider, kinia);
        RefreshExplorationSlider(explorationJohoSlider, joho);

        if (explorationMaizinText != null)
            explorationMaizinText.text = $"Maizin : {maizin:0.#}%";

        if (explorationKiniaText != null)
            explorationKiniaText.text = $"Kinia : {kinia:0.#}%";

        if (explorationJohoText != null)
            explorationJohoText.text = $"Joho : {joho:0.#}%";
    }

    private void RefreshExplorationSlider(Slider slider, float value)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = Mathf.Clamp(value, 0f, 100f);
        slider.interactable = false;
    }

    private void RefreshStatsFixes(SCOBJ_PROVINCE data)
    {
        if (etriniumText != null)
            etriniumText.text = data != null ? $"Étrinium : {data.etrinium}" : "Étrinium : -";

        if (prestigeText != null)
            prestigeText.text = data != null ? $"Prestige : {data.prestige}" : "Prestige : -";

        if (poidsPolitiqueText != null)
            poidsPolitiqueText.text = data != null ? $"Poids politique : {data.poidsPolitique}" : "Poids politique : -";

        if (accessibiliteText != null)
            accessibiliteText.text = data != null ? $"Accessibilité : {data.accesibilite}" : "Accessibilité : -";
    }

    private string GetProprietaireTexte()
    {
        if (provinceActuelle == null || !provinceActuelle.proprietaireActuel.HasValue)
            return "Aucun";

        switch (provinceActuelle.proprietaireActuel.Value)
        {
            case ENUM_Compagnie.Maizin:
                return "Maizin";

            case ENUM_Compagnie.Kinia:
                return "Kinia";

            case ENUM_Compagnie.Joho:
                return "Joho";

            default:
                return "Aucun";
        }
    }

    private string FormatPourcentage(float value, float total)
    {
        if (total <= 0f)
            return "0%";

        float percent = value / total * 100f;
        return $"{percent:0.#}%";
    }
}