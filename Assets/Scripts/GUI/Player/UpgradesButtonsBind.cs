using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesButtonsBind : MonoBehaviour
{
    [KeyCodeAlpha]
    [SerializeField] private KeyCode hpUpgradeKey = KeyCode.Z;
    [KeyCodeAlpha]
    [SerializeField] private KeyCode damageUpgradeKey = KeyCode.X;
    
    [SerializeField] private Button hpButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private TextMeshProUGUI hpHotKey;
    [SerializeField] private TextMeshProUGUI damageHotkey;

    void Awake()
    {
        hpHotKey.text = hpUpgradeKey.ToString();
        damageHotkey.text = damageUpgradeKey.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(hpUpgradeKey))
        {
            SimulateButtonPress(hpButton);
        }

        if (Input.GetKeyDown(damageUpgradeKey))
        {
            SimulateButtonPress(damageButton);
        }
    }

    private void SimulateButtonPress(Button button)
    {
        button.Select();

        ExecuteEvents.Execute(
            button.gameObject,
            new BaseEventData(EventSystem.current),
            ExecuteEvents.submitHandler
        );
    }
}
