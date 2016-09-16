using UnityEngine;
using System.Collections;

public class VehicleGUI : MonoBehaviour
{
    //gui stuff during vehicle driving.
    private string speedString;
    private GUIStyle guiStyle;
    private Rect speedRect;

    void Start()
    {
        speedRect = new Rect(Screen.width / 50, Screen.height * 10 / 11, 100, 40);
        guiStyle = GUIFunctions.GetStandardGUIStyle(12);
        guiStyle.alignment = TextAnchor.MiddleLeft;

        StartCoroutine(Cycle());
    }

    public IEnumerator Cycle()
    {
        while (true)
        {
            VehicleController vc = CrewManager.GetActiveCrew().GetVehicleController();
            float velocity = 0;
            if (vc != null)
            {
                velocity = vc.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                speedString = vc.gameObject.name;
                speedString+="\n" + string.Format("{0:0.0}", velocity) + " m/s";
                speedString += "\n" + string.Format("{0:0.0}", velocity*3.6) + " km/h";

            }
            else
            {
                speedString = "";
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnGUI()
    {
        GUI.Label(speedRect, speedString,guiStyle);
    }
}
