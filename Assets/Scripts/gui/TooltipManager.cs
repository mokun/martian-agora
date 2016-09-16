using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour
{
    //This is a singleton.
    //It needs to be a MonoBehaviour/component so that it can have its very own OnGUI call, so that GUI.depth=0 and
    //the tooltip will appear on top of anything.

    private GUIStyle tooltipStyle;

    //this helps us use TooltipManager as though it weren't MonoBehaviour
    private static TooltipManager tooltipManager;

    private static float timeCount = 0;
    private const float timeRequirement = 1f;
    private static Tooltip currentTooltip;

    private class Tooltip
    {
        //tooltips have a mouse hover area (mouseRect) where it's activated
        public Rect mouseRect;
        public string tooltipID;
        public string tooltipText;

        //this is how we know if the tooltip hover area is still being requested
        public int lastFrameCount;

        public Tooltip(Rect mouseRect, string tooltipID, string tooltipText)
        {
            this.mouseRect = mouseRect;
            this.tooltipID = tooltipID;
            this.tooltipText = GUIFunctions.MakeTextMultilined(tooltipText, 20);
            lastFrameCount = 0;
        }
    }
    private static Dictionary<string, Tooltip> tooltips;

    public static TooltipManager GetTooltipManager()
    {
        if (tooltipManager == null)
        {
            GameObject gm = GameObject.Find("game manager");
            tooltipManager = gm.AddComponent<TooltipManager>();
        }
        return tooltipManager;
    }

    void Start()
    {
        tooltipStyle = GUIFunctions.GetStandardGUIStyle(12);
        tooltipStyle.alignment = TextAnchor.MiddleCenter;
        tooltipStyle.normal.background = GUIFunctions.Get1x1Texture(new Color(0, .8f, .8f, 0.8f));
    }

    private static Dictionary<string, Tooltip> GetTooltips()
    {
        if (tooltips == null)
            tooltips = new Dictionary<string, Tooltip>();
        return tooltips;
    }

    public static void SetTooltip(string tooltipID, Rect mouseRect, string tooltipText)
    {
        //as long as SetTooltip is called every OnGUI frame, a tooltip will be available on mouse hover.
        //tooltipID must be unique to that tooltip.
        //mouseRect is the hover area. Uses the same coordinate system as GUI draw rects. Top left origin.

        GetTooltipManager();

        Tooltip tooltip;
        if (!GetTooltips().ContainsKey(tooltipID))
        {
            tooltip = new Tooltip(mouseRect, tooltipID, tooltipText);
            tooltip.lastFrameCount = Time.frameCount;
            tooltips.Add(tooltipID, tooltip);
        }
        else
        {
            tooltip = tooltips[tooltipID];
            tooltip.lastFrameCount = Time.frameCount;

            //update the mouseRect for the tooltip if we potentially just unclicked a window drag
            if (Input.GetMouseButtonUp(0))
                tooltip.mouseRect = mouseRect;
        }

    }

    private static void RemoveTooltip(string tooltipID)
    {
        tooltips.Remove(tooltipID);
    }

    void Update()
    {
        Vector3 mousePosition = GUIFunctions.GetGUIMousePositionFromInputMousePosition();
        if (currentTooltip == null)
        {
            //no tooltip was hovered over recently.
            //check all tooltips if we're hovering over them
            foreach (Tooltip tooltip in tooltips.Values)
            {
                if (tooltip.mouseRect.Contains(mousePosition))
                {
                    currentTooltip = tooltip;
                    break;
                }
            }
        }
        else if (currentTooltip.lastFrameCount < Time.frameCount - 1)
        //this means SetTooltip wasn't called recently and the tooltip shouldn't display anymore.
        {
            RemoveTooltip(currentTooltip.tooltipID);
            currentTooltip = null;
        }
        else
        {
            //we were just hovering over a tooltip that is still active.
            //are we still hovering?
            if (currentTooltip.mouseRect.Contains(mousePosition))
                timeCount += Time.deltaTime;
            else
            {
                timeCount = 0;
                currentTooltip = null;
            }
        }
    }

    public static void ClearAllTooltips()
    {
        //used externally when the entire toolbar/gui changes.
        //also clears tooltips dictionary for performance. This forces a rebuild for next toolbar but
        //it means we won't be iterating over all tooltips from all previously visited guis.
        timeCount = 0;
        tooltips = new Dictionary<string, Tooltip>();
    }

    void OnGUI()
    {
        //show the tooltip if we've been hovering long enough.
        if (timeCount > timeRequirement)
        {
            GUI.depth = 0;

            Vector2 mousePosition = Event.current.mousePosition;
            Vector2 textSize = tooltipStyle.CalcSize(new GUIContent(currentTooltip.tooltipText));

            Vector2 offset = new Vector2(GUIFunctions.margin, -textSize.y - GUIFunctions.margin);
            if (mousePosition.x + textSize.x + offset.x > Screen.width)
                offset.x = -textSize.x - GUIFunctions.margin;
            if (mousePosition.y + offset.y < 0)
                offset.y = GUIFunctions.margin;

            float borderSize = GUIFunctions.margin;
            Rect drawRect = new Rect(mousePosition.x + offset.x - borderSize, mousePosition.y + offset.y - borderSize,
                textSize.x + borderSize * 2, textSize.y + borderSize * 2);

            GUI.Label(drawRect, currentTooltip.tooltipText, tooltipStyle);
        }
    }
}
