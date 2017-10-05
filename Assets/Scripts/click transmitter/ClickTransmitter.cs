    using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickTransmitter : MonoBehaviour
{
    //clickController monitors when a player clicks a clickTransmitter and uses it to send signals to clickReceivers.

    //if clickReceiverGameObjects is empty, find closest clickReceiver
    public List<GameObject> clickReceiverGameObjects;
		private GameManager gameManager;
    private GameObject player;

    void Start()
    {
				gameManager = FindObjectOfType<GameManager> ();
				player = gameManager.GetPlayer ();

        if (gameObject.GetComponent<ClickableItem>() == null)
            gameObject.AddComponent<ClickableItem>();
    }

    public bool FindsClosestReceiver()
    {
        //returns true if this transmitter is meant to find the closest clickReceiver
        return clickReceiverGameObjects.Count == 0;
    }
}
