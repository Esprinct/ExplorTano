using UnityEngine;

[CreateAssetMenu(fileName = "MalusBonusData", menuName = "Game/MalusBonus")]
public class MalusBonusData : ScriptableObject
{
    public string titre;
    public string description;
    public int MontantStatModifiee;
    public bool isBonus;
    public bool isMalus;

}