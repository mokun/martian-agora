    using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickTransmitter : MonoBehaviour
{
    //clickController monitors when a player clicks a clickTransmitter and uses it to send signals to clickReceivers.

    //if clickReceiverGameObjects is empty, find closest clickReceiver
    public List<GameObject> clickReceiverGameObjects;
    public VehicleController vehicleController;
    private GameObject player;

    void Start()
    {
        player = CrewManager.GetActiveCrewGameObject();
        if (player == null)
        {
            Debug.LogError("Couldn't find a GameObject named \"player\".");
            enabled = false;
            return;
        }

        if (gameObject.GetComponent<ClickableItem>() == null)
            gameObject.AddComponent<ClickableItem>();

        if (FindObjectOfType<ClickController>() == null)
            player.AddComponent<ClickController>();
    }

    public bool FindsClosestReceiver()
    {
        //returns true if this transmitter is meant to find the closest clickReceiver
        return vehicleController==null && clickReceiverGameObjects.Count == 0;
    }
}
