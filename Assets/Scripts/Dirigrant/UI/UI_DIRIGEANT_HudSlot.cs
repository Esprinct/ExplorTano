using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DIRIGEANT_HudSlot : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nomText;
    [SerializeField] private TMP_Text niveauText;
    [SerializeField] private Button button;

    [Header("XP Courbe")]
    [SerializeField] private Image xpRingFill;

    [Header("Dépendances")]
    [SerializeField] private HudController hudController;
    [SerializeField] private UI_DIRIGEANT_DetailController detailController;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    public void Refresh(SCOBJ_DIRIGEANT dirigeant)
    {
        if (portraitImage != null)
        {
            portraitImage.sprite = dirigeant != null ? dirigeant.portraitDirigeant : null;
            portraitImage.enabled = portraitImage.sprite != null;
        }

        if (nomText != null)
            nomText.text = dirigeant != null ? dirigeant.nomDirigeant : "Dirigeant";

        if (niveauText != null)
            niveauText.text = dirigeant != null ? $"Nv. {dirigeant.niveauDirigeant}" : "";

        RefreshXp(dirigeant);
    }

    private void RefreshXp(SCOBJ_DIRIGEANT dirigeant)
    {
        if (xpRingFill == null)
            return;

        if (dirigeant == null || dirigeant.xpPourNiveauSuivant <= 0)
        {
            xpRingFill.fillAmount = 0f;
            return;
        }

        float ratio = dirigeant.xpDirigeant / (float)dirigeant.xpPourNiveauSuivant;
        xpRingFill.fillAmount = Mathf.Clamp01(ratio);
    }

    private void OnClick()
    {
        SCOBJ_DIRIGEANT dirigeant = hudController != null
            ? hudController.GetDirigeantActuel()
            : null;

        Debug.Log(
            $"[DIRIGEANT CLICK] hudController={(hudController != null ? hudController.name : "null")} | " +
            $"dirigeant null ? {dirigeant == null} | " +
            $"detailController null ? {detailController == null}"
        );

        if (dirigeant == null || detailController == null)
            return;

        detailController.OpenDirigeantMenu(dirigeant);
    }
}