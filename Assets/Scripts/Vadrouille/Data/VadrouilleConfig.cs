using UnityEngine;

[CreateAssetMenu(fileName = "VadrouilleConfig", menuName = "Game/Vadrouille Config")]
public class VadrouilleConfig : ScriptableObject
{
    [Header("Base Vadrouille")]
    public int toursBase = 3;
    public int coutParTourBase = 5;
    public int prestigeBase = 1;

    [Header("Occupation")]
    public float gainOccupationBase = 10f;
    public float reductionOccupationAdverseBase = 10f;

    [Header("XP")]
    public int xpPersonnageParVadrouille = 20;
    public int xpEquipeParVadrouille = 40;
}