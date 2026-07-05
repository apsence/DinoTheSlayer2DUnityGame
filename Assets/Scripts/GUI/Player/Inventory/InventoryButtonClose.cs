using UnityEngine;

public class InventoryButtonClose : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    public void CloseInventory()
    {
        inventory.CloseInventory();
    }
}
