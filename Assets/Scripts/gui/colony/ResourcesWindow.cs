using UnityEngine;
using System.Collections;

public enum ResourceTypes
{
    water, oxygen, food, electricity, inspiration
}

public class ResourcesWindow
{
    //this window displays all the colony resources.
    private int windowId;
    private Rect windowRect, resourceRect;

    private const int resourceCount = 5;
    private static int iconCounter=0;

    private static ResourceReusable[] resourceReusables;

    public ResourcesWindow()
    {
        //used in every toolbar window to make a unique id
        windowId = Toolbar.GetNextWindowID();

        AddNextResource(ResourceTypes.water, "Kilograms of water available to the colony.");
        AddNextResource(ResourceTypes.oxygen, "Kilograms of oxygen available to the colony.");
        AddNextResource(ResourceTypes.food, "Kilograms of food available to the colony.");
        AddNextResource(ResourceTypes.electricity, "Kilowatt-hours of electricity available to the colony.");
        AddNextResource(ResourceTypes.inspiration, "Amount of inspiration points available to the colony.");

        windowRect = new Rect(Screen.width / 4, 0, Screen.width / 2, Screen.height / 10);
        resourceRect = new Rect(0, GUIFunctions.heightOfWindowBar, 
            windowRect.width / resourceCount, windowRect.height - GUIFunctions.heightOfWindowBar);
    }

    private void AddNextResource(ResourceTypes resourceType, string tooltipText)
    {
        if (resourceReusables == null)
            resourceReusables = new ResourceReusable[resourceCount];

        resourceReusables[iconCounter] = new ResourceReusable(resourceType, tooltipText);
        iconCounter++;
    }

    public void Draw()
    {
        windowRect = GUI.Window(windowId, windowRect, WindowFunction, "Resources");

    }

    private void WindowFunction(int id)
    {
        for (int i = 0; i < resourceCount; i++)
        {
            resourceRect.x = i * windowRect.width / resourceCount;
            ResourceReusable resourceReusable = resourceReusables[i];
            resourceReusable.Draw(resourceRect,ResourceController.GetResourceQuantityFromType(resourceReusable.resourceType));

            Rect tooltipRect = GUIFunctions.AddRectAndRectCoords(resourceRect, windowRect);
            string stringID = "resource-" + i;
            TooltipManager.SetTooltip(stringID, tooltipRect, resourceReusable.tooltipText);
        }
        GUI.DragWindow();
    }
}

