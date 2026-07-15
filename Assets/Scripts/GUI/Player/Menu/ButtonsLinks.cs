using UnityEngine;

public class ButtonsLinks : MonoBehaviour
{
    [SerializeField] private MenuVisibilty menuVisibilty;
    //[SerializeField] private SaveManager saveManager;
    //[SerializeField] private LoadManager loadManager;
    //[SerializeField] private Settings settings;
    //[SerializeField] private LeaveGameManager leaveGameManager;

    public void Resume()
    {
        menuVisibilty.Hide();
    }
    
    
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
