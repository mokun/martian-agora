using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueprintWindow
{
    //this window shows all blueprints and allows you to select one.

    private Dictionary<BlueprintDesign, StructureReusable> structureReusables;

    private int windowId, blueprintIndex;

    private Rect windowRect, selectedRect, scrollRect, viewRect;
    private Vector2 scrollPosition;

    public BlueprintWindow()
    {
        //used in every toolbar window to make a unique id
        windowId = Toolbar.GetNextWindowID();
        blueprintIndex = 0;

        structureReusables = new Dictionary<BlueprintDesign, StructureReusable>();

        windowRect = new Rect(Screen.width / 2, 0, Screen.width / 2, Screen.height * 7 / 8);
        selectedRect = new Rect(0, GUIFunctions.heightOfWindowBar, windowRect.width, Screen.height / 5);
        scrollRect = new Rect(0, selectedRect.y + selectedRect.height + GUIFunctions.margin, selectedRect.width, 0);
        scrollRect.height = windowRect.height - scrollRect.y;
        viewRect = new Rect(0, 0, selectedRect.width, 0);
    }

    public void Draw()
    {
        windowRect = GUI.Window(windowId, windowRect, WindowFunction, "Blueprints");
    }

    public BlueprintDesign GetSelectedBlueprintDesign()
    {
        return BlueprintDesignManager.blueprintDesigns[blueprintIndex];
    }

    private StructureReusable GetStructureReusable(Rect drawRect, BlueprintDesign blueprintDesign)
    {
        if (!structureReusables.ContainsKey(blueprintDesign))
        {
            Thing thing = ThingFactory.MakeThing(blueprintDesign.thingType);
            StructureReusable sr = new StructureReusable(drawRect, thing.iconTexture, thing.longName, blueprintDesign);
            structureReusables.Add(blueprintDesign, sr);
        }
        return structureReusables[blueprintDesign];
    }

    private void DrawSelected()
    {
        BlueprintDesign bd = GetSelectedBlueprintDesign();
        StructureReusable sr = GetStructureReusable(selectedRect, bd);
        sr.Draw(selectedRect, true);
    }

    private void DrawList()
    {
        viewRect.height = BlueprintDesignManager.blueprintDesigns.Count * selectedRect.height;
        scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, viewRect);

        int hoverIndex = GetMouseHoverListIndex();
        int row = 0;
        foreach (BlueprintDesign bd in BlueprintDesignManager.blueprintDesigns)
        {
            float y = row * selectedRect.height;
            Rect rowRect = new Rect(selectedRect.x, y, selectedRect.width, selectedRect.height);

            StructureReusable sr = GetStructureReusable(rowRect, bd);

            bool drawHighlighted = row == blueprintIndex || (hoverIndex != -1 && row == hoverIndex);
            sr.Draw(rowRect, drawHighlighted);
            row++;
        }
        GUI.EndScrollView();
    }

    private int GetMouseHoverListIndex()
    {
        Vector2 mousePosition = GUIFunctions.GetMousePositionFromInputMousePosition();

        float relativeX = mousePosition.x - scrollRect.x - windowRect.x;
        if (relativeX < 0 || relativeX > scrollRect.width - GUIFunctions.verticalBarWidth)
            return -1;

        float relativeY = mousePosition.y - scrollRect.y - windowRect.y + scrollPosition.y;
        if (relativeY > 0 && relativeY < viewRect.height)
            return Mathf.FloorToInt(relativeY / selectedRect.height);

        return -1;
    }

    private void WindowFunction(int id)
    {
        DrawSelected();
        DrawList();

        if (Input.GetMouseButtonDown(0))
        {
            int index = GetMouseHoverListIndex();
            if (index > -1)
                blueprintIndex = index;
        }

        GUI.DragWindow();
    }

}

