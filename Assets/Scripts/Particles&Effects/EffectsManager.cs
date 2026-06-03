using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public void Create(
        GameObject prefab,
        Transform targetTransform,
        float xOffSet,
        float yOffSet)
    {
        GameObject effect = Instantiate(
            prefab,
            new Vector3(
                targetTransform.position.x + xOffSet,
                targetTransform.position.y + yOffSet,
                targetTransform.position.z),
            Quaternion.identity
        );

        effect.transform.SetParent(targetTransform);
    }
}