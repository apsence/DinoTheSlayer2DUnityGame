using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothSpeed = 5f;
    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position - _target.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = _target.position + _offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, _smoothSpeed * Time.unscaledDeltaTime);
        transform.position = smoothedPos;
    }
}