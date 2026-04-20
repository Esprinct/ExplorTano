using UnityEngine;

[CreateAssetMenu(fileName = "Artefact", menuName = "Game/Objets/Artefact")]
public class SCOBJ_ARTEFACT : SCOBJ_OBJET_EQUIPPABLE
{
    [Header("Artefact")]
    public bool estRare = false;

    [TextArea]
    public string lore;

    private void OnValidate()
    {
        categorie = ENUM_OBJET_Categorie.Equipable;
    }
}