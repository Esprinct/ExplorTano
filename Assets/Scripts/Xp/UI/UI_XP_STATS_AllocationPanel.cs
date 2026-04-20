using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_XP_STATS_AllocationPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text pointsDisponiblesText;

    [Header("Rows")]
    [SerializeField] private UI_XP_STATS_AllocationRow forceRow;
    [SerializeField] private UI_XP_STATS_AllocationRow intelligenceRow;
    [SerializeField] private UI_XP_STATS_AllocationRow dexteriteRow;
    [SerializeField] private UI_XP_STATS_AllocationRow enduranceRow;

    [Header("Actions")]
    [SerializeField] private Button appliquerAutoButton;
    [SerializeField] private Button confirmerButton;
    [SerializeField] private Button annulerButton;

    private SCOBJ_Personnage personnage;
    private DATA_STATS_AllocationDraft draft;
    private System.Action onCommitted;

    private void Awake()
    {
        UTIL_UiEventBinder.Bind(appliquerAutoButton, OnApplyAutoClicked, this, nameof(appliquerAutoButton));
        UTIL_UiEventBinder.Bind(confirmerButton, OnConfirmClicked, this, nameof(confirmerButton));
        UTIL_UiEventBinder.Bind(annulerButton, OnCancelClicked, this, nameof(annulerButton));
    }

    private void OnDestroy()
    {
        UTIL_UiEventBinder.Unbind(appliquerAutoButton, OnApplyAutoClicked);
        UTIL_UiEventBinder.Unbind(confirmerButton, OnConfirmClicked);
        UTIL_UiEventBinder.Unbind(annulerButton, OnCancelClicked);
    }

    public void Setup(SCOBJ_Personnage personnage, System.Action onCommitted = null)
    {
        this.personnage = personnage;
        this.onCommitted = onCommitted;
        this.draft = DATA_STATS_AllocationDraft.FromPersonnage(personnage);

        if (forceRow != null)
            forceRow.Setup(personnage, draft, EffetENUM_Stats.Force, Refresh);

        if (intelligenceRow != null)
            intelligenceRow.Setup(personnage, draft, EffetENUM_Stats.Intelligence, Refresh);

        if (dexteriteRow != null)
            dexteriteRow.Setup(personnage, draft, EffetENUM_Stats.Dexterite, Refresh);

        if (enduranceRow != null)
            enduranceRow.Setup(personnage, draft, EffetENUM_Stats.Endurance, Refresh);

        Refresh();
    }

    public void Refresh()
    {
        if (root != null)
            root.SetActive(personnage != null);

        if (pointsDisponiblesText != null)
        {
            int points = draft != null ? draft.pointsRestants : 0;
            pointsDisponiblesText.text = $"Points disponibles : {points}";
        }

        forceRow?.Refresh();
        intelligenceRow?.Refresh();
        dexteriteRow?.Refresh();
        enduranceRow?.Refresh();

        if (appliquerAutoButton != null)
            appliquerAutoButton.interactable = draft != null && draft.pointsRestants > 0;

        if (confirmerButton != null)
            confirmerButton.interactable = personnage != null && draft != null;

        if (annulerButton != null)
            annulerButton.interactable = personnage != null && draft != null;
    }

    private void OnApplyAutoClicked()
    {
        if (draft == null)
            return;

        DATA_STATS_AllocationDraftService.ApplyAutoAllocation(draft);
        Refresh();
    }

   private void OnConfirmClicked()
{
    Debug.Log("OnConfirmClicked appelé");

    if (personnage == null || draft == null)
    {
        Debug.LogWarning("Confirm bloqué : personnage ou draft null");
        return;
    }

    DATA_STATS_AllocationDraftService.Commit(personnage, draft);
    draft = DATA_STATS_AllocationDraft.FromPersonnage(personnage);

    Debug.Log("Commit effectué");
    Refresh();
    onCommitted?.Invoke();
}

private void OnCancelClicked()
{
    Debug.Log("OnCancelClicked appelé");

    if (personnage == null)
    {
        Debug.LogWarning("Cancel bloqué : personnage null");
        return;
    }

    draft = DATA_STATS_AllocationDraft.FromPersonnage(personnage);
    Refresh();
}
}