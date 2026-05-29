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

        if (treesRoot == null && rocksRoot == null && bushesRoot == null && plantsRoot == null && waterRoot == null)
            return;

        foreach (GameObject obj in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj.name.StartsWith("Tree") && obj.transform.parent == null)
            {
                obj.transform.SetParent(treesRoot.transform);
            }
            else if(obj.name.StartsWith("Rock") && obj.transform.parent == null)
            {
                obj.transform.SetParent(rocksRoot.transform);
            }
            else if(obj.name.StartsWith("Bush") && obj.transform.parent == null)
            {
                obj.transform.SetParent(bushesRoot.transform);
            }
            else if(obj.name.StartsWith("Plant") && obj.transform.parent == null)
            {
                obj.transform.SetParent(plantsRoot.transform);
            }
            else if(obj.name.StartsWith("Water") && obj.transform.parent == null)
            {
                obj.transform.SetParent(waterRoot.transform);
            }
            else if(obj.name.StartsWith("Collectable") && obj.transform.parent == null)
            {
                obj.transform.SetParent(collectablesRoot.transform);
            }
            else if(obj.name.StartsWith("Building") && obj.transform.parent == null)
            {
                obj.transform.SetParent(buildingsRoot.transform);
            }
        }
    }
}