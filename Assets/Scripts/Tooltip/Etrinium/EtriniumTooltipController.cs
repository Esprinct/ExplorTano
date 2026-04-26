using System.Text;
using TMPro;
using UnityEngine;

public class EtriniumTooltipController : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text contenuText;

    public void Show(EtriniumBreakdownData data)
{
    if (panelRoot == null || contenuText == null || data == null)
        return;

    Debug.Log(
        $"Tooltip breakdown | revenus={data.totalRevenus} | " +
        $"depenses={data.totalDepenses} | net={data.totalNet}"
    );

    StringBuilder sb = new StringBuilder();

    sb.AppendLine("<size=120%><b>Revenus :</b></size>");
    sb.AppendLine("Provinces :");

    if (data.revenusProvinces == null || data.revenusProvinces.Count == 0)
    {
        sb.AppendLine("Aucune");
    }
    else
    {
        foreach (EtriniumLineData ligne in data.revenusProvinces)
        {
            sb.AppendLine($"{ligne.label} : +{ligne.valeurFinale}");
        }
    }

    sb.AppendLine();
    sb.AppendLine("<size=120%><b>Dépenses :</b></size>");
    sb.AppendLine(
        $"Personnages (après calcul endurance) : " +
        $"-{data.depensesPersonnagesFinales} (base -{data.depensesPersonnagesBase})"
    );
    sb.AppendLine($"Entretien fixe des équipes : -{data.depensesEquipesFixes}");
    sb.AppendLine($"Surcoût équipes en exploration : -{data.depensesEquipesExploration}");

    sb.AppendLine();
    sb.AppendLine($"Total revenus : +{Mathf.RoundToInt(data.totalRevenus)}");
    sb.AppendLine($"Total dépenses : -{Mathf.RoundToInt(data.totalDepenses)}");
    sb.AppendLine($"Net : {Mathf.RoundToInt(data.totalNet)}");

    contenuText.text = sb.ToString();
    panelRoot.SetActive(true);
}

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }
}