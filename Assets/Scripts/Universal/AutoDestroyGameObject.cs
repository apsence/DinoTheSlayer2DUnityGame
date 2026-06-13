using UnityEngine;

public class AutoDestroyGameObject : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
