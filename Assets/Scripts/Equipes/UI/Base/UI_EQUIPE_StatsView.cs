using TMPro;
using UnityEngine;

public class UI_EQUIPE_StatsView : MonoBehaviour
{
    [Header("Stats Equipe")]
    [SerializeField] private TMP_Text curiositeEquipe;
    [SerializeField] private TMP_Text ingeniositeEquipe;
    [SerializeField] private TMP_Text combativiteEquipe;
    [SerializeField] private TMP_Text enduranceEquipe;
    [SerializeField] private TMP_Text nombreMembresEquipe;
    [SerializeField] private TMP_Text coutParTour;

    public void Refresh(STATE_EQUIPE equipe)
    {
        if (equipe == null)
            return;

        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        if (curiositeEquipe != null)
            curiositeEquipe.text = stats.curiositeTotale.ToString();

        if (ingeniositeEquipe != null)
            ingeniositeEquipe.text = stats.ingeniositeTotale.ToString();

        if (combativiteEquipe != null)
            combativiteEquipe.text = stats.combativiteTotale.ToString();

        if (enduranceEquipe != null)
            enduranceEquipe.text = stats.enduranceTotale.ToString();

        if (nombreMembresEquipe != null)
            nombreMembresEquipe.text = stats.nombreMembres.ToString();

        if (coutParTour != null)
        {
            int surcoutExploration = CALC_EQUIPE_StatsCalculator.CalculerSurcoutExploration(equipe);
            coutParTour.text = surcoutExploration.ToString();
        }
    }
}