public static class UTIL_UiTooltipHelper
{
    public static void SetTooltip(UI_StatTooltip_Trigger trigger, string value)
    {
        if (trigger != null)
        {
            trigger.SetValeurOverride(value);
        }
    }
}