using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CFG_LevelBonus", menuName = "Game/Progression/Level Bonus Config")]
public class CFG_LevelBonus : ScriptableObject
{
    public List<DATA_LevelBonusDefinition> bonusParNiveau = new();

    public DATA_LevelBonusDefinition GetBonusForLevel(int niveau)
    {
        DATA_LevelBonusDefinition cumul = new();

        foreach (DATA_LevelBonusDefinition bonus in bonusParNiveau)
        {
            if (bonus == null)
                continue;

            if (bonus.niveau > niveau)
                continue;

            cumul.bonusForce += bonus.bonusForce;
            cumul.bonusIntelligence += bonus.bonusIntelligence;
            cumul.bonusDexterite += bonus.bonusDexterite;
            cumul.bonusEndurance += bonus.bonusEndurance;
        }

        return cumul;
    }
}