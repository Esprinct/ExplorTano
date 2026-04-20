using UnityEngine;

[CreateAssetMenu(fileName = "PersonnageEffet", menuName = "Game/Personnages/Effet")]
public class SCOBJ_PERSONNAGE_EFFET : SCOBJ_EFFET
{
    [Header("Infos UI - Variantes de genre")]
    public string titreNeutre;
    public string titreMasculin;
    public string titreFeminin;

    [TextArea(2, 5)]
    public string descriptionNeutre;

    [TextArea(2, 5)]
    public string descriptionMasculine;

    [TextArea(2, 5)]
    public string descriptionFeminine;

    private void OnValidate()
    {
        if (genererValeurAfficheeAutomatiquement)
            valeurAffichee = FMT_EFFET.BuildValeurAffichee(this);
    }

    public string GetTitre(ENUM_PERSONNAGE_Genre genre)
    {
        return SelectionnerTexteSelonGenre(
            genre,
            titreMasculin,
            titreFeminin,
            titreNeutre,
            titre
        );
    }

    public string GetDescription(ENUM_PERSONNAGE_Genre genre)
    {
        return SelectionnerTexteSelonGenre(
            genre,
            descriptionMasculine,
            descriptionFeminine,
            descriptionNeutre,
            description
        );
    }

    public override string GetTitreAffiche()
    {
        return !string.IsNullOrWhiteSpace(titreNeutre)
            ? titreNeutre
            : titre;
    }

    public override string GetDescriptionAffiche()
    {
        return !string.IsNullOrWhiteSpace(descriptionNeutre)
            ? descriptionNeutre
            : description;
    }

    private static string SelectionnerTexteSelonGenre(
        ENUM_PERSONNAGE_Genre genre,
        string masculin,
        string feminin,
        string neutre,
        string fallback)
    {
        switch (genre)
        {
            case ENUM_PERSONNAGE_Genre.Masculin:
                if (!string.IsNullOrWhiteSpace(masculin))
                    return masculin;
                break;

            case ENUM_PERSONNAGE_Genre.Feminin:
                if (!string.IsNullOrWhiteSpace(feminin))
                    return feminin;
                break;
        }

        if (!string.IsNullOrWhiteSpace(neutre))
            return neutre;

        return fallback;
    }
}