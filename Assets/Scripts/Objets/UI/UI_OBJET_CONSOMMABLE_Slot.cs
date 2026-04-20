using TMPro;
using UnityEngine;

public class UI_OBJET_CONSOMMABLE_Slot : UI_OBJET_Slot
{
    [Header("UI Consommable")]
    [SerializeField] private TMP_Text nombreExemplairesText;

    protected override void ValidateReferences()
    {
        base.ValidateReferences();
        UTIL_UiReferenceValidator.Require(nombreExemplairesText, nameof(nombreExemplairesText), this);
    }

    public void RefreshStack(DATA_OBJET_CONSOMMABLE_EQUIPE_Stack stack)
    {
        if (stack == null || stack.objet == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Refresh(stack.objet);
        RefreshQuantite(stack.quantite);
    }

    private void RefreshQuantite(int quantite)
    {
        if (nombreExemplairesText == null)
            return;

        nombreExemplairesText.text = Mathf.Max(0, quantite).ToString();
    }
}