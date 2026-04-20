[System.Serializable]
public class DATA_PERSONNAGE_DisplayContext
{
    public ENUM_Compagnie compagnie = ENUM_Compagnie.Aucune;
    public STATE_PERSONNAGE state = null;

    public static DATA_PERSONNAGE_DisplayContext Default => new();

    public DATA_PERSONNAGE_DisplayContext()
    {
    }

    public DATA_PERSONNAGE_DisplayContext(ENUM_Compagnie compagnie, STATE_PERSONNAGE state = null)
    {
        this.compagnie = compagnie;
        this.state = state;
    }
}