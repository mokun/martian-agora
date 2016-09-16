using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VehicleTypes
{
    test_rover
}

public enum VehicleModes
{
    parked,
    driving,
    braking
}

public class VehicleController : MonoBehaviour
{
    public VehicleTypes vehicleType;
    public bool showGizmos = false;
    public VehicleModes vehicleMode;
    public List<Camera> cameras;
    private Rigidbody rb;
    private DriverSeat driverSeat;
    private int cameraIndex;

    private float deadZone;
    private float maxSuspensionForce, forwardAcceleration, backwardAcceleration, sidewaysMultiplier;
    private float currentThrust, currentTurn, turnStrength;
    //if the car is going slower than this and is breaking, it will immediately stop.
    private float minimumBreakSpeed;
    private float minimumBreakSpeedSquared, wheelBreakForce;

    private float wheelRotateMinSpeedSquared = 0.001f;
    private List<Wheel> wheels;

    //optimization: test_rover eats frames, up until it is moved forward 1cm..?

    void Start()
    {
        SetWheels();
        SetConstants();
        SetColliders();

        SetVehicleMode(vehicleMode);
    }

    private Rigidbody GetRigidbody()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        return rb;
    }

    private void SetColliders()
    {
        string[] excludeSubstrings = new string[2] { "wheel", "driver-seat" };
        foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, excludeSubstrings, true))
        {
            Collider collider = SetCollider(child, true);
            if (collider != null)
                collider.gameObject.layer = LayerMask.NameToLayer("VehicleMesh");
        }
        gameObject.layer = LayerMask.NameToLayer("VehicleBox");
    }

    private Collider SetCollider(GameObject go, bool isEnabled)
    {
        //already has MeshCollider        
        if (go.GetComponent<MeshCollider>() != null)
        {
            MeshCollider mc = go.GetComponent<MeshCollider>();
            mc.enabled = isEnabled;
            return mc;
        }

        //already has BoxCollider
        if (go.GetComponent<BoxCollider>() != null)
        {
            BoxCollider bc = go.GetComponent<BoxCollider>();
            bc.enabled = isEnabled;
            return bc;
        }

        //does not have any collider
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        //empty gameobjects can have renderers. only add mesh collider if renderer has something in it.
        if (isEnabled && mr != null && go.GetComponent<MeshCollider>() == null && mr.bounds.size.magnitude > 0)
        {
            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.enabled = isEnabled;
            return mc;
        }

        return null;
    }

    public void SetVehicleMode(VehicleModes newVehicleMode)
    {
        vehicleMode = newVehicleMode;
        DisableCameras();
    }

    public DriverSeat GetDriverSeat()
    {
        if (driverSeat == null)
        {
            foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, false))
            {
                DriverSeat ds = child.GetComponent<DriverSeat>();
                if (ds != null)
                {
                    driverSeat = ds;
                    break;
                }
            }
        }
        if (driverSeat == null)
            Debug.LogError("GetDriverSeat failed for: " + gameObject.name);
        return driverSeat;
    }

    private void SetConstants()
    {
        if (vehicleType == VehicleTypes.test_rover)
        {
            maxSuspensionForce = 7000;
            forwardAcceleration = 500f;
            backwardAcceleration = 200f;
            turnStrength = 100f;
            sidewaysMultiplier = 100f;
            minimumBreakSpeed = 0.5f;
            wheelBreakForce = 2000f;

            GetRigidbody().mass = 1200;
            GetRigidbody().drag = 0;
            GetRigidbody().angularDrag = 0.05f;

            GetRigidbody().centerOfMass -= new Vector3(0, 2, 0);
        }

        minimumBreakSpeedSquared = Mathf.Pow(minimumBreakSpeed, 2);
    }

    private void SetWheels()
    {
        wheels = new List<Wheel>();
        foreach (GameObject wheelGameObject in ParentChildFunctions.GetAllChildrenWithName(gameObject, "wheel", "wheels"))
        {
            Wheel wheel = wheelGameObject.AddComponent<Wheel>();
            wheel.showGizmos = showGizmos;
            wheel.Initialize(transform);
            wheels.Add(wheel);
        }
    }

    void OnDrawGizmos()
    {
        if (wheels == null || !showGizmos)
            return;

        foreach (Wheel wheel in wheels)
        {
            Wheel.Modes wheelMode = wheel.GetMode();

            if (wheelMode == Wheel.Modes.below_terrain)
                Gizmos.color = Color.blue;
            else if (wheelMode == Wheel.Modes.contact)
                Gizmos.color = Color.green;
            else if (wheelMode == Wheel.Modes.no_contact)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.white;
            Gizmos.DrawSphere(wheel.transform.position, 0.5f);
        }
    }

    private void NextCamera()
    {
        //cameraIndex=cameras.Count when we should use the crewCamera.
        if (cameras == null)
            return;

        for (int i = 0; i < cameras.Count; i++)
            cameras[i].enabled = cameraIndex == i;

        CrewManager.GetActiveCrewCamera().enabled = cameraIndex == cameras.Count;

        cameraIndex++;
        if (cameraIndex > cameras.Count)
            cameraIndex = 0;
    }

    private void DisableCameras()
    {
        for (int i = 0; i < cameras.Count; i++)
            cameras[i].enabled = false;
    }

    private void OccupiedUpdate()
    {
        //forward and reverse
        currentThrust = 0.0f;
        float accelerationAxis = Input.GetAxis("Vertical");
        if (accelerationAxis > deadZone)
            currentThrust = accelerationAxis * forwardAcceleration;
        else if (accelerationAxis < -deadZone)
            currentThrust = accelerationAxis * backwardAcceleration;

        //turning
        currentTurn = 0.0f;
        float turnAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(turnAxis) > deadZone)
            currentTurn = turnAxis * turnStrength;

        //other
        if (Input.GetKeyDown(KeyCode.M))
            NextCamera();

        if (Input.GetKey(KeyCode.Space))
            vehicleMode = VehicleModes.braking;
        else
            vehicleMode = VehicleModes.driving;

        if (Input.GetKeyDown(KeyCode.X))
        {
            driverSeat.ReleaseActivePlayer();
            vehicleMode = VehicleModes.parked;
        }
    }

    public static VehicleController GetVehicleControllerFromChild(GameObject child)
    {
        foreach (GameObject parent in ParentChildFunctions.GetAllParents(child))
        {
            VehicleController vc = parent.GetComponent<VehicleController>();
            if (vc != null)
                return vc;
        }
        return null;
    }

    void Update()
    {
        if (vehicleMode != VehicleModes.parked)
            OccupiedUpdate();

        if (GetRigidbody().velocity.sqrMagnitude > wheelRotateMinSpeedSquared)
            RotateWheels();
    }

    private void RotateWheels()
    {
        return;
        float circumference = Mathf.PI * wheels[0].wheelRadius * 2;
        Quaternion wheelParentQuat = wheels[0].transform.parent.transform.rotation;
        foreach (Wheel wheel in wheels)
        {
            Vector3 right = wheel.transform.parent.transform.right;
            float angle =- rb.velocity.magnitude/circumference * Time.deltaTime*180;
            float x = wheel.gameObject.transform.rotation.eulerAngles.x;
            //wheel.gameObject.transform.Rotate(right, angle);
        }
    }

    private void DampenMovementWithSuspension(int wheelTouchCount)
    {
        const float squareMagnitudeDeadzone = 0.0001f;

        //dampen vertical bobbing
        float dampCoefficient = 0.002f * wheelTouchCount;
        Vector3 verticalVelocity = Vector3.Project(GetRigidbody().velocity, transform.up);
        Vector3 dampenVelocity = verticalVelocity * dampCoefficient;
        //Debug.Log("rigidbody.angularVelocity=" + rigidbody.angularVelocity + " verticalVelocity=" + verticalVelocity + " dampenVelocity=" + dampenVelocity);
        Vector3 newVelocity = GetRigidbody().velocity - dampenVelocity;
        if (newVelocity.sqrMagnitude > squareMagnitudeDeadzone)
            GetRigidbody().velocity = newVelocity;
        else
            GetRigidbody().velocity = Vector3.zero;

        //dampen rotational bobbing
        Vector3 newAngularVelocity = GetRigidbody().angularVelocity * (1 - dampCoefficient);

        if (newAngularVelocity.sqrMagnitude > squareMagnitudeDeadzone)
            GetRigidbody().angularVelocity = newAngularVelocity;
        else
            GetRigidbody().angularVelocity = Vector3.zero;

    }

    private void ApplyBrakes(int wheelTouchCount)
    {
        if (wheelTouchCount == 0)
            return;

        if (GetRigidbody().velocity.sqrMagnitude < minimumBreakSpeedSquared)
        {
            GetRigidbody().velocity = Vector3.zero;
        }
        else
        {
            GetRigidbody().AddForce(GetRigidbody().velocity.normalized * -1 * wheelTouchCount * wheelBreakForce);
        }
    }

    void FixedUpdate()
    {
        int wheelTouchCounter = 0;
        foreach (Wheel wheel in wheels)
        {
            Wheel.Modes wheelMode = wheel.GetMode();

            if (wheelMode == Wheel.Modes.contact || wheelMode == Wheel.Modes.below_terrain)
            {
                wheelTouchCounter++;

                //apply upward force to vehicle from wheel
                GetRigidbody().AddForceAtPosition(transform.up * maxSuspensionForce * wheel.GetForceRatio(),
                    wheel.transform.position);

                //apply force for sideways skidding
                Vector3 sidewaysProjection = Vector3.Project(GetRigidbody().velocity, transform.right);
                GetRigidbody().AddForceAtPosition(sidewaysProjection * sidewaysMultiplier * -1,
                    wheel.transform.position);

                //apply driving forward force
                if (vehicleMode == VehicleModes.driving)
                    GetRigidbody().AddForceAtPosition(transform.forward * currentThrust, wheel.gameObject.transform.position);

            }
        }

        DampenMovementWithSuspension(wheelTouchCounter);

        //apply turning
        if (vehicleMode == VehicleModes.driving)
            GetRigidbody().AddRelativeTorque(transform.up * currentTurn * turnStrength);

        if (vehicleMode == VehicleModes.braking || vehicleMode == VehicleModes.parked)
        {
            ApplyBrakes(wheelTouchCounter);
        }
    }
}
