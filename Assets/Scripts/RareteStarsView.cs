using UnityEngine;
using UnityEngine.UI;

public class RareteStarsView : MonoBehaviour
{
    [SerializeField] private Image[] etoiles;
    [SerializeField] private Sprite etoilePleine;
    [SerializeField] private Sprite etoileVide;

    public void Refresh(int rareteEtoiles)
    {
        rareteEtoiles = Mathf.Clamp(rareteEtoiles, 1, 5);

        if (etoiles == null || etoiles.Length == 0)
        {
            Debug.LogWarning("RareteStarsView : aucune étoile assignée.");
            return;
        }

        for (int i = 0; i < etoiles.Length; i++)
        {
            if (etoiles[i] == null)
            {
                continue;
            }

            etoiles[i].sprite = i < rareteEtoiles ? etoilePleine : etoileVide;
            etoiles[i].enabled = etoiles[i].sprite != null;
        }
    }
}