using UnityEngine;

public class UpgradesGUI : MonoBehaviour
{
    [SerializeField] private UpgradesGUIVisibility _visibility;
    [SerializeField] private UpgradesGUIHotkey _hotkey;
    
    [SerializeField] private ActionBar actionBar;
    [SerializeField] private Minimap minimap;
    
    [SerializeField] private ScreenBlurController screenBlurController;

    public bool IsNearMerchant => _visibility.IsNearMerchant;
    public bool IsOpen => _visibility.IsOpen;

    public void Show()
    {
        _visibility.Show();
        actionBar.Hide();
        minimap.Hide();
        GamePause.Pause();
        screenBlurController.Blur();
    }

    public void Hide()
    {
        _visibility.Hide();
        actionBar.Show();
        minimap.Show();
        GamePause.Resume();
        screenBlurController.Unblur();
    }
}