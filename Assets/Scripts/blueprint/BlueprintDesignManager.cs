using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BlueprintDesignManager
{
    public static List<BlueprintDesign> blueprintDesigns;
    private static Dictionary<ThingTypes, BlueprintDesign> thingTypeDesigns;

    public static void Initialize()
    {
        thingTypeDesigns = new Dictionary<ThingTypes, BlueprintDesign>();
        blueprintDesigns = new List<BlueprintDesign>();

        SetAllDefaultDesigns();
    }

    private static void SetAllDefaultDesigns()
    {
        foreach (ThingTypes thingType in ThingFactory.blueprintThingTypes)
            GetDefaultDesign(thingType);
    }

    public static BlueprintDesign GetDefaultDesignFromThingType(ThingTypes thingType)
    {
        if (!thingTypeDesigns.ContainsKey(thingType))
            GetDefaultDesign(thingType);

        return thingTypeDesigns[thingType];
    }

    private static void AddBuildRequirements(BlueprintDesign bd)
    {
        if (bd.thingType == ThingTypes.awg)
        {
            bd.AddThing(ThingTypes.compressor, 1, 1);
            bd.AddThing(ThingTypes.condenser, 1, 1);
            bd.AddThing(ThingTypes.evaporator, 1, 1);
            bd.AddThing(ThingTypes.glass, 6, 1);
            bd.AddThing(ThingTypes.propeller, 1, 2);
            bd.AddThing(ThingTypes.pump, 1, 1);
            bd.AddThing(ThingTypes.iron, 2, 2,"tank");
            bd.AddThing(ThingTypes.rubbertube, 4, 1);
            bd.AddThing(ThingTypes.valve, 1, 2);

            bd.AddTool(ThingTypes.greasegun, 15, 2, "propeller");
            bd.AddTool(ThingTypes.wrench, 5, 2, "valve");
        }
        else if (bd.thingType == ThingTypes.windturbine)
        {
            bd.AddThing(ThingTypes.mppt, 1, 1);
            bd.AddThing(ThingTypes.turbineblade, 1, 1);
            bd.AddThing(ThingTypes.powercable, 4, 1);

            bd.AddThing(ThingTypes.steel, 2, 1, "frame");

            bd.AddTool(ThingTypes.shovel, 40, 1, "cement-base");
            bd.AddThing(ThingTypes.martiancement, 4, 1, "cement-base");

            bd.AddTool(ThingTypes.wirecutters, 5, 1, "power-cable");
            bd.AddTool(ThingTypes.greasegun, 10, 1, "hub");
        }
        else if (bd.thingType == ThingTypes.solarpanel)
        {
            bd.AddThing(ThingTypes.mppt, 1, 1);
            bd.AddThing(ThingTypes.solarcell, 1, 4);
            bd.AddThing(ThingTypes.powercable, 1, 1);

            bd.AddThing(ThingTypes.steel, 1, 1, "supports");
            bd.AddTool(ThingTypes.wirecutters, 5, 1, "power-cable");
        }
        else if (bd.thingType == ThingTypes.chargingstation)
        {
            bd.AddThing(ThingTypes.lithiumbattery, 12, 1, "batteries");
            bd.AddThing(ThingTypes.powercable, 1, 1, "cable-big");
            bd.AddThing(ThingTypes.powercable, 5, 1, "cables");
            bd.AddTool(ThingTypes.wirecutters, 25, 1, "cables");
        }
        else if (bd.thingType == ThingTypes.plasticdome)
        {
            foreach (Vector3 point in GetPointsOnCircle(20, 8, 1))
                bd.AddThing(ThingTypes.plasticsheet, 5, point);
        }
        else if (bd.thingType == ThingTypes.tanksmall || bd.thingType == ThingTypes.tanklarge)
        {
            float multiplier = bd.thingType == ThingTypes.tanksmall ? 1 : 5;
            foreach (Vector3 point in GetPointsOnCircle(2 * multiplier, 6, 1))
                bd.AddThing(ThingTypes.iron, 1, point);
            bd.AddThing(ThingTypes.valve, 1, 1);
            bd.AddTool(ThingTypes.wrench, 15 * multiplier, 1, "valve");
        }
        else if (bd.thingType == ThingTypes.oxygencrate)
        {
            bd.AddThing(ThingTypes.iron, 3, 1, "cage");
            bd.AddThing(ThingTypes.iron, 9, 1, "tank");
            bd.AddThing(ThingTypes.valve, 9, 1);
            bd.AddTool(ThingTypes.wrench, 30, 1, "valve");
        }
        else if (bd.thingType == ThingTypes.lithiumcrate)
        {
            bd.AddThing(ThingTypes.iron, 3, 1, "cage");
            bd.AddThing(ThingTypes.lithiumbattery, 8, 1, "battery");
            bd.AddTool(ThingTypes.wirecutters, 25, 1, "display");
        }
        else if (bd.thingType == ThingTypes.flyRoverWheel)
        {
            bd.AddThing(ThingTypes.steel, 3, 1, "cylinder");
            bd.AddThing(ThingTypes.steel, 3, 1, "top");
            bd.AddTool(ThingTypes.wirecutters, 1, 1, "frame");
            bd.AddTool(ThingTypes.solderinggun, 15, 1, "top");
        }
    }

    public static BlueprintDesign GetDefaultDesign(ThingTypes thingType)
    {
        if (thingTypeDesigns.ContainsKey(thingType))
            return thingTypeDesigns[thingType];

        BlueprintDesign bd = new BlueprintDesign();
        bd.thingType = thingType;
        AddBuildRequirements(bd);

        if (!bd.IsSet())
            Debug.LogError("GetDefaultDesign did not set the BlueprintDesign at all for: " + thingType);
                
        blueprintDesigns.Add(bd);
        thingTypeDesigns.Add(thingType, bd);
        return bd;
    }

    private static List<Vector3> GetPointsOnCircle(float radius, int numberOfPoints, float height)
    {
        //returns a list of points that are evenly distributed on a circle with this "radius", elevated to "height".
        List<Vector3> list = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            float x = Mathf.Cos(2*Mathf.PI /numberOfPoints* i) * radius;
            float y = Mathf.Sin(2*Mathf.PI /numberOfPoints* i) * radius;
            list.Add(new Vector3(x, height, y));
        }

        return list;
    }
}
