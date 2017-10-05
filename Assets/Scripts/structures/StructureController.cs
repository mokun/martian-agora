using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StructureTypes
{
    awg,
    chargingstation,
    windturbine,
    solarpanel,
    plasticdome,
    flyRoverWheel,
    oxygencrate,
    lithiumcrate,
    tanksmall,
    tanklarge
}

public class StructureController : MonoBehaviour
{
    //this is the generic class attached to every structure.

    public bool isRealStructure, isGettingEnoughResources = false;
    public StructureTypes structureType;
    public Thing matchingThing;

    //this is a messenger class used to communicate between the generic StructureController and the specific structure script
    public StructureInfo structureInfo;
    //this icon group shows structureInfo: production and consumption
    private IconGroupReusable igr;

    //the enum thing type that matches structureType
    private ThingTypes matchingThingType;

    private static Dictionary<StructureTypes, ThingTypes> thingAndStructurePairs;
    //a list of all structure controllers for real structures
    private static List<StructureController> structureControllers;
    private static int structureControllerCount = 0;

    public void SetupBlueprint()
    {
        isRealStructure = false;
        SetupGeneric();
        SetAllAnimations();
    }

    private void SetupGeneric()
    {
        //this stuff happens for blueprint structures and real structures

        //static
        SetThingStructurePairs();
        structureControllerCount++;

        matchingThingType = thingAndStructurePairs[structureType];
        matchingThing = ThingFactory.MakeThing(matchingThingType);
        structureInfo = new StructureInfo(matchingThingType);
    }

    public void SetupRealStructure()
    {
        isRealStructure = true;
        SetupGeneric();

        AddCollidersIfNone();
        AddScriptForStructureType();
        GetStructureControllers().Add(this);
        
        StartCoroutine(Cycle());
        SetAllAnimations();
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            UpdateCycleInterval();
            yield return new WaitForSeconds(GameManager.RecalculateWaitSeconds);
        }
    }

    private void UpdateCycleInterval()
    {
        structureInfo.timeSinceLastCycle = Time.timeSinceLevelLoad-structureInfo.timeOfLastCycle;
        structureInfo.timeOfLastCycle = Time.timeSinceLevelLoad;
    }

    public bool HasEnoughResourcesToOperate()
    {
        //returns true when this cycle has enough resources to operate.
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
        {
            float amount = GetAmountForThisInterval(resourceType);
            if (amount >= 0)
                continue;
            if (!ResourceController.ColonyHasAtLeastThisMuch(resourceType, Mathf.Abs(amount)))
                return false;
        }
        return true;
    }

    private float GetAmountForThisInterval(ResourceTypes resourceType)
    {
        //returns the amount of resourceType that this structure wishes to consume this cycle.
        float rate = structureInfo.rates[resourceType];
        return rate * structureInfo.timeSinceLastCycle * GameManager.timeFactor;
    }

    public void ConsumeAndProduceResources()
    {
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
        {
            float amount = GetAmountForThisInterval(resourceType);
            if (amount != 0)
                ResourceController.ChangeResource(resourceType, amount);
        }
    }

    public void SetAllHourlyRatesToOptimum()
    {
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
            structureInfo.rates[resourceType] = structureInfo.optimums[resourceType];
    }

    public void SetAllHourlyRates(float rate)
    {
        foreach (ResourceTypes resourceType in System.Enum.GetValues(typeof(ResourceTypes)))
            structureInfo.rates[resourceType] = rate;
    }

    public static List<StructureController> GetStructureControllers()
    {
        if (structureControllers == null)
            structureControllers = new List<StructureController>();
        return structureControllers;
    }

    public IconGroupReusable GetIconGroup()
    {
        if (igr == null)
            RecalculateIconGroup();
        return igr;
    }

    public void RecalculateIconGroup()
    {
        //this icon group is the production, consumption, storage, etc for each resource.

        //building an IconGroupReusable is too costly for all structures to do every frame.
        //RecalculateIconGroup is used on initialization and sparingly after that.

        igr = IconGroupReusable.GetResourcesIconGroupReusable(matchingThing.GetBlueprintDesign(), false);

    }

    public Texture GetStatusTexture()
    {
        return StatusManager.GetStatusTexture(structureInfo.status);
    }

    public string GetStatusLabel()
    {
        return StatusManager.GetStatusLabel(structureInfo.status);
    }

    public static StructureController GetStructureControllerFromChild(GameObject child)
    {
        Transform parentTransform = child.transform;
        while (parentTransform.parent != null && parentTransform.gameObject.GetComponent<StructureController>() == null)
            parentTransform = parentTransform.parent;

        return parentTransform.gameObject.GetComponent<StructureController>();
    }

    public bool IsUsableByAWG()
    {
        //if an awg is inside this structure, can it work?
        return structureType == StructureTypes.plasticdome;
    }

    private static void SetThingStructurePairs()
    {
        if (thingAndStructurePairs == null)
        {
            thingAndStructurePairs = new Dictionary<StructureTypes, ThingTypes>();
            thingAndStructurePairs.Add(StructureTypes.awg, ThingTypes.awg);
            thingAndStructurePairs.Add(StructureTypes.chargingstation, ThingTypes.chargingstation);
            thingAndStructurePairs.Add(StructureTypes.solarpanel, ThingTypes.solarpanel);
            thingAndStructurePairs.Add(StructureTypes.windturbine, ThingTypes.windturbine);
            thingAndStructurePairs.Add(StructureTypes.plasticdome, ThingTypes.plasticdome);
            thingAndStructurePairs.Add(StructureTypes.flyRoverWheel, ThingTypes.flyRoverWheel);
            thingAndStructurePairs.Add(StructureTypes.oxygencrate, ThingTypes.oxygencrate);
            thingAndStructurePairs.Add(StructureTypes.lithiumcrate, ThingTypes.lithiumcrate);
            thingAndStructurePairs.Add(StructureTypes.tanklarge, ThingTypes.tanklarge);
            thingAndStructurePairs.Add(StructureTypes.tanksmall, ThingTypes.tanksmall);
        }
    }

    private void SetAllAnimations()
    {
        //used to pause blueprint structure animations and set them to the end (for balloons)
        //or activates animation and sets to start.
        foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, true))
        {
            Animation a = child.GetComponent<Animation>();
            if (a == null)
                continue;

            AnimationState take = a["Default Take"];
            if (isRealStructure)
            {
                take.time = 0;
                take.speed = 1;
                a.Play();
            }
            else
            {
                take.time = take.length;
                take.speed = 0;
            }
        }
    }

    private void AddScriptForStructureType()
    {
        if (structureType == StructureTypes.windturbine)
            gameObject.AddComponent<WindTurbine>();
        if (structureType == StructureTypes.solarpanel)
            gameObject.AddComponent<SolarPanel>();
        if (structureType == StructureTypes.awg)
            gameObject.AddComponent<AtmosphericWaterGenerator>();
        if (structureType == StructureTypes.plasticdome)
            gameObject.AddComponent<PlasticDome>();

        //generic storage
        if (structureType == StructureTypes.tanklarge || structureType == StructureTypes.tanksmall ||
            structureType == StructureTypes.flyRoverWheel || structureType == StructureTypes.lithiumcrate ||
            structureType == StructureTypes.oxygencrate || structureType == StructureTypes.chargingstation)
            gameObject.AddComponent<GenericStorage>();

    }

    private void AddCollidersIfNone()
    {
        //adds mesh colliders to all children if there are no colliders.
        if (!HasColliders())
        {
            ParentChildFunctions.AddCollidersForChildren(gameObject);
            ParentChildFunctions.SetCollisionForChildren(gameObject, true);
        }

    }

    public bool HasColliders()
    {
        //checks if any child has a collider.
        foreach (GameObject child in ParentChildFunctions.GetAllChildren(gameObject, true))
            if (child.GetComponent<Collider>() != null)
                return true;
        return false;
    }
}
