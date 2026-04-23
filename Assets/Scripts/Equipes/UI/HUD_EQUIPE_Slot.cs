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
    [SerializeField] private TMP_Text autoActionText;

    private DATA_EQUIPE_DetailData equipeData;
    private STATE_EQUIPE equipeSource;
    private UI_EQUIPE_DetailController equipeDetailController;

    private void Awake()
    {
        if (button == null)
            button = GetComponentInChildren<Button>();

        if (button != null)
            button.onClick.AddListener(OnClickSlot);
        else
            Debug.LogWarning("Aucun Button trouvé dans HUD_EQUIPE_Slot.");
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClickSlot);
    }

    public void SetDetailController(UI_EQUIPE_DetailController controller)
    {
        equipeDetailController = controller;
    }

    public void Refresh(DATA_EQUIPE_DetailData data)
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
            niveauEquipeText.text = $"Nv {data.niveau}";

        if (nomProvince != null)
        {
            nomProvince.text = string.IsNullOrEmpty(data.nomProvince)
                ? "Aucune province"
                : data.nomProvince;
        }

        if (statutExplorationText != null)
            statutExplorationText.text = data.statutAction;

        if (autoActionText != null)
        {
            autoActionText.gameObject.SetActive(data.lancementActionAutomatique);
            autoActionText.text = data.lancementActionAutomatique ? "Auto" : "";
        }

        RefreshActionProgress(data);

        if (button != null)
            button.interactable = equipeSource != null;
    }

    private void RefreshActionProgress(DATA_EQUIPE_DetailData data)
    {
        if (explorationSlider == null)
            return;

        bool afficherProgression =
            data != null &&
            data.aUneActionEnCours &&
            data.toursTotaux > 0;

        explorationSlider.gameObject.SetActive(afficherProgression);

        if (!afficherProgression)
        {
            explorationSlider.minValue = 0f;
            explorationSlider.maxValue = 1f;
            explorationSlider.value = 0f;

            if (progressionExplorationText != null)
                progressionExplorationText.text = "";

            return;
        }

        int toursEffectues = data.toursTotaux - data.toursRestants;
        toursEffectues = Mathf.Clamp(toursEffectues, 0, data.toursTotaux);

        explorationSlider.minValue = 0f;
        explorationSlider.maxValue = Mathf.Max(1, data.toursTotaux);
        explorationSlider.value = toursEffectues;

        if (progressionExplorationText != null)
        {
            string nomAction = string.IsNullOrWhiteSpace(data.nomActionEnCours)
                ? "Action"
                : data.nomActionEnCours;

            progressionExplorationText.text = $"{nomAction} {toursEffectues} / {data.toursTotaux}";
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