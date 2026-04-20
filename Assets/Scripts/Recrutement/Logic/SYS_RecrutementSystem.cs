using System.Collections.Generic;
using UnityEngine;

public class SYS_RecrutementSystem
{
    private readonly List<SCOBJ_Personnage> poolGlobal = new();
    private readonly List<SCOBJ_Personnage> marcheCourant = new();
    private readonly List<DATA_OffreRecrutement> offresCourantes = new();
    private readonly Dictionary<int, DATA_RareteDistribution> distributionsParTour = new();

    public IReadOnlyList<SCOBJ_Personnage> MarcheCourant => marcheCourant;
    public IReadOnlyList<DATA_OffreRecrutement> OffresCourantes => offresCourantes;

    public void InitialiserDistributions()
    {
        distributionsParTour.Clear();

        distributionsParTour[0] = new DATA_RareteDistribution
        {
            tour = 0,
            r1 = 1f,
            r2 = 0f,
            r3 = 0f,
            r4 = 0f,
            r5 = 0f
        };

        distributionsParTour[10] = new DATA_RareteDistribution
        {
            tour = 10,
            r1 = 0.60f,
            r2 = 0.30f,
            r3 = 0.10f,
            r4 = 0f,
            r5 = 0f
        };

        distributionsParTour[50] = new DATA_RareteDistribution
        {
            tour = 50,
            r1 = 0.00f,
            r2 = 0.10f,
            r3 = 0.43f,
            r4 = 0.05f,
            r5 = 0.02f
        };
    }

    public void InitialiserPool(List<SCOBJ_Personnage> personnagesDisponibles)
    {
        poolGlobal.Clear();
        marcheCourant.Clear();
        offresCourantes.Clear();

        if (personnagesDisponibles == null)
            return;

        foreach (SCOBJ_Personnage personnage in personnagesDisponibles)
        {
            if (personnage != null && !poolGlobal.Contains(personnage))
            {
                poolGlobal.Add(personnage);
            }
        }
    }

    public void GenererMarche(int nombre = 5, int tourActuel = 1)
    {
        marcheCourant.Clear();
        offresCourantes.Clear();

        List<SCOBJ_Personnage> copie = new(poolGlobal);

        while (marcheCourant.Count < nombre)
        {
            int rareteVoulue = TirerRaretePonderee(tourActuel);
            SCOBJ_Personnage choisi = null;

            if (rareteVoulue <= 2)
            {
                choisi = CALC_PERSONNAGE_Generator.Generer(rareteVoulue);
            }
            else
            {
                choisi = TrouverPersonnageParRarete(copie, rareteVoulue);

                if (choisi == null)
                {
                    choisi = TrouverPersonnageUniqueLePlusProche(copie, rareteVoulue);
                }

                if (choisi != null)
                {
                    copie.Remove(choisi);
                }
            }

            if (choisi == null)
            {
                if (copie.Count > 0)
                {
                    int index = Random.Range(0, copie.Count);
                    choisi = copie[index];
                    copie.RemoveAt(index);
                }
                else
                {
                    choisi = CALC_PERSONNAGE_Generator.Generer(Random.Range(1, 3));
                }
            }

            if (choisi != null)
            {
                marcheCourant.Add(choisi);

                offresCourantes.Add(new DATA_OffreRecrutement
                {
                    personnage = choisi,
                    prixMinimum = Mathf.Max(0, choisi.coutRecrutementBase),
                    estResolue = false,
                    encheres = new List<DATA_EnchereCompagnie>()
                });
            }
        }

        Debug.Log($"Marché généré : {marcheCourant.Count} personnages | offres : {offresCourantes.Count}");
    }

    public DATA_OffreRecrutement GetOffre(SCOBJ_Personnage personnage)
    {
        if (personnage == null)
            return null;

        return offresCourantes.Find(x =>
            x != null &&
            x.personnage != null &&
            !string.IsNullOrWhiteSpace(x.personnage.idUnique) &&
            x.personnage.idUnique == personnage.idUnique
        );
    }

    public bool SoumettreOffre(SCOBJ_Personnage personnage, DATA_JOUEUR joueur, int montant)
    {
        if (personnage == null || joueur == null)
            return false;

        DATA_OffreRecrutement offre = GetOffre(personnage);
        if (offre == null || offre.estResolue)
            return false;

        int minimum = Mathf.Max(0, offre.prixMinimum);
        int maximum = Mathf.FloorToInt(joueur.etrinium);

        if (montant < minimum)
        {
            Debug.LogWarning($"Offre refusée : montant {montant} < minimum {minimum}");
            return false;
        }

        if (montant > maximum)
        {
            Debug.LogWarning($"Offre refusée : montant {montant} > fonds {maximum}");
            return false;
        }

        offre.SetEnchere(joueur.compagnie, montant);

        Debug.Log(
            $"Offre soumise | joueur={joueur.nomJoueur} | compagnie={joueur.compagnie} | " +
            $"personnage={personnage.nom} {personnage.prenom} | montant={montant}"
        );

        return true;
    }

    public void FaireJouerEncheresIA(SYS_GameManager gameManager)
    {
        if (gameManager == null)
            return;

        JouerEnchereIA(gameManager, gameManager.Joueur2);
        JouerEnchereIA(gameManager, gameManager.Joueur3);
    }

    public void FaireJouerEnchereIAPourJoueur(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        JouerEnchereIA(gameManager, joueur);
    }

    private void JouerEnchereIA(SYS_GameManager gameManager, DATA_JOUEUR joueur)
    {
        if (gameManager == null || joueur == null || joueur.estHumain)
            return;

        if (joueur.etrinium <= 0)
            return;

        DATA_OffreRecrutement meilleureOffre = null;
        int meilleurScore = int.MinValue;

        foreach (DATA_OffreRecrutement offre in offresCourantes)
        {
            if (offre == null || offre.personnage == null || offre.estResolue)
                continue;

            if (offre.AUneEnchere(joueur.compagnie))
                continue;

            if (joueur.etrinium < offre.prixMinimum)
                continue;

            int score = SVC_RECRUTEMENT_IA_ScoringService.EvaluerInteret(offre, joueur.compagnie);

            if (score > meilleurScore)
            {
                meilleurScore = score;
                meilleureOffre = offre;
            }
        }

        if (meilleureOffre == null)
            return;

        int montant = CalculerMontantOffreIA(joueur, meilleureOffre, meilleurScore);
        SoumettreOffre(meilleureOffre.personnage, joueur, montant);
    }

    private int CalculerMontantOffreIA(
        DATA_JOUEUR joueur,
        DATA_OffreRecrutement offre,
        int scoreInteret)
    {
        if (joueur == null || offre == null || offre.personnage == null)
            return 0;

        int minimum = Mathf.Max(0, offre.prixMinimum);
        int tresorerie = Mathf.Max(0, Mathf.FloorToInt(joueur.etrinium));
        int revenuParTour = Mathf.Max(0, Mathf.RoundToInt(joueur.etriniumParTour));

        if (tresorerie < minimum)
            return 0;

        float ratioBudget = 0.20f;

        if (tresorerie >= 100000) ratioBudget = 0.55f;
        else if (tresorerie >= 70000) ratioBudget = 0.45f;
        else if (tresorerie >= 40000) ratioBudget = 0.35f;
        else if (tresorerie >= 20000) ratioBudget = 0.28f;

        int budgetMax = Mathf.FloorToInt(tresorerie * ratioBudget);
        int bonusRevenu = Mathf.RoundToInt(revenuParTour * 2.5f);

        int bonusInteret = 0;
        SCOBJ_Personnage personnage = offre.personnage;

        switch (personnage.rareteEtoiles)
        {
            case 5:
                bonusInteret += 12000;
                break;
            case 4:
                bonusInteret += 7000;
                break;
            case 3:
                bonusInteret += 3000;
                break;
            case 2:
                bonusInteret += 1200;
                break;
            default:
                bonusInteret += 250;
                break;
        }

        if (personnage.aPreferenceCompagnie &&
            personnage.compagniePreferee == joueur.compagnie)
        {
            bonusInteret += 5000;
        }

        int plafondOffre = Mathf.Min(
            tresorerie,
            Mathf.Max(minimum, budgetMax + bonusRevenu + bonusInteret)
        );

        int margeMax = Mathf.Max(0, plafondOffre - minimum);

        float agressivite = 0.25f;

        if (scoreInteret >= 7000) agressivite = 1.00f;
        else if (scoreInteret >= 5500) agressivite = 0.80f;
        else if (scoreInteret >= 4000) agressivite = 0.60f;
        else if (scoreInteret >= 2500) agressivite = 0.40f;

        int surenchere = Mathf.RoundToInt(margeMax * agressivite);
        int montantFinal = minimum + surenchere;

        return Mathf.Clamp(montantFinal, minimum, tresorerie);
    }

    public DATA_RecrutementResolutionResult ResoudreOffres(SYS_GameManager gameManager)
{
    DATA_RecrutementResolutionResult resultat = new();

    if (gameManager == null)
        return resultat;

    List<DATA_OffreRecrutement> offresAResoudre = new(offresCourantes);

    foreach (DATA_OffreRecrutement offre in offresAResoudre)
    {
        if (offre == null || offre.personnage == null || offre.estResolue)
            continue;

        offre.estResolue = true;

        if (!offre.AAuMoinsUneEnchere())
            continue;

        DATA_EnchereCompagnie enchereGagnante = DeterminerEnchereGagnante(offre);
        if (enchereGagnante == null)
            continue;

        DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(enchereGagnante.compagnie);
        if (joueur == null)
            continue;

        if (joueur.etrinium < enchereGagnante.montant)
        {
            resultat.notifications.Add(new DATA_RecrutementNotificationItem
            {
                compagnie = enchereGagnante.compagnie,
                logoCompagnie = joueur.GetLogoCompagnie(),
                portraitPersonnage = offre.personnage.sprite,
                texte = $"{offre.personnage.nom} {offre.personnage.prenom} : enchère annulée, fonds insuffisants."
            });

            continue;
        }

        if (joueur.personnagesRecrutes != null && joueur.personnagesRecrutes.Contains(offre.personnage))
            continue;

        bool succesRecrutement = Recruter(offre.personnage, enchereGagnante.compagnie);
        if (!succesRecrutement)
            continue;

        joueur.personnagesRecrutes ??= new List<SCOBJ_Personnage>();
        joueur.personnagesRecrutes.Add(offre.personnage);
        joueur.etrinium -= enchereGagnante.montant;

        string texte =
            $"La compagnie {enchereGagnante.compagnie} recrute {offre.personnage.nom} {offre.personnage.prenom} " +
            $"pour {enchereGagnante.montant} étrinium.";

        resultat.notifications.Add(new DATA_RecrutementNotificationItem
        {
            compagnie = enchereGagnante.compagnie,
            logoCompagnie = joueur.GetLogoCompagnie(),
            portraitPersonnage = offre.personnage.sprite,
            texte = texte
        });

        Debug.Log(texte);
    }

    if (!resultat.ADesNotifications())
    {
        resultat.notifications.Add(new DATA_RecrutementNotificationItem
        {
            compagnie = ENUM_Compagnie.Aucune,
            logoCompagnie = null,
            portraitPersonnage = null,
            texte = "Aucun personnage recruté ce tour."
        });
    }

    return resultat;
}

    private DATA_EnchereCompagnie DeterminerEnchereGagnante(DATA_OffreRecrutement offre)
    {
        if (offre == null || offre.encheres == null || offre.encheres.Count == 0)
            return null;

        int meilleurMontant = int.MinValue;
        List<DATA_EnchereCompagnie> meilleures = new();

        foreach (DATA_EnchereCompagnie enchere in offre.encheres)
        {
            if (enchere == null)
                continue;

            if (enchere.montant > meilleurMontant)
            {
                meilleurMontant = enchere.montant;
                meilleures.Clear();
                meilleures.Add(enchere);
            }
            else if (enchere.montant == meilleurMontant)
            {
                meilleures.Add(enchere);
            }
        }

        if (meilleures.Count == 0)
            return null;

        if (meilleures.Count == 1)
            return meilleures[0];

        int index = Random.Range(0, meilleures.Count);
        return meilleures[index];
    }

    public bool Recruter(SCOBJ_Personnage personnage, ENUM_Compagnie compagnieRecruteuse)
    {
        if (personnage == null)
            return false;

        PERSONNAGE_EFFET_AutoAssigner.AssignerEffetsAffinite(
            personnage,
            compagnieRecruteuse,
            CALC_PERSONNAGE_Generator.effetAffiniteRespectee,
            CALC_PERSONNAGE_Generator.effetAffiniteNonRespectee
        );

        bool retireMarche = marcheCourant.Remove(personnage);
        offresCourantes.RemoveAll(x =>
            x == null ||
            x.personnage == null ||
            x.personnage == personnage ||
            (!string.IsNullOrWhiteSpace(x.personnage.idUnique) && x.personnage.idUnique == personnage.idUnique)
        );

        bool retirePool = false;
        if (personnage.rareteEtoiles >= 3)
        {
            retirePool = poolGlobal.Remove(personnage);
        }

        Debug.Log($"Retrait du marché : {personnage.nom} {personnage.prenom} | retireMarche={retireMarche}");
        Debug.Log($"Retrait du pool : {personnage.nom} | retirePool={retirePool} | Restants={poolGlobal.Count}");

        return retireMarche;
    }

    private int TirerRaretePonderee(int tourActuel)
    {
        DATA_RareteDistribution distribution = GetDistributionPourTour(tourActuel);

        float total = distribution.r1 + distribution.r2 + distribution.r3 + distribution.r4 + distribution.r5;
        if (total <= 0f)
            return 1;

        float tirage = Random.value * total;

        if (tirage < distribution.r1) return 1;
        tirage -= distribution.r1;

        if (tirage < distribution.r2) return 2;
        tirage -= distribution.r2;

        if (tirage < distribution.r3) return 3;
        tirage -= distribution.r3;

        if (tirage < distribution.r4) return 4;

        return 5;
    }

    private DATA_RareteDistribution GetDistributionPourTour(int tourActuel)
    {
        DATA_RareteDistribution meilleure = distributionsParTour[0];

        foreach (KeyValuePair<int, DATA_RareteDistribution> kvp in distributionsParTour)
        {
            if (tourActuel >= kvp.Key && kvp.Key >= meilleure.tour)
            {
                meilleure = kvp.Value;
            }
        }

        return meilleure;
    }

    private SCOBJ_Personnage TrouverPersonnageParRarete(List<SCOBJ_Personnage> personnages, int rarete)
    {
        if (personnages == null)
            return null;

        List<SCOBJ_Personnage> candidats = new();

        foreach (SCOBJ_Personnage personnage in personnages)
        {
            if (personnage != null && personnage.rareteEtoiles == rarete)
            {
                candidats.Add(personnage);
            }
        }

        if (candidats.Count == 0)
            return null;

        return candidats[Random.Range(0, candidats.Count)];
    }
private Sprite GetLogoCompagnie(SYS_GameManager gameManager, ENUM_Compagnie compagnie)
{
    if (gameManager == null)
        return null;

    DATA_JOUEUR joueur = gameManager.GetDATA_JOUEURByCompagnie(compagnie);
    if (joueur == null)
        return null;

    // On récupère le logo via les données HUD du joueur si tu les relies ailleurs,
    // sinon il faudra le stocker directement sur DATA_JOUEUR.
    if (joueur.estHumain && gameManager.JoueurData != null)
        return gameManager.JoueurData.logoCompagnie;

    return null;
}
    private SCOBJ_Personnage TrouverPersonnageUniqueLePlusProche(List<SCOBJ_Personnage> personnages, int rareteVoulue)
    {
        if (personnages == null || personnages.Count == 0)
            return null;

        SCOBJ_Personnage meilleur = null;
        int meilleurEcart = int.MaxValue;

        foreach (SCOBJ_Personnage personnage in personnages)
        {
            if (personnage == null)
                continue;

            int ecart = Mathf.Abs(personnage.rareteEtoiles - rareteVoulue);
            if (ecart < meilleurEcart)
            {
                meilleurEcart = ecart;
                meilleur = personnage;
            }
        }

        return meilleur;
    }
}