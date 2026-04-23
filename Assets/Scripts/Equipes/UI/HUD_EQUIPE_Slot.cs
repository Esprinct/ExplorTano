using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_EQUIPE_Slot : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Image portraitChefImage;
    [SerializeField] private TMP_Text niveauEquipeText;
    [SerializeField] private Slider explorationSlider;
    [SerializeField] private TMP_Text statutExplorationText;
    [SerializeField] private TMP_Text nomProvince;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text progressionExplorationText;

    private DATA_EQUIPE_DetailData SCOBJ_EQUIPE;
    private STATE_EQUIPE equipeSource;
    private UI_EQUIPE_DetailController equipeDetailController;

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
            Debug.LogWarning("Aucun Button trouvé dans HUD_EQUIPE_Slot.");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClickSlot);
        }
    }

    public void SetDetailController(UI_EQUIPE_DetailController controller)
    {
        equipeDetailController = controller;
    }

    public void Refresh(DATA_EQUIPE_DetailData data)
    {
        SCOBJ_EQUIPE = data;
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
            nomProvince.text = string.IsNullOrEmpty(data.nomProvince)
                ? "Aucune province"
                : data.nomProvince;
        }

        if (statutExplorationText != null)
        {
            statutExplorationText.text = data.statutExploration;
        }

        RefreshExplorationProgress(data);

        if (button != null)
        {
            button.interactable = equipeSource != null;
        }
    }

   private void RefreshExplorationProgress(DATA_EQUIPE_DetailData data)
{
    if (explorationSlider == null)
        return;

    bool actionEnCours =
        data != null &&
        (data.explorationEnCours || data.vadrouilleEnCours) &&
        data.toursTotaux > 0;

    explorationSlider.gameObject.SetActive(actionEnCours);

    if (!actionEnCours)
    {
        if (progressionExplorationText != null)
            progressionExplorationText.text = "";

        return;
    }

    int toursEffectues = data.toursTotaux - data.toursRestants;

    explorationSlider.minValue = 0f;
    explorationSlider.maxValue = data.toursTotaux;
    explorationSlider.value = Mathf.Clamp(toursEffectues, 0, data.toursTotaux);

    if (progressionExplorationText != null)
    {
        string prefixe = data.vadrouilleEnCours ? "Vadrouille" : "Exploration";
        progressionExplorationText.text = $"{prefixe} {toursEffectues} / {data.toursTotaux}";
    }
}

    public void Hide()
    {
        SCOBJ_EQUIPE = null;
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

        if (equipeDetailController == null)
        {
            equipeDetailController =
                FindAnyObjectByType<UI_EQUIPE_DetailController>(FindObjectsInactive.Include);
        }

        if (equipeDetailController == null)
        {
            Debug.LogWarning("Aucun UI_EQUIPE_DetailController assigné au HUD_EQUIPE_Slot.");
            return;
        }

        equipeDetailController.OpenEquipeMenu(equipeSource);
    }
}