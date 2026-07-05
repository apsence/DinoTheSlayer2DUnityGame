using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBinds", menuName = "Player/PlayerBinds")]
public class PlayerBinds : ScriptableObject
{
    [KeyCodeAlpha] public KeyCode dashBind = KeyCode.Space;
    [KeyCodeAlpha] public KeyCode openInventory = KeyCode.Tab;
    [KeyCodeAlpha] public KeyCode attack = KeyCode.Mouse0;
    [KeyCodeAlpha] public KeyCode hideAllGUI = KeyCode.Escape;
    [KeyCodeAlpha] public KeyCode showUpgradesMenu = KeyCode.Q;
    [KeyCodeAlpha] public KeyCode resetCameraZoom = KeyCode.X;
}
