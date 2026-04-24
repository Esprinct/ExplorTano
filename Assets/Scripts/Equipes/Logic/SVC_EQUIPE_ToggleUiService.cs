using UnityEngine;
using UnityEngine.UI;

public static class SVC_EQUIPE_ToggleUiService
{
    public static void RefreshToggles(
        STATE_EQUIPE equipe,
        Toggle toggleAffectationAutomatique,
        Toggle toggleLancementActionAutomatique)
    {
        if (equipe == null)
            return;

        if (toggleAffectationAutomatique != null)
        {
            toggleAffectationAutomatique.SetIsOnWithoutNotify(
                equipe.affectationAutomatique
            );
        }

        if (toggleLancementActionAutomatique != null)
        {
            toggleLancementActionAutomatique.SetIsOnWithoutNotify(
                equipe.lancementActionAutomatique
            );
        }
    }

    public static void OnToggleAffectationAutomatiqueChanged(
        STATE_EQUIPE equipe,
        bool value)
    {
        if (equipe == null)
            return;

        equipe.affectationAutomatique = value;
    }

    public static void OnToggleLancementAutomatiqueChanged(
        STATE_EQUIPE equipe,
        bool value,
        Toggle toggleAffectationAutomatique)
    {
        if (equipe == null)
            return;

        equipe.lancementActionAutomatique = value;

        if (value)
        {
            equipe.affectationAutomatique = true;

            if (toggleAffectationAutomatique != null)
                toggleAffectationAutomatique.SetIsOnWithoutNotify(true);
        }
    }
}