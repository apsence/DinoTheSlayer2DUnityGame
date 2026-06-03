using UnityEngine;

public enum WorldObjectType
{
    Tree,
    Rock,
    Bush,
    Plant,
    Water,
    Collectable,
    Building
}

public class WorldObject : MonoBehaviour
{
    public WorldObjectType Type;
}