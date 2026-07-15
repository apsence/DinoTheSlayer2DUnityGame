using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private MenuVisibilty menuVisibilty;
    [SerializeField] private ActionBar actionBar;
    [SerializeField] private Minimap minimap;
    
    [SerializeField] private ScreenBlurController screenBlurController;
    
    public bool IsVisible => menuVisibilty.IsVisible;
    
    public void Show()
    {
        menuVisibilty.Show();
        actionBar.Hide();
        //minimap.Hide();
        if (GlobalOpenedGUI.Instance.TryOpen(this))
        {
            GamePause.Pause();
        }
        screenBlurController.Blur();
    }
    public void Hide()
    {
        menuVisibilty.Hide();
        actionBar.Show();
        //minimap.Show();
        GlobalOpenedGUI.Instance.Close(this);
        GamePause.Resume();
        screenBlurController.Unblur();
    }

    public void Toggle()
    {
        if (IsVisible)
            Hide();
        else
            Show();
    }
}