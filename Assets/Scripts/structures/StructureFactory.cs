using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StructureFactory : MonoBehaviour
{
    private static Dictionary<ThingTypes, GameObject> structurePrefabs;

    public static GameObject MakeStructure(ThingTypes thingType, bool isBlueprint)
    {
        if (!ThingFactory.IsBlueprint(thingType))
        {
            Debug.LogError("StructureFactory.MakeStructure got a non-blueprint thingType: " + thingType);
            return null;
        }

        GameObject prefab = GetStructurePrefab(thingType);
        GameObject structure = Instantiate(prefab) as GameObject;
        StructureController sc = structure.GetComponent<StructureController>();
        if (isBlueprint)
            sc.SetupBlueprint();
        else
        {
            sc.SetupRealStructure();
        }

        if (isBlueprint)
        {
            ParentChildFunctions.SetMaterialOfChildren(structure, Blueprint.GetBlueprintMaterial());
            ParentChildFunctions.SetCollisionForChildren(structure, false);
        }
        else if (!sc.HasColliders())
        {
            //real structures with no colliders get mesh colliders
            ParentChildFunctions.SetCollisionForChildren(structure, !isBlueprint);
        }

        return structure;
    }

    private static GameObject GetStructurePrefab(ThingTypes thingType)
    {
        if (structurePrefabs == null)
            structurePrefabs = new Dictionary<ThingTypes, GameObject>();

        if (!ThingFactory.IsBlueprint(thingType))
        {
            Debug.LogError("GetStructurePrefab got thingType that isn't a blueprint. " + thingType);
            return null;
        }

        if (!structurePrefabs.ContainsKey(thingType))
        {
            Thing thingTemplate = ThingFactory.MakeThing(thingType);
            string path = "structures/" + thingTemplate.key;
            GameObject structurePrefab = Resources.Load(path) as GameObject;

            if (structurePrefab == null)
                Debug.LogError("GetStructurePrefab failed to load prefab: " + path);
            structurePrefabs.Add(thingType, structurePrefab);
        }
        return structurePrefabs[thingType];
    }
}
