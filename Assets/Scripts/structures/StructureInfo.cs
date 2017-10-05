using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StructureInfo
{
    //used by structure controllers and specific structure scripts to communicate standard information like
    //resources, status, storage
    public Dictionary<ResourceTypes, float> rates, optimums, capacities, reserves;
    private Dictionary<InfoTypes, Dictionary<ResourceTypes, float>> resourceDictionary;
    public ThingTypes thingType;

    public float timeOfLastCycle, timeSinceLastCycle;

    public Statuses status = Statuses.active;

    public enum InfoTypes
    {
        rate,
        optimum,
        capacity,
        reserve
    }

    public StructureInfo(ThingTypes thingType)
    {
        this.thingType = thingType;
        SetDictionaries();
        SetConstants();
    }

    private void SetDictionaries()
    {
        rates = new Dictionary<ResourceTypes, float>();
        optimums = new Dictionary<ResourceTypes, float>();
        capacities = new Dictionary<ResourceTypes, float>();
        reserves = new Dictionary<ResourceTypes, float>();

        resourceDictionary = new Dictionary<InfoTypes, Dictionary<ResourceTypes, float>>();
        resourceDictionary.Add(InfoTypes.rate, rates);
        resourceDictionary.Add(InfoTypes.optimum, optimums);
        resourceDictionary.Add(InfoTypes.capacity, capacities);
        resourceDictionary.Add(InfoTypes.reserve, reserves);

        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
        {
            rates.Add(resourceType, 0);
            optimums.Add(resourceType, 0);
            capacities.Add(resourceType, 0);
            reserves.Add(resourceType, 0);
        }
    }

    private void SetConstants()
    {
        //water and electricity
        if (thingType == ThingTypes.awg)
        {
            SetConstant(ResourceTypes.electricity, -8f, 0);
            SetConstant(ResourceTypes.water, 0.1f, 100);
        }
        if (thingType == ThingTypes.solarpanel)
            SetConstant(ResourceTypes.electricity, 0.5f, 0);
        if (thingType == ThingTypes.windturbine)
            SetConstant(ResourceTypes.electricity, 3f, 0);
        if (thingType == ThingTypes.chargingstation)
            SetConstant(ResourceTypes.electricity, 0, 40);

        //storage
        if (thingType == ThingTypes.tanksmall)
            SetConstant(ResourceTypes.water, 0, 12000);
        if (thingType == ThingTypes.tanklarge)
            SetConstant(ResourceTypes.water, 0, 96000);
        if (thingType == ThingTypes.flyRoverWheel)
            SetConstant(ResourceTypes.electricity, 0, 120);
        if (thingType == ThingTypes.oxygencrate)
            SetConstant(ResourceTypes.oxygen, 0, 1800);
        if (thingType == ThingTypes.lithiumcrate)
            SetConstant(ResourceTypes.electricity, 0, 180);
    }

    private void SetConstant(ResourceTypes resourceType, float optimumRate, float capacity)
    {
        optimums[resourceType] = optimumRate;
        capacities[resourceType] = capacity;
    }

    public float GetSpaceLeft(ResourceTypes resourceType)
    {
        return capacities[resourceType] - reserves[resourceType];
    }

    public bool HasSpaceLeft(ResourceTypes resourceType)
    {
        return GetSpaceLeft(resourceType) > 0.0001;
    }

    public void ChangeResourceAmount(ResourceTypes resourceType, float amount)
    {
        //where amount can be negative or positive.
        reserves[resourceType] += amount;
        if (reserves[resourceType] > capacities[resourceType])
            Debug.LogError("r>c capacity=" + capacities[resourceType] + " reserves=" + reserves[resourceType] + " amount=" + amount);
        if (reserves[resourceType] < 0)
            Debug.LogError("r<0 capacity=" + capacities[resourceType] + " reserves=" + reserves[resourceType] + " amount=" + amount);
    }

    public int GetInfoIconCount(ResourceTypes resourceType, InfoTypes infoType)
    {
        //used to decide how many pictographs to show based on the consumption/production/capacity
        //when infoType is "rate", can return a negative which means consuming resources.
        if (infoType == InfoTypes.rate)
        {
            float rate = rates[resourceType];
            return RoundAwayFromZero(rate);
        }
        else if (infoType == InfoTypes.optimum)
        {
            if (resourceType == ResourceTypes.electricity)
                return RoundAwayFromZero(optimums[resourceType]);
            if (resourceType == ResourceTypes.inspiration)
                return 0;

            return RoundAwayFromZero(optimums[resourceType] / 100);
        }
        else if (infoType == InfoTypes.capacity)
        {
            float capacity = capacities[resourceType];
            if (resourceType == ResourceTypes.water || resourceType == ResourceTypes.oxygen)
                capacity /= 1000;

            return Mathf.CeilToInt(Mathf.Sqrt(capacity));
        }

        Debug.LogError("GetInfoIconCount infoType is not implemented. infoType=" + infoType);
        return 0;
    }

    private int RoundAwayFromZero(float number)
    {
        if (number < 0)
            return Mathf.FloorToInt(number);
        return Mathf.CeilToInt(number);
    }

}
