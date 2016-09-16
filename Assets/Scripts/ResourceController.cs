using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ResourceController
{
    private static Dictionary<ResourceTypes, float> resources;

    private static Dictionary<ResourceTypes, float> GetResources()
    {
        if (resources == null)
        {
            resources = new Dictionary<ResourceTypes, float>();
            foreach (ResourceTypes rt in System.Enum.GetValues(typeof(ResourceTypes)))
                resources.Add(rt, 0);
        }
        return resources;
    }

    public static float GetResourceQuantityFromType(ResourceTypes resourceType)
    {
        return GetResources()[resourceType];
    }

    public static bool ColonyHasAtLeastThisMuch(ResourceTypes resourceType, float thisMuch)
    {
        if (thisMuch < 0)
            Debug.LogError("ColonyHasAtLeastThisMuch got a negative value. " + thisMuch);
        foreach (StructureController sc in StructureController.GetStructureControllers())
        {
            thisMuch -= sc.structureInfo.reserves[resourceType];
            if (thisMuch < 0)
                return true;
        }
        return false;
    }

    private static void AddResources(ResourceTypes resourceType, float positiveAmount)
    {
        float amountRemaining = positiveAmount;
        foreach (StructureController sc in StructureController.GetStructureControllers())
        {
            if (!sc.structureInfo.HasSpaceLeft(resourceType))
                continue;

            float freeSpace = sc.structureInfo.GetSpaceLeft(resourceType);
            float givenAmount = amountRemaining > freeSpace ? freeSpace : amountRemaining;
            amountRemaining -= givenAmount;
            sc.structureInfo.ChangeResourceAmount(resourceType, givenAmount);
        }

        GetResources()[resourceType] += positiveAmount-amountRemaining;
    }

    private static void TakeResources(ResourceTypes resourceType, float negativeAmount)
    {
        float debtRemaining = Mathf.Abs(negativeAmount);
        foreach (StructureController sc in StructureController.GetStructureControllers())
        {
            float reserve = sc.structureInfo.reserves[resourceType];
            if (reserve < 0.01)
                continue;

            float takenAmount = debtRemaining;
            if (debtRemaining > reserve)
                takenAmount = reserve;

            debtRemaining -= takenAmount;
            sc.structureInfo.ChangeResourceAmount(resourceType, -takenAmount);
        }

        GetResources()[resourceType] += negativeAmount;
    }

    public static void ChangeResource(ResourceTypes resourceType, float changeAmount)
    {
        if (changeAmount == 0)
            return;
        if (changeAmount > 0)
            AddResources(resourceType, changeAmount);
        else
            TakeResources(resourceType, changeAmount);

    }
}
