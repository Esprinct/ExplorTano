public static class UI_PROVINCE_StateFactory
{
    public static STATE_PROVINCE CreerDepuisData(SCOBJ_PROVINCE data)
    {
        if (data == null)
            return null;

        return new STATE_PROVINCE
        {
            data = data,
            proprietaireActuel = null,
            estClaim = false,
            explorationEnCours = false,
            toursRestants = 0,
            influenceMaizin = data.influenceMaizinInitiale,
            influenceKinia = data.influenceKiniaInitiale,
            influenceJoho = data.influenceJohoInitiale,
            influenceAutre = data.influenceAutreInitiale
        };
    }
}