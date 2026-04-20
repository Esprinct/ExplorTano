public static class FMT_STATS_ValueFormatter
{
    private const string BonusColor = "#2E7D32";
    private const string MalusColor = "#B71C1C";

    public static string FormatValeurTotaleAvecDelta(int totalValue, int delta)
    {
        if (delta == 0)
            return totalValue.ToString();

        string color = delta > 0 ? BonusColor : MalusColor;
        string signe = delta > 0 ? "+" : "";

        return $"{totalValue} <b><color={color}>({signe}{delta})</color></b>";
    }
}