using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CameraZoomSettings cameraSettings;
    [SerializeField] private PlayerBinds playerBinds;

    private float _minZoom;
    private float _maxZoom;
    private float _defaultZoom;
    private float _targetZoom;

    private void Awake()
    {
        _defaultZoom = cam.orthographicSize;
        _minZoom = cam.orthographicSize * cameraSettings.minZoomMultiplier;
        _maxZoom = cam.orthographicSize * cameraSettings.maxZoomMultiplier;

        _targetZoom = cam.orthographicSize;
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            _targetZoom -= scroll * cameraSettings.zoomSpeed;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
        }

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            _targetZoom,
            cameraSettings.zoomSmoothness * Time.deltaTime
        );

        if (!GamePause.IsPaused && Input.GetKeyDown(playerBinds.resetCameraZoom))
        {
            
            _targetZoom = _defaultZoom;
        }
    }
}