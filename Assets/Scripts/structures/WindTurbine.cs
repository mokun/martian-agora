using UnityEngine;
using System.Collections;

public class WindTurbine : MonoBehaviour
{
    private float rotateSpeed = 0;
    GameObject rotatingHub;

    public StructureController structureController;

    void Start()
    {
        StartCoroutine(Cycle());
    }

    private StructureInfo GetStructureInfo()
    {
        if (structureController == null)
            structureController = gameObject.GetComponent<StructureController>();
        return structureController.structureInfo;
    }

    private StructureController GetStructureController()
    {
        if (structureController == null)
            structureController = gameObject.GetComponent<StructureController>();
        return structureController;
    }

    private void SetHourlyRates()
    {
        //this makes tweaks to the resource hourly production rates.
        //setting hourly rates to zero/off is controlled by the structure controller as a generic task.
        if (GetStructureController().isRealStructure)
        {
            float effectiveness = rotateSpeed / GetTerminalRotateSpeed();
            float newRate = GetStructureInfo().optimums[ResourceTypes.electricity] * effectiveness;
            GetStructureInfo().rates[ResourceTypes.electricity] = newRate;
        }
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            SetHourlyRates();
            if (GetStructureInfo().status == Statuses.active)
                GetStructureController().ConsumeAndProduceResources();
            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }

    private GameObject GetRotatingHub()
    {
        if (rotatingHub == null)
        {
            foreach (GameObject child in ParentChildFunctions.GetAllChildren(this.gameObject))
            {
                if (child.name.Equals("hub"))
                {
                    rotatingHub = child;
                    break;
                }
            }
        }
        return rotatingHub;
    }

    private float GetTerminalRotateSpeed()
    {
        //this is the maximum speed the blades will approach.
        return 12;
    }

    private void UpdateRotateSpeed()
    {
        if (rotateSpeed < GetTerminalRotateSpeed())
            rotateSpeed += Time.deltaTime;
    }

    void Update()
    {
        UpdateRotateSpeed();
        GetRotatingHub().transform.Rotate(new Vector3(0, 0, rotateSpeed));
    }
}
