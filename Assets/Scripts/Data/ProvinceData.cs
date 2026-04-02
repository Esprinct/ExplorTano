using UnityEngine;

[CreateAssetMenu(fileName = "ProvinceData", menuName = "Game/Province")]
public class ProvinceData : ScriptableObject
{
    public string nom;
    public int prestige;
    public int etrinium;

    public int populationShiki;
    public int populationFrisien;
    public int populationAutre;

    public int poidsPolitique;
    public int accesibilite;

    public Sprite sprite;

    [Header("Influences initiales")]
    public float influenceMaizinInitiale;
    public float influenceKiniaInitiale;
    public float influenceJohoInitiale;
    public float influenceAutreInitiale;
}