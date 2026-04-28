using UnityEngine;

[CreateAssetMenu(fileName = "SCOBJ_PROVINCE", menuName = "Game/Province")]
public class SCOBJ_PROVINCE : ScriptableObject
{
    public string nom;
    public int prestige;
    public int etrinium;

    public int populationShiki;
    public int populationFrisien;
    public int populationAutre;

    public int poidsPolitique;

public float enclavement;
    public Sprite sprite;

    [Header("Influences initiales")]
    public float influenceMaizinInitiale;
    public float influenceKiniaInitiale;
    public float influenceJohoInitiale;
    public float influenceAutreInitiale;
   
}