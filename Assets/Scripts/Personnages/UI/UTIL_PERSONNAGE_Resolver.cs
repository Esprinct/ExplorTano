public static class UTIL_PERSONNAGE_Resolver
{
    public static SCOBJ_Personnage ResolveById(SYS_GameManager gameManager, string idUnique)
    {
        if (gameManager == null || string.IsNullOrWhiteSpace(idUnique))
            return null;

        DATA_JOUEUR humain = gameManager.GetHumanPlayer();
        if (humain != null && humain.personnagesRecrutes != null)
        {
            foreach (SCOBJ_Personnage personnage in humain.personnagesRecrutes)
            {
                if (personnage == null)
                    continue;

                if (personnage.idUnique == idUnique)
                    return personnage;
            }
        }

        if (gameManager.EquipesRuntime != null)
        {
            foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
            {
                if (equipe == null || equipe.membresActuels == null)
                    continue;

                foreach (SCOBJ_Personnage personnage in equipe.membresActuels)
                {
                    if (personnage == null)
                        continue;

                    if (personnage.idUnique == idUnique)
                        return personnage;
                }
            }
        }

        return null;
    }

    public static SCOBJ_Personnage ResolveFromDetailData(SYS_GameManager gameManager, DATA_PERSONNAGE_Detail detailData)
    {
        if (detailData == null)
            return null;

        return ResolveById(gameManager, detailData.idUnique);
    }
}