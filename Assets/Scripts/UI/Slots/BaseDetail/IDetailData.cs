using System.Collections.Generic;
using UnityEngine;

public interface IDetailData
{
    string IdUnique { get; }
    string NomAffiche { get; }
    string DescriptionAffichee { get; }
    Sprite IconeAffichee { get; }
    IReadOnlyList<SCOBJ_EFFET> Effets { get; }
}