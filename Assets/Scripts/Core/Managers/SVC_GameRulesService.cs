using System.Collections.Generic;

public class SYS_GameRulesService
{
    public int GetNombreEquipesJoueur(DATA_JOUEUR joueur)
    {
        if (joueur == null || joueur.equipes == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe != null)
                total++;
        }

        return total;
    }

    public bool PeutCreerEquipe(DATA_JOUEUR joueur, int maxEquipesParJoueur, int coutCreationEquipe)
    {
        if (joueur == null)
            return false;

        if (GetNombreEquipesJoueur(joueur) >= maxEquipesParJoueur)
            return false;

        if (joueur.etrinium < coutCreationEquipe)
            return false;

        return true;
    }

    public bool PeutCreerEquipeCeTour(DATA_JOUEUR joueur, int maxEquipesParJoueur, int coutCreationEquipe)
    {
        return PeutCreerEquipe(joueur, maxEquipesParJoueur, coutCreationEquipe);
    }

    public bool PeutRecruterCeTour(bool aDejaRecruteCeTour)
    {
        return !aDejaRecruteCeTour;
    }

    public void MarquerRecrutementEffectue(ref bool aDejaRecruteCeTour)
    {
        aDejaRecruteCeTour = true;
    }

    public void ResetRecrutementTour(ref bool aDejaRecruteCeTour)
    {
        aDejaRecruteCeTour = false;
    }

    public bool EstPersonnageDansUneEquipe(SCOBJ_Personnage personnage, List<STATE_EQUIPE> equipesRuntime)
    {
        if (personnage == null || equipesRuntime == null)
            return false;

        foreach (STATE_EQUIPE equipe in equipesRuntime)
        {
            if (equipe == null || equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
    }
}