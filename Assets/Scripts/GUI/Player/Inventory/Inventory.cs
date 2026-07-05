using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryVisibility inventoryVisibility;
    [SerializeField] private PlayerBinds playerBinds;
    [SerializeField] private Minimap minimap;
    [SerializeField] private ActionBar actionBar;

    private bool _isOpened = false;

    private void Awake()
    {
        inventoryVisibility.HideInstant();
        _isOpened = false;
    }

    private void Update()
    {
        // Блокируем ввод во время анимации fade
        if (inventoryVisibility.IsAnimating) return;

        if (Input.GetKeyDown(playerBinds.openInventory))
        {
            if (!_isOpened)
                OpenInventory();
            else
                CloseInventory();
        }
        else if (Input.GetKeyDown(playerBinds.hideAllGUI) && _isOpened)
        {
            CloseInventory();
        }
    }

    public void OpenInventory()
    {
        if (_isOpened) return;

        _isOpened = true;
        inventoryVisibility.Show();

        minimap.Hide();
        actionBar.Hide();
        GamePause.Pause();
    }

    public void CloseInventory()
    {
        if (!_isOpened) return;

        _isOpened = false;
        inventoryVisibility.Hide();

        minimap.Show();
        actionBar.Show();
        GamePause.Resume();
    }
    
}