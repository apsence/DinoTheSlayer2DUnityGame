using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinimapArrowSmooth : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private float rotationDuration = 0.3f;
    
    private float currentAngle;
    private float targetAngle;
    private Coroutine rotationCoroutine;
    
    void Start()
    {
        if (arrowRect == null)
            arrowRect = GetComponent<RectTransform>();
            
        currentAngle = arrowRect.eulerAngles.z;
        targetAngle = currentAngle;
    }
    
    void Update()
    {
        if (player == null || arrowRect == null) return;
        
        Vector2 moveDirection = GetPlayerMoveDirection();
        if (moveDirection.magnitude < 0.01f) return;
        
        // Вычисляем целевой угол
        float newTargetAngle = Mathf.Atan2(-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
        
        // Если угол изменился - запускаем плавный поворот
        if (Mathf.Abs(newTargetAngle - targetAngle) > 0.1f)
        {
            targetAngle = newTargetAngle;
            
            // Останавливаем предыдущую анимацию
            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);
            
            // Запускаем новую
            rotationCoroutine = StartCoroutine(RotateSmoothly(currentAngle, targetAngle, rotationDuration));
        }
    }
    
    IEnumerator RotateSmoothly(float fromAngle, float toAngle, float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Используем SmoothStep для более плавного движения
            float smoothT = Mathf.SmoothStep(0, 1, t);
            currentAngle = Mathf.LerpAngle(fromAngle, toAngle, smoothT);
            
            arrowRect.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }
        
        // Финальное значение
        currentAngle = toAngle;
        arrowRect.rotation = Quaternion.Euler(0, 0, currentAngle);
        rotationCoroutine = null;
    }
    
    private Vector2 GetPlayerMoveDirection()
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            return rb.linearVelocity.normalized;
        }
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        return new Vector2(x, y).normalized;
    }
}