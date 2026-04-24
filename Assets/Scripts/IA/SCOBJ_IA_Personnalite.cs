using UnityEngine;

[CreateAssetMenu(
    fileName = "IA_Personnalite",
    menuName = "Explortano/IA/Personnalité IA"
)]
public class SCOBJ_IA_Personnalite : ScriptableObject
{
    [Header("Identité")]
    public string nomAffiche;
    public ENUM_IA_Personnalite type = ENUM_IA_Personnalite.Equilibree;

    [Header("Roster")]
    public int tailleCibleEquipe = 4;
    public int tailleMinEquipePourAction = 1;
    public float ratioMinimalBudgetPourAction = 1f;

    [Header("Scoring personnage")]
    public float poidsForce = 1f;
    public float poidsIntelligence = 1f;
    public float poidsDexterite = 1f;
    public float poidsEndurance = 1f;
    public float poidsRarete = 100f;
    public float poidsCoutParTour = 0f;
    public float bonusPreferenceCompagnie = 150f;

    [Header("Biais de spécialisation")]
    public float bonusExploration = 0f;
    public float bonusArcheologues = 0f;
    public float bonusArpenteurs = 0f;
    public float bonusMiliciens = 0f;
    public float bonusGardienPaix = 0f;
    public float bonusIntervention = 0f;
    public float bonusConstruction = 0f;
    public float bonusColons = 0f;
    public float bonusGenieCivil = 0f;
}