using UnityEngine;

[CreateAssetMenu(fileName = "DirigeantEffet", menuName = "Game/Effets/Dirigeant")]
public class SCOBJ_DIRIGEANT_EFFET : SCOBJ_EFFET
{
    [Header("Dirigeant")]
    public int niveauRequis = 1;
    public ENUM_DirigeantConcerne dirigeantConcerne = ENUM_DirigeantConcerne.Aucun;

    [Header("Portée / logique spéciale")]
    public EffetPorteeType portee = EffetPorteeType.Globale;
    public EffetSpecialType effetSpecial = EffetSpecialType.Aucun;
}