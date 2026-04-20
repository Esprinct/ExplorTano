using System.Collections.Generic;

[System.Serializable]
public class EtriniumBreakdownData
{
    public List<EtriniumLineData> revenusProvinces = new();

    public int depensesPersonnagesFinales;
    public int depensesPersonnagesBase;

    public int depensesEquipesFixes;
    public int depensesEquipesExploration;

    public float totalRevenus;
    public float totalDepenses;
    public float totalNet;
}

[System.Serializable]
public class EtriniumLineData
{
    public string label;
    public int valeurFinale;
    public int valeurBase;
}