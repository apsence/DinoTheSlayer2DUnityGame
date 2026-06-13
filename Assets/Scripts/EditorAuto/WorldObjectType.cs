using UnityEngine;

public enum WorldObjectType
{
    Tree,
    Rock,
    Bush,
    Plant,
    Water,
    Collectable,
    Building,
    Decoration,
    Creature,
    Destructible
}

public class WorldObject : MonoBehaviour
{
    public WorldObjectType Type;
}