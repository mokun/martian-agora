using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wheel : MonoBehaviour
{
    public enum Modes
    {
        no_contact,
        contact,
        below_terrain
    }

    public Transform vehiculeTransform;
    public bool showGizmos;

    public float wheelRadius;
    private float wheelContactBuffer, ratioContactBuffer, extendLength, radiusPlusBuffer;
    private GameObject retractedGO, extendedGO, extendedContactPointGO;

    //from 0 to 1, where 0 is fully retracted, 1 is fully extended
    private float positionRatio;
    private float ratioSpeed, ratioAcceleration;

    RaycastHit lastRaycastHit;

    public void Initialize(Transform vehicleTransform)
    {
        this.vehiculeTransform = vehicleTransform;
        positionRatio = 0;
        ratioSpeed = 0;
        ratioAcceleration = 0.1f;

        float wheelDiameter = GetComponent<Renderer>().bounds.size.y;
        wheelRadius = wheelDiameter / 2;
        wheelContactBuffer = wheelRadius * 0.1f;
        extendLength = wheelDiameter * 2 / 3;
        ratioContactBuffer = wheelContactBuffer / extendLength;
        radiusPlusBuffer = wheelRadius + wheelContactBuffer;

        SetPointGameObjects();
    }

    private void SetPointGameObjects()
    {
        retractedGO = new GameObject(gameObject.name + " retracted");
        retractedGO.transform.position = transform.position;
        retractedGO.transform.parent = transform.parent;

        extendedGO = new GameObject(gameObject.name + " extended");
        extendedGO.transform.position = transform.position + vehiculeTransform.up * -1 * extendLength; ;
        extendedGO.transform.parent = transform.parent;

        extendedContactPointGO = new GameObject(gameObject.name + " mostExtendedContactPoint");
        extendedContactPointGO.transform.position = transform.position + vehiculeTransform.up * -1 * (extendLength + radiusPlusBuffer);
        extendedContactPointGO.transform.parent = transform.parent;
    }

    void OnDrawGizmos()
    {
        return;
        if (showGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(retractedGO.transform.position, 0.2f);
            Gizmos.DrawSphere(extendedGO.transform.position, 0.2f);
            Gizmos.DrawSphere(extendedContactPointGO.transform.position, 0.1f);
        }
    }

    private void Debug1(string text)
    {
        if (gameObject.name.Equals("wheel-1R"))
            Debug.Log(Time.frameCount + " " + text);
    }

    private float GetTargetRatio()
    {
        //if the rover freezes its position and the wheel falls all the way, this returns that position ratio.

        if (IsRetractedBelowTerrain())
        {
            Debug1("IsRetractedBelowTerrain=true");
            return 0;
        }

        bool isHit = Physics.Linecast(retractedGO.transform.position, extendedContactPointGO.transform.position, out lastRaycastHit);
        if (!isHit)
            return 1;

        float ratio = (lastRaycastHit.distance - radiusPlusBuffer) / extendLength;
        /*
        if (Mathf.Clamp(ratio, 0, 1) == 0)
        Debug1(("ratio=" + ratio.ToString() + " lastRaycastHit.distance=" +
            lastRaycastHit.distance.ToString() + " radiusPlusBuffer=" +
            radiusPlusBuffer.ToString() + " extendLength=" + extendLength.ToString())+" hitobject="+lastRaycastHit.collider.gameObject.name);
         */
        return Mathf.Clamp(ratio, 0, 1);
    }

    public Modes GetMode()
    {
        float targetRatio = GetTargetRatio();

        if (positionRatio < targetRatio - ratioContactBuffer || lastRaycastHit.collider == null)
            return Modes.no_contact;

        if (positionRatio > targetRatio + ratioContactBuffer)
        {
            //Debug1("positionRatio=" + positionRatio.ToString() + " targetRatio=" + targetRatio.ToString() + " ratioContactBuffer=" + ratioContactBuffer.ToString());
            return Modes.below_terrain;
        }

        return Modes.contact;
    }

    private bool IsRetractedBelowTerrain()
    {
        float altitude = Environment.GetAltitude(transform.position);
        return retractedGO.transform.position.y + radiusPlusBuffer < altitude;
    }

    public float GetForceRatio()
    {
        //from 0 to 1
        //where 1 means the wheel should push with 100% of its force
        float ratio = 1 - positionRatio;
        return Mathf.Clamp(ratio, 0, 1);
    }

    private void MakeWheelFall()
    {
        ratioSpeed += Time.deltaTime * ratioAcceleration;
        positionRatio += ratioSpeed;
        float targetRatio = GetTargetRatio();
        if (positionRatio > targetRatio)
        {
            positionRatio = targetRatio;
            ratioSpeed = 0;
        }
    }

    void Update()
    {
        //this whole class is begging for optimization.
        MakeWheelFall();


        transform.position = Vector3.Lerp(retractedGO.transform.position, extendedGO.transform.position, positionRatio);
    }
}
