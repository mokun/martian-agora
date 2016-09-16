using UnityEngine;
using System.Collections;

public class StructuresWindow
{
    //this window shows a list and summary of all structures.

    private int windowId;
    private Rect windowRect, scrollRect, cellRect, bigCellRect, tinyCellRect;

    private float rowHeight;

    private Vector2 scrollPosition;
    private static GUIStyle rowStyle;

    public StructuresWindow()
    {
        //used in every toolbar window to make a unique id
        windowId = Toolbar.GetNextWindowID();
        scrollPosition = Vector2.zero;

        windowRect = new Rect(0, 0, Screen.width / 2, Screen.height * 2 / 3);
        windowRect = new Rect(Screen.width / 2 - windowRect.width / 2, Screen.height / 2 - windowRect.height / 2,
            windowRect.width, windowRect.height);

        scrollRect = new Rect(GUIFunctions.margin, GUIFunctions.heightOfWindowBar,
            windowRect.width - GUIFunctions.margin * 2, windowRect.height - GUIFunctions.heightOfWindowBar - GUIFunctions.margin);
        rowHeight = Screen.height / 20;

        cellRect = new Rect(0, 0, Screen.width / 6, rowHeight);
        bigCellRect = new Rect(0, 0, Screen.width / 4, rowHeight);
        tinyCellRect = new Rect(0, 0, Screen.width / 8, rowHeight);

        rowStyle = GUIFunctions.GetStandardGUIStyle(10);
        rowStyle.alignment = TextAnchor.MiddleLeft;
    }

    public void Draw()
    {
        windowRect = GUI.Window(windowId, windowRect, WindowFunction, "Structures");
    }

    private void WindowFunction(int id)
    {
        int row = 0;
        Rect viewRect = new Rect(0, 0, scrollRect.width, StructureController.GetStructureControllers().Count * rowHeight);

        scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, viewRect);
        foreach (StructureController structureController in StructureController.GetStructureControllers())
        {
            DrawStructureRow(structureController, row * rowHeight);
            row++;
        }
        GUI.EndScrollView();

        GUI.DragWindow();
    }

    private void DrawStructureRow(StructureController structureController, float yPosition)
    {
        bigCellRect.y = yPosition;
        cellRect.y = yPosition;
        tinyCellRect.y = yPosition;

        float x = 0;
        cellRect.x = x;
        PicNameStatusReusable pnr = new PicNameStatusReusable(structureController.matchingThing.name, structureController.matchingThing.iconTexture);
        pnr.Draw(cellRect, structureController.GetStatusTexture());

        x += cellRect.width;
        tinyCellRect.x = x;
        pnr = new PicNameStatusReusable(structureController.GetStatusLabel(), structureController.GetStatusTexture());
        pnr.Draw(tinyCellRect);

        x += tinyCellRect.width;
        cellRect.x = x;
        IconGroupReusable igr = structureController.GetIconGroup();
        igr.Draw(cellRect);
    }
}

