using UnityEngine;

[CreateAssetMenu(fileName = "ObjetConsommable", menuName = "Game/Objets/Consommable")]
public class SCOBJ_OBJET_CONSOMMABLE : SCOBJ_OBJET
{
    [Header("Consommation expédition")]
    public int quantiteConsommeeParExpedition = 1;

    [Header("Gameplay")]
    public bool obligatoirePourExpedition = true;

    private void OnValidate()
    {
        categorie = ENUM_OBJET_Categorie.Consommable;
    }
}