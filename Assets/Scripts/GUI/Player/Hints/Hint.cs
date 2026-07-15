using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private HintVisibility hintVisibility;

    public void CreateHint(string header, string text, float duration) =>
        hintVisibility.CreateHint(header, text, duration);

    public void ForciblyHide() => hintVisibility.ForciblyHide();
}
