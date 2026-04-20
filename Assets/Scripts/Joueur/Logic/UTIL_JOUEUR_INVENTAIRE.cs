using System;
using System.Collections.Generic;
using UnityEngine;

public static class UTIL_JOUEUR_INVENTAIRE
{
    public static void AjouterObjetAuJoueur(DATA_JOUEUR joueur, SCOBJ_OBJET objet, int quantite = 1)
    {
        if (joueur == null)
        {
            Debug.LogWarning("AjouterObjetAuJoueur : joueur null");
            return;
        }

        if (objet == null)
        {
            Debug.LogWarning("AjouterObjetAuJoueur : objet null");
            return;
        }

        if (quantite <= 0)
        {
            Debug.LogWarning("AjouterObjetAuJoueur : quantité invalide");
            return;
        }

        if (objet is SCOBJ_OBJET_CONSOMMABLE consommable)
        {
            AjouterConsommableAuJoueur(joueur, consommable, quantite);
            return;
        }

        if (joueur.objetsPossedes == null)
            joueur.objetsPossedes = new List<SCOBJ_OBJET>();

        for (int i = 0; i < quantite; i++)
        {
            SCOBJ_OBJET instanceObjet = CreerInstanceInventaire(objet);
            joueur.objetsPossedes.Add(instanceObjet);

            Debug.Log(
                $"Objet ajouté au joueur : {instanceObjet.nom} | " +
                $"instanceId={instanceObjet.idUnique}"
            );
        }
    }

    public static void AjouterConsommableAuJoueur(DATA_JOUEUR joueur, SCOBJ_OBJET_CONSOMMABLE objet, int quantite = 1)
    {
        if (joueur == null)
        {
            Debug.LogWarning("AjouterConsommableAuJoueur : joueur null");
            return;
        }

        if (objet == null)
        {
            Debug.LogWarning("AjouterConsommableAuJoueur : objet null");
            return;
        }

        if (quantite <= 0)
        {
            Debug.LogWarning("AjouterConsommableAuJoueur : quantité invalide");
            return;
        }

        if (joueur.consommablesPossedes == null)
            joueur.consommablesPossedes = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();

        DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stackExistant = TrouverStackConsommable(joueur, objet);

        if (stackExistant != null)
        {
            stackExistant.quantite += quantite;
            Debug.Log($"Consommable empilé : {objet.nom} +{quantite} => {stackExistant.quantite}");
            return;
        }

        joueur.consommablesPossedes.Add(new DATA_OBJET_CONSOMMABLE_EQUIPE_Stack
        {
            objet = objet,
            quantite = quantite
        });

        Debug.Log($"Nouveau consommable ajouté : {objet.nom} x{quantite}");
    }

    public static bool RetirerConsommableAuJoueur(DATA_JOUEUR joueur, SCOBJ_OBJET_CONSOMMABLE objet, int quantite = 1)
    {
        if (joueur == null || objet == null || quantite <= 0)
            return false;

        if (joueur.consommablesPossedes == null)
            return false;

        DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack = TrouverStackConsommable(joueur, objet);
        if (stack == null)
            return false;

        if (stack.quantite < quantite)
            return false;

        stack.quantite -= quantite;

        if (stack.quantite <= 0)
        {
            joueur.consommablesPossedes.Remove(stack);
        }

        Debug.Log($"Consommable retiré : {objet.nom} x{quantite}");
        return true;
    }

    public static DATA_OBJET_CONSOMMABLE_EQUIPE_Stack TrouverStackConsommable(DATA_JOUEUR joueur, SCOBJ_OBJET_CONSOMMABLE objet)
    {
        if (joueur == null || objet == null || joueur.consommablesPossedes == null)
            return null;

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in joueur.consommablesPossedes)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (ReferenceEquals(stack.objet, objet))
                return stack;

            if (!string.IsNullOrWhiteSpace(stack.objet.idUnique) &&
                stack.objet.idUnique == objet.idUnique)
            {
                return stack;
            }
        }

        return null;
    }

    private static SCOBJ_OBJET CreerInstanceInventaire(SCOBJ_OBJET objetSource)
    {
        if (objetSource == null)
            return null;

        SCOBJ_OBJET instance = ScriptableObject.Instantiate(objetSource);

        string baseId = string.IsNullOrWhiteSpace(objetSource.idUnique)
            ? objetSource.name
            : objetSource.idUnique;

        instance.idUnique = $"{baseId}__{Guid.NewGuid():N}";
        instance.name = $"{objetSource.name}_Runtime_{instance.idUnique}";

        return instance;
    }
}