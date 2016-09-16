using UnityEngine;
using System.Collections;

public static class BlueprintFactory {
    //creates new blueprints
    private static int blueprintCount = 0;
    private static GameObject blueprintContainer;

    public static Blueprint MakeNewCursorBlueprint()
    {
		Debug.Log ("MakeNewCursorBlueprint");
        blueprintCount++;
        GameObject go = new GameObject("blueprint" + blueprintCount) as GameObject;
        go.transform.parent = GetBlueprintContainer().transform;

        Blueprint blueprint= go.AddComponent<Blueprint>();
        blueprint.StopBlueprint();
        return blueprint;
    }

    private static GameObject GetBlueprintContainer()
    {
        if (blueprintContainer == null)
            blueprintContainer = new GameObject("blueprint container");
        return blueprintContainer;
    }
}
