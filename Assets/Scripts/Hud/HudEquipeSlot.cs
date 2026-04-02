using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HudEquipeSlot : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Image portraitChefImage;
    [SerializeField] private TMP_Text niveauEquipeText;
    [SerializeField] private Slider explorationSlider;
    [SerializeField] private TMP_Text statutExplorationText;
    [SerializeField] private TMP_Text nomProvince;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text progressionExplorationText;

    private EquipeHudData equipeData;
    private EquipeState equipeSource;
    private EquipeMenuController equipeMenuController;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponentInChildren<Button>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnClickSlot);
        }
        else
        {
            Debug.LogWarning("Aucun Button trouvé dans HudEquipeSlot.");
        }
    }

    public void SetMenuController(EquipeMenuController controller)
    {
        equipeMenuController = controller;
    }

  public void Refresh(EquipeHudData data)
{
    equipeData = data;
    equipeSource = data != null ? data.source : null;

    if (data == null)
    {
        Hide();
        return;
    }

    gameObject.SetActive(true);

    if (portraitChefImage != null)
    {
        portraitChefImage.sprite = data.portraitChef;
        portraitChefImage.enabled = data.portraitChef != null;
    }

    if (niveauEquipeText != null)
    {
        niveauEquipeText.text = $"Nv {data.niveau}";
    }

    if (nomProvince != null)
    {
        nomProvince.text = string.IsNullOrEmpty(data.nomProvince) ? "Aucune province" : data.nomProvince;
    }

    if (statutExplorationText != null)
    {
        statutExplorationText.text = data.statutExploration;
    }

    if (explorationSlider != null)
    {
        if (data.explorationEnCours && data.toursTotaux > 0)
        {
            int toursEffectues = data.toursTotaux - data.toursRestants;

            explorationSlider.gameObject.SetActive(true);
            explorationSlider.minValue = 0f;
            explorationSlider.maxValue = data.toursTotaux;
            explorationSlider.value = Mathf.Clamp(toursEffectues, 0, data.toursTotaux);

            if (progressionExplorationText != null)
            {
                progressionExplorationText.text = $"{toursEffectues} / {data.toursTotaux}";
            }
        }
        else
        {
            explorationSlider.gameObject.SetActive(false);

            if (progressionExplorationText != null)
            {
                progressionExplorationText.text = "";
            }
        }
    }

    if (button != null)
    {
        button.interactable = true;
    }
}

public void Hide()
{
    equipeData = null;
    equipeSource = null;
    gameObject.SetActive(false);
}

 private void OnClickSlot()
{
    if (equipeSource == null)
    {
        Debug.LogWarning("equipeSource est null");
        return;
    }

    if (equipeMenuController == null)
    {
        Debug.LogWarning("Aucun EquipeMenuController assigné au HudEquipeSlot.");
        return;
    }

    equipeMenuController.OpenEquipeMenu(equipeSource);
}}