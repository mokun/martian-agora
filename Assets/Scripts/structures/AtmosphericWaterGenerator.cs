using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtmosphericWaterGenerator : MonoBehaviour
{
    public StructureController structureController;
    private StructureController domeStructureController;
    private float propellerRotateSpeed;
    private Vector3 rotateAxis;
    private GameObject[] propellers;

    void Start()
    {
        propellerRotateSpeed = 0;
        rotateAxis = GetPropeller(0).transform.forward;
        StartCoroutine(Cycle());
    }

    private float GetMaxPropellerRotateSpeed()
    {
        return 20;
    }

    private GameObject GetPropeller(int index)
    {
        if (propellers == null)
        {
            propellers = new GameObject[2];
            propellers[0] = ParentChildFunctions.GetChildWithNameSubstring(gameObject, "propeller1");
            propellers[1] = ParentChildFunctions.GetChildWithNameSubstring(gameObject, "propeller2");
        }
        return propellers[index];
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
        if (domeStructureController == null)
        {
            GetStructureInfo().status = Statuses.notsetup;
            GetStructureController().SetAllHourlyRates(0);
        }
        else
        {
            GetStructureController().SetAllHourlyRatesToOptimum();
        }
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            if (GetStructureController().isRealStructure)
            {
                SetDomeStructureController();
                SetHourlyRates();
                UpdateStatus();

                if (GetStructureInfo().status == Statuses.active)
                    GetStructureController().ConsumeAndProduceResources();
            }
            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }

    private void UpdatePropellerRotateSpeed()
    {
        if (GetStructureInfo().status == Statuses.active)
        {
            float multiplier = 3;
            if (propellerRotateSpeed < GetMaxPropellerRotateSpeed())
                propellerRotateSpeed += Time.deltaTime * multiplier;
        }
        else
        {            
            if (propellerRotateSpeed < 1)
                propellerRotateSpeed = 0;
            else
                propellerRotateSpeed *= 1 - 0.01f * (1 / Time.deltaTime);
        }
    }

    private void UpdateStatus()
    {
        if (GetStructureInfo().status == Statuses.off)
            return;

        if (domeStructureController == null)
            GetStructureInfo().status = Statuses.notsetup;
        else if (GetStructureController().HasEnoughResourcesToOperate())
            GetStructureInfo().status = Statuses.active;
        else
            GetStructureInfo().status = Statuses.lowpower;

    }

    private void SetDomeStructureController()
    {
        if (domeStructureController == null || !domeStructureController.IsUsableByAWG())
        {
            foreach (RaycastHit raycastHit in Physics.RaycastAll(transform.position, Vector3.up, 100))
            {
                StructureController sc = StructureController.GetStructureControllerFromChild(raycastHit.transform.gameObject);
                if (sc != null && sc.IsUsableByAWG())
                {
                    domeStructureController = sc;
                    return;
                }
            }
        }
    }

    void Update(){
        if (GetStructureController().isRealStructure)
        {
            UpdatePropellerRotateSpeed();
            for (int i = 0; i < 2; i++)
                GetPropeller(i).transform.Rotate(rotateAxis, propellerRotateSpeed);
        }
    }
}
