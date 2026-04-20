using System.Collections.Generic;
using UnityEngine;

public class SYS_IA_PlayerSystem
{
    private const int TailleMaxEquipe = 12;
private readonly SYS_IA_ProgressionEquipementSystem progressionEquipementSystem = new();
 public void JouerTourIA(SYS_GameManager gameManager, DATA_JOUEUR joueur)
{
    if (gameManager == null || joueur == null || joueur.estHumain)
        return;

    progressionEquipementSystem.OptimiserRosterIA(gameManager, joueur);

    TenterRecrutement(gameManager, joueur);
    TenterCreationEquipe(gameManager, joueur);
    CompleterEquipes(gameManager, joueur);
    AffecterEquipesAuxProvinces(gameManager, joueur);
    LancerExplorations(gameManager, joueur);

    Debug.Log($"[IA] Tour joué : {joueur.nomJoueur} | personnalité={joueur.personnaliteIA}");
}

    private void TenterRecrutement(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return;

        if (gameManager.SYS_RecrutementSystem == null)
            return;

        if (!gameManager.PeutRecruterCeTour(joueur))
            return;

        gameManager.SYS_RecrutementSystem.FaireJouerEnchereIAPourJoueur(gameManager, joueur);
        gameManager.MarquerRecrutementEffectue(joueur);
    }

   private void TenterCreationEquipe(SYS_GameManager gameManager, DATA_JOUEUR joueur)
{
    if (gameManager == null || joueur == null)
        return;

    int personnagesLibres = CompterPersonnagesLibres(gameManager, joueur);
    int nbEquipes = GetNombreEquipesValides(joueur);

    bool aUneEquipeExplorable = false;

    if (joueur.equipes != null)
    {
        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            int nbMembres = equipe.membresActuels != null ? equipe.membresActuels.Count : 0;

            if (!equipe.explorationEnCours && nbMembres >= 1)
            {
                aUneEquipeExplorable = true;
                break;
            }
        }
    }

    bool doitCreerEquipe =
        (nbEquipes == 0 && personnagesLibres > 0) ||
        (!aUneEquipeExplorable && personnagesLibres > 0) ||
        personnagesLibres >= GetTailleCibleEquipe(joueur);

    if (!doitCreerEquipe)
        return;

    if (!gameManager.PeutCreerEquipe(joueur))
        return;

    STATE_EQUIPE nouvelleEquipe = ConstruireNouvelleEquipe(gameManager, joueur);
    if (nouvelleEquipe == null)
        return;

    int coutCreation = gameManager.GetCoutCreationEquipe(joueur);
    joueur.etrinium -= coutCreation;

    gameManager.EquipesRuntime.Add(nouvelleEquipe);

    joueur.equipes ??= new List<STATE_EQUIPE>();
    joueur.equipes.Add(nouvelleEquipe);

    Debug.Log($"[IA] Création équipe | joueur={joueur.nomJoueur} | équipe={nouvelleEquipe.data?.nomEquipe}");
}
private void GererCreationEquipesIA(SYS_GameManager gameManager, DATA_JOUEUR joueur)
{
    if (gameManager == null || joueur == null)
        return;

    int nbEquipes = gameManager.GetNombreEquipesJoueur(joueur);
    int coutCreation = gameManager.GetCoutCreationEquipe(joueur);

    if (coutCreation <= 0)
        return;

    if (nbEquipes < 2 && joueur.etrinium >= coutCreation)
    {
        gameManager.CreerEquipePourIA(joueur);
        return;
    }

    if (nbEquipes < 3 && joueur.etrinium >= coutCreation * 1.5f)
    {
        gameManager.CreerEquipePourIA(joueur);
        return;
    }

    if (nbEquipes < gameManager.MaxEquipesParJoueur && joueur.etrinium >= coutCreation * 2.5f)
    {
        gameManager.CreerEquipePourIA(joueur);
    }
}
    private void CompleterEquipes(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        List<SCOBJ_Personnage> personnagesLibres = RecupererPersonnagesLibres(gameManager, joueur);
        if (personnagesLibres.Count == 0)
            return;

        int tailleCibleEquipe = GetTailleCibleEquipe(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.explorationEnCours)
                continue;

            equipe.membresActuels ??= new List<SCOBJ_Personnage>();
            equipe.membresActuels.RemoveAll(p => p == null);

            while (equipe.membresActuels.Count < tailleCibleEquipe &&
                   equipe.membresActuels.Count < TailleMaxEquipe &&
                   personnagesLibres.Count > 0)
            {
                SCOBJ_Personnage meilleur = ChoisirMeilleurPersonnagePourEquipe(personnagesLibres, joueur);
                if (meilleur == null)
                    break;

                equipe.membresActuels.Add(meilleur);
                personnagesLibres.Remove(meilleur);
            }
        }
    }

    private void AffecterEquipesAuxProvinces(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = GetTailleMinEquipePourExploration(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.explorationEnCours)
                continue;

            if (equipe.membresActuels == null || equipe.membresActuels.Count < tailleMinEquipe)
                continue;

            bool garderProvinceActuelle = PeutGarderProvinceActuelle(joueur, equipe);
            if (garderProvinceActuelle)
                continue;

            STATE_PROVINCE cible = ChoisirProvincePourEquipe(gameManager, joueur, equipe);
            if (cible == null)
                continue;

            equipe.provinceAffectee = cible;
            equipe.explorationTerminee = false;

            Debug.Log(
                $"[IA] Affectation | joueur={joueur.nomJoueur} | personnalité={joueur.personnaliteIA} | " +
                $"équipe={equipe.data?.nomEquipe} | province={cible.data?.nom}"
            );
        }
    }

    private void LancerExplorations(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.equipes == null)
            return;

        int tailleMinEquipe = GetTailleMinEquipePourExploration(joueur);
        float ratioMinimalBudget = GetRatioMinimalBudgetPourExplorer(joueur);

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null)
                continue;

            if (equipe.explorationEnCours)
                continue;

            if (equipe.provinceAffectee == null || equipe.provinceAffectee.data == null)
                continue;

            if (equipe.provinceAffectee.estClaim)
                continue;

            if (equipe.membresActuels == null || equipe.membresActuels.Count < tailleMinEquipe)
                continue;

            int coutExploration = CalculerCoutExploration(gameManager, equipe);
            if (coutExploration <= 0)
                continue;

            if (joueur.etrinium < coutExploration)
                continue;

            if (joueur.etriniumParTour < 0f && joueur.etrinium < coutExploration * ratioMinimalBudget)
                continue;

            gameManager.DemarrerExploration(equipe, 0);

            Debug.Log(
                $"[IA] Exploration lancée | joueur={joueur.nomJoueur} | personnalité={joueur.personnaliteIA} | " +
                $"équipe={equipe.data?.nomEquipe} | province={equipe.provinceAffectee.data?.nom} | coût={coutExploration}"
            );
        }
    }

    private bool PeutGarderProvinceActuelle(DATA_JOUEUR joueur, STATE_EQUIPE equipe)
    {
        if (joueur == null || equipe == null || equipe.provinceAffectee == null)
            return false;

        STATE_PROVINCE province = equipe.provinceAffectee;
        if (province.data == null)
            return false;

        if (province.estClaim)
            return false;

        float influenceIA = GetInfluenceCompagnie(province, joueur.compagnie);

        if (province.influenceAutre > 0.01f)
            return true;

        if (influenceIA > 0f && influenceIA < 50f)
            return true;

        return false;
    }

    private STATE_EQUIPE ConstruireNouvelleEquipe(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null)
            return null;

        SCOBJ_EQUIPE dataEquipe = ScriptableObject.CreateInstance<SCOBJ_EQUIPE>();
        if (dataEquipe == null)
            return null;

        int index = GetNombreEquipesValides(joueur) + 1;

        dataEquipe.name = $"Equipe_IA_{joueur.compagnie}_{index}";
        dataEquipe.nomEquipe = $"Équipe {joueur.compagnie} {index}";
        dataEquipe.niveauDeBase = 1;
        dataEquipe.membres = new List<SCOBJ_Personnage>();

        return new STATE_EQUIPE
        {
            data = dataEquipe,
            compagnie = joueur.compagnie,
            niveauActuel = 1,
            provinceAffectee = null,
            explorationEnCours = false,
            explorationTerminee = false,
            toursRestants = 0,
            toursTotaux = 0,
            membresActuels = new List<SCOBJ_Personnage>(),
            affectationAutomatique = true,
            lancementExplorationAutomatique = true,
            objetsEquipes = new List<SCOBJ_OBJET_EQUIPPABLE>(),
            consommables = new List<DATA_OBJET_CONSOMMABLE_EQUIPE_Stack>()
        };
    }

    private int GetNombreEquipesValides(DATA_JOUEUR joueur)
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

    private int CompterPersonnagesLibres(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        return RecupererPersonnagesLibres(gameManager, joueur).Count;
    }

    private List<SCOBJ_Personnage> RecupererPersonnagesLibres(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        List<SCOBJ_Personnage> resultat = new();

        if (gameManager == null || joueur == null || joueur.personnagesRecrutes == null)
            return resultat;

        foreach (SCOBJ_Personnage personnage in joueur.personnagesRecrutes)
        {
            if (personnage == null)
                continue;

            if (EstPersonnageDejaAffecte(joueur, personnage))
                continue;

            resultat.Add(personnage);
        }

        return resultat;
    }

    private bool EstPersonnageDejaAffecte(DATA_JOUEUR joueur, SCOBJ_Personnage personnage)
    {
        if (joueur == null || personnage == null || joueur.equipes == null)
            return false;

        foreach (STATE_EQUIPE equipe in joueur.equipes)
        {
            if (equipe == null || equipe.membresActuels == null)
                continue;

            if (equipe.membresActuels.Contains(personnage))
                return true;
        }

        return false;
    }

    private SCOBJ_Personnage ChoisirMeilleurPersonnagePourEquipe(List<SCOBJ_Personnage> candidats, DATA_JOUEUR joueur)
    {
        if (candidats == null || candidats.Count == 0 || joueur == null)
            return null;

        SCOBJ_Personnage meilleur = null;
        float meilleurScore = float.MinValue;

        foreach (SCOBJ_Personnage personnage in candidats)
        {
            if (personnage == null)
                continue;

            float score = EvaluerPersonnagePourIA(personnage, joueur);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleur = personnage;
            }
        }

        return meilleur;
    }

    private float EvaluerPersonnagePourIA(SCOBJ_Personnage personnage, DATA_JOUEUR joueur)
    {
        if (personnage == null || joueur == null)
            return float.MinValue;

        int force = CALS_PERSONNAGE_STATS_Calculator.GetForceEffective(personnage, joueur.compagnie);
        int intelligence = CALS_PERSONNAGE_STATS_Calculator.GetIntelligenceEffective(personnage, joueur.compagnie);
        int dexterite = CALS_PERSONNAGE_STATS_Calculator.GetDexteriteEffective(personnage, joueur.compagnie);
        int endurance = CALS_PERSONNAGE_STATS_Calculator.GetEnduranceEffective(personnage, joueur.compagnie);

        float score = personnage.rareteEtoiles * 100f;

        switch (joueur.personnaliteIA)
        {
          case ENUM_IA_Personnalite.Agressive:
    score += force * 1.5f;
    score += endurance * 1.2f;
    score += dexterite * 0.9f;
    score += intelligence * 0.5f;
    score -= personnage.coutParTour * 0.12f;
    break;

            case ENUM_IA_Personnalite.Prestige:
                score += force * 1.4f;
                score += intelligence * 1.2f;
                score += dexterite * 0.8f;
                score += endurance * 0.7f;
                break;

            case ENUM_IA_Personnalite.Economique:
                score += intelligence * 1.4f;
                score += endurance * 1.3f;
                score += dexterite * 1.0f;
                score += force * 0.7f;
                score -= personnage.coutParTour * 0.08f;
                break;

            case ENUM_IA_Personnalite.Expansionniste:
                score += dexterite * 1.5f;
                score += endurance * 1.2f;
                score += force * 0.9f;
                score += intelligence * 0.9f;
                break;

            case ENUM_IA_Personnalite.Opportuniste:
                score += intelligence * 1.3f;
                score += dexterite * 1.3f;
                score += force * 0.9f;
                score += endurance * 0.9f;
                break;

            case ENUM_IA_Personnalite.Equilibree:
            default:
                score += force;
                score += intelligence;
                score += dexterite;
                score += endurance;
                break;
        }

        if (personnage.aPreferenceCompagnie && personnage.compagniePreferee == joueur.compagnie)
        {
            score += 150f;
        }

        return score;
    }

    private STATE_PROVINCE ChoisirProvincePourEquipe(
        SYS_GameManager gameManager,
        DATA_JOUEUR joueur,
        STATE_EQUIPE equipe)
    {
        if (gameManager == null || joueur == null || equipe == null)
            return null;

        STATE_PROVINCE meilleureProvince = null;
        float meilleurScore = float.MinValue;

        foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
        {
            if (province == null || province.data == null)
                continue;

            if (province.estClaim)
                continue;

            float score = EvaluerProvincePourCompagnie(gameManager, province, joueur, equipe);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleureProvince = province;
            }
        }

        return meilleureProvince;
    }
private float GetInfluenceAdverseDominante(STATE_PROVINCE province, ENUM_Compagnie compagnie)
{
    if (province == null)
        return 0f;

    float maizin = compagnie == ENUM_Compagnie.Maizin ? 0f : province.influenceMaizin;
    float kinia = compagnie == ENUM_Compagnie.Kinia ? 0f : province.influenceKinia;
    float joho = compagnie == ENUM_Compagnie.Joho ? 0f : province.influenceJoho;

    return Mathf.Max(maizin, Mathf.Max(kinia, joho));
}
  private float EvaluerProvincePourCompagnie(
    SYS_GameManager gameManager,
    STATE_PROVINCE province,
    DATA_JOUEUR joueur,
    STATE_EQUIPE equipe)
{
    if (province == null || province.data == null || joueur == null)
        return float.MinValue;

    float score = 0f;

    float etrinium = province.data.etrinium;
    float prestige = province.data.prestige;
    float politique = province.data.poidsPolitique;
    float accessibilite = province.data.accesibilite;

    float influenceAutre = province.influenceAutre;
    float influenceIA = GetInfluenceCompagnie(province, joueur.compagnie);
    float influenceAdverse = GetInfluenceAdverseDominante(province, joueur.compagnie);

    bool estClaim = province.estClaim;
    bool claimParEnnemi =
        estClaim &&
        province.proprietaireActuel.HasValue &&
        province.proprietaireActuel.Value != joueur.compagnie;

    bool claimParIA =
        estClaim &&
        province.proprietaireActuel.HasValue &&
        province.proprietaireActuel.Value == joueur.compagnie;
bool resteDesZonesNeutres = ExisteEncoreDesZonesNeutresInteressantes(gameManager);

if (resteDesZonesNeutres && claimParEnnemi)
{
    return float.MinValue;
}
    // 1) Base économique / stratégique
    switch (joueur.personnaliteIA)
    {
        case ENUM_IA_Personnalite.Agressive:
            score += etrinium * 1.6f;
            score += prestige * 1.2f;
            score += politique * 1.8f;
            score += accessibilite * 0.8f;
            break;

        case ENUM_IA_Personnalite.Prestige:
            score += etrinium * 1.0f;
            score += prestige * 2.8f;
            score += politique * 1.5f;
            score += accessibilite * 0.7f;
            break;

        case ENUM_IA_Personnalite.Economique:
            score += etrinium * 3.0f;
            score += prestige * 0.9f;
            score += politique * 0.9f;
            score += accessibilite * 1.2f;
            break;

        case ENUM_IA_Personnalite.Expansionniste:
            score += etrinium * 1.8f;
            score += prestige * 1.3f;
            score += politique * 1.1f;
            score += accessibilite * 1.0f;
            break;

        case ENUM_IA_Personnalite.Opportuniste:
            score += etrinium * 1.8f;
            score += prestige * 1.8f;
            score += politique * 1.4f;
            score += accessibilite * 0.8f;
            break;

        case ENUM_IA_Personnalite.Equilibree:
        default:
            score += etrinium * 2.0f;
            score += prestige * 1.8f;
            score += politique * 1.3f;
            score += accessibilite * 0.9f;
            break;
    }

    // 2) PRIORITÉ ABSOLUE AUX ZONES NEUTRES
    if (!estClaim)
    {
        score += 250f;                  // énorme bonus de base
        score += influenceAutre * 5.0f; // très grosse priorité aux zones encore neutres
        score += influenceIA * 1.5f;    // finir ce qu'on a déjà commencé
        score -= influenceAdverse * 0.8f;
    }

    // 3) Bonus spécial si la province est "facile à prendre"
    if (!estClaim && influenceAutre > 25f)
    {
        score += 120f;
    }

    // 4) Bonus si on a déjà une petite présence, pour favoriser le snowball
    if (!estClaim && influenceIA > 0f)
    {
        score += influenceIA * 2.5f;
    }

    // 5) Attaquer l'ennemi seulement en second rideau
    if (claimParEnnemi)
    {
        score -= 180f;                 // forte pénalité de base
        score += influenceAdverse * 0.4f;

        // on attaque seulement si on a déjà une présence correcte
        if (influenceIA > 20f)
        {
            score += 80f;
        }
    }

    // 6) Ne jamais retarget une province déjà sécurisée par soi
    if (claimParIA)
    {
        score -= 1000f;
    }

    // 7) Très grosse pénalité si la même équipe reste collée à sa province
    if (equipe != null && equipe.provinceAffectee == province)
    {
        score -= 150f;
    }

    // 8) Très grosse pénalité si plusieurs équipes alliées visent déjà la même province
    int nbEquipesAllieesDejaSurPlace =
        CompterEquipesAllieesSurProvince(gameManager, joueur.compagnie, province, equipe);
    score -= nbEquipesAllieesDejaSurPlace * 140f;

    // 9) Léger bruit pour casser les égalités parfaites
    score += Random.Range(0f, 8f);

    return score;
}
private bool ExisteEncoreDesZonesNeutresInteressantes(SYS_GameManager gameManager)
{
    if (gameManager == null)
        return false;

    foreach (STATE_PROVINCE province in gameManager.ProvincesRuntime)
    {
        if (province == null || province.data == null)
            continue;

        if (!province.estClaim && province.influenceAutre > 15f)
            return true;
    }

    return false;
}
    private int CompterEquipesAllieesSurProvince(
        SYS_GameManager gameManager,
        ENUM_Compagnie compagnie,
        STATE_PROVINCE province,
        STATE_EQUIPE equipeIgnoree)
    {
        if (gameManager == null || province == null)
            return 0;

        int total = 0;

        foreach (STATE_EQUIPE equipe in gameManager.EquipesRuntime)
        {
            if (equipe == null || equipe == equipeIgnoree)
                continue;

            if (equipe.compagnie != compagnie)
                continue;

            if (equipe.provinceAffectee == province)
                total++;
        }

        return total;
    }

    private float GetInfluenceCompagnie(STATE_PROVINCE province, ENUM_Compagnie compagnie)
    {
        if (province == null)
            return 0f;

        switch (compagnie)
        {
            case ENUM_Compagnie.Maizin:
                return province.influenceMaizin;
            case ENUM_Compagnie.Kinia:
                return province.influenceKinia;
            case ENUM_Compagnie.Joho:
                return province.influenceJoho;
            default:
                return 0f;
        }
    }

    private int CalculerCoutExploration(SYS_GameManager gameManager, STATE_EQUIPE equipe)
    {
        if (gameManager == null || equipe == null || gameManager.ExplorationConfig == null)
            return 0;

        ExplorationConfig config = gameManager.ExplorationConfig;
        EQUIPE_StatsSnapshot stats = CALC_EQUIPE_StatsCalculator.Calculer(equipe);

        int enclavement = 0;
        if (equipe.provinceAffectee != null && equipe.provinceAffectee.data != null)
        {
            enclavement = Mathf.RoundToInt(equipe.provinceAffectee.data.accesibilite);
        }

        ENUM_EXPLORATION_Resultat resultat = CALC_EXPLORATION_Resolver.CalculerResultat(
            stats,
            config.toursBase,
            config.coutParTourBase,
            config.prestigeBase,
            config.chanceArtefactBase,
            config.chanceArtefactRareBase,
            enclavement
        );

        return resultat != null ? resultat.coutTotal : 0;
    }

    private int GetTailleCibleEquipe(DATA_JOUEUR joueur)
    {
        if (joueur == null)
            return 4;

        switch (joueur.personnaliteIA)
        {
            case ENUM_IA_Personnalite.Agressive:
                return 5;

            case ENUM_IA_Personnalite.Economique:
                return 3;

            case ENUM_IA_Personnalite.Expansionniste:
                return 4;

            case ENUM_IA_Personnalite.Prestige:
                return 4;

            case ENUM_IA_Personnalite.Opportuniste:
                return 4;

            case ENUM_IA_Personnalite.Equilibree:
            default:
                return 4;
        }
    }

   private int GetTailleMinEquipePourExploration(DATA_JOUEUR joueur)
{
    if (joueur == null)
        return 1;

    switch (joueur.personnaliteIA)
    {
        case ENUM_IA_Personnalite.Agressive:
            return 1;

        case ENUM_IA_Personnalite.Prestige:
            return 1;

        case ENUM_IA_Personnalite.Economique:
            return 1;

        case ENUM_IA_Personnalite.Expansionniste:
            return 1;

        case ENUM_IA_Personnalite.Opportuniste:
            return 1;

        case ENUM_IA_Personnalite.Equilibree:
        default:
            return 1;
    }
}

  private float GetRatioMinimalBudgetPourExplorer(DATA_JOUEUR joueur)
{
    if (joueur == null)
        return 1f;

    switch (joueur.personnaliteIA)
    {
        case ENUM_IA_Personnalite.Agressive:
            return 0.9f;

        case ENUM_IA_Personnalite.Prestige:
            return 1.0f;

        case ENUM_IA_Personnalite.Economique:
            return 1.15f;

        case ENUM_IA_Personnalite.Expansionniste:
            return 1.0f;

        case ENUM_IA_Personnalite.Opportuniste:
            return 1.05f;

        case ENUM_IA_Personnalite.Equilibree:
        default:
            return 1.0f;
    }
}
}