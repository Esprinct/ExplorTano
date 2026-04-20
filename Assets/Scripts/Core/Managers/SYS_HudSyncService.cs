using UnityEngine;

public class SYS_HudSyncService
{
    public void SynchroniserHudAvecJoueurHumain(
        DATA_JOUEUR_HUD joueurData,
        DATA_JOUEUR humain,
        SCOBJ_DIRIGEANT dirigeantHumain)
    {
        if (humain == null)
        {
            Debug.LogWarning("SynchroniserHudAvecJoueurHumain : aucun joueur humain trouvé.");
            return;
        }

        if (joueurData == null)
        {
            Debug.LogWarning("SynchroniserHudAvecJoueurHumain : joueurData est null.");
            return;
        }

        joueurData.dirigeant = dirigeantHumain;

        humain.SynchroniserCompagnieDepuisDirigeant();

        joueurData.etriniumTotal = Mathf.RoundToInt(humain.etrinium);
        joueurData.prestige = Mathf.RoundToInt(humain.prestige);
        joueurData.provincesControlees = humain.provincesControlees;
        joueurData.etriniumParTour = Mathf.RoundToInt(humain.etriniumParTour);
        joueurData.etriniumBreakdown = humain.etriniumBreakdown ?? new EtriniumBreakdownData();

        joueurData.portraitDirigeant = humain.GetPortraitDirigeant();
        joueurData.logoCompagnie = humain.GetLogoCompagnie();
        joueurData.nomDirigeant = humain.GetNomDirigeant();
        joueurData.niveauDirigeant = humain.GetNiveauDirigeant();

        Debug.Log(
            $"HUD Dirigeant sync | joueur={humain.nomJoueur} | " +
            $"compagnie={humain.compagnie} | " +
            $"dirigeant={joueurData.nomDirigeant} | " +
            $"portrait={(joueurData.portraitDirigeant != null)} | " +
            $"logo={(joueurData.logoCompagnie != null)} | " +
            $"niveau={joueurData.niveauDirigeant}"
        );

        joueurData.dirigeant = dirigeantHumain;

        if (dirigeantHumain != null)
        {
            joueurData.xpDirigeant = dirigeantHumain.xpDirigeant;
            joueurData.xpDirigeantPourNiveauSuivant = dirigeantHumain.xpPourNiveauSuivant;
        }
        else
        {
            joueurData.xpDirigeant = 0;
            joueurData.xpDirigeantPourNiveauSuivant = 0;
        }
    }
}