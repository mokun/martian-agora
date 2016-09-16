using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Environment
{
    private static Terrain marsTerrain;
    private static int gridSize;

    private static Dictionary<IntVector3, Atmosphere> atmospheres;

    public static void Initialize()
    {
        //atmosphere chunks are 10x10x10m
        gridSize = 10;
        atmospheres = new Dictionary<IntVector3, Atmosphere>();
    }

    public static float GetAltitude(Vector3 worldPosition)
    {
        float altitude = GetTerrain().SampleHeight(worldPosition);
        return altitude + GetTerrain().transform.position.y;
    }

    public static Terrain GetTerrain()
    {
        if (marsTerrain == null)
        {
            marsTerrain = GameObject.FindObjectOfType<Terrain>();
			Debug.Log ("setting layer for "+marsTerrain);
            marsTerrain.gameObject.layer = LayerMask.NameToLayer("Terrain");
        }
        return marsTerrain;
    }

    public static Atmosphere GetAtmosphere(Vector3 position)
    {
        StructureController sc = GetStructureControllerWithAtmosphere(position);
        if (sc == null)
            return GetOutsideAtmosphere(position);
        return new Atmosphere(position) ;
    }

    public static bool IsInsideStructureWithAtmosphere(Vector3 position)
    {
        return GetStructureControllerWithAtmosphere(position)!=null;
    }

    public static StructureController GetStructureControllerWithAtmosphere(Vector3 position)
    {
        //if position is inside a structure that has its own atmosphere, returns that structure controller
        float maxCeilingHeight = 1000;
        RaycastHit[] rayCastHits = Physics.RaycastAll(position, Vector3.up, maxCeilingHeight);
        foreach(RaycastHit rayCastHit in rayCastHits)
        {
            GameObject hitObject = rayCastHit.transform.gameObject;
            StructureController sc = StructureController.GetStructureControllerFromChild(hitObject);
            if (sc != null)
                return sc;
        }
        return null;
    }

    private static Atmosphere GetOutsideAtmosphere(Vector3 position)
    {
        //returns the outside atmosphere at this position.

        IntVector3 gridPosition = new IntVector3(position.x / gridSize, position.y / gridSize, position.z / gridSize);
        if (atmospheres.ContainsKey(gridPosition))
            return atmospheres[gridPosition];

        Atmosphere atmosphere = new Atmosphere(position);
        atmospheres.Add(gridPosition, atmosphere);
        return atmosphere;
    }
}
