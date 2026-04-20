using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PROVINCE_MenuController : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject panelRoot;

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
    [SerializeField] private Color couleurKinia = Color.purple;
    [SerializeField] private Color couleurJoho = Color.green;
    [SerializeField] private Color couleurAutre = Color.gray;

    [Header("Couleurs Population")]
    [SerializeField] private Color couleurShiki = new Color(0.8f, 0.4f, 0.9f);
    [SerializeField] private Color couleurFrisien = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private Color couleurPopulationAutre = Color.gray;

    private STATE_PROVINCE provinceActuelle;

    private void Awake()
    {
        CloseMenu();
    }

    public void OpenProvinceMenu(STATE_PROVINCE province)
    {
        provinceActuelle = province;

        if (provinceActuelle == null)
        {
            Debug.LogWarning("OpenProvinceMenu : provinceActuelle est null");
            return;
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

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
        {
            panelRoot.SetActive(false);
        }
    }

    public bool IsOpen()
    {
        return panelRoot != null && panelRoot.activeInHierarchy;
    }

    private void RefreshUI()
    {
        SCOBJ_PROVINCE data = provinceActuelle.data;

        if (nomProvinceText != null)
        {
            nomProvinceText.text = $"Province : {(data != null ? data.nom : "Province inconnue")}";
        }

        if (proprietaireText != null)
        {
            proprietaireText.text = $"Propriétaire : {GetProprietaireTexte()}";
        }

        if (claimText != null)
        {
            claimText.text = $"Claim : {(provinceActuelle.estClaim ? "Oui" : "Non")}";
        }

        if (explorationText != null)
        {
            explorationText.text = $"Exploration : {(provinceActuelle.explorationEnCours ? "En cours" : "Aucune")}";
        }

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
        }

        RefreshInfluenceChart();
        RefreshPopulationChart();
        RefreshStatsFixes();
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
        {
            influenceMaizinText.text = $"Influence Maizin : {FormatPourcentage(maizin, total)}";
        }

        if (influenceKiniaText != null)
        {
            influenceKiniaText.text = $"Influence Kinia : {FormatPourcentage(kinia, total)}";
        }

        if (influenceJohoText != null)
        {
            influenceJohoText.text = $"Influence Joho : {FormatPourcentage(joho, total)}";
        }

        if (influenceAutreText != null)
        {
            influenceAutreText.text = $"Influence Autre : {FormatPourcentage(autre, total)}";
        }
    }

    private void RefreshPopulationChart()
    {
        if (provinceActuelle.data == null)
        {
            if (populationPieChart != null)
            {
                populationPieChart.ClearChart();
            }

            return;
        }

        float shiki = provinceActuelle.data.populationShiki;
        float frisien = provinceActuelle.data.populationFrisien;
        float autre = provinceActuelle.data.populationAutre;

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
        {
            populationShikiText.text = $"Peuple Shiki : {FormatPourcentage(shiki, total)}";
        }

        if (populationFrisienText != null)
        {
            populationFrisienText.text = $"Peuple Frisien : {FormatPourcentage(frisien, total)}";
        }

        if (populationAutreText != null)
        {
            populationAutreText.text = $"Autre : {FormatPourcentage(autre, total)}";
        }
    }

    private void RefreshStatsFixes()
    {
        SCOBJ_PROVINCE data = provinceActuelle.data;

        if (data == null)
            return;

        if (etriniumText != null)
        {
            etriniumText.text = $"Etrinium : {data.etrinium}";
        }

        if (prestigeText != null)
        {
            prestigeText.text = $"Prestige : {data.prestige}";
        }

        if (poidsPolitiqueText != null)
        {
            poidsPolitiqueText.text = $"Poids politique : {data.poidsPolitique}";
        }

        if (accessibiliteText != null)
        {
            accessibiliteText.text = $"Accessibilité : {data.accesibilite}";
        }
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