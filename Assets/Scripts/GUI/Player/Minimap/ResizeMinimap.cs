using UnityEngine;

public class ResizeMinimap : MonoBehaviour
{
    [SerializeField] private MinimapSettings minimapSettings;
    [SerializeField] private GameObject minimap;
    
    private int currentScale = 1;

    private void Start()
    {
        if (minimapSettings == null || minimapSettings.sizeList == null || minimapSettings.sizeList.Count == 0)
        {
            Debug.LogError($"[{gameObject.name}] Список sizeList в настройках миникарты пуст! Заполни его в Инспекторе ассета MinimapSettings.");
            enabled = false; 
            return;
        }
        
        currentScale = Mathf.Clamp(currentScale, 0, minimapSettings.sizeList.Count - 1);
        ApplyScale();
    }
    
    public void Plus()
    {
        if (minimapSettings.sizeList.Count > 0 && currentScale < minimapSettings.sizeList.Count - 1)
        {
            currentScale++;
            ApplyScale();
            //Debug.Log($"Текущий индекс скейла: {currentScale}");
        }
    }

    public void Minus()
    {
        if (minimapSettings.sizeList.Count > 0 && currentScale > 0)
        {
            currentScale--;
            ApplyScale();
            //Debug.Log($"Текущий индекс скейла: {currentScale}");
        }
    }

    private void ApplyScale()
    {
        float targetScale = minimapSettings.sizeList[currentScale];
        minimap.transform.localScale = new Vector3(targetScale, targetScale, 1f);
    }
}