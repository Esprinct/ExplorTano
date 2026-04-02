using UnityEngine;

[CreateAssetMenu(fileName = "PersonnageData", menuName = "Game/Personnage")]
public class PersonnageData : ScriptableObject
{
    public string nom;
    public string prenom;
    public RolePersonnage roleActuel;
    public int niveau;
    [Range(1, 5)]
    public int rareteEtoiles = 1;
    public int xpActuel;
    public int xpNiveauSuivant;
    public Sprite sprite;
    public string description;
    public MalusBonus malusBonus; 
}