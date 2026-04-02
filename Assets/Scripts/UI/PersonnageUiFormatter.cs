using UnityEngine;

public static class PersonnageUiFormatter
{
    public static string FormatNiveau(int niveau)
    {
        return $"Lv. {niveau}";
    }

    public static string FormatNom(string nom)
    {
        return string.IsNullOrEmpty(nom) ? "Aucun nom" : nom;
    }

    public static string FormatPrenom(string prenom)
    {
        return string.IsNullOrEmpty(prenom) ? "" : prenom;
    }

    public static string FormatRole(string role)
    {
        return string.IsNullOrEmpty(role) ? "Membre" : role;
    }

    public static string FormatEtoiles(int nbEtoiles)
    {
        nbEtoiles = Mathf.Clamp(nbEtoiles, 1, 5);
        return new string('★', nbEtoiles) + new string('☆', 5 - nbEtoiles);
    }

    public static string FormatEtoilesRichText(int nbEtoiles)
    {
        nbEtoiles = Mathf.Clamp(nbEtoiles, 1, 5);

        string pleines = "<color=#FFD700>" + new string('★', nbEtoiles) + "</color>";
        string vides = "<color=#666666>" + new string('☆', 5 - nbEtoiles) + "</color>";

        return pleines + vides;
    }
}