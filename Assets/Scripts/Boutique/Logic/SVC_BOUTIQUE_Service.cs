using System;
using System.Collections.Generic;
using UnityEngine;

public static class SVC_BOUTIQUE_Service
{
    public static bool EstStackable(SCOBJ_OBJET objet)
    {
        return objet is SCOBJ_OBJET_CONSOMMABLE;
    }

    public static int GetQuantitePossedee(DATA_JOUEUR joueur, SCOBJ_OBJET objet)
    {
        if (joueur == null || objet == null)
            return 0;

        if (objet is SCOBJ_OBJET_CONSOMMABLE consommable)
        {
            if (joueur.consommablesPossedes == null)
                return 0;

            int total = 0;

            foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in joueur.consommablesPossedes)
            {
                if (stack == null || stack.objet == null)
                    continue;

                if (MemeObjet(stack.objet, consommable))
                    total += stack.quantite;
            }

            return total;
        }

        if (joueur.objetsPossedes == null)
            return 0;

        int count = 0;

        foreach (SCOBJ_OBJET objetPossede in joueur.objetsPossedes)
        {
            if (objetPossede == null)
                continue;

            if (MemeModele(objetPossede, objet))
                count++;
        }

        return count;
    }

    public static int GetPrixUnitaire(DATA_BOUTIQUE_Offre offre)
    {
        if (offre == null)
            return 0;

        return Mathf.Max(0, offre.GetPrix());
    }

    public static int GetPrixTotal(DATA_BOUTIQUE_Offre offre, int quantite)
    {
        return GetPrixUnitaire(offre) * Mathf.Max(1, quantite);
    }

    public static int GetQuantiteMaxAchetable(DATA_JOUEUR joueur, DATA_BOUTIQUE_Offre offre, bool autoriserAchatMultipleNonStackable = true)
    {
        if (joueur == null || offre == null || offre.objet == null)
            return 0;

        int prixUnitaire = GetPrixUnitaire(offre);
        if (prixUnitaire <= 0)
            return 99;

        int maxSelonArgent = Mathf.FloorToInt(joueur.etrinium / prixUnitaire);
        maxSelonArgent = Mathf.Max(0, maxSelonArgent);

        if (!EstStackable(offre.objet) && !autoriserAchatMultipleNonStackable)
            return Mathf.Min(1, maxSelonArgent);

        return maxSelonArgent;
    }

    public static bool PeutAcheter(DATA_JOUEUR joueur, DATA_BOUTIQUE_Offre offre, int quantite)
    {
        if (joueur == null || offre == null || offre.objet == null)
            return false;

        quantite = Mathf.Max(1, quantite);
        int prixTotal = GetPrixTotal(offre, quantite);

        return joueur.etrinium >= prixTotal;
    }

    public static bool Acheter(DATA_JOUEUR joueur, DATA_BOUTIQUE_Offre offre, int quantite)
    {
        if (!PeutAcheter(joueur, offre, quantite))
            return false;

        quantite = Mathf.Max(1, quantite);

        int prixTotal = GetPrixTotal(offre, quantite);
        joueur.etrinium -= prixTotal;

        AjouterObjetAuJoueur(joueur, offre.objet, quantite);

        Debug.Log(
            $"[BOUTIQUE] Achat : {offre.objet.nom} x{quantite} pour {prixTotal} étrinium"
        );

        return true;
    }

    private static void AjouterObjetAuJoueur(DATA_JOUEUR joueur, SCOBJ_OBJET objetSource, int quantite)
    {
        if (joueur == null || objetSource == null)
            return;

        quantite = Mathf.Max(1, quantite);

        if (objetSource is SCOBJ_OBJET_CONSOMMABLE consommable)
        {
            AjouterConsommable(joueur, consommable, quantite);
            return;
        }

        joueur.objetsPossedes ??= new List<SCOBJ_OBJET>();

        for (int i = 0; i < quantite; i++)
        {
            SCOBJ_OBJET instance = CreerInstanceInventaire(objetSource);
            if (instance != null)
                joueur.objetsPossedes.Add(instance);
        }
    }

    private static void AjouterConsommable(DATA_JOUEUR joueur, SCOBJ_OBJET_CONSOMMABLE objet, int quantite)
    {
        joueur.consommablesPossedes ??= new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in joueur.consommablesPossedes)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (MemeObjet(stack.objet, objet))
            {
                stack.quantite += quantite;
                return;
            }
        }

        joueur.consommablesPossedes.Add(new DATA_OBJET_CONSOMMABLE_EQUIPE_Stack
        {
            objet = objet,
            quantite = quantite
        });
    }

    private static SCOBJ_OBJET CreerInstanceInventaire(SCOBJ_OBJET objetSource)
    {
        if (objetSource == null)
            return null;

        SCOBJ_OBJET instance = ScriptableObject.Instantiate(objetSource);

        string baseId = string.IsNullOrWhiteSpace(objetSource.idUnique)
            ? objetSource.name
            : objetSource.idUnique;

        string runtimeId = $"{baseId}__{Guid.NewGuid():N}";

        instance.idUnique = runtimeId;
        instance.name = $"{objetSource.name}_Runtime_{runtimeId}";

        return instance;
    }

    private static bool MemeObjet(SCOBJ_OBJET a, SCOBJ_OBJET b)
    {
        if (a == null || b == null)
            return false;

        if (ReferenceEquals(a, b))
            return true;

        if (!string.IsNullOrWhiteSpace(a.idUnique) &&
            !string.IsNullOrWhiteSpace(b.idUnique) &&
            a.idUnique == b.idUnique)
            return true;

        return false;
    }

    private static bool MemeModele(SCOBJ_OBJET objetPossede, SCOBJ_OBJET modeleBoutique)
    {
        if (objetPossede == null || modeleBoutique == null)
            return false;

        if (ReferenceEquals(objetPossede, modeleBoutique))
            return true;

        if (!string.IsNullOrWhiteSpace(modeleBoutique.idUnique) &&
            !string.IsNullOrWhiteSpace(objetPossede.idUnique))
        {
            if (objetPossede.idUnique == modeleBoutique.idUnique)
                return true;

            if (objetPossede.idUnique.StartsWith(modeleBoutique.idUnique + "__"))
                return true;
        }

        return objetPossede.name.StartsWith(modeleBoutique.name);
    }
}