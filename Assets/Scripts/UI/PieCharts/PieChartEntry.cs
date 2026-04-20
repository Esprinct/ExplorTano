using UnityEngine;

[System.Serializable]
public struct PieChartEntry
{
    public string id;
    public float value;
    public Color color;

    public PieChartEntry(string id, float value, Color color)
    {
        this.id = id;
        this.value = value;
        this.color = color;
    }
}