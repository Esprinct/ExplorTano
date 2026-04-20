using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DirigeantXpView : MonoBehaviour
{
    [SerializeField] private Image xpFillImage;
    [SerializeField] private TMP_Text xpText;

    public void Refresh(int xpActuelle, int xpMax)
    {
        float ratio = xpMax > 0 ? xpActuelle / (float)xpMax : 0f;

        if (xpFillImage != null)
            xpFillImage.fillAmount = Mathf.Clamp01(ratio);

        if (xpText != null)
            xpText.text = $"{xpActuelle} / {xpMax}";
    }
}