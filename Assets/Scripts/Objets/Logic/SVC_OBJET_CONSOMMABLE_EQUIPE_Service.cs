using System.Collections.Generic;

public static class SVC_OBJET_CONSOMMABLE_EQUIPE_Service
{
    public static bool ADesConsommables(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.consommables == null)
            return false;

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in equipe.consommables)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (stack.quantite > 0)
                return true;
        }

        return false;
    }

    public static bool ConsommerPourExpedition(STATE_EQUIPE equipe)
    {
        if (equipe == null || equipe.consommables == null)
            return false;

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in equipe.consommables)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (stack.quantite <= 0)
                continue;

            int cout = stack.objet.quantiteConsommeeParExpedition;
            if (cout <= 0)
                cout = 1;

            if (stack.quantite >= cout)
            {
                stack.quantite -= cout;
                return true;
            }
        }

        return false;
    }

    public static void AjouterConsommable(STATE_EQUIPE equipe, SCOBJ_OBJET_CONSOMMABLE objet, int quantite)
    {
        if (equipe == null || objet == null || quantite <= 0)
            return;

        if (equipe.consommables == null)
        {
            equipe.consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>();
        }

        foreach (DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack in equipe.consommables)
        {
            if (stack == null || stack.objet == null)
                continue;

            if (stack.objet == objet)
            {
                stack.quantite += quantite;
                return;
            }
        }

        equipe.consommables.Add(new DATA_OBJET_CONSOMMABLE_EQUIPE_Stack
        {
            objet = objet,
            quantite = quantite
        });
    }
}