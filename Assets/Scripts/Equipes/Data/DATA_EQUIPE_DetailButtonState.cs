public class DATA_EQUIPE_DetailButtonState
{
    public bool equipeValide;
    public bool aDesMembres;
    public bool actionEnCours;
    public bool provinceAffectee;
    public bool enAttenteSelectionProvince;
    public bool peutFaireAction;
    public bool aLesFonds;

    public ENUM_EQUIPE_ACTION actionCourante = ENUM_EQUIPE_ACTION.Aucune;
    public int coutLancement = 0;
public bool provinceAssezExploreePourVadrouille;    public bool boutonAffecterInteractable;
    public bool boutonDemarrerInteractable;
    public bool boutonAjouterInteractable;
    public bool toggleAffectationInteractable;
    public bool toggleLancementAutoInteractable;

    public string texteBoutonAffecter = "Affecter à une province";
    public string texteBoutonDemarrer = "Démarrer l'action";
    public string texteVerrouillage = "";
    public bool afficherTexteVerrouillage = false;
}