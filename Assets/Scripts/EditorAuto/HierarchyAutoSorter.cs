using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyAutoSorter
{
    static HierarchyAutoSorter()
    {
        EditorApplication.hierarchyChanged += SortObjects;
    }

    static void SortObjects()
    {
        GameObject treesRoot = GameObject.Find("Trees");
        GameObject rocksRoot = GameObject.Find("Rocks");
        GameObject bushesRoot = GameObject.Find("Bushes");
        GameObject plantsRoot = GameObject.Find("Plants");
        GameObject waterRoot = GameObject.Find("Water");
        GameObject collectablesRoot = GameObject.Find("Collectables");
        GameObject buildingsRoot = GameObject.Find("Buildings");
        GameObject decorationsRoot = GameObject.Find("Decorations");
        GameObject creaturesRoot = GameObject.Find("Creatures");
        GameObject destructiblesRoot = GameObject.Find("Destructibles");

        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.transform.parent != null)
                continue;

            WorldObject worldObject = obj.GetComponent<WorldObject>();

            if (worldObject == null)
                continue;

            switch (worldObject.Type)
            {
                case WorldObjectType.Tree:
                    obj.transform.SetParent(treesRoot.transform);
                    break;

                case WorldObjectType.Rock:
                    obj.transform.SetParent(rocksRoot.transform);
                    break;

                case WorldObjectType.Bush:
                    obj.transform.SetParent(bushesRoot.transform);
                    break;

                case WorldObjectType.Plant:
                    obj.transform.SetParent(plantsRoot.transform);
                    break;

                case WorldObjectType.Water:
                    obj.transform.SetParent(waterRoot.transform);
                    break;

                case WorldObjectType.Collectable:
                    obj.transform.SetParent(collectablesRoot.transform);
                    break;

                case WorldObjectType.Building:
                    obj.transform.SetParent(buildingsRoot.transform);
                    break;
                
                case WorldObjectType.Decoration:
                    obj.transform.SetParent(decorationsRoot.transform);
                    break;

                case WorldObjectType.Creature:
                    obj.transform.SetParent(creaturesRoot.transform);
                    break;

                case WorldObjectType.Destructible:
                    obj.transform.SetParent(destructiblesRoot.transform);
                    break;
            }
        }
    }
}