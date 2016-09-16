using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FramesPerSecond : MonoBehaviour {
    private float fpsUpdateValue = 0;
    private Rect fpsRect;
    private GUIStyle guiStyle;

    void Start()
    {
        fpsRect = new Rect(Screen.width/50, 0, 100, 40);
        guiStyle = GUIFunctions.GetStandardGUIStyle(12);
        guiStyle.alignment = TextAnchor.MiddleLeft;

        StartCoroutine(CalculateFPS());
    }
    void OnGUI()
    {
        GUI.Label(fpsRect, "FPS: "+string.Format("{0:0.0}", fpsUpdateValue), guiStyle);
    }

    private IEnumerator CalculateFPS()
    {
        while (true)
        {
            fpsUpdateValue = 1 / Time.deltaTime;
            yield return new WaitForSeconds(1);
        }
    }
}
