using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ThingTypes
{
    awg = 0,
    compressor = 1,
    condenser = 2,
    evaporator = 3,
    glass = 4,
    propeller = 5,
    pump = 6,
    tanksmall = 7,
    rubbertube = 8,
    valve = 9,
    electricdrill = 10,
    greasegun = 11,
    hammerdrill = 12,
    shovel = 13,
    solderinggun = 14,
    wrench = 15,
    chargingstation = 16,
    windturbine = 17,
    solarpanel = 18,
    wirecutters = 19,
    steel = 20,
    solarcell = 21,
    powercable = 22,
    mppt = 23,
    electricgenerator = 24,
    turbineblade = 25,
    martiancement = 26,
    lithiumbattery = 27,
    plasticdome = 28,
    vrvisor=29,
    iron=30,
    plexiglass=31,
    plasticsheet=32,
    flyRoverWheel=33,
    oxygencrate=34,
    lithiumcrate=35,
    tanklarge=36,
}

public class Thing
{
    public ThingTypes thingType;
    public string key, name, longName, workVerb;
    public bool isBlueprint, isTool;

    public Texture iconTexture;
    public int quantity = 1;
    public float durability, maxDurability;
    public float workRate = 10;
    public BlueprintDesign blueprintDesign;

    private Texture iconFrameTexture;
    private static Texture frameBlueprint, frameInventory;
    private const string frameFolder = "gui/things/";

    public override string ToString()
    {
        return "Thing (" + key + ") '" + longName + "'";
    }

    public BlueprintDesign GetBlueprintDesign()
    {
        if (blueprintDesign == null)
            blueprintDesign = BlueprintDesignManager.GetDefaultDesign(thingType);
        return blueprintDesign;
    }

    public bool IsTool()
    {
        return ThingFactory.IsTool(thingType);
    }

    public static Texture GetFrameBlueprintTexture()
    {
        if (frameBlueprint == null)
            frameBlueprint = Resources.Load(frameFolder + "frame blueprint") as Texture;
        return frameBlueprint;
    }

    public static Texture GetFrameInventoryTexture()
    {
        if (frameInventory == null)
            frameInventory = Resources.Load(frameFolder + "frame inventory") as Texture;
        return frameInventory;
    }

    public Texture GetIconFrameTexture()
    {
        if (iconFrameTexture == null)
        {
            if (isBlueprint)
                iconFrameTexture = GetFrameBlueprintTexture();
            else
                iconFrameTexture = GetFrameInventoryTexture();
        }
        return iconFrameTexture;
    }

}


