using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintNode : MonoBehaviour
{
    //one node can contain any number of tasks (BlueprintNodeTask). 
    //blueprints require all their blueprint nodes to be complete in order for that blueprint to be considered built.

    //the blueprint this node belongs to.
    private Blueprint blueprintParent;

    private TextMesh[] textMeshes;
    private GameObject thingGameObject, workGameObject;

    private static Material thingMaterial, doneMaterial, workMaterial;
    private AudioSource audioSource;

    private List<BlueprintNodeTask> nodeTasks;
    public enum NodeModes
    {
        thing,
        work,
        constructed
    }

    private List<BlueprintNodeTask> GetNodeTasks()
    {
        if (nodeTasks == null)
            nodeTasks = new List<BlueprintNodeTask>();
        return nodeTasks;
    }

    private static Material GetThingMaterial()
    {
        if (thingMaterial == null)
            thingMaterial = Resources.Load("gui/blueprint") as Material;
        return thingMaterial;
    }

    private static Material GetWorkMaterial()
    {
        if (workMaterial == null)
            workMaterial = Resources.Load("gui/work node") as Material;
        return workMaterial;
    }

    private static Material GetDoneMaterial()
    {
        if (doneMaterial == null)
            doneMaterial = Resources.Load("gui/done node") as Material;
        return doneMaterial;
    }

    private TextMesh GetTextMesh(int index)
    {
        if (textMeshes == null)
            textMeshes = new TextMesh[3];

        if (textMeshes[index] == null)
        {
            GameObject go = transform.GetChild(0).gameObject;
            GameObject textGameObject = go.transform.GetChild(index).gameObject;
            textMeshes[index] = textGameObject.GetComponent<TextMesh>();
        }
        return textMeshes[index];
    }

    public GameObject GetThingGameObject()
    {
        if (thingGameObject == null)
            thingGameObject = transform.Find("thing node").gameObject;
        return thingGameObject;
    }

    private GameObject GetWorkGameObject()
    {
        if (workGameObject == null)
            workGameObject = transform.Find("work node").gameObject;
        return workGameObject;
    }

    private AudioSource GetAudioSource()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        return audioSource;
    }

    private BlueprintNodeTask GetCurrentBlueprintNodeTask()
    {
        for (int i = 0; i < GetNodeTasks().Count; i++)
        {
            if (!nodeTasks[i].IsDone())
                return nodeTasks[i];
        }
        return null;
    }

    private Blueprint GetBlueprintParent()
    {
        if (blueprintParent == null)
            blueprintParent = transform.parent.gameObject.GetComponent<Blueprint>();
        return blueprintParent;
    }

    void Start()
    {
        UpdateNodeTask();
    }

    public void AddRequiredTool(ThingTypes toolThingType, float requiredWork)
    {
        //this node will require work done with this tool type.
        BlueprintNodeTask nodeTask = new BlueprintNodeTask(requiredWork, toolThingType);
        GetNodeTasks().Add(nodeTask);
    }

    public void AddRequiredThing(ThingTypes thingType, int requiredQuantity)
    {
        //this node will require a quantity of this thing
        BlueprintNodeTask nodeTask = new BlueprintNodeTask(requiredQuantity, thingType);
        GetNodeTasks().Add(nodeTask);
    }

    public void HoldClickedByPlayer(Thing selectedThing, Crew crewStatus)
    {
        //called when the player is holding click on this node.
        //applies work done by the thing if it matches the required tool
        if (selectedThing == null)
            return;

        BlueprintNodeTask currentTask = GetCurrentBlueprintNodeTask();

        if (currentTask.nodeMode == NodeModes.work)
        {
            if (selectedThing.thingType == currentTask.requiredThingType &&
                currentTask.workFilled < currentTask.workRequired && crewStatus.GetBatteryEnergy() > 0)
            {
                float energyUsed = Time.deltaTime * selectedThing.workRate;
                if (energyUsed > crewStatus.GetBatteryEnergy())
                    energyUsed = crewStatus.GetBatteryEnergy();

                currentTask.workFilled += energyUsed;
                crewStatus.UseEnergyThing(selectedThing, energyUsed);
                StartToolSound();
            }
            else
            {
                StopToolSound();
            }

            UpdateNodeTask();
        }
    }

    private void SetupAudioSource(ThingTypes thingType)
    {
        GetAudioSource().clip = GetCurrentBlueprintNodeTask().audioClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void StartToolSound()
    {
        if (audioSource != null && !audioSource.isPlaying && audioSource.clip!=null)
        {
            audioSource.time = Random.Range(0, audioSource.clip.length);
            audioSource.Play();
        }
    }

    private void StopToolSound()
    {
        if (audioSource!=null)
            audioSource.Stop();
    }

    public void UnholdClick()
    {
        StopToolSound();
    }

    public void ClickedByPlayer(Crew crewStatus)
    {
        //called when the player clicks on this node.
        //if necessary, takes items from the player inventory, if they have it, and applies it to the thing node.
        if (GetCurrentBlueprintNodeTask().nodeMode == NodeModes.thing)
        {
            BlueprintNodeTask nodeTask = GetCurrentBlueprintNodeTask();

            Thing foundThing = crewStatus.FindThingFromInventory(nodeTask.requiredThingType);
            if (foundThing != null && nodeTask.quantityFilled < nodeTask.quantityRequired && foundThing.quantity > 0)
            {
                int quantityUsed = nodeTask.quantityRequired - nodeTask.quantityFilled;
                if (quantityUsed > foundThing.quantity)
                    quantityUsed = foundThing.quantity;

                nodeTask.quantityFilled += quantityUsed;
                crewStatus.UseThing(foundThing, quantityUsed);
            }

            UpdateNodeTask();
        }
    }

    private void SetNodeMaterial(Material newMaterial)
    {
        thingGameObject.GetComponent<Renderer>().material = newMaterial;
        workGameObject.GetComponent<Renderer>().material = newMaterial;
    }

    private NodeModes GetCurrentNodeMode()
    {
        //BlueprintNodes can contain many BlueprintNodeTasks. This returns the current nodeMode, or if they're
        //all constructed, returns constructed.
        BlueprintNodeTask nodeTask = GetCurrentBlueprintNodeTask();
        if (nodeTask == null)
            return NodeModes.constructed;
        return nodeTask.nodeMode;
    }

    private void SetWorkVisibility(bool isVisible)
    {
        //makes the work node appear in game
        GetWorkGameObject().GetComponent<MeshCollider>().enabled = isVisible;
        workGameObject.GetComponent<Renderer>().enabled = isVisible;
    }

    private void SetThingVisibility(bool isVisible)
    {
        //makes the thing node appear in game
        GetThingGameObject().GetComponent<SphereCollider>().enabled = isVisible;
        thingGameObject.GetComponent<Renderer>().enabled = isVisible;
    }

    private void SetNodeAppearanceAndSound()
    {
        //update the text, appearance, and sound for this node.

        BlueprintNodeTask nodeTask = GetCurrentBlueprintNodeTask();
        NodeModes nodeMode = GetCurrentNodeMode();

        //set render and collide on sphere and cone
        SetThingVisibility(nodeMode == NodeModes.thing);
        SetWorkVisibility(nodeMode == NodeModes.work);

        //Debug.Log("SetNodeAppearanceAndSound " + nodeMode+" "+nodeTask.requiredThingType+" "+nodeTask.requiredThingType);

        if (nodeMode == NodeModes.constructed)
        {
            GetTextMesh(0).text = "";
            GetTextMesh(1).text = "";
            GetTextMesh(2).text = "";
            SetNodeMaterial(GetDoneMaterial());
            GetBlueprintParent().MakeIfConstructed();
            StopToolSound();
        }
        else if (nodeMode == NodeModes.thing)
        {
            GetTextMesh(0).text = ThingFactory.thingTemplates[nodeTask.requiredThingType].longName;
            GetTextMesh(1).text = nodeTask.quantityFilled + " / " + nodeTask.quantityRequired;
            GetTextMesh(2).text = "";
            SetNodeMaterial(GetThingMaterial());
        }
        else if (nodeMode == NodeModes.work)
        {
            GetTextMesh(0).text = ThingFactory.thingTemplates[nodeTask.requiredThingType].workVerb;
            GetTextMesh(1).text = Mathf.FloorToInt(nodeTask.workFilled) + " / " + nodeTask.workRequired;

            GetTextMesh(2).text = nodeTask.quantityRequired > 0 ?
                ThingFactory.thingTemplates[nodeTask.requiredThingType].longName : "";

            SetNodeMaterial(GetWorkMaterial());
            SetupAudioSource(nodeTask.requiredThingType);
        }
    }

    public override string ToString()
    {
        string result = "BlueprintNode '" + name + " nodeTasks.Count=" + GetNodeTasks().Count + " nodeTasks: ";
        for (int i = 0; i < nodeTasks.Count; i++)
            result += " " + i + "=" + nodeTasks[i];
        return result;
    }

    public bool IsConstructed()
    {
        //checks if all node tasks are done.
        for (int i = 0; i < GetNodeTasks().Count; i++)
        {
            if (!nodeTasks[i].IsDone())
                return false;
        }
        if (GetNodeTasks().Count == 0)
            Debug.LogError("IsConstructed called with empty nodeTasks list.");
        return true;
    }

    private void UpdateNodeTask()
    {
        BlueprintNodeTask nodeTask;
        do
        {
            nodeTask = GetCurrentBlueprintNodeTask();
            if (nodeTask != null && nodeTask.IsDone())
                nodeTasks.Remove(nodeTask);

        } while (nodeTask != null && nodeTask.IsDone());

        SetNodeAppearanceAndSound();
    }
}
