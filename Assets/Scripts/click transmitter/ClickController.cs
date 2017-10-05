using UnityEngine;
using System.Collections;
public class ClickController : MonoBehaviour
{
    //detects when the player clicks on a click transmitter.
    //if they do, it transmits the signal to right click receivers.
    private float range = 10;
    
    private MouseHoverInfo lastMouseHouseInfo;
		private GameManager gameManager;
		private Crew crew;

    void Start()
    {
				gameManager = FindObjectOfType<GameManager> ();
				crew = gameManager.GetPlayer ().GetComponent<Crew> ();
    }

    public MouseHoverInfo GetMouseHoverInfo(float range)
    {
        //update lastMouseHouseInfo if we haven't done it this frame
        if (lastMouseHouseInfo == null || lastMouseHouseInfo.frameNumber != Time.frameCount)
        {
						Vector3 pos = gameManager.GetPlayer().transform.position;
            RaycastHit rayCastHit = new RaycastHit();
            bool isHit = Physics.Linecast(pos, pos + crew.GetCrewCamera().transform.forward * range, out rayCastHit);
            lastMouseHouseInfo = new MouseHoverInfo(rayCastHit);
			//Debug.Log (crew.GetCrewCamera()+ " "+isHit+" "+range);
        }
        return lastMouseHouseInfo;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
						MouseHoverInfo mhi = GetMouseHoverInfo(crew.reachRange);
            if (mhi.IsHit)
            {
                ClickTransmitter clickTransmitter = mhi.hoverObject.GetComponent<ClickTransmitter>();
                if (clickTransmitter != null)
                    HitClickTransmitter(clickTransmitter);
            }
        }
    }

    private void HitClickTransmitter(ClickTransmitter clickTransmitter)
    {
        //called when clickTransmitter has been activated.
        if (clickTransmitter.FindsClosestReceiver())
        {
            ActivateClosestClickReceiver(clickTransmitter);
        }
        else
        {
            foreach (GameObject go in clickTransmitter.clickReceiverGameObjects)
            {
                ClickReceiver clickReceiver = go.GetComponent<ClickReceiver>();
                if (clickReceiver == null)
                {
                    Debug.LogError("an item in clickReceiverGameObjects has no clickReceiver. " + clickTransmitter.gameObject);
                    continue;
                }
                clickReceiver.Activate();
            }
        }

    }

    private void ActivateClosestClickReceiver(ClickTransmitter clickTransmitter)
    {
        //this handles two cases:
        //activate all clickReceivers with matching frequency, or
        //if frequency is zero, activate the closest clickReceiver.
        ClickReceiver closestReceiver = null;
        float minDistance = 0;

        foreach (ClickReceiver clickReceiver in FindObjectsOfType<ClickReceiver>())
        {

            float newDistance = Vector3.Distance(clickTransmitter.gameObject.transform.position, clickReceiver.startPosition);
            if (newDistance < minDistance || closestReceiver == null)
            {
                closestReceiver = clickReceiver;
                minDistance = newDistance;
            }
        }
        closestReceiver.Activate();
    }
}

public class MouseHoverInfo
{
    public GameObject hoverObject;
    public Terrain terrain;
    public Vector3 point;
    public RaycastHit raycastHit;
    public int frameNumber;

    public MouseHoverInfo(RaycastHit raycastHit)
    {
        if (raycastHit.collider != null)
            hoverObject = raycastHit.collider.gameObject;
        point = raycastHit.point;
        this.raycastHit = raycastHit;
        frameNumber = Time.frameCount;
    }

    public Terrain GetTerrain()
    {
        if (terrain == null && hoverObject != null)
            terrain = hoverObject.GetComponent<Terrain>();
        return terrain;
    }

    public bool IsHit
    {
				get{
						return hoverObject != null;
				}
    }
}