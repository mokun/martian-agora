using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WindowTypes
{
    //colony toolbar
    crew,
    log,
    operations,
    research,
    researchprojects,
    resources,
    robots,
    structures,
    widgetdesign,
    basedesign,

    //personal toolbar
    inventory,
    equipment,
    vehicleinventory,
    blueprints
}

public class Toolbar
{
    //this is a generic toolbar. Different instances can have any number of buttons that will active the appropriate windows.
    //note: windows are static and shared across all toolbar instances.

    private static int windowCounter=0;
    private Rect toolbarRect;

    private int buttonCount = 0;
    private static float buttonHeight;

    private List<ToolbarButton> toolbarButtons;

    //colony windows
    private static ResourcesWindow resourcesWindow;
    private static StructuresWindow structuresWindow;

    //personal windows
    private static InventoryWindow inventoryWindow;
    public static BlueprintWindow blueprintWindow;

    public Toolbar()
    {
        Initialize();
    }

    public static BlueprintWindow GetBlueprintWindow()
    {
        if (blueprintWindow == null)
            Initialize();
        return blueprintWindow;
    }

    public static int GetNextWindowID()
    {
        windowCounter++;
        return windowCounter;
    }

    private static void Initialize()
    {
        if (resourcesWindow == null)
        {
            buttonHeight = Screen.height / 8;

            resourcesWindow = new ResourcesWindow();
            structuresWindow = new StructuresWindow();

            inventoryWindow = new InventoryWindow();
            blueprintWindow = new BlueprintWindow();
        }
    }

    public int GetButtonCount()
    {
        return buttonCount;
    }

    public float GetToolbarWidth()
    {
        return buttonCount * buttonHeight;
    }

    public float GetButtonWidth()
    {
        return buttonHeight;
    }

    public void AddButton(WindowTypes windowType, string texturePath, string tooltipText)
    {
        if (toolbarButtons == null)
            toolbarButtons = new List<ToolbarButton>();
        
        toolbarButtons.Add(new ToolbarButton(this,buttonCount, texturePath, windowType, tooltipText));
        buttonCount++;
    }

    public void Draw()
    {
        //draw the toolbar buttons and windows if they are activated.
        GUI.depth = 1;

        foreach (ToolbarButton toolbarButton in toolbarButtons)
        {

            if (toolbarButton.activated)
                GUI.color = Color.cyan;
            else
                GUI.color = Color.white;

            if (GUI.Button(toolbarButton.GetDrawRect(), toolbarButton.buttonTexture))
                toolbarButton.activated = !toolbarButton.activated;
            if (toolbarButton.activated)
                DrawWindow(toolbarButton.windowType);

            //Rect tooltipRect = GUIFunctions.AddRectAndRectCoords(toolbarButton.GetDrawRect(), toolbarRect);
            TooltipManager.SetTooltip(toolbarButton.tooltipID, toolbarButton.GetDrawRect(), toolbarButton.tooltipText);
        }
    }

    private void DrawWindow(WindowTypes windowType)
    {
        //colony toolbar
        if (windowType == WindowTypes.resources)
            resourcesWindow.Draw();
        if (windowType == WindowTypes.structures)
            structuresWindow.Draw();

        //personal toolbar
        if (windowType == WindowTypes.inventory)
            inventoryWindow.Draw();
        if (windowType == WindowTypes.blueprints)
            blueprintWindow.Draw();
    }
}
