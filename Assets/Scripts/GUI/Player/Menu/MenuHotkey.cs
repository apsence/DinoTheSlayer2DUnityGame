using UnityEngine;

public class MenuHotkey : MonoBehaviour
{
    [SerializeField] private Menu Menu;
    [SerializeField] private PlayerBinds playerBinds;

    void Update()
    {
        if (Input.GetKeyDown(playerBinds.showMenu))
        {
            Menu.Toggle();
        }
    }

    public void Toggle() => Menu.Toggle();
}