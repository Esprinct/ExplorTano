using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Recrutement_NotificationItem : MonoBehaviour
{
    [SerializeField] private Image logoCompagnieImage;
    [SerializeField] private TMP_Text texteNotificationText;
    [SerializeField] private Image portraitPersonnageImage;

    public void Refresh(DATA_RecrutementNotificationItem data)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (logoCompagnieImage != null)
        {
            logoCompagnieImage.sprite = data.logoCompagnie;
            logoCompagnieImage.enabled = data.logoCompagnie != null;
        }

        if (texteNotificationText != null)
        {
            texteNotificationText.text = data.texte;
        }

        if (portraitPersonnageImage != null)
        {
            portraitPersonnageImage.sprite = data.portraitPersonnage;
            portraitPersonnageImage.enabled = data.portraitPersonnage != null;
        }
    }
}