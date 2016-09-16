using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintNodeTask
{
    //these represent work that needs to be done to complete a node.
    //one BlueprintNode can have many BlueprintNodeTasks. Maybe first you dig, then you pour cement.

    public ThingTypes requiredThingType;
    //thing related
    public int quantityRequired, quantityFilled;

    //work related
    public float workRequired, workFilled;
    public AudioClip audioClip;
    private static Dictionary<string, AudioClip> audioClips;

    public bool isSet = false;

    public BlueprintNode.NodeModes nodeMode;

    public BlueprintNodeTask(int quantityRequired, ThingTypes requiredThingType)
    {
        isSet = true;
        nodeMode = BlueprintNode.NodeModes.thing;

        this.quantityRequired = quantityRequired;
        quantityFilled = 0;
        this.requiredThingType = requiredThingType;
    }

    public BlueprintNodeTask(float workRequired, ThingTypes requiredThingType)
    {
        isSet = true;
        nodeMode = BlueprintNode.NodeModes.work;

        this.workRequired = workRequired;
        workFilled = 0;
        this.requiredThingType = requiredThingType;

        SetAudioClip(requiredThingType);
    }


    public bool IsDone()
    {
        if (nodeMode == BlueprintNode.NodeModes.thing && quantityFilled >= quantityRequired)
            return true;
        if (nodeMode == BlueprintNode.NodeModes.work && workFilled >= workRequired)
            return true;
        if (nodeMode == BlueprintNode.NodeModes.constructed)
            return true;
        return false;
    }

    public void SetAudioClip(ThingTypes thingType)
    {
        
        if (thingType == ThingTypes.greasegun || thingType == ThingTypes.hammerdrill ||
            thingType == ThingTypes.shovel || thingType == ThingTypes.wrench || thingType == ThingTypes.electricdrill)
        {
            if (audioClips == null)
                audioClips = new Dictionary<string, AudioClip>();

            string filename = ThingFactory.GetKeyFromThingType(thingType);
            string path = "sounds/tools/" + filename;

            if (!audioClips.ContainsKey(path))
            {
                audioClip = Resources.Load(path) as AudioClip;
                if (audioClip == null)
                    Debug.LogError("Expected to find an audioclip, but did not. '" + path + "'");
                audioClips.Add(path, audioClip);
            }
            audioClip = audioClips[path];
        }
    }

    public override string ToString()
    {
        string result = "BlueprintNodeTask "+nodeMode;
                    if (nodeMode == BlueprintNode.NodeModes.thing)
                        result+=" quantityRequired="+quantityRequired+" quantityFilled="+quantityFilled;

        if (nodeMode == BlueprintNode.NodeModes.work)
            result+=" workRequired="+workRequired+" workFilled="+workFilled;
        return result;
    }

}