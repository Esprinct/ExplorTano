using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipeSpecialisation", menuName = "Game/Equipes/Specialisation")]
public class SCOBJ_EQUIPE_SPECIALISATION : ScriptableObject
{
    [Header("Identité")]
    public ENUM_EQUIPE_SPECIALISATION type;
    public ENUM_EQUIPE_TIER tier;
    public string nomAffiche;
    [TextArea] public string description;
    public Sprite icone;

    [Header("Conditions")]
    public int niveauMinimum;
    public ENUM_EQUIPE_SPECIALISATION specialisationParent;
    public int coutDeblocage;

    [Header("Action principale")]
    public ENUM_EQUIPE_ACTION_PRINCIPALE actionPrincipale;

    [Header("Effets")]
    public List<SCOBJ_EQUIPE_EFFET> effets = new();
}