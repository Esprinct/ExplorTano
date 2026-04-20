using UnityEngine;

public static class UTIL_UiReferenceValidator
{
    public static void Require(Object reference, string fieldName, MonoBehaviour owner)
    {
        Debug.Assert(
            reference != null,
            $"[{owner.GetType().Name}] {fieldName} non assigné dans {owner.name}"
        );
    }
}