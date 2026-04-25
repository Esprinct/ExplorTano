using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_XP_STATS_AllocationRow : MonoBehaviour
{
    [SerializeField] private TMP_Text statNameText;
    [SerializeField] private TMP_Text investedValueText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private TMP_InputField valueInput;
    [SerializeField] private Toggle autoToggle;

    private SCOBJ_Personnage personnage;
    private DATA_STATS_AllocationDraft draft;
    private EffetENUM_Stats stat;
    private Action onChanged;
    private bool refreshing;

    public void Setup(
        SCOBJ_Personnage personnage,
        DATA_STATS_AllocationDraft draft,
        EffetENUM_Stats stat,
        Action onChanged)
    {
        this.personnage = personnage;
        this.draft = draft;
        this.stat = stat;
        this.onChanged = onChanged;

        if (statNameText != null)
            statNameText.text = GetLabel(stat);

        BindEvents();
        Refresh();
    }

    public void Refresh()
    {
        refreshing = true;

        int invested = DATA_STATS_AllocationDraftService.GetValue(draft, stat);

        if (investedValueText != null)
            investedValueText.text = invested.ToString();

        if (valueInput != null)
            valueInput.text = invested.ToString();

        int minimumConfirme = personnage != null && personnage.allocation != null
            ? SVC_STATS_Allocation.GetAllocatedValue(personnage, stat)
            : 0;

        int pointsRestants = draft != null ? draft.pointsRestants : 0;

        if (plusButton != null)
            plusButton.interactable = pointsRestants > 0;

        if (minusButton != null)
            minusButton.interactable = invested > minimumConfirme;

        if (valueInput != null)
            valueInput.interactable = true;

        if (autoToggle != null)
            autoToggle.SetIsOnWithoutNotify(DATA_STATS_AllocationDraftService.IsAutoEnabled(draft, stat));

        refreshing = false;
    }

    private void BindEvents()
    {
        if (minusButton != null)
        {
            minusButton.onClick.RemoveListener(OnMinusClicked);
            minusButton.onClick.AddListener(OnMinusClicked);
        }

        if (plusButton != null)
        {
            plusButton.onClick.RemoveListener(OnPlusClicked);
            plusButton.onClick.AddListener(OnPlusClicked);
        }

        if (valueInput != null)
        {
            valueInput.onEndEdit.RemoveListener(OnInputEndEdit);
            valueInput.onEndEdit.AddListener(OnInputEndEdit);
        }

        if (autoToggle != null)
        {
            autoToggle.onValueChanged.RemoveListener(OnAutoToggleChanged);
            autoToggle.onValueChanged.AddListener(OnAutoToggleChanged);
        }
    }

    private void OnPlusClicked()
    {
        if (DATA_STATS_AllocationDraftService.TryAddPoint(draft, stat))
            NotifyChanged();
        else
            Refresh();
    }

    private void OnMinusClicked()
    {
        if (DATA_STATS_AllocationDraftService.TryRemovePoint(draft, stat, personnage))
            NotifyChanged();
        else
            Refresh();
    }

    private void OnInputEndEdit(string value)
    {
        if (refreshing)
            return;

        if (!int.TryParse(value, out int parsed))
        {
            Refresh();
            return;
        }

        if (DATA_STATS_AllocationDraftService.TrySetValue(draft, stat, parsed, personnage))
            NotifyChanged();
        else
            Refresh();
    }

    private void OnAutoToggleChanged(bool value)
    {
        if (refreshing)
            return;

        DATA_STATS_AllocationDraftService.SetAuto(draft, stat, value);
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        Refresh();
        onChanged?.Invoke();
    }

    private string GetLabel(EffetENUM_Stats stat)
    {
        return stat switch
        {
            EffetENUM_Stats.Curiosite => "Curiosite",
            EffetENUM_Stats.Ingeniosite => "Ingeniosite",
            EffetENUM_Stats.Combativite => "Dextérité",
            EffetENUM_Stats.Endurance => "Endurance",
            _ => "Stat"
        };
    }
}