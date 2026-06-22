using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private MinimapVisibility minimapVisibility;

    public void Show() => minimapVisibility.Show();
    public void Hide() => minimapVisibility.Hide();
}
