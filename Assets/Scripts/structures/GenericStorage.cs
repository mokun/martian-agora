using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericStorage : MonoBehaviour
{
    //this class is the "specific structure script" that can be attached to any basic storage structure
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

    public IEnumerator Cycle()
    {
        while (true)
        {
            if (GetStructureController().isRealStructure)
                UpdateStatus();

            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }

    private void UpdateStatus()
    {
        if (GetStructureInfo().status == Statuses.off)
            return;

        if (IsFull())
            GetStructureInfo().status = Statuses.full;
        else if (IsEmpty())
            GetStructureInfo().status = Statuses.empty;
        else
            GetStructureInfo().status = Statuses.active;
    }

    private bool IsFull()
    {
        //returns true if any of the resource capacities are filled nearly 100%
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
        {
            float reserve = GetStructureInfo().reserves[resourceType];
            float capacity = GetStructureInfo().capacities[resourceType];
            if (capacity > 0 && capacity - reserve < 0.001)
                return true;
        }
        return false;
    }

    private bool IsEmpty()
    {
        //returns true if and all the resource reserves are empty.
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
            if (GetStructureInfo().reserves[resourceType] > 0)
                return false;
        return true;
    }
}