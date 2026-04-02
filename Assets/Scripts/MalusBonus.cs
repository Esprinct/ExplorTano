using UnityEngine;

[CreateAssetMenu(fileName = "MalusBonus", menuName = "Game/Malus Bonus")]
public class MalusBonus : ScriptableObject
{
    public string nom;
    [TextArea] public string description;

    public int bonusForce;
    public int bonusIntelligence;
    public int bonusDexterite;
    public int bonusEndurance;

    public int coutEtriniumParTourMod;
    public int toursExplorationMod;
    public int prestigeExplorationMod;

    public float chanceReliqueMod;
    public float chanceReliqueRareMod;
}
