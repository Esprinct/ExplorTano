using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DATA_StatModifier
{
    [FormerlySerializedAs("cible")]
    public EffetENUM_Stats stat;
    public EffetValeurType valeurType;
    public int valeur;
    public bool estMalus;
}