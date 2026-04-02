using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EquipeState
{
    public EquipeData data;
    public int niveauActuel;
    public ProvinceState provinceAffectee;
    public Compagnie compagnie;
    public bool explorationEnCours;
    public int toursRestants;
    public int toursTotaux;
    public List<PersonnageData> membresActuels = new List<PersonnageData>();
}