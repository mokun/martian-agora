using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameObject activePlayer;
    private static Crew crewStatus;

    public static float RecalculateWaitSeconds = 1;

    //real world seconds = game hours * timeFactor
    public const float timeFactor = 1;

    void Start()
    {
        gameObject.AddComponent<GUIManager>();
        gameObject.AddComponent<CrewManager>();
        CrewManager.GetActiveCrewGameObject().transform.position = transform.position;
        //gameObject.AddComponent<BaseManager>();

        ThingFactory.Initialize();
        GUIFunctions.Initialize();
        BlueprintDesignManager.Initialize();

        SetColliderMasks();
        SetupPreplacedStructures();
    }

    private void SetupPreplacedStructures()
    {
        foreach (StructureController sc in GameObject.FindObjectsOfType<StructureController>())
            sc.SetupRealStructure();
    }
    
    private static void SetColliderMasks()
    {
        //int mask = 1 << LayerMask.NameToLayer(layerName);
        //mask = ~mask;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("VehicleBox"), LayerMask.NameToLayer("People"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("VehicleMesh"), LayerMask.NameToLayer("Terrain"));
    }

}
