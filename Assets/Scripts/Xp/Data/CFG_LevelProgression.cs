using UnityEngine;

[CreateAssetMenu(fileName = "CFG_LevelProgression", menuName = "Game/Progression/Level Progression Config")]
public class CFG_LevelProgression : ScriptableObject
{
    [Min(1)] public int niveauMax = 100;
    [Min(1)] public int xpBase = 100;
    [Min(0)] public int xpParNiveau = 25;
    [Min(0)] public int pointsParNiveau = 5;
    

    public int GetXpRequiredForLevel(int niveau)
    {
        if (niveau <= 1)
            return xpBase;

        return xpBase + (niveau - 1) * xpParNiveau;
    }
}