using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradesButtonsBind : MonoBehaviour
{
    [SerializeField] private KeyCode hpUpgradeKey = KeyCode.Z;
    [SerializeField] private KeyCode damageUpgradeKey = KeyCode.X;
    [SerializeField] private Button hpButton;
    [SerializeField] private Button damageButton;
    private Upgrades upgrades;

    void Awake()
    {
        upgrades = GameObject.FindAnyObjectByType<Upgrades>();
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
