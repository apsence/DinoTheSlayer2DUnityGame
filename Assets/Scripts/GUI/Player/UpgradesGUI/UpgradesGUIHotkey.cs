using UnityEngine;

public class UpgradesGUIHotkey : MonoBehaviour
{
    [SerializeField] private PlayerBinds playerBinds;
    [SerializeField] private UpgradesGUI upgradesGUI;

    private void Update()
    {
        
        if (upgradesGUI.IsNearMerchant && !upgradesGUI.IsOpen && Input.GetKeyDown(playerBinds.showUpgradesMenu))
        {
            upgradesGUI.Show();
        }
        else if (upgradesGUI.IsOpen && (Input.GetKeyDown(playerBinds.hideAllGUI) || Input.GetKeyDown(playerBinds.showUpgradesMenu)))
        {
            upgradesGUI.Hide();
        }
    }
}