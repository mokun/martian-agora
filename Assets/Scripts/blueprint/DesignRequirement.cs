using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DesignRequirement
{
    //a blueprint design consists of many design requirements.
    //all must be filled in order for the blueprint to be considered built.
    public ThingTypes requiredThingType;
    public int quantityPerNode, replacedNodeCount;
    public float energyPerNode;
    public string nodeSubstring;
    public Vector3 position;

    public bool isLocationSubstring,
        isLocationVector,
        isThingRequirement,
        isWorkRequirement= false;

    public DesignRequirement(ThingTypes requiredThingType)
    {
        this.requiredThingType = requiredThingType;
    }

    public void SetSubstringLocation(string nodeSubstring, int replacedNodeCount)
    {
        isLocationSubstring = true;
        this.nodeSubstring = nodeSubstring;
        this.replacedNodeCount = replacedNodeCount;
    }

    public void SetVectorLocation(Vector3 position)
    {
        isLocationVector = true;
        this.position = position;
    }

    public void SetThingRequirement(int quantityPerNode)
    {
        isThingRequirement = true;
        this.quantityPerNode = quantityPerNode;
    }

    public void SetWorkRequirement(float energyPerNode)
    {
        isWorkRequirement = true;
        this.energyPerNode = energyPerNode;
    }

    public bool IsSet()
    {
        return (isThingRequirement || isWorkRequirement) && (isLocationSubstring || isLocationVector);
    }

    public override string ToString()
    {
        return "DesignRequirement requiredThingType="+requiredThingType+" quantityPerNode="+quantityPerNode+" replacedNodeCount="+replacedNodeCount+" energyPerNode="+energyPerNode+" nodeSubstring="+nodeSubstring;
    }
}