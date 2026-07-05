using TMPro;
using UnityEngine;

public class SetUpgradesGUIHotkey : MonoBehaviour
{
    [SerializeField] private PlayerBinds playerBinds;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        text.text = playerBinds.showUpgradesMenu.ToString();
    }
}