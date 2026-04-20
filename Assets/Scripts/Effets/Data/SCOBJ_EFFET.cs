using System.Collections.Generic;
using UnityEngine;

public abstract class SCOBJ_EFFET : ScriptableObject
{
    [Header("Infos UI - Legacy")]
    public string titre;

    [TextArea(2, 5)]
    public string description;

    public Sprite icone;
    public EffetType type = EffetType.Bonus;

    [Header("Valeur affichée")]
    [TextArea(1, 2)]
    public string valeurAffichee;

    [Header("Condition d'affichage / activation")]
    public EffetConditionType conditionType = EffetConditionType.Aucune;

    [Header("Gameplay")]
    public List<DATA_StatModifier> modificateurs = new();

    [Header("Options")]
    public bool cacherSiInactif = true;
    public bool genererValeurAfficheeAutomatiquement = true;

    public virtual string GetTitreAffiche()
    {
        return titre;
    }

    public virtual string GetDescriptionAffiche()
    {
        return description;
    }
}