using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickableItem : MonoBehaviour
{
    //attach this to a gameobject when it should be highlighted when the player looks at it.
    //also attaches an invisible cube for collision clicking.

    private BoxCollider boxCollider;
    private Dictionary<GameObject, Material[]> originalMaterials, hoverMaterials;

    private static Texture normalTexture;
    private static List<GameObject> clickableGameObjects;

    void Start()
    {
        AddBoxCollider();

        originalMaterials = new Dictionary<GameObject, Material[]>();
        hoverMaterials = new Dictionary<GameObject, Material[]>();

        GetClickableGameObjects().Add(gameObject);
        SetHoverAppearance(false);
    }

    private static Texture GetNormalTexture()
    {
        if (normalTexture == null)
        {
            normalTexture = Resources.Load("textures/chain-texture") as Texture;
        }
        if (normalTexture == null)
            Debug.LogError("GetNormalTexture failed.");
        return normalTexture;
    }

    private static Material[] GetHoverMaterials(GameObject target)
    {
        if (target.GetComponent<Renderer>() == null)
        {
            Debug.LogError("GetHoverMaterials got gameobject with no renderer.");
            return new Material[0];
        }

        int count = target.GetComponent<Renderer>().materials.Length;
        Material[] hoverMaterials = new Material[count];
        for (int i = 0; i < count; i++)
        {
            Material originalMaterial = target.GetComponent<Renderer>().materials[i];
            hoverMaterials[i] = GetHoverMaterial(originalMaterial);
        }

        return hoverMaterials;
    }

    private static Material GetHoverMaterial(Material originalMaterial)
    {
        Material hoverMaterial = new Material(originalMaterial);
        hoverMaterial.color = new Color(0.2f, 0.2f, 1f);

        return hoverMaterial;
    }

    private static List<GameObject> GetClickableGameObjects()
    {
        if (clickableGameObjects == null)
            clickableGameObjects = new List<GameObject>();
        return clickableGameObjects;
    }

    public static bool IsClickable(GameObject candidate)
    {
        return GetClickableGameObjects().Contains(candidate);
    }

    public void SetHoverAppearance(bool isHovering)
    {
        foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, true))
        {
            if (child.GetComponent<Renderer>() != null)
            {
                if (!originalMaterials.ContainsKey(child))
                {
                    Material[] newMaterials = GetHoverMaterials(child);
                    originalMaterials.Add(child, child.GetComponent<Renderer>().materials);
                    hoverMaterials.Add(child, newMaterials);
                }

                if (isHovering)
                    child.GetComponent<Renderer>().materials = hoverMaterials[child];
                else
                    child.GetComponent<Renderer>().materials = originalMaterials[child];
            }
        }
    }

    private List<Vector3> GetCubeFaceDirections()
    {
        List<Vector3> directions = new List<Vector3>();
        directions.Add(new Vector3(1, 0, 0));
        directions.Add(new Vector3(-1, 0, 0));
        directions.Add(new Vector3(0, 1, 0));
        directions.Add(new Vector3(0, -1, 0));
        directions.Add(new Vector3(0, 0, 1));
        directions.Add(new Vector3(0, 0, -1));

        return directions;
    }

    private void AddBoxCollider()
    {
        //this adds a box collider that just barely surrounds every visible part of a gameobject and its children.
        Quaternion startRotation = transform.rotation;
        transform.Rotate(transform.rotation.eulerAngles * -1);

        boxCollider = gameObject.AddComponent<BoxCollider>();
        RectangularPrism rp = GetSmallestBoxContainingRenderers(gameObject);
        boxCollider.size = rp.GetSizeVector();
        boxCollider.center = rp.GetPositionVector() - transform.position + rp.GetSizeVector() / 2;
        transform.rotation = startRotation;
    }

    private RectangularPrism GetSmallestBoxContainingRenderers(GameObject parentGameObject)
    {
        //returns a box with world coordinates that will just barely fit every renderer in the parentGameObject
        Vector3 smallest = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        Vector3 largest = new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity);

        foreach (GameObject child in ParentChildFunctions.GetAllChildren(parentGameObject, true))
        {
            if (child.GetComponent<Renderer>() != null)
            {
                if (child.GetComponent<Renderer>().bounds.min.x < smallest.x)
                    smallest.x = child.GetComponent<Renderer>().bounds.min.x;
                if (child.GetComponent<Renderer>().bounds.min.y < smallest.y)
                    smallest.y = child.GetComponent<Renderer>().bounds.min.y;
                if (child.GetComponent<Renderer>().bounds.min.z < smallest.z)
                    smallest.z = child.GetComponent<Renderer>().bounds.min.z;

                if (child.GetComponent<Renderer>().bounds.max.x > largest.x)
                    largest.x = child.GetComponent<Renderer>().bounds.max.x;
                if (child.GetComponent<Renderer>().bounds.max.y > largest.y)
                    largest.y = child.GetComponent<Renderer>().bounds.max.y;
                if (child.GetComponent<Renderer>().bounds.max.z > largest.z)
                    largest.z = child.GetComponent<Renderer>().bounds.max.z;
            }
        }

        return new RectangularPrism(smallest.x, smallest.y, smallest.z,
            largest.x - smallest.x, largest.y - smallest.y, largest.z - smallest.z);
    }
}
