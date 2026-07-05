using UnityEngine;

[CreateAssetMenu(fileName = "CameraZoomSettings", menuName = "Player/CameraZoomSettings")]
public class CameraZoomSettings : ScriptableObject
{ 
    public float zoomSpeed = 2f;
    public float zoomSmoothness = 8f;
    public float minZoomMultiplier = 1f;
    public float maxZoomMultiplier = 1.5f;
}
