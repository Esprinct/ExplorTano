using UnityEngine;

public static class CALC_PERSONNAGE_Generator
{
    public static Sprite[] spritesGeneriques;
    public static SCOBJ_PERSONNAGE_EFFET effetAffiniteRespectee;
    public static SCOBJ_PERSONNAGE_EFFET effetAffiniteNonRespectee;

    private static string[] noms = { "Kael", "Mira", "Doran", "Lyra", "Zek" };
    private static string[] prenoms = { "Val", "Nor", "Zen", "Kai", "Lun" };
public static CFG_LevelProgression progressionConfigParDefaut;
  public static SCOBJ_Personnage Generer(int rarete)
{
    SCOBJ_Personnage perso = ScriptableObject.CreateInstance<SCOBJ_Personnage>();
    perso.idUnique = System.Guid.NewGuid().ToString();

    perso.nom = noms[Random.Range(0, noms.Length)];
    perso.prenom = prenoms[Random.Range(0, prenoms.Length)];
    perso.rareteEtoiles = rarete;
perso.progression = new STATE_LevelProgression();
perso.progressionConfig = progressionConfigParDefaut;
    perso.estUnique = false;

        if (spritesGeneriques != null && spritesGeneriques.Length > 0)
        {
            perso.sprite = spritesGeneriques[Random.Range(0, spritesGeneriques.Length)];
        }

        int baseStat = 10 + rarete * 5;
        perso.force = baseStat + Random.Range(-10, 10);
        perso.intelligence = baseStat + Random.Range(-10, 10);
        perso.dexterite = baseStat + Random.Range(-10, 10);
        perso.endurance = baseStat + Random.Range(-10, 10);

        perso.aPreferenceCompagnie = Random.value > 0.5f;
        perso.compagniePreferee = perso.aPreferenceCompagnie
            ? (ENUM_Compagnie)Random.Range(1, 4)
            :  ENUM_Compagnie.Aucune;

        switch (rarete)
        {
            case 1:
                perso.coutParTour = 600;
                perso.coutRecrutementBase = 8000;
                break;

            case 2:
                perso.coutParTour = 1200;
                perso.coutRecrutementBase = 14000;
                break;

            case 3:
                perso.coutParTour = 1600;
                perso.coutRecrutementBase = 24000;
                break;

            case 4:
                perso.coutParTour = 2100;
                perso.coutRecrutementBase = 40000;
                break;

            case 5:
                perso.coutParTour = 2800;
                perso.coutRecrutementBase = 65000;
                break;

            default:
                perso.coutParTour = 1600;
                perso.coutRecrutementBase = 24000;
                break;
        }

        // On ne met PAS les effets ici :
        // à ce stade on ne connaît pas encore la compagnie recruteuse.

        return perso;
    }
}