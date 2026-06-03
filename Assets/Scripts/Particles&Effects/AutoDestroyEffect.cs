using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
