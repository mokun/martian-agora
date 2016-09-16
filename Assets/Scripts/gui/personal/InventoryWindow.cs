using UnityEngine;
using System.Collections;

public class InventoryWindow
{
    //this window shows a personal inventory and allows you to move items around.
    public Thing[] things;

    private Rect windowRect;

    public static int thingsPerRow = 8;

    private int rowCount;
    public float iconSize;
    private Rect[,] thingIconRects;
    private string[,] thingTooltipIDs;

    private int dragIndex;
    private Thing dragThing;

    public InventoryWindow()
    {
        Crew crew = CrewManager.GetActiveCrew() ;
        things = crew.inventory;

        float windowSize = Screen.height * 0.8f;
        windowRect = new Rect(Screen.width / 4 - windowSize / 2, Screen.height * 0.05f,
            windowSize, windowSize);

        //set all thing icon rects
        rowCount = Crew.inventorySize / thingsPerRow;
        thingIconRects = new Rect[thingsPerRow, rowCount];
        thingTooltipIDs = new string[thingsPerRow, rowCount];

        iconSize = windowSize / (thingsPerRow + 2);
        for (int i = 0; i < thingsPerRow; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                thingIconRects[i, j] = new Rect(iconSize + i * iconSize, iconSize + j * iconSize, iconSize, iconSize);
                thingTooltipIDs[i, j] = "inventory(" + i + "," + j + ")";
            }
        }
    }



    private IntVector2 GetDragCoords()
    {
        //takes into account the position of the inventory window, and 
        //its things, to find which icon coordinate the mouse is on.

        Vector2 mousePoint = GUIFunctions.GetGUIMousePositionFromInputMousePosition();

        Vector2 point = new Vector2(mousePoint.x - windowRect.x - iconSize,
            mousePoint.y - windowRect.y - iconSize);

        return new IntVector2(Mathf.FloorToInt(point.x / iconSize),
        Mathf.FloorToInt(point.y / iconSize));
    }

    private void SetDragThing()
    {
        IntVector2 dragCoords = GetDragCoords();

        if (IsWithinInventorySlots(dragCoords))
        {
            dragIndex = GetDragIndex(dragCoords);
            dragThing = things[dragIndex];
        }

    }

    private int GetDragIndex(IntVector2 dragCoords)
    {
        return dragCoords.x + dragCoords.y * thingsPerRow;
    }

    private void ReleaseDragThing()
    {
        IntVector2 dragCoords = GetDragCoords();

        if (IsWithinInventorySlots(dragCoords))
        {
            int dragIndex = GetDragIndex(dragCoords);
            if (things[dragIndex] == null)
            {
                for (int i = 0; i < Crew.inventorySize; i++)
                    if (dragThing.Equals(things[i]))
                        things[i] = null;

                things[dragIndex] = dragThing;
            }
        }

        dragThing = null;
    }

    private bool IsWithinInventorySlots(IntVector2 dragCoords)
    {
        return (dragCoords.x >= 0 && dragCoords.y >= 0 &&
            dragCoords.x < thingsPerRow && dragCoords.y < rowCount);
    }

    public void Draw()
    {
        windowRect = GUI.Window(0, windowRect, WindowFunction, "Inventory");

        if (Input.GetMouseButtonDown(0) && Event.current.type == EventType.Layout)
            SetDragThing();
        if (Input.GetMouseButtonUp(0) && dragThing != null && Event.current.type == EventType.Layout)
            ReleaseDragThing();
    }

    private void WindowFunction(int id)
    {
        //GUI.Button(new Rect(10, 10, 50, 50), "button!!");

        for (int i = 0; i < thingsPerRow; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                Rect rect = thingIconRects[i, j];
                if (j == rowCount - 1)
                    GUI.Label(rect, (i + 1).ToString(), GUIFunctions.hotkeyStyle);

                Thing thing = things[i + j * thingsPerRow];
                if (thing == null)
                {
                    GUI.DrawTexture(rect, Thing.GetFrameInventoryTexture());
                }
                else
                {
                    GUIFunctions.DrawThing(rect, thing, true);
                    Rect tooltipRect = GUIFunctions.AddRectAndRectCoords(rect, windowRect);
                    TooltipManager.SetTooltip(thingTooltipIDs[i, j], tooltipRect, thing.longName);
                }
            }
        }

        if (dragThing == null)
        {
            GUI.DragWindow();
        }
        else
        {
            Rect rect = new Rect(Input.mousePosition.x - iconSize / 2 - windowRect.x,
                Screen.height - Input.mousePosition.y - iconSize / 2 - windowRect.y,
                iconSize, iconSize);
            if (dragThing.isBlueprint)
                GUIFunctions.DrawThing(rect, dragThing, true);
            else
                GUIFunctions.DrawThing(rect, dragThing, false);
        }
    }
}
