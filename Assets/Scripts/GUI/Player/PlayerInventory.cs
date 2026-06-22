using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private PlayerBinds playerBinds;
    [SerializeField] private Minimap minimap;
    [SerializeField] private ActionBar actionBar;
    private bool isOpened;

    void Awake()
    {
        if(inventory.activeSelf == true) inventory.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(playerBinds.openInventory) && !isOpened)
        {
            inventory.SetActive(true);
            isOpened = true;
            minimap.Hide();
            actionBar.Hide();
            GamePause.Pause();
        }
        else if(Input.GetKeyDown(playerBinds.openInventory) && isOpened)
        {
            inventory.SetActive(false);
            isOpened = false;
            minimap.Show();
            actionBar.Show();
            GamePause.Resume();
        }
        else if(Input.GetKeyDown(playerBinds.hideAllGUI) && isOpened)
        {
            inventory.SetActive(false);
            isOpened = false;
            minimap.Show();
            actionBar.Show();
            GamePause.Resume();
        }
    }
}
