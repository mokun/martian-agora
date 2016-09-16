using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintDesign
{
    //other classes can ask this class what it takes to make the structure.
    //or it can be used by a Blueprint to build its BlueprintNodes.
    private List<DesignRequirement> designRequirements;
    public ThingTypes thingType;

    public List<DesignRequirement> GetDesignRequirements()
    {
        if (designRequirements == null)
            designRequirements = new List<DesignRequirement>();
        return designRequirements;
    }

    public Dictionary<ThingTypes, int> thingTotal;
    public Dictionary<ThingTypes, float> toolTotal;

    private void SetTotalDictionaries()
    {
        //this adds up all the things and tools from design requirements needed to complete this blueprint design
        thingTotal = new Dictionary<ThingTypes, int>();
        toolTotal = new Dictionary<ThingTypes, float>();

        foreach (DesignRequirement dr in designRequirements)
        {
            if (dr.isWorkRequirement)
            {
                if (!toolTotal.ContainsKey(dr.requiredThingType))
                    toolTotal.Add(dr.requiredThingType, 0);
                float count = dr.energyPerNode;
                if (dr.isLocationSubstring)
                    count *= dr.replacedNodeCount;
                toolTotal[dr.requiredThingType] += count;
            }
            else
            {
                if (!thingTotal.ContainsKey(dr.requiredThingType))
                    thingTotal.Add(dr.requiredThingType, 0);
                int count = dr.quantityPerNode;
                if (dr.isLocationSubstring)
                    count *= dr.replacedNodeCount;
                thingTotal[dr.requiredThingType] += count;
            }
        }
    }

    public Dictionary<ThingTypes, int> GetThingTotal()
    {
        //returns the number of required things for this blueprint design
        if (thingTotal == null)
            SetTotalDictionaries();
        return thingTotal;
    }

    public Dictionary<ThingTypes, float> GetToolTotal()
    {
        //returns the number of required things for this blueprint design
        if (toolTotal == null)
            SetTotalDictionaries();
        return toolTotal;
    }

    public bool IsSet()
    {
        //checks whether BlueprintDesign has been configured at all.
        return GetDesignRequirements().Count > 0;
    }

    public void AddThing(ThingTypes requiredThingType, int quantityPerNode, int replacedNodeCount)
    {
        string nodeSubstring = ThingFactory.GetKeyFromThingType(requiredThingType);
        AddThing(requiredThingType, quantityPerNode, replacedNodeCount, nodeSubstring);
    }

    public void AddThing(ThingTypes requiredThingType, int quantityPerNode, int replacedNodeCount, string nodeSubstring)
    {
        //adds a thing requirement for this blueprint design.
        //requiredThingType is the needed thing

        //nodeSubstring is used to help Blueprint build its BlueprintNodes. It uses nodeSubstring to find which GameObjects
        //to replace with a node.
        //quantityPerNode is how many requiredThingTypes are needed per node. replacedNodeCount is how many nodes to create.

        DesignRequirement dr = new DesignRequirement(requiredThingType);
        dr.SetSubstringLocation(nodeSubstring,replacedNodeCount);
        dr.SetThingRequirement(quantityPerNode);

        AddDesignRequirement(dr);
    }

    public void AddThing(ThingTypes requiredThingType, int quantity, Vector3 position)
    {
        //adds a thing requirement for this blueprint design.
        //adds one node that needs "quantity" number of "requiredThingType", offset by "position"

        DesignRequirement dr = new DesignRequirement(requiredThingType);
        dr.SetVectorLocation(position);
        dr.SetThingRequirement(quantity);
        AddDesignRequirement(dr);
    }

    public void AddTool(ThingTypes requiredThingType, float energyPerNode, int replacedNodeCount)
    {
        string nodeSubstring = ThingFactory.GetKeyFromThingType(requiredThingType);
        AddTool(requiredThingType, energyPerNode, replacedNodeCount, nodeSubstring);
    }

    public void AddTool(ThingTypes requiredThingType, float energyPerNode, int replacedNodeCount, string nodeSubstring)
    {
        //see comments for AddThing.
        DesignRequirement dr = new DesignRequirement(requiredThingType);
        dr.SetWorkRequirement(energyPerNode);
        dr.SetSubstringLocation(nodeSubstring, replacedNodeCount);
        AddDesignRequirement(dr);
    }

    public void AddDesignRequirement(DesignRequirement dr)
    {
        if (!dr.IsSet())
            Debug.LogError("AddDesignRequirement asked to add unset DesignRequirement. " + dr);
        GetDesignRequirements().Add(dr);
    }
}

