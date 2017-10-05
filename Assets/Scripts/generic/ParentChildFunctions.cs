using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ParentChildFunctions
{
    public static GameObject GetChildWithNameSubstring(GameObject parentGameObject, string substring)
    {
        foreach (GameObject child in GetAllChildren(parentGameObject, true))
        {
            if (Blueprint.NameMatchesIgnoringNumber(child.name, substring))
                return child;
        }
        Debug.LogError("GetChildWithNameSubstring returned null. substring=" + substring + " parentGameObject=" + parentGameObject);
        return null;
    }

    public static List<GameObject> GetAllChildrenWithName(GameObject parentGameObject, string substring, string excludeSubstring)
    {
        //when substring is "RoverWheel", returns all gameobjects with names like "RoverWheel2" and "RoverWheel-FR" and "superRoverWheel"
        List<GameObject> children = new List<GameObject>();
        foreach (GameObject child in GetAllChildren(parentGameObject, true))
        {
            if (child.name.Contains(substring) && (excludeSubstring.Length == 0 || !child.name.Contains(excludeSubstring)))
                children.Add(child);
        }
        return children;
    }

    public static List<GameObject> GetAllParents(GameObject childGameObject)
    {
        List<GameObject> parents = new List<GameObject>();
        while (childGameObject.transform.parent != null)
        {
            GameObject parent = childGameObject.transform.parent.gameObject;
            parents.Add(parent);
            childGameObject = parent;
        }
        return parents;
    }

    public static ArrayList GetAllChildren(GameObject parentGameObject, bool includeParent = false)
    {
        string[] excludeSubstrings = new string[0];
        return GetAllChildren(parentGameObject, excludeSubstrings, includeParent);
    }

    public static ArrayList GetAllChildren(GameObject parentGameObject, string[] excludeSubstrings, bool includeParent = false)
    {
        //returns an arraylist of all children, grandchildren, etc.
        //excludes all objects and their children if their name contains any string in excludeSubstrings

        ArrayList children = new ArrayList();

        if (includeParent)
            children.Add(parentGameObject);

        for (int i = 0; i < parentGameObject.transform.childCount; i++)
        {
            GameObject child = parentGameObject.transform.GetChild(i).gameObject;
            bool excludeChild = false;
            foreach (string substring in excludeSubstrings)
            {
                if (child.name.Contains(substring))
                {
                    excludeChild = true;
                    break;
                }
            }
            if (excludeChild)
                continue;

            children.Add(child);
            if (child.transform.childCount > 0)
                children.AddRange(GetAllChildren(child, excludeSubstrings, false));
        }
        return children;
    }

    public static void SetMaterialOfChildren(GameObject parentGameObject, Material newMaterial)
    {
        foreach (GameObject child in GetAllChildren(parentGameObject))
            if (child.GetComponent<Renderer>() != null)
                child.GetComponent<Renderer>().material = newMaterial;
    }

    public static void SetCollisionForChildren(GameObject parentGameObject, bool isEnabled)
    {
        SetCollisionForChildren(parentGameObject, isEnabled, new string[0]);
    }

    public static void SetCollisionForChildren(GameObject parentGameObject, bool isEnabled, string[] excludeSubstrings)
    {
        //does not do anything to children (and their children) whose name contains any of the excludeSubstrings
        foreach (GameObject child in GetAllChildren(parentGameObject, excludeSubstrings, true))
            if (child.GetComponent<Collider>() != null)
                child.GetComponent<Collider>().enabled = isEnabled;
    }

    public static void AddCollidersForChildren(GameObject parentGameObject)
    {
        AddCollidersForChildren(parentGameObject, new string[0]);
    }

    public static void AddCollidersForChildren(GameObject parentGameObject, string[] excludeSubstrings)
    {
        //does not do anything to children (and their children) whose name contains any of the excludeSubstrings
        foreach (GameObject child in GetAllChildren(parentGameObject, excludeSubstrings,true))
            if (child.GetComponent<Collider>() == null)
                child.AddComponent<MeshCollider>();
    }
}
