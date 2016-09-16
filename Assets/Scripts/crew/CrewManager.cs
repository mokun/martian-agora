using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrewManager : MonoBehaviour
{
    private static GameObject activeCrewGameObject;
    private static Crew activeCrew;

    private static Material skyboxMaterial;

    public static GameObject MakeNewCrew()
    {
        GameObject prefab = Resources.Load("player") as GameObject;
        GameObject newCrew = Instantiate(prefab) as GameObject;
        newCrew.layer = LayerMask.NameToLayer("People");
        
        newCrew.AddComponent<FPSInputController>();
        newCrew.AddComponent<Crew>();

        CharacterMotor cm = newCrew.GetComponent<CharacterMotor>();
        cm.movement.maxForwardSpeed = 12;
        cm.movement.maxSidewaysSpeed = 9;
        cm.movement.maxBackwardsSpeed = 9;
        cm.movement.maxAirAcceleration = 40;
        //cm.movement.regularGravity = 9.81f * 0.38f;

        CharacterController cc = newCrew.GetComponent<CharacterController>();
        cc.radius = 0.4f;

        newCrew.transform.position = new Vector3(0, 2, 0);

        return newCrew;
    }

    private static Material GetSkyboxMaterial()
    {
        if (skyboxMaterial == null)
            skyboxMaterial = Resources.Load("skybox/basic skybox") as Material;
        return skyboxMaterial;
    }

    public static GameObject GetActiveCrewGameObject()
    {
        if (activeCrewGameObject == null)
            activeCrewGameObject = MakeNewCrew();
        return activeCrewGameObject;
    }

    public static Crew GetActiveCrew()
    {
        return GetActiveCrewGameObject().GetComponent<Crew>();
    }

    public static Camera GetActiveCrewCamera()
    {
        return GetActiveCrew().GetCrewCamera();
    }
}
