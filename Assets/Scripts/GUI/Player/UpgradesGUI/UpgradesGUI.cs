using UnityEngine;

public class UpgradesGUI : MonoBehaviour
{
    [SerializeField] private UpgradesGUIVisibility _visibility;
    [SerializeField] private UpgradesGUIHotkey _hotkey;

    public bool IsNearMerchant => _visibility.IsNearMerchant;
    public bool IsOpen => _visibility.IsOpen;

    public void Show() => _visibility.Show();
    public void Hide() => _visibility.Hide();
}