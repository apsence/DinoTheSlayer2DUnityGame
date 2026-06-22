using UnityEngine;

public class ActionBar : MonoBehaviour
{
    [SerializeField] private ActionBarVisibilty actionBarVisibilty;

    public void Show() => actionBarVisibilty.Show();
    public void Hide() => actionBarVisibilty.Hide();
}
