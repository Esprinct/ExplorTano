using TMPro;

public static class UTIL_UiStatTextHelper
{
    public static void SetStatValue(TMP_Text target, int valeur, int delta)
    {
        if (target != null)
        {
            target.SetText(FMT_STATS_ValueFormatter.FormatValeurTotaleAvecDelta(valeur, delta));
        }
    }
}