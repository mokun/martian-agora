using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DriverSeat : MonoBehaviour
{
    public enum Modes
    {
        empty,
        entering,
        occupied
    }
    private Modes mode;

    private class TransferInfo
    {
        public float timer;
        public float duration = 1;
        public GameObject startPosition, driverPosition;
        public Quaternion startRotation, driverRotation;
        public GameObject crewGameObject;
        public Transform originalCrewTransformParent;
        public Crew crew;
    }
    private TransferInfo transfer = new TransferInfo();

    private VehicleController vehicleController;

    void Start()
    {
        ClickTransmitter ct = gameObject.AddComponent<ClickTransmitter>();
        ct.vehicleController = GetVehicleController();
        mode = Modes.empty;

        transfer.driverPosition = new GameObject("driver position");
        transfer.driverPosition.transform.parent = gameObject.transform;
        transfer.driverPosition.transform.position = gameObject.transform.position + new Vector3(0, 1, 0);
        transfer.startPosition = new GameObject("start position");
        transfer.startPosition.transform.parent = gameObject.transform;
    }

    private VehicleController GetVehicleController()
    {
        if (vehicleController == null)
            vehicleController = VehicleController.GetVehicleControllerFromChild(gameObject);
        return vehicleController;
    }

    public void GrabActivePlayer()
    {
        mode = Modes.entering;

        transfer.crewGameObject = CrewManager.GetActiveCrewGameObject();
        transfer.timer = 0;
        transfer.startPosition.transform.position = transfer.crewGameObject.transform.position;
        transfer.startRotation = transfer.crewGameObject.transform.rotation;
        transfer.driverRotation = transform.rotation;
        transfer.crew = CrewManager.GetActiveCrew();
        transfer.originalCrewTransformParent = transfer.crewGameObject.transform.parent;
        
        CrewManager.GetActiveCrew().SetDrivingMode(true);
        GUIManager.SetDrivingStatus(true);
    }

    public void ReleaseActivePlayer()
    {
        mode = Modes.empty;

        transfer.crewGameObject.transform.position = transfer.startPosition.transform.position;
        transfer.crewGameObject.transform.rotation = transfer.startRotation;
        transfer.crewGameObject.transform.parent = transfer.originalCrewTransformParent;

        transfer.crew.SetDrivingMode(false);
        GUIManager.SetDrivingStatus(false);

        CrewManager.GetActiveCrewCamera().enabled = true;
        GetVehicleController().SetVehicleMode(VehicleModes.parked);
    }

    void Update()
    {
        if (mode == Modes.entering)
        {
            transfer.timer += Time.deltaTime;
            if (transfer.timer < transfer.duration)
            {
                float ratio = transfer.timer / transfer.duration;
                transfer.crewGameObject.transform.position = Vector3.Lerp(transfer.startPosition.transform.position,
                    transfer.driverPosition.transform.position, ratio);
                transfer.crewGameObject.transform.rotation = Quaternion.Slerp(transfer.startRotation, transfer.driverRotation,
                    ratio);
            }
            else
            {
                mode = Modes.occupied;
                transfer.timer = 0;

                GetVehicleController().SetVehicleMode(VehicleModes.driving);
                transfer.crewGameObject.transform.position = transfer.driverPosition.transform.position;
                transfer.crewGameObject.transform.rotation = transfer.driverRotation;
                transfer.crewGameObject.transform.parent = gameObject.transform;
            }
        }
    }
}
