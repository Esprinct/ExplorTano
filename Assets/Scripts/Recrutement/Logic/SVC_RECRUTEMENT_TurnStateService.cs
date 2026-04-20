using System.Collections.Generic;

public class SYS_RecruitmentTurnStateService
{
    public bool PeutRecruterCeTour(DATA_JOUEUR joueur, HashSet<ENUM_Compagnie> compagniesAyantRecruteCeTour)
    {
        if (joueur == null)
            return false;

        if (compagniesAyantRecruteCeTour == null)
            return true;

        return !compagniesAyantRecruteCeTour.Contains(joueur.compagnie);
    }

    public void MarquerRecrutementEffectue(DATA_JOUEUR joueur, HashSet<ENUM_Compagnie> compagniesAyantRecruteCeTour)
    {
        if (joueur == null || compagniesAyantRecruteCeTour == null)
            return;

        compagniesAyantRecruteCeTour.Add(joueur.compagnie);
    }

    public void ResetRecrutementTour(HashSet<ENUM_Compagnie> compagniesAyantRecruteCeTour)
    {
        compagniesAyantRecruteCeTour?.Clear();
    }
}